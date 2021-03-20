using System;
using SubHubT;

namespace PerformanceTests
{
public class TestPubMessageStruct_New_NoSub : IPerformanceTest
{
	public TestPubMessageStruct_New_NoSub(Int32 iterations)
	{
		_iterations = iterations;
	}

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
			SubH.I.Pub(new MessageStruct(i));
		}
	}
}
}