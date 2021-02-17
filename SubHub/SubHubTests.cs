using System;
using System.Collections.Generic;
using System.Linq;

namespace SubH
{
public partial class SubHub<TMessage> : ISubHubTests<TMessage>
		where TMessage : IMessage
{
	public			List<ISubscription<TMessage>>	GetSubscriptions	(  )
	{
		return _subscriptions;
	}
}
}