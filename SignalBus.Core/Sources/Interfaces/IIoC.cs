using System;

namespace SignalHubT
{
public interface IIoC
{
	ISignalBus<T>			CreateSignalBus<T>		(  ) where T : ISignalData;
	IPool<T>				CreatePool<T>			( Func<T> factory ) where T : IPoolable, new();
	T						Rent<T>					(  ) where T : IPoolable, new();
	void					Repool<T>				( T poolable ) where T : IPoolable, new();
	ISubscription<T>		RentSubscription<T>		(  ) where T : ISignalData;
	void					RepoolSubscription<T>	( ISubscription<T> subscription ) where T : ISignalData;
}
}