using System;
using SubHubT;
using Tests;

namespace PerformanceTests
{
public class TestSubH_PubMessageStruct : IPerformanceTest, IToTestString
{
	public TestSubH_PubMessageStruct(Int32 iterations, Int32 subCount)
	{
		_iterations = iterations;
		_subCount = subCount;
	}

	private Int32 _iterations;
	private Int32 _value;
	private readonly Int32 _subCount;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		SubH.I = IoC.I.CreateSubH();

		for ( int i = 0; i < _subCount; i++ )
		{
			SubH.I.Sub<MessageStruct>(HandleMessageStruct);
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
			SubH.I.Pub(new MessageStruct(i));
		}
	}

	public String ToTestString( ) => $"{typeof(TestSubHLocal_PubMessageStruct)}:s_{_subCount}";
}
}