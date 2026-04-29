using System.Collections.Immutable;

public abstract class GroupComposite : IComposite
{
	protected readonly ImmutableArray<IComposite> BehaviorList;

	public readonly OnBeforeExecuteDelegate? OnBeforeExecute;

	protected GroupComposite(ImmutableArray<IComposite> behaviorList, OnBeforeExecuteDelegate? onBeforeExecute)
	{
		BehaviorList = behaviorList;
		OnBeforeExecute = onBeforeExecute;
	}

	protected GroupComposite(IComposite singleChildComposite, OnBeforeExecuteDelegate? onBeforeExecute)
	{
		BehaviorList = ImmutableArray.Create(singleChildComposite);
		OnBeforeExecute = onBeforeExecute;
	}
}
