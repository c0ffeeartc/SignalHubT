using NSpec;
using Shouldly;
using SubH;

namespace Tests
{
public sealed class describe_SubHub : nspec
{
	private					void					test_SubUnsub				(  )
	{
		ISubHubTests<Message1> subHubM1	= null;
		beforeEach = ()=>
		{
			subHubM1				= new SubHub<Message1>();
		};

		it["Sub twice count matches"] = ()=>
		{
			subHubM1.GetSubscriptions(  ).Count.ShouldBe( 0 );
			subHubM1.Sub( m1 => {} );
			subHubM1.Sub( m1 => {} );
			subHubM1.GetSubscriptions(  ).Count.ShouldBe( 2 );
		};

		it["Unsub count matches"] = ()=>
		{
			subHubM1.GetSubscriptions(  ).Count.ShouldBe( 0 );
			var subscription1 = subHubM1.Sub( m1 => {} );
			var subscription2 = subHubM1.Sub( m1 => {} );
			subHubM1.GetSubscriptions(  ).Count.ShouldBe( 2 );

			subHubM1.Unsub( subscription1 );
			subHubM1.GetSubscriptions(  ).Count.ShouldBe( 1 );
			subHubM1.Unsub( subscription2 );
			subHubM1.GetSubscriptions(  ).Count.ShouldBe( 0 );
		};

		it["Unsubs correct subscription"] = ()=>
		{
			var subscription1 = subHubM1.Sub( m1 => {} );
			var subscription2 = subHubM1.Sub( m1 => {} );

			subHubM1.Unsub( subscription1 );
			subHubM1.GetSubscriptions(  ).Contains( subscription1 ).ShouldBe( false );
			subHubM1.GetSubscriptions(  ).Contains( subscription2 ).ShouldBe( true );
		};
	}

	private					void					test_SubHub_SortOrder	(  )
	{
		ISubHubTests<Message1> subHubM1	= null;
		beforeEach = ()=>
		{
			subHubM1				= new SubHub<Message1>();
		};

		it["Sub default order == 0"] = ()=>
		{
			var subscription = subHubM1.Sub( (Message1 m1) => {} );
			subscription.Order.ShouldBe( 0 );
		};

		it["Sub explicit order matches"] = ()=>
		{
			var subscription = subHubM1.Sub( m1 => {}, order: 123 );
			subscription.Order.ShouldBe( 123 );
		};

		it["Sub order is sorted"] = ()=>
		{
			var subscription1 = subHubM1.Sub( m1 => {}, 123 );
			var subscription2 = subHubM1.Sub( m1 => {}, 0 );
			subHubM1.GetSubscriptions(  )[0].Order.ShouldBe( subscription2.Order );
			subHubM1.GetSubscriptions(  )[1].Order.ShouldBe( subscription1.Order );
		};
	}
}
}