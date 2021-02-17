using System;
using System.Collections;
using System.Collections.Generic;

namespace SubH
{
public partial class SubHub : ISubHub
{
	private readonly		IDictionary<Type, IList> _subscriptions			= new Dictionary<Type, IList>();

	public					void					Publish<TMessage>		( TMessage message ) where TMessage : IMessage
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		Type messageType			= typeof(TMessage);
		if ( !_subscriptions.ContainsKey( messageType ) )
		{
			return;
		}

		var subscriptionList		= _subscriptions[messageType];
		foreach( var subscription in subscriptionList )
		{
			((ISubscription<TMessage>) subscription).Action.Invoke( message );
		}
	}

	public					ISubscription<TMessage>	Sub<TMessage>			( Action<TMessage> action, int order = 0 ) where TMessage : IMessage
	{
		Type messageType			= typeof(TMessage);
		var subscription			= new Subscription<TMessage>( action, order );

		if( _subscriptions.ContainsKey( messageType ) )
		{
			_subscriptions[messageType].Add(subscription);
		}
		else
		{
			_subscriptions.Add(messageType, new List<ISubscription<TMessage>>{subscription});
		}

		return subscription;
	}

	public					void					Unsub<TMessage>			( ISubscription<TMessage> subscription ) where TMessage : IMessage
	{
		Type messageType			= typeof(TMessage);
		if ( _subscriptions.ContainsKey( messageType ) )
		{
			_subscriptions[messageType].Remove( subscription );
		}
	}

	public					void					ClearAllSubscriptions	(  )
	{
		_subscriptions.Clear(  );
	}
}
}