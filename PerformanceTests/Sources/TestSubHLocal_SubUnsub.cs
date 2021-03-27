using System;
using SubHubT;
using Tests;

namespace PerformanceTests
{
public class TestSubHLocal_SubUnsub : IPerformanceTest, IToTestString
{
	public TestSubHLocal_SubUnsub(Int32 iterations, Int32 subCount)
	{
		_iterations = iterations;
		_subCount = subCount;
	}

	private Int32 _iterations;
	private Int32 _value;
	private readonly Int32 _subCount;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		SubH.I = IoC.I.CreateSubHLocal();
	}

	private void HandleMessageStruct(ref MessageStruct message)
	{
		_value = message.Value;
	}

	public void Run( )
	{
		for ( int i = 0; i < _subCount; i++ )
		{
			var subscription = SubH.I.Sub<MessageStruct>(HandleMessageStruct);
			SubH.I.Unsub(subscription);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount.ToString("e0")}";
}
}