// Copyright (c) DreamsFactory. All rights reserved.

using AutoFixture.Xunit2;

namespace Breeze.Tests;

public class FailableTests
{
	[Fact]
	public void CanCreateAndRecoverSuccess()
	{
		var failable = Failable.Success(42);

		Assert.True(failable.IsSuccess);
		Assert.Equal(42, failable.GetSuccess());
	}

	[Fact]
	public void CanCreateAndRecoverError()
	{
		var failable = Failable.Failure<int>("error");

		Assert.False(failable.IsSuccess);
		Assert.Equal("error", failable.GetFailure());
	}

	[Theory, AutoData]
	public void CanTransformSuccesses(Guid guid)
	{
		var success = Failable.Success(guid)
			.IfSuccess(s => s.ToString());

		Assert.True(success.IsSuccess);
		Assert.IsType<string>(success.GetSuccess());
		Assert.Equal(guid.ToString(), success.GetSuccess());
	}

	[Fact]
	public void IfSuccess_PropagatesErrors()
	{
		var error = Failable.Failure<Guid>("Message")
			.IfSuccess(g => g.ToByteArray());

		Assert.False(error.IsSuccess);
		Assert.Equal("Message", error.GetFailure());
	}

	[Fact]
	public void IfSuccess_Success_ThenSuccess_ResultIsSuccess()
	{
		var result = Success1("myId").IfSuccess(Success2);

		Assert.True(result.IsSuccess);
		Assert.Equal("myId", result.GetSuccess()?[0]?.Value);
	}

	[Fact]
	public void IfSuccess_Success_ThenError_ResultIsError2()
	{
		var s1 = Success1("myId");
		var result = s1.IfSuccess(Error2);

		Assert.False(result.IsSuccess);
		Assert.Equal("Error2", result.GetFailure());
	}

	[Fact]
	public void IfSuccess_Error_ThenError_ResultIsError1()
	{
		var result = Error1().IfSuccess(Error2);

		Assert.False(result.IsSuccess);
		Assert.Equal("Error1", result.GetFailure());
	}

	[Theory, AutoData]
	public void Equals_RespectsInnerEquality(int data)
	{
		Failable<int> f1 = data;
		Failable<int> f2 = data;

		Assert.Equal(f1, f2);
		Assert.True(f1 == f2);
		Assert.False(f1 != f2);
	}

	[Theory, AutoData]
	public void Equals_RespectsInnerInequality(int data, int diff)
	{
		Failable<int> f1 = data;
		Failable<int> f2 = data + diff + 1;

		Assert.NotEqual(f1, f2);
		Assert.False(f1 == f2);
		Assert.True(f1 != f2);
	}

	[Fact]
	public void DemonstrateMultipleCalls()
	{
		var result = Success1("myId")
			.IfSuccess(Error2)
			.IfSuccess(users => users.FirstOrDefault());

		Assert.False(result.IsSuccess);
		Assert.Equal("Error2", result);
	}

	private static Failable<Foo> Success1(string value) => new Foo(value);

	private static Failable<Foo> Error1() => "Error1";

	private static Failable<List<Foo>> Success2(Foo foo) => new List<Foo>([foo]);

	private static Failable<List<Foo>> Error2(Foo user) => "Error2";

	private class Foo(string value)
	{
		public string Value { get; } = value;
	}
}
