using System;

namespace SubH
{
public interface IPoolable
{
	Boolean					IsInPool				{ get;set; }
	void					BeforeRent				(  );
	void					BeforeRepool			(  );
}
}