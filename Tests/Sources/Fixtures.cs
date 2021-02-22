using System;
using SubH;

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
public sealed class Message2 : IMessage
{
}
}