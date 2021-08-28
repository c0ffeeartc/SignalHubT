namespace SignalBusT
{
public interface IPool<T> where T : IPoolable
{
	T						Rent					(  );
	void					Repool					( T poolable );
}
}