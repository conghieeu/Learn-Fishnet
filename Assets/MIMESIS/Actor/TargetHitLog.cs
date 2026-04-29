public class TargetHitLog
{
	public int HitCount;

	public long LastHitTimestamp;

	public TargetHitLog(int hitCount, long currentTick)
	{
		HitCount = hitCount;
		LastHitTimestamp = currentTick;
	}
}
