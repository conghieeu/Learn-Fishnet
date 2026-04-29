using ReluProtocol.Enum;

public class ApplyDamageArgs : VActorEventArgs
{
	public readonly VActor? Attacker;

	public readonly VActor Victim;

	public readonly MutableStatChangeCause MutableStatChangeCause;

	public readonly long Damage;

	public readonly string SkillName;

	public readonly HitType HitType;

	public readonly int SkillSequenceID;

	public readonly long GrogyValue;

	public readonly int SkillMasterID;

	public ApplyDamageArgs(VActor? attacker, VActor victim, MutableStatChangeCause mutableStatChangeCause, long damage, long groggyValue, int skillMasterID = 0, int skillSequenceID = 0, HitType hitType = HitType.None, string skillName = "")
		: base(VActorEventType.ApplyDamage)
	{
		Attacker = attacker;
		Victim = victim;
		MutableStatChangeCause = mutableStatChangeCause;
		Damage = damage;
		SkillName = skillName;
		SkillMasterID = skillMasterID;
		SkillSequenceID = skillSequenceID;
		HitType = hitType;
		GrogyValue = groggyValue;
	}
}
