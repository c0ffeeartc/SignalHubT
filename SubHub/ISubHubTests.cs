using System.Collections.Generic;

namespace SubH
{
public interface ISubHubTests
{
	List<ISubscription<TMessage>> Tests_GetFor<TMessage>() where TMessage : IMessage;
}
}