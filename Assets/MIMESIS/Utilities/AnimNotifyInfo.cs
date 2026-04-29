public abstract class AnimNotifyInfo
{
	public readonly double Start;

	public readonly AnimNotifyType Type;

	public string ActionName;

	public readonly int sequenceIndex;

	public AnimNotifyInfo(AnimNotifyType type, double start, string actionName, int sequenceIndex)
	{
		Type = type;
		Start = start;
		ActionName = actionName;
		this.sequenceIndex = sequenceIndex;
	}
}
