using NSpec;
using Shouldly;
using SubH;

namespace Tests
{
public class descirbe_Pool : nspec
{
	private					void					before_each				(  )
	{
		Pool<ISubscription<Message1>>.I = new Pool<ISubscription<Message1>>( ()=> new Subscription<Message1>() );
		Pool<Message1>.I			= new Pool<Message1>( () => new Message1(  ) );
	}

	private					void					test_Pool_Message		(  )
	{
		it["Poolable isInPool"] = ()=>
		{
			var m1					= Pool<Message1>.I.Rent();
			m1.IsInPool.ShouldBe( false );

			Pool<Message1>.I.Repool(m1);
			m1.IsInPool.ShouldBe( true );

			Pool<Message1>.I.Rent();
			m1.IsInPool.ShouldBe( false );
		};
	}

	private					void					test_Pool_Subscription	(  )
	{
		it["Pool Subscription"] = ()=>
		{
			var s1					= Pool<ISubscription<Message1>>.I.Rent();
			s1.IsInPool.ShouldBe( false );

			Pool<ISubscription<Message1>>.I.Repool(s1);
			s1.IsInPool.ShouldBe( true );

			Pool<ISubscription<Message1>>.I.Rent();
			s1.IsInPool.ShouldBe( false );
		};
	}
}
}