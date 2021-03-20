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

		R.Log("\nPub with subs. Expected O(n)");
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 1));
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 1)); // to confirm correct setup (didn't work with SubH.I)
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 2));
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 10));
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 20));

		R.Log("\nFIXME: SubH Pub with subs. Expected O(n)");
		R.Run(()=>new TestSubH_PubMessageStruct(_1m, 1));
		R.Run(()=>new TestSubH_PubMessageStruct(_1m, 1));

		MemoryHelper.MemoryEnd();
	}
}
}