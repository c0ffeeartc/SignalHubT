using NSpec;
using NSubstitute;
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
			// given
			var subscription1 = subHubM1.Sub( m1 => {} );
			var subscription2 = subHubM1.Sub( m1 => {} );
			var subscription3 = subHubM1.Sub( m1 => {} );

			// when
			subHubM1.Unsub( subscription2 );

			// then
			subHubM1.GetSubscriptions(  ).Contains( subscription1 ).ShouldBe( true );
			subHubM1.GetSubscriptions(  ).Contains( subscription2 ).ShouldBe( false );
			subHubM1.GetSubscriptions(  ).Contains( subscription3 ).ShouldBe( true );
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
			var subscription		= subHubM1.Sub( (Message1 m1) => {} );
			subscription.Order.ShouldBe( 0 );
		};

		it["Sub explicit order matches"] = ()=>
		{
			var subscription		= subHubM1.Sub( m1 => {}, order: 123 );
			subscription.Order.ShouldBe( 123 );
		};

		it["Sub order is sorted"] = ()=>
		{
			// when
			var subscription1		= subHubM1.Sub( m1 => {}, 5 );
			var subscription2		= subHubM1.Sub( m1 => {}, 0 );
			var subscription3		= subHubM1.Sub( m1 => {}, 3 );

			// then
			subHubM1.GetSubscriptions(  )[0].Order.ShouldBe( subscription2.Order );
			subHubM1.GetSubscriptions(  )[1].Order.ShouldBe( subscription3.Order );
			subHubM1.GetSubscriptions(  )[2].Order.ShouldBe( subscription1.Order );

			subHubM1.GetSubscriptions(  )[0].ShouldBe( subscription2 );
			subHubM1.GetSubscriptions(  )[1].ShouldBe( subscription3 );
			subHubM1.GetSubscriptions(  )[2].ShouldBe( subscription1 );
		};
	}

	private					void					test_SubHub_Filter		(  )
	{
		ISubHubTests<Message1> subHubM1	= null;
		beforeEach = ()=>
		{
			subHubM1				= new SubHub<Message1>();
		};

		it["Sub Global is invoked"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns( null );
			subscription.HasFilter.Returns( false );
			subHubM1.Sub(subscription);

			// when
			subHubM1.Publish( message1 );

			// then
			subscription
				.Received( 1 )
				.Invoke( message1 );
		};

		it["Sub Global is invoked, when publish with filter"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns( null );
			subscription.HasFilter.Returns( false );
			subHubM1.Sub( subscription );

			// when
			subHubM1.Publish( "filter", message1 );

			// then
			subscription
				.Received( 1 )
				.Invoke( message1 );
		};

		it["Sub Filtered is invoked, when publish with filter"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var filter				= "filter";

			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns( filter );
			subscription.HasFilter.Returns( true );
			subHubM1.Sub( subscription );

			// when
			subHubM1.Publish( filter, message1 );

			// then
			subscription
				.Received( 1 )
				.Invoke( message1 );
		};

		it["Sub Filtered is NOT invoked, when published WITHOUT filter"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var filter				= "filter";

			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns( filter );
			subscription.HasFilter.Returns( true );
			subHubM1.Sub( subscription );

			// when
			subHubM1.Publish( message1 );

			// then
			subscription
				.DidNotReceiveWithAnyArgs(  )
				.Invoke( null );
		};

		it["Sub Filtered is NOT invoked, when published with DIFFERENT filter"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var filter				= "filter";
			var filter2				= "filter2";

			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns( filter );
			subscription.HasFilter.Returns( true );
			subHubM1.Sub( subscription );

			// when
			subHubM1.Publish( filter2, message1 );

			// then
			subscription
				.DidNotReceiveWithAnyArgs(  )
				.Invoke( null );
		};
	}
}
}