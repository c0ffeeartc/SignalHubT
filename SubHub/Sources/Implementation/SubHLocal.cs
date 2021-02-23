using System;
using System.Collections.Generic;

namespace SubHubT
{
public partial class SubHLocal : ISubH
{
	private				Dictionary<Type, Object>	_subHubTs				= new Dictionary<Type, Object>();

	public					ISubscription<T>		Sub<T>					( Action<T> action, Int32 order = 0 )
		where T : IMessage, IPoolable
	{
		return GetOrCreateSubHubT<T>(  ).Sub( action, order );
	}

	public					ISubscription<T>		Sub<T>					( Object filter, Action<T> action, Int32 order = 0 )
		where T : IMessage, IPoolable
	{
		return GetOrCreateSubHubT<T>(  ).Sub( filter, action, order );
	}

	public					void					Unsub<T>				( ISubscription<T> subscription )
		where T : IMessage, IPoolable
	{
		GetOrCreateSubHubT<T>(  ).Unsub( subscription );
	}

	public					void					Publish<T>				( T message )
		where T : IMessage, IPoolable
	{
		GetOrCreateSubHubT<T>(  ).Publish( message );
	}

	public					void					Publish<T>				( Object filter, T message )
		where T : IMessage, IPoolable
	{
		GetOrCreateSubHubT<T>(  ).Publish( filter, message );
	}

	public					T						Args<T>					(  ) where T : IMessage, IPoolable
	{
		return Pool<T>.I.Rent(  );
	}

	private					ISubHub<T>				GetOrCreateSubHubT<T>	(  )
		where T : IMessage, IPoolable
	{
		var messageType				= typeof(T);
		ISubHub<T> subHubT;
		if ( _subHubTs.TryGetValue( messageType, out Object obj ) )
		{
			return (ISubHub<T>)obj ;
		}

		subHubT						= Activator.CreateInstance<SubHub<T>>(  );  // TODO: use factory method returning ISubHubT
		_subHubTs[messageType]		= subHubT;
		return subHubT;
	}
}

public partial class SubHLocal : ISubHTests
{
	public					ISubHub<T>				GetSubHubT<T>			(  )
			where T : IMessage, IPoolable
	{
		return GetOrCreateSubHubT<T>(  );
	}
}
}