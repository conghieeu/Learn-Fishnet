public sealed class ApplyCrowdControlArgs : VActorEventArgs
{
	public int AttackerActorID;

	public readonly VCreature Victim;

	public ApplyCrowdControlArgs(int attackerID, VCreature victim)
		: base(VActorEventType.ApplyCrowdControl)
	{
		AttackerActorID = attackerID;
		Victim = victim;
	}
}
