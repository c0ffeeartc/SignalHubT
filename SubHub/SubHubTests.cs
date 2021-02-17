using System;
using System.Collections.Generic;
using System.Linq;

namespace SubH
{
public partial class SubHub : ISubHubTests
{
	public			List<ISubscription<TMessage>>	Tests_GetFor<TMessage>	( ) where TMessage : IMessage
	{
		var subs 					= new List<ISubscription<TMessage>>();
		Type messageType			= typeof(TMessage);
		if ( !_subscriptions.ContainsKey( messageType ) )
		{
			return subs;
		}

		subs.AddRange( _subscriptions[messageType].Cast<ISubscription<TMessage>>() );
		return subs;
	}
}
}