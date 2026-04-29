using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

public class Selector : GroupComposite
{
	public Selector(ImmutableArray<IComposite> children, OnBeforeExecuteDelegate? onBeforeExecute)
		: base(children, onBeforeExecute)
	{
	}

	public sealed override BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE)
	{
		OnBeforeExecuteDelegate? onBeforeExecute = OnBeforeExecute;
		if (onBeforeExecute != null && !onBeforeExecute(state))
		{
			return BehaviorResult.FAILURE;
		}
		int savedChildIndex = state.GetSavedChildIndex(this);
		if (nestedResult != BehaviorResult.NONE && savedChildIndex == 100000)
		{
			Debug.LogWarning("NestedResult is not NONE in Selector. " + GetType().Name);
			return BehaviorResult.FAILURE;
		}
		if (nestedResult == BehaviorResult.NONE && savedChildIndex == 100000)
		{
			Push(state);
			savedChildIndex = 0;
		}
		else
		{
			savedChildIndex++;
		}
		if (savedChildIndex >= BehaviorList.Length)
		{
			Pop(state);
			return nestedResult;
		}
		if (nestedResult == BehaviorResult.SUCCESS)
		{
			Pop(state);
			return BehaviorResult.SUCCESS;
		}
		BehaviorResult behaviorResult = BehaviorResult.FAILURE;
		state.SnapBTComposite("Selector", null, "Composite");
		for (int i = savedChildIndex; i < BehaviorList.Length; i++)
		{
			behaviorResult = BehaviorList[i].Behave(state);
			switch (behaviorResult)
			{
			case BehaviorResult.RUNNING:
				state.SaveChildIndex(this, i);
				return BehaviorResult.RUNNING;
			default:
				continue;
			case BehaviorResult.SUCCESS:
				break;
			}
			break;
		}
		Pop(state);
		return behaviorResult;
	}

	public override IComposite Clone()
	{
		List<IComposite> list = new List<IComposite>();
		ImmutableArray<IComposite>.Enumerator enumerator = BehaviorList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			IComposite current = enumerator.Current;
			list.Add(current.Clone());
		}
		return new Selector(list.ToImmutableArray(), OnBeforeExecute);
	}
}
