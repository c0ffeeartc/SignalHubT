using System;

namespace SubHubT
{
public delegate void ActionRef<T> (ref T obj);

public interface ISubscription<T>
		: IComparable<ISubscription<T>>  // IComparable for Order
		, IPoolable
		where T : IMessage
{
	Int32					Order					{ get; }  // lower values get invoked before higher values
	Boolean					HasFilter				{ get; }
	Object 					Filter					{ get; }
	Int32 					CreationIndex			{ get; set; }

	void 					Invoke					( ref T message );
	ISubscription<T>		Init					(
			Boolean hasFilter
			, Object filter
			, ActionRef<T> action
			, Int32 order
			);

	Int32 					CompareToInternal		( ISubscription<T> other );
}
public static class SubState
{
	public const			Int32					Inactive				= -1;  // Consider distinguish inactive from waitForUnsub
}
}