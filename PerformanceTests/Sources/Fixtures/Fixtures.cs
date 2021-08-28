using System;
using SubHubT;

namespace PerformanceTests
{
public struct MessageStruct : ISignalData
{
	public MessageStruct(Int32 value )
	{
		Value = value;
	}

	public Int32 Value;
}
}