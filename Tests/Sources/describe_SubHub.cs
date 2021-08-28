using System;
using NSpec;
using NSubstitute;
using Shouldly;
using SubHubT;

namespace Tests
{
public sealed class describe_SubHub : nspec
{
	private					void					test_SubUnsub				(  )
	{
		ISignalBusTests<Message1> signalBusM1	= null;
		before = ()=>
		{
			signalBusM1				= IoC.I.CreateSignalBus<Message1>(  ) as ISignalBusTests<Message1>;
		};

		it["Sub twice count matches"] = ()=>
		{
			signalBusM1.GetSubscriptions( null ).Count.ShouldBe( 0 );
			signalBusM1.Sub( (ref Message1 m1) => {} );
			signalBusM1.Sub( (ref Message1 m1) => {Console.Write("");} );
			signalBusM1.Pub(new Message1());
			signalBusM1.GetSubscriptions( null ).Count.ShouldBe( 2 );
		};

		it["Unsub count matches"] = ()=>
		{
			signalBusM1.GetSubscriptions( null ).Count.ShouldBe( 0 );
			var subscription1 = signalBusM1.Sub( (ref Message1 m1) => {} );
			var subscription2 = signalBusM1.Sub( (ref Message1 m1) => {} );
			signalBusM1.GetSubscriptions( null ).Count.ShouldBe( 2 );

			signalBusM1.Unsub( subscription1 );
			signalBusM1.GetSubscriptions( null ).Count.ShouldBe( 1 );
			signalBusM1.Unsub( subscription2 );
			signalBusM1.GetSubscriptions( null ).Count.ShouldBe( 0 );
		};

		it["Unsubs correct subscription"] = ()=>
		{
			// given
			var subscription1 = signalBusM1.Sub( (ref Message1 m1) => {} );
			var subscription2 = signalBusM1.Sub( (ref Message1 m1) => {} );
			var subscription3 = signalBusM1.Sub( (ref Message1 m1) => {} );

			// when
			signalBusM1.Unsub( subscription2 );

			// then
			signalBusM1.GetSubscriptions( null ).Contains( subscription1 ).ShouldBe( true );
			signalBusM1.GetSubscriptions( null ).Contains( subscription2 ).ShouldBe( false );
			signalBusM1.GetSubscriptions( null ).Contains( subscription3 ).ShouldBe( true );
		};
	}

	private					void					test_SubUnsub_Pub			(  )
	{
		ISignalBusTests<MessageNoPool> signalBus = null;
		before = ()=>
		{
			signalBus				= IoC.I.CreateSignalBus<MessageNoPool>(  ) as ISignalBusTests<MessageNoPool>;
		};

		it["Pub can publish non-poolable message"] = ()=>
		{
			// given
			var subscription		= Substitute.For<ISubscription<MessageNoPool>>(  );
			subscription.Filter.Returns(GlobalFilter.I);
			subscription.HasFilter.Returns( false );
			signalBus.Sub( subscription );

			// when
			MessageNoPool message = new MessageNoPool(  );
			signalBus.Pub( message );
			// subHubT.Publish( message ); // compile time error

			// then
			subscription
				.Received( 1 )
				.Invoke( ref message );
		};
	}

	private					void					test_SubHub_SortOrder	(  )
	{
		ISignalBusTests<Message1> signalBusM1	= null;
		before = ()=>
		{
			signalBusM1				= IoC.I.CreateSignalBus<Message1>(  ) as ISignalBusTests<Message1>;
		};

		it["Sub default order == 0"] = ()=>
		{
			var subscription		= signalBusM1.Sub( (ref Message1 m1) => {} );
			subscription.Order.ShouldBe( 0 );
		};

		it["Sub explicit order matches"] = ()=>
		{
			var subscription		= signalBusM1.Sub( (ref Message1 m1) => {}, order: 123 );
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
				signalBusM1.Sub( (ref Message1 m1) => {}, givenOrder );
			}

			// then
			var subscriptions			= signalBusM1.GetSubscriptions( null );
			for ( var i = 0; i < subscriptions.Count; i++ )
			{
				var sub = subscriptions[i];
				sub.Order.ShouldBe( expected[i] );
			}
		});

