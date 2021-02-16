using System;

namespace SubHub
{
public interface ISubscription<TMessage> where TMessage : IMessage
{
	Action<TMessage>		Action					{ get; }
}
}