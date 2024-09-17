// Copyright (c) DreamsFactory. All rights reserved.

namespace Breeze;

/// <summary>
/// Represents a result of an operation that can fail with a string representing
/// the failure.
/// </summary>
/// <inheritdoc/>
public class Failable<TSuccess> : Failable<TSuccess, string>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Failable{TSuccess}"/> class.
	/// </summary>
	/// <param name="success">The successful value to be wrapped.</param>
	internal protected Failable(TSuccess success)
		: base(success, default, true)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Failable{TSuccess}"/> class.
	/// </summary>
	/// <param name="failure">The failure value to be wrapped.</param>
	internal protected Failable(string failure)
		: base(default, failure, false)
	{
	}

	/// <summary>
	/// Converts a <typeparamref name="TSuccess"/> value to a <see cref="Failable{TSuccess}"/>.
	/// </summary>
	/// <param name="success">The successful value to be wrapped.</param>
	public static implicit operator Failable<TSuccess>(TSuccess success)
	{
		return new Failable<TSuccess>(success);
	}

	/// <summary>
	/// Converts a <see cref="string"/> value to a <see cref="Failable{TSuccess}"/>.
	/// </summary>
	/// <param name="failure">
	/// The failure value to be wrapped.
	/// </param>
	public static implicit operator Failable<TSuccess>(string failure)
	{
		return new Failable<TSuccess>(failure);
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
	/// should handle the failure by checking the <see cref="Failable{TSuccess, String}.IsSuccess"/> variable.
	/// </remarks>
	public Failable<TNewSuccess> IfSuccess<TNewSuccess>(Func<TSuccess, Failable<TNewSuccess>> tranformSuccess)
	{
		if (IsSuccess)
		{
			return tranformSuccess(GetSuccess());
		}
		else
		{
			return GetFailure();
		}
	}
}
