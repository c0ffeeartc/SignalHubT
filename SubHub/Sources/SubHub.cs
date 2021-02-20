using System;

namespace SubH
{
// Facade for an arguably better API
public class SubHub : ISubHub
{
	public static SubHub I = new SubHub();

	public ISubscription<T> Sub<T>( Action<T> action, Int32 order = 0) where T : IMessage
	{
		return SubHub<T>.I.Sub( action, order);
	}

	public ISubscription<T> Sub<T>( Object filter, Action<T> action, Int32 order = 0) where T : IMessage
	{
		return SubHub<T>.I.Sub( filter, action, order);
	}

	public void Unsub<T>( ISubscription<T> subscription) where T : IMessage
	{
		SubHub<T>.I.Unsub(subscription);
	}

	public void Publish<T>(T message) where T : IMessage
	{
		SubHub<T>.I.Publish(message);
	}

	public void Publish<T>( Object filter, T message) where T : IMessage
	{
		SubHub<T>.I.Publish(filter, message);
	}
}
}