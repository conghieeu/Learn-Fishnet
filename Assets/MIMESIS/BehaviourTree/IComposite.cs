public abstract class IComposite
{
	public abstract BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE);

	public abstract IComposite Clone();

	protected bool Push(IBehaviorTreeState state)
	{
		return state.PushComposite(this);
	}

	protected bool Pop(IBehaviorTreeState state)
	{
		return state.PopComposite(this);
	}
}
