namespace SignalHubT
{
public class IoCExtra : IIoCExtra
{
	public static			IIoCExtra				I						= new IoCExtra(  );

	public virtual			ISignalHub				GetSignalHubStatic		(  )
	{
		if (SignalHub.I == null)
		{
			SignalHub.I = new SignalHub(  );
		}
		return SignalHub.I;
	}

	public virtual			ISignalHub				CreateSignalHubLocal	(  ) => new SignalHubLocal(  );
}
}