		it["Sub inserts subscription after same order"] = ( )=>
		{
			// given
			signalBusM1.Sub( (ref Message1 m1) => {}, 0 );
			signalBusM1.Sub( (ref Message1 m1)=> {}, 1 );
			signalBusM1.Sub( (ref Message1 m1) => {}, 1 );
			signalBusM1.Sub( (ref Message1 m1) => {}, 2 );

			// when
			var subscripton = signalBusM1.Sub( (ref Message1 m1) => {}, 1 );

			// then
			signalBusM1.GetSubscriptions( null )[1].ShouldNotBe( subscripton );
			signalBusM1.GetSubscriptions( null )[3].ShouldBe( subscripton );
		};
	}

	private					void					test_SubHub_Filter		(  )
	{
		ISignalBusTests<Message1> signalBusM1	= null;
		before = ()=>
		{
			signalBusM1				= IoC.I.CreateSignalBus<Message1>(  ) as ISignalBusTests<Message1>;
		};

		it["Sub Global is invoked"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns(GlobalFilter.I);
			subscription.HasFilter.Returns( false );
			signalBusM1.Sub(subscription);

			// when
			signalBusM1.Publish( message1 );

			// then
			subscription
				.Received( 1 )
				.Invoke( ref message1 );
		};

		it["Sub Global is invoked, when publish with filter"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns(GlobalFilter.I);
			subscription.HasFilter.Returns( false );
			signalBusM1.Sub( subscription );

			// when
			signalBusM1.Publish( "filter", message1 );

			// then
			subscription
				.Received( 1 )
				.Invoke( ref message1 );
		};

		it["Sub Filtered is invoked, when publish with filter"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var filter				= "filter";

			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns( filter );
			subscription.HasFilter.Returns( true );
			signalBusM1.Sub( subscription );

			// when
			signalBusM1.Publish( filter, message1 );

			// then
			subscription
				.Received( 1 )
				.Invoke( ref message1 );
		};

		it["Sub Filtered is NOT invoked, when published WITHOUT filter"] = ()=>
		{
			// given
			var message1			= new Message1(  );
			var filter				= "filter";

			var subscription		= Substitute.For<ISubscription<Message1>>(  );
			subscription.Filter.Returns( filter );
			subscription.HasFilter.Returns( true );
			signalBusM1.Sub( subscription );

			// when
			signalBusM1.Publish( message1 );

			// then
			subscription
				.DidNotReceiveWithAnyArgs(  )
				.Invoke( ref message1 );
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
			signalBusM1.Sub( subscription );

			// when
			signalBusM1.Publish( filter2, message1 );

			// then
			subscription
				.DidNotReceiveWithAnyArgs(  )
				.Invoke( ref message1 );
		};
	}

	private					void					test_SubHub_NestedSubToSameMessage(  )
	{
		ISignalBusTests<Message1> signalBusM1	= null;
		before = ()=>
		{
			signalBusM1				= IoC.I.CreateSignalBus<Message1>(  ) as ISignalBusTests<Message1>;
		};

		it["During Publish inside callback subscribe to same Message with SAME or HIGHER priorityOrder.\nAdded subscription should be invoked during this same publish"] = ()=>
		{
			// given
			// var subscrpition		= Substitute.For<ISubscription<Message1>>(  );
			// subscrpition.Filter.Returns(null);
			// subscrpition.HasFilter.Returns(false);

			ISubscription<Message1> sub2;

			var sub1				= signalBusM1.Sub( (ref Message1 m1) =>
				{
					m1.Str = "m1sub1";
					// subHubM1.Sub( subscrpition );
					sub2 = signalBusM1.Sub( (ref Message1 m2) =>
						{
							m2.Str = "m1sub2";
						} );
				} );

			// when
			var message1			= IoC.I.Rent<Message1>().Init("m1");
			signalBusM1.Publish( message1 );

			// then
			message1.Str.ShouldBe( "m1sub2" );
		};

		it["During Publish inside callback subscribe to same Message with LOWER priorityOrder.\nAdded subscription should NOT be invoked during this same publish"] = ()=>
		{
			// given
			signalBusM1.Sub( (ref Message1 m1) =>
				{
					m1.Str += "sub1";
					if (m1.Str.Length > 18) // Protection against endless loop
					{
						return;
					}

					var sub2 = signalBusM1.Sub( (ref Message1 m2) =>
						{
							m2.Str += "sub2";
						}, order: -5 );

					var sub3 = signalBusM1.Sub( (ref Message1 m2) =>
						{
							m2.Str += "sub3";
						}, order: -1 );
				} );

			// when
			var message1			= IoC.I.Rent<Message1>(  )
				.Init("m1");
			signalBusM1.Publish( message1 );

			// then
			message1.Str.ShouldBe( "m1sub1" );
		};
	}

	private					void					test_SubHub_NestedUnsubToSameMessage(  )
	{
		ISignalBusTests<Message1> signalBusM1	= null;
		before = ()=>
		{
			signalBusM1				= IoC.I.CreateSignalBus<Message1>(  ) as ISignalBusTests<Message1>;
		};

		it["During Publish inside callback Unsub to current or previous subscription HAS NO effect on current invoke chain."] = ()=>
		{
			// given
			var sub1				= signalBusM1.Sub( (ref Message1 m1) =>
				{
					m1.Str			+= "sub1";
				} );

			var sub2				= signalBusM1.Sub( (ref Message1 m2) =>
			{
				m2.Str				+= "sub2";
				signalBusM1.Unsub(sub1);
			} );

			var sub3				= signalBusM1.Sub( (ref Message1 m2) =>
				{
					m2.Str			+= "sub3";
				} );

			// when
			var message1			= IoC.I.Rent<Message1>(  )
				.Init( "m1" );
			signalBusM1.Publish( message1 );

			// then
			message1.Str.ShouldBe( "m1sub1sub2sub3" );
		};

		it["During Publish inside callback Unsub to some next subscription."] = ()=>
		{
			// given
			var sub1				= signalBusM1.Sub( (ref Message1 m1) =>
				{
					m1.Str			+= "sub1";
				} );

			var sub3				= signalBusM1.Sub( (ref Message1 m2) =>
				{
					m2.Str			+= "sub3";
				}, order: 10 );

			var sub4				= signalBusM1.Sub( (ref Message1 m2) =>
				{
					m2.Str			+= "sub4";
				}, order: 11 );

			var sub2				= signalBusM1.Sub( (ref Message1 m2) =>
				{
					m2.Str			+= "sub2";
					signalBusM1.Unsub(sub3);
				} );

			// when
			var message1			= IoC.I.Rent<Message1>()
				.Init( "m1" );
			signalBusM1.Publish( message1 );

			// then
			message1.Str.ShouldBe( "m1sub1sub2sub4" );
		};
	}

	private					void					test_SubHub_PublishInsideCallback(  )
	{
		ISignalBusTests<Message1> signalBusM1	= null;
		before = ()=>
		{
			signalBusM1				= (ISignalBusTests<Message1>) IoC.I.CreateSignalBus<Message1>(  );
		};

		it["Publish inside callback to message with same type works as a regular call stack."] = ()=>
		{
			// given
			var counter				= 0;
			var sub1				= signalBusM1.Sub( (ref Message1 m1) =>
				{
					m1.Str			+= "sub1";
				} );

			Message1 message2		= null;
			var sub2				= signalBusM1.Sub( (ref Message1 m2) =>
				{
					m2.Str			+= "sub2";
					if ( counter < 1 )
					{
						++counter;
						message2	= IoC.I.Rent<Message1>()
							.Init( "m2" );
						signalBusM1.Publish( message2 );
					}
				} );

			var sub3				= signalBusM1.Sub( (ref Message1 m2) =>
				{
					m2.Str			+= "sub3";
				});

			// when
			var message1			= IoC.I.Rent<Message1>()
				.Init( "m1" );
			signalBusM1.Publish( message1 );

			// then
			message1.Str.ShouldBe( "m1sub1sub2sub3" );
			message2.Str.ShouldBe( "m2sub1sub2sub3" );
		};
	}

	private					void					test_SubHubLocal(  )
	{
		ISignalBusTests<Message1> signalBusM1	= null;
		before = ()=>
		{
			signalBusM1				= (ISignalBusTests<Message1>) IoC.I.CreateSignalBus<Message1>(  );
		};

		it["Two SubHubLocal have different inner subHubT instances."] = ()=>
		{
			// given
			ISubHTests subHLocal = IoC.I.CreateSubHLocal(  ) as ISubHTests;
			ISubHTests subHLocal2 = IoC.I.CreateSubHLocal(  ) as ISubHTests;

			// then
			subHLocal.GetSignalBus<Message1>().ShouldNotBeNull(  );
			subHLocal2.GetSignalBus<Message1>().ShouldNotBeNull(  );
			subHLocal.GetSignalBus<Message1>().ShouldNotBe( subHLocal2.GetSignalBus<Message1>() );
		};

		it["SubHubLocal has different inner subHubT instances compared to SubH static instances."] = ()=>
		{
			// given
			ISubHTests subHLocal = IoC.I.CreateSubHLocal(  ) as ISubHTests;

			// then
			subHLocal.GetSignalBus<Message1>().ShouldNotBeNull(  );
			(SubH.I as ISubHTests).GetSignalBus<Message1>().ShouldNotBeNull(  );
			subHLocal.GetSignalBus<Message1>().ShouldBe( subHLocal.GetSignalBus<Message1>() );
			(SubH.I as ISubHTests).GetSignalBus<Message1>().ShouldBe( (SubH.I as ISubHTests).GetSignalBus<Message1>() );
			subHLocal.GetSignalBus<Message1>().ShouldNotBe( (SubH.I as ISubHTests).GetSignalBus<Message1>() );
		};
	}

	private					void					test_MessageStruct(  )
	{
		ISignalBusTests<MessageStruct> hub = null;
		before = ()=>
		{
			hub					= (ISignalBusTests<MessageStruct>) IoC.I.CreateSignalBus<MessageStruct>(  );
		};

		it["Pub passes same message struct by reference"] = ()=>
		{
			// given
			var sub1				= hub.Sub( (ref MessageStruct m1) =>
				{
					m1.Str			+= "sub1";
				} );

			var sub2				= hub.Sub( (ref MessageStruct m2) =>
				{
					m2.Str			+= "sub2";

					// then
					m2.Str.ShouldBe("m1sub1sub2");
				} );

			// when
			hub.Pub( new MessageStruct( "m1" ) );
		};

		it["Pub returns modified message struct"] = ()=>
		{
			// given
			var sub1				= hub.Sub( (ref MessageStruct m1) =>
				{
					m1.Str			+= "sub1";
				} );

			// when
			var messageStruct		= new MessageStruct( "m1" );
			var messageResult		= hub.Pub( messageStruct );
			hub.Pub( new MessageStruct("2") );

			// then
			messageResult.Str.ShouldBe( "m1sub1" );
			messageStruct.Str.ShouldBe( "m1" ); // this check is only to detect behaviour change. Remove when necessary
		};

		it["Pub message correct order. Lower order first, same order - first unfiltered, then filtered"] = ()=>
		{
			// given
			var filterA				= new Object();
			var filterB				= new Object();

			var subFb5				= hub.Sub( filter: filterB
				, (ref MessageStruct m1) =>
				{
					m1.Str			+= "subFb5";
				}
				, order:5 );

			var subA5				= hub.Sub( (ref MessageStruct m1) =>
				{
					m1.Str			+= "subA5";
				}
				, order:5 );

			var subFa5				= hub.Sub( filter: filterA
				, (ref MessageStruct m1) =>
				{
					m1.Str			+= "subFa5";
				}
				, order:5 );

			var subFa2				= hub.Sub( filter: filterA
				, (ref MessageStruct m1) =>
				{
					m1.Str			+= "subFa2";
				}
				, order:2 );

			var sub1				= hub.Sub( (ref MessageStruct m1) =>
				{
					m1.Str			+= "sub1";
				}
				, order: 1 );

			var subB5				= hub.Sub( (ref MessageStruct m1) =>
				{
					m1.Str			+= "subB5";
				}
				, order:5 );

			// when
			var messageResultA		= hub.Pub( filterA, new MessageStruct( "m1" ) );
			var messageResultB		= hub.Pub( filterB, new MessageStruct( "m1" ) );
			var messageResult		= hub.Pub( new MessageStruct( "m1" ) );

			// then
			messageResultA.Str.ShouldBe( "m1sub1subFa2subA5subFa5subB5" );
			messageResultB.Str.ShouldBe( "m1sub1subFb5subA5subB5" );
			messageResult.Str.ShouldBe( "m1sub1subA5subB5" );
		};
	}
}
}