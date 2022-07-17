using System;
using SignalHubT;
using Tests;

namespace PerformanceTests
{
public class TestHub_SubAll_UnsubAll : IPerformanceTest, IToTestString
{
	public TestHub_SubAll_UnsubAll(ISignalHub hub, Int32 iterations, Int32 subCount)
	{
		_iterations = iterations;
		_subCount = subCount;
		_hub = hub;
	}

	private Int32 _iterations;
	private Int32 _value;
	private readonly Int32 _subCount;
	private ISubscription<MessageStruct>[] _subs;
	private ISignalHub _hub;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		_subs = new ISubscription<MessageStruct>[_subCount];
	}

	private void HandleMessageStruct(ref MessageStruct message)
	{
		_value = message.Value;
	}

	public void Run( )
	{
		for ( int i = 0; i < _subCount; i++ )
		{
			_subs[i] = _hub.Sub<MessageStruct>(HandleMessageStruct);
		}

		for ( int i = 0; i < _subCount; i++ )
		{
			_hub.Unsub(_subs[i]);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}{(_hub is SignalHubLocal ? "(Local)":"")}:s_{_subCount.ToString("e0")}";
}
}