using System;

namespace SubH
{
public interface ISubscription<T>
		: IComparable<ISubscription<T>>  // IComparable for Order
		, IPoolable
		where T : IMessage
{
	Int32					Order					{ get; }
	Boolean					HasFilter				{ get; }
	Object 					Filter					{ get; }

	void 					Invoke					( T message );
	ISubscription<T>		Init					(
			Boolean hasFilter
			, Object filter
			, Action<T> action
			, Int32 order
			);
}
}