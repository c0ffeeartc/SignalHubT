using System;
using SubHubT;

namespace Tests
{
public sealed class Message1 : ISignalData, IPoolable
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

public sealed class MessageNoPool : ISignalData
{
	public int Value;
}

public struct MessageStruct : ISignalData
{
	public MessageStruct(string str)
	{
		Str = str;
	}

	public string Str;
}
}