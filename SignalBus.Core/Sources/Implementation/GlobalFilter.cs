using System;

namespace SignalBusT
{
public sealed class GlobalFilter
{
	public static			GlobalFilter			I						= new GlobalFilter(  );

	public override			Int32					GetHashCode				(  ) => 128;
}
}