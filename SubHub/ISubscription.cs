using System;

namespace SubH
{
public interface ISubscription<TMessage>
		where TMessage : IMessage
{
	Int32					Order					{ get; }
	Boolean					HasFilter				{ get; }
	Object 					Filter					{ get; }
	Action<TMessage>		Action					{ get; }
}
}