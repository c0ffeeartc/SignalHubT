using System;
using SignalBusT;

namespace PerformanceTests
{
public class TestPubMessageStruct_New_NoSub_Filter_OverrideHashCode : IPerformanceTest
{
	public TestPubMessageStruct_New_NoSub_Filter_OverrideHashCode(ISignalHub signalHub,Int32 iterations)
	{
		_signalHub = signalHub;
		_iterations = iterations;
	}

	private OverrideHashCode _filter = new OverrideHashCode(100);
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
			_signalHub.Pub(_filter, new MessageStruct(i));
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