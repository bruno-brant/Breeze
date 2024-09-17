// Copyright (c) DreamsFactory. All rights reserved.

namespace Breeze;

/// <summary>
/// Represents a result of an operation that can fail.
/// </summary>
/// <typeparam name="TSuccess">The type of a successful result.</typeparam>
/// <typeparam name="TFailure">The type of a failed result.</typeparam>
/// <remarks>
/// <see cref="Failable{TSuccess, TFailure}"/> should be used whenever you have
/// a method that can predictably fail and you want to communicate the failure
/// to the caller.
///
/// This should be used instead of exceptions when the failure is predictable,
/// because if forces the caller to handle the failure case. It also should only
/// be applied where the failure is unavoidable - if the failure is avoidable,
/// an exception should be thrown instead. The rationale is that, when avoiadable,
/// the failure is a breach of contract - of the method precondition. In that
/// case, you shouldn't expect the method to fail, it's a bug in the caller code.
///
/// On the other hand, there are multiple situations where the caller can't guarantee
/// that the method should pass, but it's required that the caller handles the failure.
/// A good example is a file read operation - the file may not exist, the file may be
/// locked, the file may be corrupted, etc. In all these cases, the caller usually
/// should handle the failure and it's predictable that the operation may fail.
///
/// Of course, the use is subjective - you should use your best judgement to decide
/// when to force the caller to deal with failure and when to throw exceptions.
/// As a rule of thumb, though, exceptions are for unexpected cases and usually
/// the caller doesn't handle it - it only propagates the issue to the user.
///
/// <see cref="Failable{TSuccess, TFailure}"/> is implemented as a monad. This means
/// that you can chain operations that may fail and the failure will be propagated
/// to the caller. You should use the <see cref="IfSuccess{TNewSuccess}(Func{TSuccess, Failable{TNewSuccess, TFailure}})"/>
/// throughout code that receives a Failable instance, chaining calls. Preferably,
/// you don't transform the error until you're ready to handle it.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// public Failable<User, string> GetUser(int key);
///
/// var user = GetUser(1);
/// var name = user.IfSuccess(u => u.Name);
///
/// return name;
/// ]]></code>
/// </example>
public class Failable<TSuccess, TFailure>
{
	private readonly TSuccess? _success;
	private readonly TFailure? _failure;

	/// <summary>
	/// Initializes a new instance of the <see cref="Failable{TSuccess, TFailure}"/> class.
	/// </summary>
	/// <param name="success">A successful value.</param>
	/// <param name="failure">A failure value.</param>
	/// <param name="isSuccess">Indicates whether this is a success or a failure.</param>
	internal protected Failable(TSuccess? success, TFailure? failure, bool isSuccess)
	{
		_success = success;
		_failure = failure;
		IsSuccess = isSuccess;
	}

	/// <summary>
	/// Gets a value indicating whether this is a success or a failure.
	/// </summary>
	public bool IsSuccess { get; private set; }

	/// <summary>
	/// Converts a <typeparamref name="TSuccess"/> value to a successful <see cref="Failable{TSuccess, TFailure}"/>.
	/// </summary>
	/// <param name="success">The success value that will be wrapped in a <see cref="Failable{TSuccess, TFailure}"/>.</param>
	public static implicit operator Failable<TSuccess, TFailure>(TSuccess success)
	{
		return Success(success);
	}

	/// <summary>
	/// Converts a <typeparamref name="TFailure"/> value to a failed <see cref="Failable{TSuccess, TFailure}"/>.
	/// </summary>
	/// <param name="failure">The failure value that will be wrapped in a <see cref="Failable{TSuccess, TFailure}"/>.</param>
	public static implicit operator Failable<TSuccess, TFailure>(TFailure failure)
	{
		return Failure(failure);
	}

	/// <inheritdoc/>
	public static bool operator ==(Failable<TSuccess, TFailure> left, Failable<TSuccess, TFailure> right)
	{
		return left.Equals(right);
	}

	/// <inheritdoc/>
	public static bool operator !=(Failable<TSuccess, TFailure> left, Failable<TSuccess, TFailure> right)
	{
		return !left.Equals(right);
	}

	/// <summary>
	/// Creates successful result for <see cref="Failable{TSuccess, TFailure}"/>.
	/// </summary>
	/// <param name="success">
	/// The successful value.
	/// </param>
	/// <returns>
	/// A new instance of <see cref="Failable{TSuccess, TFailure}"/> with the successful value.
	/// </returns>
	public static Failable<TSuccess, TFailure> Success(TSuccess success)
	{
		return new Failable<TSuccess, TFailure>(success, default, true);
	}

