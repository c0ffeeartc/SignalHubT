using System;
using System.Collections.Generic;

namespace SubHubT
{
public class Pool<T> : IPool<T>
		where T : IPoolable, new(  )
{
	public					Pool					( Func<T> factory = null )
	{
		_factory					= factory;
	}

	public static			IPool<T>				I						= IoC.I.CreatePool( () => new T() );

	private					Stack<T>				_items					= new Stack<T>();
	private					Func<T>					_factory;

	public					T						Rent					(  )
	{
		var item					= _items.Count == 0
			? _factory.Invoke(  ) // item.IsInPool == false here(depends on factory method). Can be used in BeforeRent for some weird check
			: _items.Pop(  );
		item.BeforeRent(  );
		item.IsInPool				= false;
		return item;
	}

	public					void					Repool					( T poolable )
	{
		poolable.BeforeRepool(  );
		poolable.IsInPool			= true;
		_items.Push(poolable);
	}
}
}