using System;

namespace SubH
{
public interface ISubHub
{
	ISubscription<TMessage>	Sub<TMessage>			( Action<TMessage> action, int order = 0 ) where TMessage : IMessage;
	void					Unsub<TMessage>			( ISubscription<TMessage> subscription ) where TMessage : IMessage;

	void					Publish<TMessage>		( TMessage message ) where TMessage : IMessage;

	void					ClearAllSubscriptions	(  );
}
}