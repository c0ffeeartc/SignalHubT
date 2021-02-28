using System;

namespace SubHubT
{
public interface ISubH
{
	ISubscription<T>		Sub<T>					( ActionRef<T> action, int order = 0 ) where T : IMessage;
	ISubscription<T>		Sub<T>					( Object filter, ActionRef<T> action, int order = 0 ) where T : IMessage;

	void					Unsub<T>				( ISubscription<T> subscription ) where T : IMessage;

	void					Pub<T>					( T message ) where T : IMessage;
	void					Pub<T>					( Object filter, T message ) where T : IMessage;

	void					Publish<T>				( T message ) where T : IMessage, IPoolable, new();
	void					Publish<T>				( Object filter, T message ) where T : IMessage, IPoolable, new();

	T						Args<T>					(  ) where T : IMessage, IPoolable, new();
}

public interface ISubHTests : ISubH
{
	ISubHub<T>				GetSubHubT<T>			(  ) where T : IMessage;
}
}