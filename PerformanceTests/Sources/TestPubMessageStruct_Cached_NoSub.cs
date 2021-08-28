using System;
using SignalHubT;

namespace PerformanceTests
{
public class TestPubMessageStruct_Cached_NoSub : IPerformanceTest
{
	public TestPubMessageStruct_Cached_NoSub(ISignalHub signalHub,Int32 iterations)
	{
		_signalHub = signalHub;
		_iterations = iterations;
	}

	private Int32 _iterations;
	private ISignalHub _signalHub;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		_signalHub.UnsubAll();
	}

	public void Run( )
	{
		var message = new MessageStruct(5);
		for ( int i = 0; i < _iterations; i++ )
		{
			_signalHub.Pub(message);
		}
	}
}
}