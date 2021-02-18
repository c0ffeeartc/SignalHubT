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
		return _subscriptions
			.Select( kv => kv.Value )
			.ToList(  );
	}

	public					void					Sub					( ISubscription<TMessage> subscription )
	{
		AddSubscription( subscription );
	}
}
}