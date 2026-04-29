public class ApplyHealArgs : VActorEventArgs
{
	public readonly VActor? Healer;

	public readonly VActor Victim;

	public readonly MutableStatChangeCause mutableStatChangeCause;

	public readonly long HealAmount;

	public readonly int SkillSequenceID;

	public ApplyHealArgs(VActor? healer, VActor victim, MutableStatChangeCause mutableStatChangeCause, long heal, int skillSequenceID = 0)
		: base(VActorEventType.ApplyHeal)
	{
		Healer = healer;
		Victim = victim;
		this.mutableStatChangeCause = mutableStatChangeCause;
		HealAmount = heal;
		SkillSequenceID = skillSequenceID;
	}
}
