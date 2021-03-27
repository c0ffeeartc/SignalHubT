using System;
using System.Collections.Generic;
using SubHubT;
using Tests;

namespace PerformanceTests
{
public class TestSortedList_RemoveAll : IPerformanceTest, IToTestString
{
	public TestSortedList_RemoveAll(Int32 iterations, Int32 itemCount)
	{
		_iterations = iterations;
		_itemCount = itemCount;
	}

	private Int32 _iterations;
	private readonly Int32 _itemCount;
	private SortedList<Int32,Int32> _list;
	public Int32 Iterations => _iterations;

	public void Before( )
	{
		_list = new SortedList<Int32,Int32>(_itemCount);
		for ( int i = 0; i < _itemCount; i++ )
		{
			_list.Add(i, i);
		}
	}

	public void Run( )
	{
		for ( int i = 0; i < _itemCount; i++ )
		{
			_list.Remove(i);
		}
	}

	public String ToTestString( ) => $"{GetType().Name}:c_{_itemCount.ToString("e0")}";
}
}