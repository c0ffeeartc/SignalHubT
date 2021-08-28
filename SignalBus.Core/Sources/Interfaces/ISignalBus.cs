using System;
using System.Collections.Generic;

namespace SignalHubT
{
public interface ISignalBus<T>
		: ISignalBus
		where T : ISignalData
{
	ISubscription<T>		Sub						( ActionRef<T> action, int order = 0 );
	ISubscription<T>		Sub						( Object filter, ActionRef<T> action, int order = 0 );

	void					Unsub					( ISubscription<T> subscription );

	T						Pub						( T message );
	T						Pub						( Object filter, T message );

	void					Publish<T2>				( T2 message ) where T2 : T, IPoolable, new();
	void					Publish<T2>				( Object filter, T2 message ) where T2 : T, IPoolable, new();
}

public interface ISignalBus
{
	void					UnsubAll				(  );
}

public interface ISignalBusTests<T> : ISignalBus<T>
		where T : ISignalData
{
	List<ISubscription<T>>	GetSubscriptions		( Object filter );
	void					Sub						( ISubscription<T> subscription);
}
}