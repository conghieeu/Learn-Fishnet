public class GameActorDeadEventArgs : VActorEventArgs
{
	public int AttackerActorID;

	public readonly VCreature Victim;

	public GameActorDeadEventArgs(int attackerID, VCreature victim)
		: base(VActorEventType.Dead)
	{
		AttackerActorID = attackerID;
		Victim = victim;
	}
}
