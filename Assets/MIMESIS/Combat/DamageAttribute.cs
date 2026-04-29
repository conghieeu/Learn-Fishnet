using ReluProtocol;

public class DamageAttribute
{
	public readonly long Damage;

	public readonly bool Critical;

	public readonly long GroggyValue;

	public void Convert2HitInfo(ref TargetHitInfo hitInfo)
	{
		hitInfo.damage = Damage;
	}

	public DamageAttribute(long damage, long groggyValue, bool critical)
	{
		Damage = damage;
		GroggyValue = groggyValue;
		Critical = critical;
	}
}
