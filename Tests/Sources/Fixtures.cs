using System;
using SubH;

namespace Tests
{
public sealed class Message1 : IMessage, IPoolable
{
	public string Str;

	private Boolean _isInPool;
	public Boolean IsInPool { get => _isInPool; set => _isInPool = value; }

	public void BeforeRent( )
	{
	}

	public void BeforeRepool( )
	{
		Str = null;
	}
}
public sealed class Message2 : IMessage
{
}
}