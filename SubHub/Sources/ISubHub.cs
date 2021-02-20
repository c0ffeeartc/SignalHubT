using System;

namespace SubH
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

public interface ISubHub
{
	ISubscription<T>		Sub<T>					( Action<T> action, int order = 0 ) where T : IMessage, IPoolable;
	ISubscription<T>		Sub<T>					( Object filter, Action<T> action, int order = 0 ) where T : IMessage, IPoolable;

	void					Unsub<T>				( ISubscription<T> subscription ) where T : IMessage, IPoolable;

	void					Publish<T>				( T message ) where T : IMessage, IPoolable;
	void					Publish<T>				( Object filter, T message ) where T : IMessage, IPoolable;
}
}