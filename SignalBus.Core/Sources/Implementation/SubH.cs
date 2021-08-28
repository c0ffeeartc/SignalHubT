using System;

namespace SubHubT
{
// Facade for an arguably better API
public partial class SubH : ISubH
{
	public static			ISubH					I						= IoC.I.CreateSubH(  );

	public					ISubscription<T>		Sub<T>					( ActionRef<T> action, Int32 order = 0 )
			where T : ISignalData
	{
		return SignalBus<T>.I.Sub( action, order);
	}

	public					ISubscription<T>		Sub<T>					( Object filter, ActionRef<T> action, Int32 order=0 )
			where T : ISignalData
	{
		return SignalBus<T>.I.Sub( filter, action, order);
	}

	public					void					Unsub<T>				( ISubscription<T> subscription )
			where T : ISignalData
	{
		SignalBus<T>.I.Unsub(subscription);
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

	public					void					Publish<T2>				( T2 message )
			where T2: ISignalData, IPoolable, new()
	{
		SignalBus<T2>.I.Publish(message);
	}

	public					void					Publish<T>				( Object filter, T message )
			where T : ISignalData, IPoolable, new()
	{
		SignalBus<T>.I.Publish(filter, message);
	}

	public					T						Args<T>					(  )
			where T : ISignalData, IPoolable, new()
	{
		return IoC.I.Rent<T>();
	}
}

public partial class SubH : ISubHTests
{
	public					ISignalBus<T>				GetSignalBus<T>			(  )
			where T : ISignalData
	{
		return SignalBus<T>.I;
	}
}
}