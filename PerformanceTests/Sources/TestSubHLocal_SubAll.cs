using System;
using SignalHubT;
using Tests;

namespace PerformanceTests
{
public class TestSubHLocal_SubAll: IPerformanceTest, IToTestString
{
	public TestSubHLocal_SubAll(Int32 iterations, Int32 subCount)
	{
		_iterations = iterations;
		_subCount = subCount;
	}

	private Int32 _iterations;
	private Int32 _value;
	private readonly Int32 _subCount;
	private ISignalHub _hub;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		_hub = IoCExtra.I.CreateSignalHubLocal();
	}

	private void HandleMessageStruct(ref MessageStruct message)
	{
		_value = message.Value;
	}

	public void Run( )
	{
		for ( int i = 0; i < _subCount; i++ )
		{
			_hub.Sub<MessageStruct>(HandleMessageStruct);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount.ToString("e0")}";
}
}