using System;
using SignalHubT;
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
		SignalHub.I = IoCExtra.I.CreateSignalHubLocal();
	}

	private void HandleMessageStruct(ref MessageStruct message)
	{
		_value = message.Value;
	}

	public void Run( )
	{
		for ( int i = 0; i < _subCount; i++ )
		{
			var subscription = SignalHub.I.Sub<MessageStruct>(HandleMessageStruct);
			SignalHub.I.Unsub(subscription);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount.ToString("e0")}";
}
}