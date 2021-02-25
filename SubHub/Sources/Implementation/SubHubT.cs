using System;
using System.Collections.Generic;
using System.Linq;

namespace SubHubT
{
public partial class SubHub<T> : ISubHub<T>
	where T : IMessage, IPoolable, new( )
{
	public static			ISubHub<T>				I						= IoC.I.CreateSubHub<T>(  );
	private					Int32					_publishActiveCount;
	private					Boolean					_isWaitingUnsub;
	private readonly		SortedList<ISubscription<T>,ISubscription<T>> _subscriptions	= new SortedList<ISubscription<T>, ISubscription<T>>();

	public					ISubscription<T>		Sub						( Action<T> action, int order = 0 )
	{
		var subscription			= IoC.I.RentSubscription<T>(  )
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

		var subscription			= IoC.I.RentSubscription<T>(  )
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
		if ( _publishActiveCount > 0 )
		{
			_isWaitingUnsub				= true;
			subscription.CreationIndex	= SubState.Inactive;
			return;
		}

		IoC.I.RepoolSubscription( subscription );
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
		++_publishActiveCount;
		for ( var i = 0; i < _subscriptions.Keys.Count; i++ )
		{
			var subscription		= _subscriptions.Keys[i];
			if ( subscription.HasFilter
				&& subscription.Filter != filter )
			{
				continue;
			}

			if ( subscription.CreationIndex == SubState.Inactive )
			{
				continue;
			}

			subscription.Invoke( message );
			// Ensure continue from same subscription if collection was prepended before current index
			while (_subscriptions.Keys[i] != subscription)
			{
				i++;
			}
		}
		--_publishActiveCount;

		if ( _publishActiveCount == 0
			&& _isWaitingUnsub )
		{  // complexity N_Unsubs * M_ItemsInCollection :( . Any way to RemoveAll(predicate)?
			_isWaitingUnsub			= false;
			for ( var i = _subscriptions.Count - 1; i >= 0 ;--i )
			{
				if ( _subscriptions.Keys[i].CreationIndex == SubState.Inactive )
				{
					_subscriptions.RemoveAt( i );
				}
			}
		}

		IoC.I.Repool( message );
	}

	public					void					UnsubAll				(  )
	{
		for ( var i = _subscriptions.Keys.Count - 1; i >= 0; --i )
		{
			IoC.I.RepoolSubscription( _subscriptions.Keys[i] );
		}
		_subscriptions.Clear(  );
	}
}

public partial class SubHub<T> : ISubHubTests<T> where T : IMessage, IPoolable, new( )
{
	public			List<ISubscription<T>>			GetSubscriptions	(  )
	{
		return _subscriptions
			.Select( kv => kv.Value )
			.ToList(  );
	}

	public					void					Sub					( ISubscription<T> subscription )
	{
		AddSubscription( subscription );
	}
}
}