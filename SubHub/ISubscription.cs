using System;

namespace SubH
{
public interface ISubscription<TMessage> where TMessage : IMessage
{
	Int32					Order					{ get; }

	Action<TMessage>		Action					{ get; }
}
}