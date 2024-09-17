// Copyright (c) DreamsFactory. All rights reserved.

namespace Breeze;

/// <summary>
/// Static methods that helps with creating <see cref="Failable{TSuccess, TFailure}"/> instances.
/// </summary>
public class Failable
{
	/// <summary>
	/// Creates a <see cref="Failable{TSuccess, TFailure}"/> instance with the specified success value.
	/// </summary>
	/// <typeparam name="TSuccess">The type of the success value.</typeparam>
	/// <typeparam name="TFailure">The type of the failure value.</typeparam>
	/// <param name="success">The success value.</param>
	/// <returns>A new <see cref="Failable{TSuccess, TFailure}"/> instance.</returns>
	public static Failable<TSuccess, TFailure> Success<TSuccess, TFailure>(TSuccess success)
	{
		return Failable<TSuccess, TFailure>.Success(success);
	}

	/// <summary>
	/// Creates a <see cref="Failable{TSuccess, String}"/> instance with the specified success value.
	/// </summary>
	/// <typeparam name="TSuccess">The type of the success value.</typeparam>
	/// <param name="success">The success value.</param>
	/// <returns>A new <see cref="Failable{TSuccess, String}"/> instance.</returns>
	public static Failable<TSuccess, string> Success<TSuccess>(TSuccess success)
	{
		return Failable<TSuccess, string>.Success(success);
	}

	/// <summary>
	/// Creates a <see cref="Failable{TSuccess, TFailure}"/> instance with the specified failure value.
	/// </summary>
	/// <typeparam name="TSuccess">The type of the success value.</typeparam>
	/// <typeparam name="TFailure">The type of the failure value.</typeparam>
	/// <param name="failure">The failure value.</param>
	/// <returns>A new <see cref="Failable{TSuccess, TFailure}"/> instance.</returns>
	public static Failable<TSuccess, TFailure> Failure<TSuccess, TFailure>(TFailure failure)
	{
		return Failable<TSuccess, TFailure>.Failure(failure);
	}

	/// <summary>
	/// Creates a <see cref="Failable{TSuccess, String}"/> instance with the specified failure value.
	/// </summary>
	/// <typeparam name="TSuccess">The type of the success value.</typeparam>
	/// <param name="failure">The failure value.</param>
	/// <returns>A new <see cref="Failable{TSuccess, String}"/> instance.</returns>
	public static Failable<TSuccess, string> Failure<TSuccess>(string failure)
	{
		return Failable<TSuccess, string>.Failure(failure);
	}
}
