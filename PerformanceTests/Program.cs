using System;
using R = PerformanceTests.PerformanceTestRunner;

namespace PerformanceTests
{
internal class Program
{
	private const Int32 _1m = 1000000;

	public static void Main(string[] args)
	{
		MemoryHelper.MemoryBegin();

		R.Log("Main Tests");

		R.Run(()=>new TestPubMessageStruct_Cached_NoSub(_1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub(_1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub_Filter(_1m));

		MemoryHelper.MemoryEnd();
	}
}
}