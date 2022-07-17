using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SignalHubT
{
public partial class SignalHubLocal : ISignalHub
{
	private				Dictionary<Type, ISignalBus>_signalBuses			= new Dictionary<Type, ISignalBus>();

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
		foreach ( var kv in _signalBuses )
		{
			kv.Value.UnsubAll();
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

	private					ISignalBus<T>			GetOrCreateSignalBus<T>	(  )
			where T : ISignalData
	{
		var messageType				= typeof(T);
		if ( _signalBuses.TryGetValue( messageType, out ISignalBus obj ) )
		{
			return Unsafe.As<ISignalBus<T>>(obj);
		}

		ISignalBus<T> signalBus		= IoC.I.CreateSignalBus<T>(  );
		_signalBuses[messageType]		= signalBus;
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