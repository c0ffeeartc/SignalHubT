using System;
using SubH;

namespace Tests
{
public sealed class Message1 : IMessage, IPoolable
{
	public string Str;

	public Boolean IsInPool { get; set; }

	public void BeforeRepool( )
	{
		Str = null;
	}
}
public sealed class Message2 : IMessage
{
}
}