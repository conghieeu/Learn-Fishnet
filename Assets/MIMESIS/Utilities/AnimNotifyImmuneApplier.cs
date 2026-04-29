public class AnimNotifyImmuneApplier : AnimNotifyInfo
{
	public readonly bool Flag;

	public AnimNotifyImmuneApplier(double start, string actionName, bool flag)
		: base(AnimNotifyType.ImmuneApplier, start, actionName, 0)
	{
		Flag = flag;
	}
}
