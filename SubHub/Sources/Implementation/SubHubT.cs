using System;
using System.Collections.Generic;
using System.Linq;
using C5;

namespace SubHubT
{
public partial class SubHub<T> : ISubHub<T>
		where T : IMessage
{
	public static			ISubHub<T>				I						= IoC.I.CreateSubHub<T>(  );
	private					Int32					_publishActiveCount;
	private readonly		Queue<ISubscription<T>>	_unsubQue				= new Queue<ISubscription<T>>();
	private readonly		TreeSet<ISubscription<T>> _subscriptions		= new TreeSet<ISubscription<T>>();

	public					ISubscription<T>		Sub						( ActionRef<T> action, int order = 0 )
	{
		var subscription			= IoC.I.RentSubscription<T>(  )
			.Init( false, null, action, order );
		AddSubscription( subscription );
		return subscription;
	}

	public					ISubscription<T>		Sub						( Object filter, ActionRef<T> action, int order = 0 )
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
		_subscriptions.Add( subscription );
	}

	public					void					Unsub					( ISubscription<T> subscription )
	{
		if ( _publishActiveCount > 0 )
		{
			subscription.CreationIndex	= SubState.Inactive;
			_unsubQue.Enqueue( subscription );
			return;
		}

		UnsubInternal( subscription );
	}

	private					void					UnsubInternal			( ISubscription<T> subscription )
	{
		IoC.I.RepoolSubscription( subscription );
		_subscriptions.Remove( subscription );
	}

	public					T						Pub						( T message )
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		return PublishInternal( null, message );
	}

	public					T						Pub						( Object filter, T message )
	{
		if (filter == null)
		{
			throw new ArgumentNullException( "filter == null" );
		}

		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		return PublishInternal( filter, message );
	}

	public					void					Publish<T2>				( T2 message )
			where T2 : T, IPoolable, new()
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

		IoC.I.Repool( message );
	}

	public					void					Publish<T2>					( Object filter, T2 message )
			where T2 : T, IPoolable, new()
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

		IoC.I.Repool( message );
	}

	private					T						PublishInternal			( Object filter, T message )
	{
		++_publishActiveCount;
		for ( var i = 0; i < _subscriptions.Count; i++ )
		{
			var subscription		= _subscriptions[i];
			if ( subscription.HasFilter
				&& subscription.Filter != filter )
			{
				continue;
			}

			if ( subscription.CreationIndex == SubState.Inactive )
			{
				continue;
			}

			subscription.Invoke( ref message );
			// Ensure continue from same subscription if collection was prepended before current index
			while (_subscriptions[i] != subscription)
			{
				i++;
			}
		}
		--_publishActiveCount;

		if ( _publishActiveCount == 0 )
		{  // SortedList.Remove complexity N_Unsubs * M_ItemsInCollection :( . Any way to RemoveAll(predicate)?
			while ( _unsubQue.Count > 0 )
			{
				UnsubInternal( _unsubQue.Dequeue(  ) );
			}
		}

		return message;
	}

	public					void					UnsubAll				(  )
	{
		for ( var i = _subscriptions.Count - 1; i >= 0; --i )
		{
			IoC.I.RepoolSubscription( _subscriptions[i] );
		}
		_subscriptions.Clear(  );
	}
}

public partial class SubHub<T> : ISubHubTests<T>
		where T : IMessage
{
	public			List<ISubscription<T>>			GetSubscriptions	(  )
	{
		return _subscriptions
			.ToList(  );
	}

	public					void					Sub					( ISubscription<T> subscription )
	{
		AddSubscription( subscription );
	}
}
}