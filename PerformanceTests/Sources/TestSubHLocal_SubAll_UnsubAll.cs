using System;
using SignalBusT;
using Tests;

namespace PerformanceTests
{
public class TestSubHLocal_SubAll_UnsubAll : IPerformanceTest, IToTestString
{
	public TestSubHLocal_SubAll_UnsubAll(Int32 iterations, Int32 subCount)
	{
		_iterations = iterations;
		_subCount = subCount;
	}

	private Int32 _iterations;
	private Int32 _value;
	private readonly Int32 _subCount;
	private ISubscription<MessageStruct>[] _subs;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		SignalHub.I = IoCExtra.I.CreateSignalHubLocal();
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
			_subs[i] = SignalHub.I.Sub<MessageStruct>(HandleMessageStruct);
		}

		for ( int i = 0; i < _subCount; i++ )
		{
			SignalHub.I.Unsub(_subs[i]);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount.ToString("e0")}";
}
}