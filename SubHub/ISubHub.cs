using System;

namespace SubH
{
public interface ISubHub<TMessage>
		where TMessage : IMessage
{
	ISubscription<TMessage>	Sub						( Action<TMessage> action, int order = 0 );
	ISubscription<TMessage>	Sub						( Object filter, Action<TMessage> action, int order = 0 );

	void					Unsub					( ISubscription<TMessage> subscription );

	void					Publish					( TMessage message );

	void					ClearAllSubscriptions	(  );
}
}