public class AnimNotifyHitCheck : AnimNotifyInfo
{
	public readonly double Duration;

	public readonly string socketName;

	public AnimNotifyHitCheck(string actionName, double start, double duration, string socketName, int seqIndex)
		: base(AnimNotifyType.HitCheck, start, actionName, seqIndex)
	{
		Duration = duration;
		this.socketName = socketName;
	}
}
