using System;

namespace SubH
{
public interface ISubscription<TMessage>
		where TMessage : IMessage
{
	Int32					Order					{ get; } // Not implemented
	Boolean					HasFilter				{ get; }
	Object 					Filter					{ get; }
	void 					Invoke					( TMessage message );
}
}