	/// <summary>
	/// Creates failure result for <see cref="Failable{TSuccess, TFailure}"/>.
	/// </summary>
	/// <param name="failure">
	/// The failure value.
	/// </param>
	/// <returns>
	/// A new instance of <see cref="Failable{TSuccess, TFailure}"/> with the failure value.
	/// </returns>
	public static Failable<TSuccess, TFailure> Failure(TFailure failure)
	{
		return new Failable<TSuccess, TFailure>(default, failure, false);
	}

	/// <summary>
	/// Call the provided function if this is a success.
	/// </summary>
	/// <typeparam name="TNewSuccess">The type of the result of the <paramref name="tranformSuccess"/>.</typeparam>
	/// <param name="tranformSuccess">
	/// A function that will be applied to the successful result of this type.
	/// </param>
	/// <returns>
	/// A new failable with the result of the function if this is a success, otherwise the failure is propagated.
	/// </returns>
	/// <remarks>
	/// This is the monadic bind function. It allows you to transform the success value of this failable
	/// into a new failable. If this is a failure, the failure is propagated.
	///
	/// We don't provide a bind function for the failure on purpose - we want to pass the failure through
	/// until we're ready to handle it. This is a design choice to make it easier to reason about the
	/// errors are handled in the code.
	///
	/// Of course, there are reasons to enrich a failure (for instance, a composite operation may want to
	/// transform the result of a failed operation into a more detailed error message); in that case, you
	/// should handle the failure by checking the <see cref="IsSuccess"/> variable.
	/// </remarks>
	public Failable<TNewSuccess, TFailure> IfSuccess<TNewSuccess>(Func<TSuccess, Failable<TNewSuccess, TFailure>> tranformSuccess)
	{
		if (IsSuccess)
		{
			return tranformSuccess(_success!);
		}
		else
		{
			return Failable.Failure<TNewSuccess, TFailure>(_failure!);
		}
	}

	/// <summary>
	/// Call the provided function if this is a success.
	/// </summary>
	/// <typeparam name="TNewSuccess">The type of the result of the <paramref name="tranformSuccess"/>.</typeparam>
	/// <param name="tranformSuccess">
	/// A function that will be applied to the successful result of this type.
	/// </param>
	/// <returns>
	/// A new failable with the result of the function if this is a success, otherwise the failure is propagated.
	/// </returns>
	/// <remarks>
	/// This is the monadic bind function. It allows you to transform the success value of this failable
	/// into a new failable. If this is a failure, the failure is propagated.
	///
	/// We don't provide a bind function for the failure on purpose - we want to pass the failure through
	/// until we're ready to handle it. This is a design choice to make it easier to reason about the
	/// errors are handled in the code.
	///
	/// Of course, there are reasons to enrich a failure (for instance, a composite operation may want to
	/// transform the result of a failed operation into a more detailed error message); in that case, you
	/// should handle the failure by checking the <see cref="IsSuccess"/> variable.
	/// </remarks>
	public Failable<TNewSuccess, TFailure> IfSuccess<TNewSuccess>(Func<TSuccess, TNewSuccess> tranformSuccess)
	{
		if (IsSuccess)
		{
			return Failable.Success<TNewSuccess, TFailure>(tranformSuccess(_success!));
		}
		else
		{
			return Failable.Failure<TNewSuccess, TFailure>(_failure!);
		}
	}

	/// <summary>
	/// Returns the success value if this is a success, otherwise throws an exception.
	/// </summary>
	/// <returns>
	/// The success value.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Raised when this is a failure.
	/// </exception>
	public TSuccess GetSuccess() => IsSuccess is true
		? _success!
		: throw new InvalidOperationException("Can't get success result - this is a failure.");

	/// <summary>
	/// Returns the failure value if this the Failable contains a success,
	/// otherwise throws an exception.
	/// </summary>
	/// <returns>
	/// The failure value.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Raised when this is a success.
	/// </exception>
	public TFailure GetFailure() => IsSuccess is false
		? _failure!
		: throw new InvalidOperationException("Can't get failure result - this is a success.");

	/// <summary>
	/// Returns the success value if the Failable contains a failure,
	/// otherwise throws an exception.
	/// </summary>
	/// <returns>
	/// The failure value.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Raised when this is a success.
	/// </exception>
	public object GetValue() => IsSuccess is true
		? _success!
		: _failure!;

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is Failable<TSuccess, TFailure> other)
		{
			return IsSuccess
				? _success!.Equals(other._success)
				: _failure!.Equals(other._failure);
		}

		return false;
	}

	/// <inheritdoc/>
	public override int GetHashCode() => IsSuccess
		? _success!.GetHashCode()
		: _failure!.GetHashCode();
}
