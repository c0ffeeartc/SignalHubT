using System;
using System.Collections.Generic;

namespace SubHubT
{
public interface ISubHub<T>
		where T : IMessage, IPoolable
{
	ISubscription<T>		Sub						( Action<T> action, int order = 0 );
	ISubscription<T>		Sub						( Object filter, Action<T> action, int order = 0 );

	void					Unsub					( ISubscription<T> subscription );

	void					Publish					( T message );
	void					Publish					( Object filter, T message );

	void					UnsubAll				(  );
}

public interface ISubHubTests<T> : ISubHub<T>
		where T : IMessage, IPoolable
{
	List<ISubscription<T>>	GetSubscriptions		(  );
	void					Sub						( ISubscription<T> subscription);
}
}