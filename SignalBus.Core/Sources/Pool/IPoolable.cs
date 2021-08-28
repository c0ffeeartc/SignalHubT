using System;

namespace SignalHubT
{
public interface IPoolable
{
	Boolean					IsInPool				{ get;set; }
	void					BeforeRent				(  );
	void					BeforeRepool			(  );
}
}