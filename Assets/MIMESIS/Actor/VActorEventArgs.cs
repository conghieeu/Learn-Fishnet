public abstract class VActorEventArgs
{
	public readonly VActorEventType EventType;

	protected VActorEventArgs(VActorEventType eventType)
	{
		EventType = eventType;
	}
}
