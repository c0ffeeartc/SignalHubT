using System;

namespace SubHubT
{
public interface IIoC
{
	ISubH					CreateSubH				(  );
	ISubH					CreateSubHLocal			(  );
	ISubHub<T>				CreateSubHub<T>			(  ) where T : IMessage, IPoolable, new();
	IPool<T>				CreatePool<T>			( Func<T> factory ) where T : IPoolable, new();
	T						Rent<T>					(  ) where T : IPoolable, new();
	void					Repool<T>				( T poolable ) where T : IPoolable, new();
	ISubscription<T>		RentSubscription<T>		(  ) where T : IMessage, IPoolable;
	void					RepoolSubscription<T>	( ISubscription<T> subscription ) where T : IMessage, IPoolable;
}
}