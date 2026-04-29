using System;

public class ActorListChangeEventArgs : EventArgs
{
	public readonly ActorListEventType EventType;

	public ActorListChangeEventArgs(ActorListEventType eventType)
	{
		EventType = eventType;
	}
}
