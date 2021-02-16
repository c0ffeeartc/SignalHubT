using System;

namespace SubH
{
public sealed class Subscription<TMessage> : ISubscription<TMessage> where TMessage : IMessage
{
	public					Subscription			( Action<TMessage> action, Int32 order )
	{
		Order						= order;
		Action						= action;
	}

	public					Int32					Order					{ get; }
	public					Action<TMessage>		Action					{ get; }
}
}