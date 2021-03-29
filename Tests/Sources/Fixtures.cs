using System;
using SubHubT;

namespace Tests
{
public sealed class Message1 : IMessage, IPoolable
{
	public string Str;

	public Message1 Init( String str)
	{
		Str = str;
		return this;
	}

	public Boolean IsInPool { get; set; }

	public void BeforeRent( )
	{
	}

	public void BeforeRepool( )
	{
		// Don't set to null for tests
		// Str = null;
	}
}

public sealed class MessageNoPool : IMessage
{
	public int Value;
}

public struct MessageStruct : IMessage
{
	public MessageStruct(string str)
	{
		Str = str;
	}

	public string Str;
}

public delegate void ActionRef2<T1,T2> (T1 obj, ref T2 refT);

public class MessageActionHandler<T> : IListen<T> where T : IMessage
{
	public MessageActionHandler(ActionRef2<Object,T> actionHandle)
	{
		ActionHandle = actionHandle;
	}
	public ActionRef2<Object, T> ActionHandle;

	public void Handle(Object filter, ref T message)
	{
		ActionHandle.Invoke(filter, ref message);
	}
}
}