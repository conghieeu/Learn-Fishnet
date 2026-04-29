using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public class ShuffleSelector : GroupComposite
{
	private bool _isShuffled;

	public ShuffleSelector(ImmutableArray<IComposite> children, OnBeforeExecuteDelegate? onBeforeExecute)
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
		int num = state.GetSavedChildIndex(this);
		if (nestedResult != BehaviorResult.NONE && num == 100000)
		{
			return BehaviorResult.FAILURE;
		}
		List<IComposite> list = BehaviorList.ToList();
		if (!_isShuffled)
		{
			list.Shuffle();
			_isShuffled = true;
		}
		if (num == 100000)
		{
			Push(state);
			num = 0;
		}
		BehaviorResult behaviorResult = BehaviorResult.FAILURE;
		state.SnapBTComposite("ShuffleSelector", null, "Composite");
		for (int i = num; i < BehaviorList.Length; i++)
		{
			IComposite composite = BehaviorList[i];
			behaviorResult = ((nestedResult == BehaviorResult.NONE) ? composite.Behave(state) : nestedResult);
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
		return new ShuffleSelector(list.ToImmutableArray(), OnBeforeExecute);
	}
}
