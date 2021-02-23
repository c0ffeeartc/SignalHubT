using System;

namespace SubHubT
{
public interface ISubscription<T>
		: IComparable<ISubscription<T>>  // IComparable for Order
		, IPoolable
		where T : IMessage
{
	Int32					Order					{ get; }  // lower values get invoked before higher values
	Boolean					HasFilter				{ get; }
	Object 					Filter					{ get; }
	Int32 					CreationIndex			{ get; set; }

	void 					Invoke					( T message );
	ISubscription<T>		Init					(
			Boolean hasFilter
			, Object filter
			, Action<T> action
			, Int32 order
			);
}
public static class SubState
{
	public const			Int32					Inactive				= -1;  // Consider distinguish inactive from waitForUnsub
}
}