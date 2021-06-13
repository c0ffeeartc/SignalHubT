using System;
using SubHubT;

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
		SubH.I = IoC.I.CreateSubH();
	}

	public void Run( )
	{
		for ( int i = 0; i < _iterations; i++ )
		{
			SubH.I.Pub(_filter, new MessageStruct(i));
		}
	}
}
}