using System;
using SignalHubT;
using Tests;

namespace PerformanceTests
{
public class TestHub_PubMessageStruct : IPerformanceTest, IToTestString
{
	public TestHub_PubMessageStruct(ISignalHub hub, Int32 iterations, Int32 subCount)
	{
		_hub = hub;
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
		_hub.UnsubAll();
		for ( int i = 0; i < _subCount; i++ )
		{
			_hub.Sub<MessageStruct>(HandleMessageStruct);
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
			_hub.Pub(new MessageStruct(i));
		}
	}

	public String ToTestString( ) => $"{GetType().Name}{(_hub is SignalHubLocal ? "(Local)":"")}:s_{_subCount}";
}
}