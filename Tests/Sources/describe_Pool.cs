using NSpec;
using Shouldly;
using SignalHubT;

namespace Tests
{
public class describe_Pool : nspec
{
	private					void					test_Pool_Message		(  )
	{
		it["Poolable isInPool"] = ()=>
		{
			var m1					= IoC.I.Rent<Message1>();
			m1.IsInPool.ShouldBe( false );

			IoC.I.Repool(m1);
			m1.IsInPool.ShouldBe( true );

			IoC.I.Rent<Message1>();
			m1.IsInPool.ShouldBe( false );
		};
	}

	private					void					test_Pool_Subscription	(  )
	{
		it["Pool Subscription"] = ()=>
		{
			var s1					= IoC.I.RentSubscription<Message1>();
			s1.IsInPool.ShouldBe( false );

			IoC.I.RepoolSubscription(s1);
			s1.IsInPool.ShouldBe( true );

			IoC.I.RentSubscription<Message1>();
			s1.IsInPool.ShouldBe( false );
		};
	}
}
}