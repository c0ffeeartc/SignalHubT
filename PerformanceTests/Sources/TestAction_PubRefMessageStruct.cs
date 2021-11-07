using System;
using SignalHubT;
using Tests;

namespace PerformanceTests
{
public class TestAction_PubRefMessageStruct: IPerformanceTest, IToTestString
{
	public TestAction_PubRefMessageStruct(Int32 iterations, Int32 subCount)
	{
		_iterations = iterations;
		_subCount = subCount;
	}

	private Int32 _iterations;
	private Int32 _value;
	private ActionRef<MessageStruct> _action;
	private readonly Int32 _subCount;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		for ( int i = 0; i < _subCount; i++ )
		{
			_action += HandleMessageStruct;
		}
	}

	private void HandleMessageStruct(ref MessageStruct message)
	{
		_value = message.Value;
	}

	public void Run( )
	{
		for ( int i = 0; i < _iterations; i++ )
		{
			var message = new MessageStruct(i);
			_action.Invoke(ref message);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:s_{_subCount}";
}
}