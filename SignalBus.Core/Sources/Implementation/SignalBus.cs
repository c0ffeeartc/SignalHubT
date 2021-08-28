using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using C5;

namespace SignalBusT
{
public partial class SignalBus<T> : ISignalBus<T>
		where T : ISignalData
{
	public					SignalBus				(  )
	{
		_subscriptionsGlobal		= GetOrAddSubscriptions( GlobalFilter.I );
	}

	public static			ISignalBus<T>			I						= IoC.I.CreateSignalBus<T>(  );
	private					Int32					_publishActiveCount;
	private readonly		Queue<ISubscription<T>>	_unsubQue				= new Queue<ISubscription<T>>();
	private readonly	Dictionary<Object,TreeSet<ISubscription<T>>> _filterToSubscriptions	= new Dictionary<Object,TreeSet<ISubscription<T>>>();
	private readonly		TreeSet<ISubscription<T>>_subscriptionsGlobal;

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

		BeforePublish();
		IterateGlobalMessage(ref message);
		AfterPublish();

		return message;
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

		BeforePublish();
		IterateFilteredMessage(filter, ref message);
		AfterPublish();

		return message;
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

		T m = message;
		BeforePublish();
		IterateGlobalMessage( ref m );
		AfterPublish();

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

		T m = message;
		BeforePublish();
		IterateFilteredMessage( filter, ref m );
		AfterPublish();

		IoC.I.Repool( message );
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private					void					BeforePublish			(  )
	{
		++_publishActiveCount;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private					void					AfterPublish			(  )
	{
		--_publishActiveCount;

		if ( _publishActiveCount == 0 )
		{  // SortedList.Remove complexity N_Unsubs * M_ItemsInCollection :( . Any way to RemoveAll(predicate)?
			while ( _unsubQue.Count > 0 )
			{
				UnsubInternal( _unsubQue.Dequeue(  ) );
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private					void					IterateGlobalMessage	( ref T message )
	{
		TreeSet<ISubscription<T>> subs = _subscriptionsGlobal;

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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private					void					IterateFilteredMessage	( Object filter, ref T message )
	{
		TreeSet<ISubscription<T>> subsGlobal = _subscriptionsGlobal;
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

		ISubscription<T> subGlobal = treeGlobal[treeGlobalI];
		ISubscription<T> subFilter = treeFilter[treeFilterI];
		return subGlobal.CompareToInternal(subFilter) < 0;
	}

	public					void					UnsubAll				(  )
	{
		foreach ( var kv_Filter_Subscriptions in _filterToSubscriptions )
		{
			TreeSet<ISubscription<T>> subscriptions = kv_Filter_Subscriptions.Value;
			for ( var i = subscriptions.Count - 1; i >= 0; --i )
			{
				IoC.I.RepoolSubscription( subscriptions[i] );
			}
			subscriptions.Clear(  );
		}
	}
}

public partial class SignalBus<T> : ISignalBusTests<T>
		where T : ISignalData
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