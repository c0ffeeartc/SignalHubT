using System.Collections.Generic;

namespace SubH
{
public interface ISubHubTests<TMessage> : ISubHub<TMessage>
		where TMessage : IMessage
{
	List<ISubscription<TMessage>> GetSubscriptions	(  );
	void					Sub						( ISubscription<TMessage> subscription);
}
}