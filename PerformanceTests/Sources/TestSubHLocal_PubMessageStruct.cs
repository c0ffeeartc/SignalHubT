using System;
using SignalBusT;
using Tests;

namespace PerformanceTests
{
public class TestSubHLocal_PubMessageStruct : IPerformanceTest, IToTestString
{
	public TestSubHLocal_PubMessageStruct(Int32 iterations, Int32 subCount)
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

		for ( int i = 0; i < _subCount; i++ )
		{
			SignalHub.I.Sub<MessageStruct>(HandleMessageStruct);
		}
	}

	private void HandleMessageStruct(ref MessageStruct message)
	{
		_value = message.Value;
	}

	public void Run( )
	{
		for ( int i = 0; i < _iterations; i++ )
		{
			SignalHub.I.Pub(new MessageStruct(i));
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount}";
}
}