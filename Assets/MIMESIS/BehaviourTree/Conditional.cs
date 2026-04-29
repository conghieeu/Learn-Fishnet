using System;
using UnityEngine;

public abstract class Conditional : GroupComposite
{
	public readonly string m_ActionName;

	public readonly string[]? m_ActionParams;

	private BehaviorResult _judgedResult;

	public Conditional(IComposite children, string[]? param)
		: base(children, null)
	{
		m_ActionName = GetType().Name;
		m_ActionParams = param;
	}

	public abstract BehaviorResult Execute(IBehaviorTreeState state);

	public sealed override BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE)
	{
		state.SnapBTComposite(m_ActionName, m_ActionParams, "Condition");
		if (nestedResult != BehaviorResult.NONE)
		{
			Debug.LogWarning("NestedResult is not NONE in Conditional " + m_ActionName);
			return BehaviorResult.FAILURE;
		}
		if (_judgedResult == BehaviorResult.NONE)
		{
			BehaviorResult behaviorResult;
			try
			{
				behaviorResult = Execute(state);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Condition Node Exception. Message: {ex.Message}, Source: {ex.Source}, TargetSite: {ex.TargetSite}, StackTrace: {ex.StackTrace}");
				behaviorResult = BehaviorResult.FAILURE;
			}
			switch (behaviorResult)
			{
			case BehaviorResult.RUNNING:
				Debug.LogError("It's not allowed to return RUNNING in Condition Node " + m_ActionName);
				return BehaviorResult.FAILURE;
			case BehaviorResult.FAILURE:
				return behaviorResult;
			}
		}
		if (BehaviorList.Length > 1)
		{
			Debug.LogError("Condition Node must have only one child node. " + m_ActionName);
			return BehaviorResult.FAILURE;
		}
		return BehaviorList[0].Behave(state);
	}
}
