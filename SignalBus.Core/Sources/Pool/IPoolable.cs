using System;

namespace SubHubT
{
public interface IPoolable
{
	Boolean					IsInPool				{ get;set; }
	void					BeforeRent				(  );
	void					BeforeRepool			(  );
}
}