using System;
using System.Collections.Generic;

namespace SignalHubT
{
// Facade for an arguably better API
public partial class SignalHub : ISignalHub
{
	public static			ISignalHub				I						= IoCExtra.I.GetSignalHubStatic(  );
	private				HashSet<ISignalBus>			_tracked				= new HashSet<ISignalBus>();

	public					ISubscription<T>		Sub<T>					( ActionRef<T> action, Int32 order = 0 )
			where T : ISignalData
	{
		var signalBus = SignalBus<T>.I;
		Track(signalBus);
		return signalBus.Sub( action, order );
	}

	public					ISubscription<T>		Sub<T>					( Object filter, ActionRef<T> action, Int32 order=0 )
			where T : ISignalData
	{
		var signalBus = SignalBus<T>.I;
		Track(signalBus);
		return signalBus.Sub( filter, action, order );
	}

	public					void					Unsub<T>				( ISubscription<T> subscription )
			where T : ISignalData
	{
		SignalBus<T>.I.Unsub(subscription);
	}

	public					void					UnsubAll				(  )
	{
		foreach ( ISignalBus signalBus in _tracked )
		{
			signalBus.UnsubAll(  );
		}
	}

	public					T						Pub<T>					( T message )
			where T: ISignalData
	{
		return SignalBus<T>.I.Pub(message);
	}

	public					T						Pub<T>					( Object filter, T message )
			where T : ISignalData
	{
		return SignalBus<T>.I.Pub(filter, message);
	}

	private					void					Track					( ISignalBus signalBus )
	{
		_tracked.Add(signalBus);
	}
}

public partial class SignalHub : ISignalHubTests
{
	public					ISignalBus<T>			GetSignalBus<T>			(  )
			where T : ISignalData
	{
		return SignalBus<T>.I;
	}
}
}
