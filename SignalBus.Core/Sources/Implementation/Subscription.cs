using System;

namespace SignalHubT
{
public sealed class Subscription<T> : ISubscription<T>
		where T : ISignalData
{
	private static			Int32					sCreationIndex			= 1;

	public					ISubscription<T>		Init					( Boolean hasFilter, Object filter, ActionRef<T> action, Int32 order )
	{
		HasFilter					= hasFilter;
		Filter						= filter;
		_action						= action;
		Order						= order;
		return this;
	}

	private					ActionRef<T>			_action;
	public					Boolean					HasFilter				{ get; private set; }
	public					Object					Filter					{ get; private set; }
	public					Int32					Order					{ get; private set; }
	public					Int32					CreationIndex			{ get; set; }
	public					Boolean					IsInPool				{ get; set; }

	public					void					Invoke					( ref T message )
	{
		_action.Invoke( ref message );
	}

	public int CompareTo(ISubscription<T> other)
	{
		if ( ReferenceEquals( this, other ) )
		{
			return 0;
		}
	
		if ( ReferenceEquals( null, other ) )
		{
			return 1;
		}

		return CompareToInternal( other );
	}

	public					int						CompareToInternal		( ISubscription<T> other )
	{
		Int32 result				= Order.CompareTo( other.Order );
		if ( result != 0 )
		{
			return result;
		}

		return CreationIndex.CompareTo( other.CreationIndex );
	}

	public					void					BeforeRent				(  )
	{
		CreationIndex				= ++sCreationIndex;
	}

	public					void					BeforeRepool			(  )
	{
		_action						= null;
		HasFilter					= false;
		Filter						= null;
		Order						= 0;
	}
}
}