public class DebugLog : BehaviorAction
{
	public readonly string Message;

	public DebugLog(string[]? param)
		: base(param)
	{
		Message = string.Join(",", param ?? new string[0]);
	}

	public override IComposite Clone()
	{
		return new DebugLog(new string[1] { Message });
	}

	public override BehaviorResult Execute(IBehaviorTreeState state)
	{
		return BehaviorResult.FAILURE;
	}
}
