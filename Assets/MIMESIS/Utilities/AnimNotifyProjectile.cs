public class AnimNotifyProjectile : AnimNotifyInfo
{
	public readonly string SocketName;

	public AnimNotifyProjectile(double start, string actionName, int seqIndex, string socketName)
		: base(AnimNotifyType.Projectile, start, actionName, seqIndex)
	{
		SocketName = socketName;
	}
}
