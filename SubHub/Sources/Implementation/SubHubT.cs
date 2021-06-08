using System;
using System.Collections.Generic;
using System.Linq;
using C5;

namespace SubHubT
{
public partial class SubHub<T> : ISubHub<T>
		where T : IMessage
{
	// TODO:
	// - move all filter subs into Dictionary<filter, List<subscriptions>>
	// - move unfiltered subs into separate list, or if there is a good reason to Dictionary<staticFilter, List<subscriptions>>
	// - when pub unfiltered go through 1 unfiltered list
	// - when pub filtered go through 2 lists(filtered, unfiltered) by peeking next element and compare lower orderPriority
	//		- if orderPriority matches run Unfiltered first, then filtered subscriptions (should be decided which is better to be first)
	public static			ISubHub<T>				I						= IoC.I.CreateSubHub<T>(  );
	private					Int32					_publishActiveCount;
	private readonly		Queue<ISubscription<T>>	_unsubQue				= new Queue<ISubscription<T>>();
	private readonly	Dictionary<Object,TreeSet<ISubscription<T>>> _filterToSubscriptions	= new Dictionary<Object,TreeSet<ISubscription<T>>>();

	public					ISubscription<T>		Sub						( ActionRef<T> action, int order = 0 )
	{
		var subscription			= IoC.I.RentSubscription<T>(  )
			.Init( false, GlobalFilter.I, action, order );
		AddSubscription( GlobalFilter.I, subscription );
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
		AddSubscription( filter, subscription );
		return subscription;
	}

	private					void					AddSubscription			( Object filter, ISubscription<T> subscription )
	{
		var subscriptions			= GetOrAddSubscriptions( subscription.Filter );
		subscriptions.Add( subscription );
	}

	private				TreeSet<ISubscription<T>>	GetOrAddSubscriptions	( Object filter )
	{
		if ( !_filterToSubscriptions.TryGetValue( filter, out var subscriptions ) )
		{
			subscriptions			= new TreeSet<ISubscription<T>>(  );
			_filterToSubscriptions[filter] = subscriptions;
		}
		return subscriptions;
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
		_filterToSubscriptions[subscription.Filter].Remove( subscription );
		IoC.I.RepoolSubscription( subscription );
	}

	public					T						Pub						( T message )
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		return PublishInternal( GlobalFilter.I, message );
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

		PublishInternal( GlobalFilter.I, message );

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
		if (filter == GlobalFilter.I)
		{
			IterateGlobalMessage( ref message );
		}
		else
		{
			IterateFilteredMessage( filter, ref message );
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

	private					void					IterateGlobalMessage	( ref T message )
	{
		TreeSet<ISubscription<T>> subs = GetOrAddSubscriptions(GlobalFilter.I);

		for ( var i = 0; i < subs.Count; i++ )
		{
			var subscription		= subs[i];

			if ( subscription.CreationIndex == SubState.Inactive )
			{
				continue;
			}

			subscription.Invoke( ref message );

			// Ensure continue from same subscription if collection was prepended before current index
			while (subs[i] != subscription)
			{
				i++;
			}
		}
	}

	private					void					IterateFilteredMessage	( Object filter, ref T message )
	{
		TreeSet<ISubscription<T>> subsGlobal = GetOrAddSubscriptions( GlobalFilter.I );
		TreeSet<ISubscription<T>> subsFilter = GetOrAddSubscriptions( filter ); 

		var subsGlobalI				= 0;
		var subsFilterI				= 0;

		ref int subsI				= ref subsGlobalI;
		ref var subs				= ref subsGlobal;

		while ( subsGlobalI < subsGlobal.Count
			|| subsFilterI < subsFilter.Count )
		{
			Boolean curIsGlobal		= CalcCurSubIsGlobal( subsGlobal, subsGlobalI, subsFilter, subsFilterI );
			if ( curIsGlobal )
			{
				subs				= ref subsGlobal;
				subsI				= ref subsGlobalI;
			}
			else
			{
				subs				= ref subsFilter;
				subsI				= ref subsFilterI;
			}

			var subscription		= subs[subsI];

			if ( subscription.CreationIndex == SubState.Inactive )
			{
				continue;
			}

			subscription.Invoke( ref message );

			// Ensure continue from same subscription if collection was prepended before current index
			while (subs[subsI] != subscription)
			{
				subsI++;
			}

			subsI++;
		}
	}

	private					Boolean					CalcCurSubIsGlobal		(
			TreeSet<ISubscription<T>> treeGlobal
			, Int32 treeGlobalI
			, TreeSet<ISubscription<T>> treeFilter
			, Int32 treeFilterI
			)
	{
		if ( treeGlobalI >= treeGlobal.Count )
		{
			return false;
		}

		if ( treeFilterI >= treeFilter.Count )
		{
			return true;
		}

		var subGlobal = treeGlobal[treeGlobalI];
		var subFilter = treeFilter[treeFilterI];
		return subGlobal.Order <= subFilter.Order;
	}

	public					void					UnsubAll				(  )
	{
		foreach ( var kv_Filter_Subscriptions in _filterToSubscriptions )
		{
			TreeSet<ISubscription<T>> subscriptions = kv_Filter_Subscriptions.Value;
			for ( var i = _filterToSubscriptions.Count - 1; i >= 0; --i )
			{
				IoC.I.RepoolSubscription( subscriptions[i] );
			}
			subscriptions.Clear(  );
		}
	}
}

public partial class SubHub<T> : ISubHubTests<T>
		where T : IMessage
{
	public			List<ISubscription<T>>			GetSubscriptions	( Object filter )
	{
		return GetOrAddSubscriptions( filter ?? GlobalFilter.I )
			.ToList(  );
	}

	public					void					Sub					( ISubscription<T> subscription )
	{
		AddSubscription( subscription.Filter, subscription );
	}
}
}