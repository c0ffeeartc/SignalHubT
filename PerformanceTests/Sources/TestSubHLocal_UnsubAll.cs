using System;
using SubHubT;
using Tests;

namespace PerformanceTests
{
public class TestSubHLocal_UnsubAll : IPerformanceTest, IToTestString
		, IListen<MessageStruct>
{
	public TestSubHLocal_UnsubAll(Int32 iterations, Int32 subCount)
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
		SubH.I = IoC.I.CreateSubHLocal();
		_subs = new ISubscription<MessageStruct>[_subCount];
		for ( int i = 0; i < _subCount; i++ )
		{
			_subs[i] = SubH.I.Sub<MessageStruct>(this);
		}
	}

	void IListen<MessageStruct>.Handle( ISubscription<MessageStruct> subscription, ref MessageStruct message )
	{
		_value = message.Value;
	}

	public void Run( )
	{
		for ( int i = 0; i < _subCount; i++ )
		{
			SubH.I.Unsub(_subs[i]);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount.ToString("e0")}";
}
}