using System;
using SignalHubT;
using R = PerformanceTests.PerformanceTestRunner;

namespace PerformanceTests
{
internal class Program
{
	private const Int32 _1k = 1000;
	private const Int32 _10k = 10000;
	private const Int32 _100k = 100000;
	private const Int32 _1m = 1000000;

	public static void Main(string[] args)
	{
		MemoryHelper.MemoryBegin();
		ISignalHub signalHubStatic = IoCExtra.I.GetSignalHubStatic(  );

		R.Log("Main Tests");

		R.Run(()=>new TestPubMessageStruct_Cached_NoSub(signalHubStatic, _1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub(signalHubStatic, _1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub_Filter_String(signalHubStatic, _1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub_Filter_OverrideHashCode(signalHubStatic, _1m));

		R.Log("\nSignalHubStatic Pub with subs. Expected O(n)");
		R.Run(()=>new TestSubH_PubMessageStruct(signalHubStatic, _1m, 1));
		R.Run(()=>new TestSubH_PubMessageStruct(signalHubStatic, _1m, 1));

		R.Log("\nPub with subs. Expected O(n)");
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 1));
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 1)); // to confirm correct setup (didn't work with SignalHub.I)
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 2));
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 10));
		R.Run(()=>new TestSubHLocal_PubMessageStruct(_1m, 20));

		R.Log("\nSubUnsub");
		R.Run(()=>new TestSubHLocal_SubUnsub(_1m, 1));
		R.Run(()=>new TestSubHLocal_SubUnsub(_1k, _1k));
		R.Run(()=>new TestSubHLocal_SubUnsub(100, _10k));
		R.Run(()=>new TestSubHLocal_SubUnsub(10, _100k));
		R.Run(()=>new TestSubHLocal_SubUnsub(1, _1m));

		R.Log("\nSub all, Unsub all");
		R.Run(()=>new TestSubHLocal_SubAll_UnsubAll(1, _1k));
		R.Run(()=>new TestSubHLocal_SubAll_UnsubAll(1, _10k));
		R.Run(()=>new TestSubHLocal_SubAll_UnsubAll(1, _100k));
		R.Run(()=>new TestSubHLocal_SubAll_UnsubAll(1, _1m));

		R.Log("\nSub all");
		R.Run(()=>new TestSubHLocal_SubAll(1, _1k));
		R.Run(()=>new TestSubHLocal_SubAll(1, _10k));
		R.Run(()=>new TestSubHLocal_SubAll(1, _100k));
		R.Run(()=>new TestSubHLocal_SubAll(1, _1m));

		R.Log("\nUnsub all");
		R.Run(()=>new TestSubHLocal_UnsubAll(1, _1k));
		R.Run(()=>new TestSubHLocal_UnsubAll(1, _10k));
		R.Run(()=>new TestSubHLocal_UnsubAll(1, _100k));
		R.Run(()=>new TestSubHLocal_UnsubAll(1, _1m));

		R.Log("\nSortedList. Remove all");
		R.Run(()=>new TestSortedList_RemoveAll(1, _1k));
		R.Run(()=>new TestSortedList_RemoveAll(1, _10k));
		R.Run(()=>new TestSortedList_RemoveAll(1, _100k));

		MemoryHelper.MemoryEnd();
	}
}
}