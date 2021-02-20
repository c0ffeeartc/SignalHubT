using NSpec;
using Shouldly;
using SubH;

namespace Tests
{
public class descirbe_Pool : nspec
{
	private					void					test_Pool_Message		(  )
	{
		beforeEach = ()=>
		{
			Pool<Message1>.I		= new Pool<Message1>( () => new Message1(  ) );
		};

		it["Poolable isInPool"] = ()=>
		{
			var m1 = Pool<Message1>.I.Rent();
			m1.IsInPool.ShouldBe( false );

			Pool<Message1>.I.Repool(m1);
			m1.IsInPool.ShouldBe( true );

			Pool<Message1>.I.Rent();
			m1.IsInPool.ShouldBe( false );
		};
	}
}
}