public class AnimNotifyActivateAura : AnimNotifyInfo
{
	public readonly int AuraMasterID;

	public AnimNotifyActivateAura(double start, string actionName, int auraMasterID)
		: base(AnimNotifyType.ActivateAura, start, actionName, 0)
	{
		AuraMasterID = auraMasterID;
	}
}
