using System;
using System.Collections.Generic;

namespace SubHubT
{
public partial class SubHLocal : ISubH
{
	private				Dictionary<Type, Object>	_subHubTs				= new Dictionary<Type, Object>();

	public					ISubscription<T>		Sub<T>					( ActionRef<T> action, Int32 order = 0 )
			where T : IMessage
	{
		return GetOrCreateSignalBus<T>(  ).Sub( action, order );
	}

	public					ISubscription<T>		Sub<T>					( Object filter, ActionRef<T> action, Int32 order = 0 )
			where T : IMessage
	{
		return GetOrCreateSignalBus<T>(  ).Sub( filter, action, order );
	}

	public					void					Unsub<T>				( ISubscription<T> subscription )
			where T : IMessage
	{
		GetOrCreateSignalBus<T>(  ).Unsub( subscription );
	}

	public					T						Pub<T>					( T message )
			where T : IMessage
	{
		return GetOrCreateSignalBus<T>(  ).Pub( message );
	}

	public					T						Pub<T>					( Object filter, T message )
			where T : IMessage
	{
		return GetOrCreateSignalBus<T>(  ).Pub( filter, message );
	}

	public					void					Publish<T>				( T message )
			where T : IMessage, IPoolable, new()
	{
		GetOrCreateSignalBus<T>(  ).Publish( message );
	}

	public					void					Publish<T>				( Object filter, T message )
			where T : IMessage, IPoolable, new()
	{
		GetOrCreateSignalBus<T>(  ).Publish( filter, message );
	}

	public					T						Args<T>					(  )
			where T : IMessage, IPoolable, new()
	{
		return IoC.I.Rent<T>(  );
	}

	private					ISignalBus<T>			GetOrCreateSignalBus<T>	(  )
			where T : IMessage
	{
		var messageType				= typeof(T);
		if ( _subHubTs.TryGetValue( messageType, out Object obj ) )
		{
			return (ISignalBus<T>)obj;
		}

		ISignalBus<T> signalBus		= IoC.I.CreateSignalBus<T>(  );
		_subHubTs[messageType]		= signalBus;
		return signalBus;
	}
}

public partial class SubHLocal : ISubHTests
{
	public					ISignalBus<T>			GetSignalBus<T>			(  )
			where T : IMessage
	{
		return GetOrCreateSignalBus<T>(  );
	}
}
}