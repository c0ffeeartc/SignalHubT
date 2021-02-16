using NSpec;
using Shouldly;
using SubH;

namespace Tests
{
public sealed class describe_Tests : nspec
{
	private					void					test_Tests				(  )
	{
		it["pass"] = ()=>
		{
			true.ShouldBe( true );
		};

		// it["fail"] = ()=>
		// {
		// 	true.ShouldBe( false );
		// };
	}

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
	}
}
}