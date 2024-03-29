using System;

namespace SignalHubT
{
public class IoC : IIoC
{
	public static			IIoC					I						= new IoC(  );

	public virtual			ISignalBus<T>			CreateSignalBus<T>		(  )
			where T : ISignalData
	{
		return new SignalBus<T>(  );
	}

	public virtual			IPool<T>				CreatePool<T>			( Func<T> factory )
			where T : IPoolable, new()
	{
		return new Pool<T>( factory );
	}

	public virtual			T						Rent<T>					(  )
			where T : IPoolable, new( )
	{
		return Pool<T>.I.Rent();
	}

	public virtual			void					Repool<T>				( T poolable )
			where T : IPoolable, new()
	{
		Pool<T>.I.Repool( poolable );
	}

	public virtual			ISubscription<T>		RentSubscription<T>		(  )
			where T : ISignalData
	{
		return Pool<Subscription<T>>.I.Rent();
	}

	public virtual			void					RepoolSubscription<T>	( ISubscription<T> subscription )
			where T : ISignalData
	{
		Pool<Subscription<T>>.I.Repool( subscription as Subscription<T> );
	}
}
}