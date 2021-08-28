using System;
using SignalBusT;

namespace PerformanceTests
{
public class TestPubMessageStruct_New_NoSub : IPerformanceTest
{
	public TestPubMessageStruct_New_NoSub(ISignalHub signalHub, Int32 iterations)
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
		for ( int i = 0; i < _iterations; i++ )
		{
			_signalHub.Pub(new MessageStruct(i));
		}
	}
}
}