using NSpec;
using Shouldly;
using SubH;

namespace Tests
{
public sealed class describe_Tests : nspec
{
	private					void					test_SubHub				(  )
	{
		SubHub subHub = null;
		beforeEach = ()=>
		{
			subHub = new SubHub();
		};

		it["Sub default order == 0"] = ()=>
		{
			var subscription = subHub.Sub( (Message1 m1) => {} );
			subscription.Order.ShouldBe( 0 );
		};

		it["Sub explicit order matches"] = ()=>
		{
			var subscription = subHub.Sub( (Message1 m1) => {}, order: 123 );
			subscription.Order.ShouldBe( 123 );
		};

		it["Sub twice count matches"] = ()=>
		{
			subHub.Tests_GetFor<Message1>(  ).Count.ShouldBe( 0 );
			subHub.Sub( (Message1 m1) => {} );
			subHub.Sub( (Message1 m1) => {} );
			subHub.Tests_GetFor<Message1>(  ).Count.ShouldBe( 2 );
		};

		it["Unsub count matches"] = ()=>
		{
			subHub.Tests_GetFor<Message1>(  ).Count.ShouldBe( 0 );
			var subscription1 = subHub.Sub( (Message1 m1) => {} );
			var subscription2 = subHub.Sub( (Message1 m1) => {} );
			subHub.Tests_GetFor<Message1>(  ).Count.ShouldBe( 2 );

			subHub.Unsub( subscription1 );
			subHub.Tests_GetFor<Message1>(  ).Count.ShouldBe( 1 );
			subHub.Unsub( subscription2 );
			subHub.Tests_GetFor<Message1>(  ).Count.ShouldBe( 0 );
		};

		it["Unsubs correct subscription"] = ()=>
		{
			var subscription1 = subHub.Sub( (Message1 m1) => {} );
			var subscription2 = subHub.Sub( (Message1 m1) => {} );

			subHub.Unsub( subscription1 );
			subHub.Tests_GetFor<Message1>(  ).Contains( subscription1 ).ShouldBe( false );
			subHub.Tests_GetFor<Message1>(  ).Contains( subscription2 ).ShouldBe( true );
		};

		it["Sub order is sorted"] = ()=>
		{
			var subscription1 = subHub.Sub( (Message1 m1) => {}, 123 );
			var subscription2 = subHub.Sub( (Message1 m1) => {}, 0 );
			subHub.Tests_GetFor<Message1>(  )[0].Order.ShouldBe( subscription2.Order );
			subHub.Tests_GetFor<Message1>(  )[1].Order.ShouldBe( subscription1.Order );
		};
	}
}
}