using System;
using System.Collections.Generic;

namespace SignalBusT
{
public partial class SignalHubLocal : ISignalHub
{
	private				Dictionary<Type, Object>	_subHubTs				= new Dictionary<Type, Object>();

	public					ISubscription<T>		Sub<T>					( ActionRef<T> action, Int32 order = 0 )
			where T : ISignalData
	{
		return GetOrCreateSignalBus<T>(  ).Sub( action, order );
	}

	public					ISubscription<T>		Sub<T>					( Object filter, ActionRef<T> action, Int32 order = 0 )
			where T : ISignalData
	{
		return GetOrCreateSignalBus<T>(  ).Sub( filter, action, order );
	}

	public					void					Unsub<T>				( ISubscription<T> subscription )
			where T : ISignalData
	{
		GetOrCreateSignalBus<T>(  ).Unsub( subscription );
	}

	public					void					UnsubAll				(  )
	{
		foreach ( var kv in _subHubTs)
		{
			(kv.Value as ISignalBus).UnsubAll();
		}
	}

	public					T						Pub<T>					( T message )
			where T : ISignalData
	{
		return GetOrCreateSignalBus<T>(  ).Pub( message );
	}

	public					T						Pub<T>					( Object filter, T message )
			where T : ISignalData
	{
		return GetOrCreateSignalBus<T>(  ).Pub( filter, message );
	}

	public					void					Publish<T>				( T message )
			where T : ISignalData, IPoolable, new()
	{
		GetOrCreateSignalBus<T>(  ).Publish( message );
	}

	public					void					Publish<T>				( Object filter, T message )
			where T : ISignalData, IPoolable, new()
	{
		GetOrCreateSignalBus<T>(  ).Publish( filter, message );
	}

	public					T						Args<T>					(  )
			where T : ISignalData, IPoolable, new()
	{
		return IoC.I.Rent<T>(  );
	}

	private					ISignalBus<T>			GetOrCreateSignalBus<T>	(  )
			where T : ISignalData
	{
		var messageType				= typeof(T);
		if ( _subHubTs.TryGetValue( messageType, out Object obj ) )
		{
			return (ISignalBus<T>)obj;
		}

		ISignalBus<T> signalBus		= IoC.I.CreateSignalBus<T>(  );
		_subHubTs[messageType]		= signalBus;
		return signalBus;
	}
}

public partial class SignalHubLocal : ISignalHubTests
{
	public					ISignalBus<T>			GetSignalBus<T>			(  )
			where T : ISignalData
	{
		return GetOrCreateSignalBus<T>(  );
	}
}
}