using System;
using System.Collections.Generic;

namespace SubHubT
{
public interface ISubHub<T>
		where T : IMessage
{
	ISubscription<T>		Sub						( ActionRef<T> action, int order = 0 );
	ISubscription<T>		Sub						( Object filter, ActionRef<T> action, int order = 0 );

	void					Unsub					( ISubscription<T> subscription );

	T						Pub						( T message );
	T						Pub						( Object filter, T message );

	void					Publish<T2>				( T2 message ) where T2 : T, IPoolable, new();
	void					Publish<T2>				( Object filter, T2 message ) where T2 : T, IPoolable, new();

	void					UnsubAll				(  );
}

public interface ISubHubTests<T> : ISubHub<T>
		where T : IMessage
{
	List<ISubscription<T>>	GetSubscriptions		(  );
	void					Sub						( ISubscription<T> subscription);
}
}