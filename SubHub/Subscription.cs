using System;

namespace SubH
{
public sealed class Subscription<TMessage> : ISubscription<TMessage> where TMessage : IMessage
{
	public					Subscription			( Boolean hasFilter, Object filter, Int32 order, Action<TMessage> action )
	{
		HasFilter					= hasFilter;
		Filter						= filter;
		Action						= action;
		Order						= order;
	}

	public					Boolean					HasFilter				{ get; }
	public					Object					Filter					{ get; }
	public					Action<TMessage>		Action					{ get; }
	public					Int32					Order					{ get; }
}
}