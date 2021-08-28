using System;
using SignalBusT;
using Tests;

namespace PerformanceTests
{
public class TestSubH_PubMessageStruct : IPerformanceTest, IToTestString
{
	public TestSubH_PubMessageStruct(ISignalHub signalHub, Int32 iterations, Int32 subCount)
	{
		_signalHub = signalHub;
		_iterations = iterations;
		_subCount = subCount;
	}

	private Int32 _iterations;
	private Int32 _value;
	private readonly Int32 _subCount;
	private ISignalHub _signalHub;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		_signalHub.UnsubAll();

		for ( int i = 0; i < _subCount; i++ )
		{
			_signalHub.Sub<MessageStruct>(HandleMessageStruct);
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
			_signalHub.Pub(new MessageStruct(i));
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount}";
}
}