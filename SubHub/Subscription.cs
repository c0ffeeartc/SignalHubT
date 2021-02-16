using System;

namespace SubHub
{
public sealed class Subscription<TMessage> : ISubscription<TMessage> where TMessage : IMessage
{
	public					Subscription			( Action<TMessage> action )
	{
		Action						= action;
	}

	public					Action<TMessage>		Action					{ get; private set; }
}
}