using System;

namespace SubH
{
public sealed class Subscription<TMessage> : ISubscription<TMessage>
		where TMessage : IMessage
{
	public					ISubscription<TMessage>	Init					( Boolean hasFilter, Object filter, Action<TMessage> action, Int32 order )
	{
		HasFilter					= hasFilter;
		Filter						= filter;
		_action						= action;
		Order						= order;
		return this;
	}

	private					Action<TMessage>		_action;
	public					Boolean					HasFilter				{ get; private set; }
	public					Object					Filter					{ get; private set; }
	public					Int32					Order					{ get; private set; }
	public					Boolean					IsInPool				{ get; set; }

	public					void					Invoke					( TMessage message )
	{
		_action.Invoke( message );
	}

	public int CompareTo(ISubscription<TMessage> other)
	{
		if ( ReferenceEquals( this, other ) )
		{
			return 0;
		}
	
		if ( ReferenceEquals( null, other ) )
		{
			return 1;
		}

		var order = Order.CompareTo( other.Order );
		if ( order != 0 )
		{
			return order;
		}

		return 1;
	}

	public					void					BeforeRepool			(  )
	{
		_action						= null;
		HasFilter					= false;
		Filter						= null;
		Order						= 0;
	}
}
}