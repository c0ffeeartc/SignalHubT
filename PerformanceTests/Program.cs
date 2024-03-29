﻿using System;
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
		Func<ISignalHub> createHubLocal = IoCExtra.I.CreateSignalHubLocal;

		R.Log("Main Tests");

		R.Run(()=>new TestPubMessageStruct_Cached_NoSub(signalHubStatic, _1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub(signalHubStatic, _1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub_Filter_String(signalHubStatic, _1m));
		R.Run(()=>new TestPubMessageStruct_New_NoSub_Filter_OverrideHashCode(signalHubStatic, _1m));

		R.Log("\nAction Invoke");
		R.Run(()=>new TestAction_PubRefMessageStruct(_1m, 1));
		R.Run(()=>new TestAction_PubRefMessageStruct(_1m, 1));
		R.Run(()=>new TestAction_PubRefMessageStruct(_1m, 2));
		R.Run(()=>new TestAction_PubRefMessageStruct(_1m, 10));
		R.Run(()=>new TestAction_PubRefMessageStruct(_1m, 20));
		R.Run(()=>new TestAction_PubRefMessageStruct(_1m, 40));

		R.Log("\nCompare to Action Invoke. Expected O(n). (Local)");
		R.Run(()=>new TestHub_PubMessageStruct(createHubLocal(), _1m, 1));
		R.Run(()=>new TestHub_PubMessageStruct(createHubLocal(), _1m, 1));
		R.Run(()=>new TestHub_PubMessageStruct(createHubLocal(), _1m, 2));
		R.Run(()=>new TestHub_PubMessageStruct(createHubLocal(), _1m, 10));
		R.Run(()=>new TestHub_PubMessageStruct(createHubLocal(), _1m, 20));
		R.Run(()=>new TestHub_PubMessageStruct(createHubLocal(), _1m, 40));

		R.Log("\nCompare to Action Invoke. Expected O(n). (Static)");
		R.Run(()=>new TestHub_PubMessageStruct(signalHubStatic, _1m, 1));
		R.Run(()=>new TestHub_PubMessageStruct(signalHubStatic, _1m, 1)); // to confirm correct setup (didn't work with signalHubStatic)
		R.Run(()=>new TestHub_PubMessageStruct(signalHubStatic, _1m, 2));
		R.Run(()=>new TestHub_PubMessageStruct(signalHubStatic, _1m, 10));
		R.Run(()=>new TestHub_PubMessageStruct(signalHubStatic, _1m, 20));
		R.Run(()=>new TestHub_PubMessageStruct(signalHubStatic, _1m, 40));

		R.Log("\nSubUnsub");
		R.Run(()=>new TestSubHLocal_SubUnsub(_1m, 1));
		R.Run(()=>new TestSubHLocal_SubUnsub(_1k, _1k));
		R.Run(()=>new TestSubHLocal_SubUnsub(100, _10k));
		R.Run(()=>new TestSubHLocal_SubUnsub(10, _100k));
		R.Run(()=>new TestSubHLocal_SubUnsub(1, _1m));

		R.Log("\nSub all, Unsub all. (Local)");
		R.Run(()=>new TestHub_SubAll_UnsubAll(createHubLocal(), 1, _1k));
		R.Run(()=>new TestHub_SubAll_UnsubAll(createHubLocal(), 1, _10k));
		R.Run(()=>new TestHub_SubAll_UnsubAll(createHubLocal(), 1, _100k));
		R.Run(()=>new TestHub_SubAll_UnsubAll(createHubLocal(), 1, _1m));

		R.Log("\nSub all, Unsub all. (Static)");
		R.Run(()=>new TestHub_SubAll_UnsubAll(signalHubStatic, 1, _1k));
		R.Run(()=>new TestHub_SubAll_UnsubAll(signalHubStatic, 1, _10k));
		R.Run(()=>new TestHub_SubAll_UnsubAll(signalHubStatic, 1, _100k));
		R.Run(()=>new TestHub_SubAll_UnsubAll(signalHubStatic, 1, _1m));

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