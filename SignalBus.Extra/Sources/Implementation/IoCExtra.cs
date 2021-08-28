namespace SignalBusT
{
public class IoCExtra : IIoCExtra
{
	public static			IIoCExtra					I						= new IoCExtra(  );

	public virtual			ISignalHub					CreateSignalHub				(  ) => new SignalHub(  );
	public virtual			ISignalHub					CreateSignalHubLocal			(  ) => new SignalHubLocal(  );
}
}