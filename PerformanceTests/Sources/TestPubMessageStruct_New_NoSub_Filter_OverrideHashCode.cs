using System;
using SubHubT;

namespace PerformanceTests
{
public class TestPubMessageStruct_New_NoSub_Filter_OverrideHashCode : IPerformanceTest
{
	public TestPubMessageStruct_New_NoSub_Filter_OverrideHashCode(Int32 iterations)
	{
		_iterations = iterations;
	}

	private OverrideHashCode _filter = new OverrideHashCode(100);
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

	private class OverrideHashCode
	{
		public OverrideHashCode( Int32 value )
		{
			_value = value;
		}

		private readonly Int32 _value;
		public override Int32 GetHashCode( ) => _value;
	}
}
}