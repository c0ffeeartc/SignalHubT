using System;

namespace SubH
{
public interface ISubscription<TMessage> : IComparable<ISubscription<TMessage>> // IComparable for Order
		where TMessage : IMessage
{
	Int32					Order					{ get; }
	Boolean					HasFilter				{ get; }
	Object 					Filter					{ get; }
	void 					Invoke					( TMessage message );
}
}