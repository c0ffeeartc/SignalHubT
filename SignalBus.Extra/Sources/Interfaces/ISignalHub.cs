using System;

namespace SignalHubT
{
public interface ISignalHub
{
	ISubscription<T>		Sub<T>					( ActionRef<T> action, int order = 0 ) where T : ISignalData;
	ISubscription<T>		Sub<T>					( Object filter, ActionRef<T> action, int order = 0 ) where T : ISignalData;

	void					Unsub<T>				( ISubscription<T> subscription ) where T : ISignalData;
	void					UnsubAll				(  );

	T						Pub<T>					( T message ) where T : ISignalData;
	T						Pub<T>					( Object filter, T message ) where T : ISignalData;
}

public interface ISignalHubTests : ISignalHub
{
	ISignalBus<T>			GetSignalBus<T>			(  ) where T : ISignalData;
}
}
