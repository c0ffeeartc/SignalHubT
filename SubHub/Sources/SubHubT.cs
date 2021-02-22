using System;
using System.Collections.Generic;

namespace SubH
{
public partial class SubHub<T> : ISubHub<T>
	where T : IMessage, IPoolable
{
	public static			ISubHub<T>				I						= new SubHub<T>(  );
	private readonly		SortedList<ISubscription<T>,ISubscription<T>> _subscriptions	= new SortedList<ISubscription<T>, ISubscription<T>>();

	public					ISubscription<T>		Sub						( Action<T> action, int order = 0 )
	{
		var subscription			= Pool<ISubscription<T>>.I.Rent()
			.Init( false, null, action, order );
		AddSubscription( subscription );
		return subscription;
	}

	public					ISubscription<T>		Sub						( Object filter, Action<T> action, int order = 0 )
	{
		if ( filter == null )
		{
			throw new ArgumentNullException( "filter == null" );
		}

		var subscription			= Pool<ISubscription<T>>.I.Rent()
			.Init( true, filter, action, order );
		AddSubscription( subscription );
		return subscription;
	}

	private					void					AddSubscription			( ISubscription<T> subscription )
	{
		_subscriptions.Add( subscription, subscription );
	}

	public					void					Unsub					( ISubscription<T> subscription )
	{
		_subscriptions.Remove( subscription );
	}

	public					void					Publish					( T message )
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		if ( message.IsInPool )
		{
			throw new ArgumentException( "message.IsInPool" );
		}

		PublishInternal( null, message );
	}

	public					void					Publish					( Object filter, T message )
	{
		if (filter == null)
		{
			throw new ArgumentNullException( "filter == null" );
		}

		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		if ( message.IsInPool )
		{
			throw new ArgumentException( "message.IsInPool" );
		}

		PublishInternal( filter, message );
	}

	private					void					PublishInternal			( Object filter, T message )
	{
		for ( var i = 0; i < _subscriptions.Keys.Count; i++ )
		{
			var subscription		= _subscriptions.Keys[i];
			if ( subscription.HasFilter && subscription.Filter != filter )
			{
				continue;
			}

			subscription.Invoke( message );
		}

		Pool<T>.I.Repool( message );
	}

	public					void					UnsubAll				(  )
	{
		foreach ( var kv in _subscriptions )
		{
			Pool<ISubscription<T>>.I.Repool( kv.Key );
		}
		_subscriptions.Clear(  );
	}
}
}