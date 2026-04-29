using System;

public abstract class BehaviorAction : IComposite
{
	public readonly string m_ActionName;

	public readonly string[]? m_BehaviorActionParams;

	public BehaviorAction(string[]? param)
	{
		m_ActionName = GetType().Name;
		m_BehaviorActionParams = param;
	}

	public abstract BehaviorResult Execute(IBehaviorTreeState state);

	public sealed override BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE)
	{
		BehaviorResult behaviorResult;
		try
		{
			if (nestedResult != BehaviorResult.NONE)
			{
				Logger.RWarn("NestedResult is not NONE in BehaviorAction " + m_ActionName);
				return BehaviorResult.FAILURE;
			}
			state.SnapBTComposite(m_ActionName, m_BehaviorActionParams, "Action");
			Push(state);
			behaviorResult = Execute(state);
		}
		catch (Exception ex)
		{
			behaviorResult = BehaviorResult.FAILURE;
			Logger.RWarn($"ERROR BehaviorTree {m_ActionName} Exception({ex.Message}, {ex.Source}, {ex.TargetSite}) {ex.StackTrace}");
		}
		if (behaviorResult == BehaviorResult.RUNNING)
		{
			Logger.RWarn("It's not allowed to return RUNNING in BehaviorAction " + m_ActionName);
			behaviorResult = BehaviorResult.FAILURE;
		}
		Pop(state);
		return behaviorResult;
	}
}
