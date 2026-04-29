using System;

namespace MMBehaviorTree
{
	public class ChangeBT : BehaviorAction
	{
		public readonly string BTName;

		public ChangeBT(string[]? param)
			: base(param)
		{
			BTName = ((param != null && param.Length != 0) ? param[0] : string.Empty);
			if (string.IsNullOrEmpty(BTName))
			{
				throw new ArgumentException("ChangeBT: BTName is null or empty");
			}
		}

		public override IComposite Clone()
		{
			return new ChangeBT(new string[1] { BTName });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.HandleChangeBT(BTName);
			return BehaviorResult.SUCCESS;
		}
	}
}
