using System;
using SignalBusT;

namespace PerformanceTests
{
public class TestPubMessageStruct_New_NoSub_Filter_String : IPerformanceTest
{
	public TestPubMessageStruct_New_NoSub_Filter_String(Int32 iterations)
	{
		_iterations = iterations;
	}

	private string _filter = "filter";
	private Int32 _iterations;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		SignalHub.I = IoCExtra.I.CreateSignalHub();
	}

	public void Run( )
	{
		for ( int i = 0; i < _iterations; i++ )
		{
			SignalHub.I.Pub(_filter, new MessageStruct(i));
		}
	}
}
}