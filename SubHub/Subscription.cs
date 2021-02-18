using System;

namespace SubH
{
public sealed class Subscription<TMessage> : ISubscription<TMessage>
		where TMessage : IMessage
{
	public					Subscription			( Boolean hasFilter, Object filter, Action<TMessage> action, Int32 order )
	{
		HasFilter					= hasFilter;
		Filter						= filter;
		_action						= action;
		Order						= order;
	}

	private					Action<TMessage>		_action;
	public					Boolean					HasFilter				{ get; }
	public					Object					Filter					{ get; }
	public					Int32					Order					{ get; }

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

		return -1;
	}
}
}