using System;
using System.Collections.Generic;
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
		before = ()=>
		{
			subHubM1				= new SubHub<Message1>();
		};

		it["Sub twice count matches"] = ()=>
		{
			subHubM1.GetSubscriptions(  ).Count.ShouldBe( 0 );
			subHubM1.Sub( m1 => {} );
			subHubM1.Sub( m1 => {Console.Write("");} );
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
		before = ()=>
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

		new Each<int[], int[]>
		{
			{new[]{5,0,3}, new[]{0,3,5}},
			{new[]{1,0,3,2,5,4,7,6,9,8}, new[]{0,1,2,3,4,5,6,7,8,9}},
			{new[]{0,3,0,3,5,5}, new[]{0,0,3,3,5,5}},
		}.Do( (given, expected) =>
		it["Sub order is sorted"] = ( )=>
		{
			// when
			foreach ( var givenOrder in given )
			{
				subHubM1.Sub( m1 => {}, givenOrder );
			}

			// then
			var subscriptions			= subHubM1.GetSubscriptions(  );
			for ( var i = 0; i < subscriptions.Count; i++ )
			{
				var sub = subscriptions[i];
				sub.Order.ShouldBe( expected[i] );
			}
		});

		it["Sub inserts subscription after same order"] = ( )=>
		{
			// given
			subHubM1.Sub( m1 => {}, 0 );
			subHubM1.Sub( m1 => {}, 1 );
			subHubM1.Sub( m1 => {}, 1 );
			subHubM1.Sub( m1 => {}, 2 );

			// when
			var subscripton = subHubM1.Sub( m1 => {}, 1 );

			// then
			subHubM1.GetSubscriptions(  )[1].ShouldNotBe( subscripton );
			subHubM1.GetSubscriptions(  )[3].ShouldBe( subscripton );
		};
	}

	private					void					test_SubHub_Filter		(  )
	{
		ISubHubTests<Message1> subHubM1	= null;
		before = ()=>
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

	private					void					test_SubHub_Recursion	(  )
	{
		ISubHubTests<Message1> subHubM1	= null;
		before = ()=>
		{
			subHubM1				= new SubHub<Message1>();
		};

		it["During Publish inside callback subscribe to same Message with SAME or HIGHER priorityOrder.\nAdded subscription should be invoked during this same publish"] = ()=>
		{
			// given
			// var subscrpition		= Substitute.For<ISubscription<Message1>>(  );
			// subscrpition.Filter.Returns(null);
			// subscrpition.HasFilter.Returns(false);

			ISubscription<Message1> sub2;

			var sub1				= subHubM1.Sub( m1 =>
				{
					m1.Str = "m1sub1";
					// subHubM1.Sub( subscrpition );
					sub2 = subHubM1.Sub( m2 =>
						{
							m2.Str = "m1sub2";
						} );
				} );

			// when
			var message1			= Pool<Message1>.I.Rent().Init("m1");
			subHubM1.Publish( message1 );

			// then
			message1.Str.ShouldBe( "m1sub2" );
		};

		it["During Publish inside callback subscribe to same Message with LOWER priorityOrder.\nAdded subscription should NOT be invoked during this same publish"] = ()=>
		{
			// given
			subHubM1.Sub( m1 =>
				{
					m1.Str += "sub1";
					// subHubM1.Sub( subscrpition );
					if (m1.Str.Length > 8)
					{
						return;
					}
					var sub2 = subHubM1.Sub( m2 =>
						{
							m2.Str += "sub2";
						}, order: -5 );
				} );

			// when
			var message1			= Pool<Message1>.I.Rent()
				.Init("m1");
			subHubM1.Publish( message1 );

			// then
			message1.Str.ShouldBe( "m1sub1" );
		};
	}
}
}