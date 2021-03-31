using System;

namespace SubHubT
{
public interface IListen<T> where T : IMessage
{
	 void					Handle					( ISubscription<T> subscription, ref T message );
}

public interface ISubscription<T>
		: IComparable<ISubscription<T>>  // IComparable for Order
		, IPoolable
		where T : IMessage
{
	Int32					Order					{ get; }  // lower values get invoked before higher values
	Boolean					HasFilter				{ get; }
	Object 					Filter					{ get; }
	Int32 					CreationIndex			{ get; set; }

	void 					Invoke					( ISubscription<T> subscription, ref T message );
	ISubscription<T>		Init					(
			Boolean hasFilter
			, Object filter
			, IListen<T> action
			, Int32 order
			);
}

public static class SubState
{
	public const			Int32					Inactive				= -1;  // Consider distinguish inactive from waitForUnsub
	public const			String					GlobalFilter			= "GlobalFilter";
}
}