using System.Collections.Generic;
using System.Linq;

namespace SubH
{
public partial class SubHub<T> : ISubHubTests<T> where T : IMessage, IPoolable
{
	public			List<ISubscription<T>>			GetSubscriptions	(  )
	{
		return _subscriptions
			.Select( kv => kv.Value )
			.ToList(  );
	}

	public					void					Sub					( ISubscription<T> subscription )
	{
		AddSubscription( subscription );
	}
}
}