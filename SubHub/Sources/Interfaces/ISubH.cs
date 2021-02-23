using System;

namespace SubHubT
{
public interface ISubH
{
	ISubscription<T>		Sub<T>					( Action<T> action, int order = 0 ) where T : IMessage, IPoolable;
	ISubscription<T>		Sub<T>					( Object filter, Action<T> action, int order = 0 ) where T : IMessage, IPoolable;

	void					Unsub<T>				( ISubscription<T> subscription ) where T : IMessage, IPoolable;

	void					Publish<T>				( T message ) where T : IMessage, IPoolable;
	void					Publish<T>				( Object filter, T message ) where T : IMessage, IPoolable;

	T						Args<T>					(  ) where T : IMessage, IPoolable;
}

public interface ISubHTests : ISubH
{
	ISubHub<T>				GetSubHubT<T>			(  ) where T : IMessage, IPoolable;
}
}