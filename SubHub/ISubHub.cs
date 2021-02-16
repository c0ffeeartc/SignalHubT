using System;

namespace SubHub
{
public interface ISubHub
{
	ISubscription<TMessage>	Sub<TMessage>			( Action<TMessage> action ) where TMessage : IMessage;
	void					Unsub<TMessage>			( ISubscription<TMessage> subscription ) where TMessage : IMessage;

	void					Publish<TMessage>		( TMessage message ) where TMessage : IMessage;

	void					ClearAllSubscriptions	(  );
}
}