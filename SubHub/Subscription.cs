using System;

namespace SubH
{
public sealed class Subscription<TMessage> : ISubscription<TMessage>
		where TMessage : IMessage
{
	public					Subscription			( Boolean hasFilter, Object filter, Int32 order, Action<TMessage> action )
	{
		HasFilter					= hasFilter;
		Filter						= filter;
		Order						= order;
		_action						= action;
	}

	private					Action<TMessage>		_action;
	public					Boolean					HasFilter				{ get; }
	public					Object					Filter					{ get; }
	public					Int32					Order					{ get; }

	public					void					Invoke					( TMessage message )
	{
		_action.Invoke( message );
	}
}
}