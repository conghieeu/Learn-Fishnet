using System;
using UnityEngine;

public abstract class BehaviorBlockingAction : IComposite
{
	public readonly string m_ActionName;

	public readonly string[]? m_BehaviorActionParams;

	public BehaviorBlockingAction(string[]? param)
	{
		m_ActionName = GetType().Name;
		m_BehaviorActionParams = param;
	}

	public abstract BehaviorResult Execute(IBehaviorTreeState state);

	public abstract bool IsEnd(IBehaviorTreeState state);

	public virtual BehaviorResult OnEnd(IBehaviorTreeState state)
	{
		return BehaviorResult.SUCCESS;
	}

	public override BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE)
	{
		bool flag = false;
		try
		{
			if (nestedResult != BehaviorResult.NONE)
			{
				Debug.LogWarning("NestedResult is not NONE in BehaviorBlockingAction " + m_ActionName);
				return BehaviorResult.FAILURE;
			}
			BehaviorResult result;
			if (!Push(state))
			{
				if (!IsEnd(state))
				{
					return BehaviorResult.RUNNING;
				}
				result = OnEnd(state);
				flag = true;
				return result;
			}
			state.SnapBTComposite(m_ActionName, m_BehaviorActionParams, "Action");
			result = Execute(state);
			if (result == BehaviorResult.RUNNING)
			{
				return result;
			}
			if (!IsEnd(state))
			{
				return BehaviorResult.RUNNING;
			}
			result = OnEnd(state);
			flag = true;
			return result;
		}
		catch (Exception ex)
		{
			BehaviorResult result = BehaviorResult.FAILURE;
			Debug.LogWarning($"ERROR BehaviorTree {m_ActionName} Exception {ex.Message}, {ex.Source}, {ex.TargetSite} {ex.StackTrace}");
			return result;
		}
		finally
		{
			if (flag)
			{
				Pop(state);
			}
		}
	}
}
