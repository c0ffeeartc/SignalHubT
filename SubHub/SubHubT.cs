using System;
using System.Collections.Generic;

namespace SubH
{
public partial class SubHub<TMessage> : ISubHub<TMessage>
	where TMessage : IMessage
{
	public static			ISubHub<TMessage>		I						= new SubHub<TMessage>(  );
	private readonly		SortedDictionary<ISubscription<TMessage>,ISubscription<TMessage>>	_subscriptions	= new SortedDictionary<ISubscription<TMessage>, ISubscription<TMessage>>();

	public					ISubscription<TMessage>	Sub						( Action<TMessage> action, int order = 0 )
	{
		var subscription			= new Subscription<TMessage>( false, null, action, order );
		AddSubscription( subscription );
		return subscription;
	}

	public					ISubscription<TMessage>	Sub						( Object filter, Action<TMessage> action, int order = 0 )
	{
		if ( filter == null )
		{
			throw new ArgumentNullException( "filter == null" );
		}

		var subscription			= new Subscription<TMessage>( true, filter, action, order );
		AddSubscription( subscription );
		return subscription;
	}

	private					void					AddSubscription			( ISubscription<TMessage> subscription )
	{
		_subscriptions.Add( subscription, subscription );
	}

	public					void					Unsub					( ISubscription<TMessage> subscription )
	{
		_subscriptions.Remove( subscription );
	}

	public					void					Publish					( TMessage message )
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		foreach( var kv in _subscriptions )
		{
			if ( kv.Value.HasFilter )
			{
				continue;
			}

			kv.Value.Invoke( message );
		}
	}

	public					void					Publish					( Object filter, TMessage message )
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		foreach( var kv in _subscriptions )
		{
			if ( kv.Value.HasFilter && kv.Value.Filter != filter )
			{
				continue;
			}

			kv.Value.Invoke( message );
		}
	}

	public					void					ClearAllSubscriptions	(  )
	{
		_subscriptions.Clear(  );
	}
}
}