using System;

namespace MMBehaviorTree
{
	public class SetTrace : BehaviorAction
	{
		private bool _flag;

		public SetTrace(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			if (!bool.TryParse(param[0], out _flag))
			{
				throw new ArgumentException("Invalid flag " + param[0]);
			}
		}

		public override IComposite Clone()
		{
			return new SetTrace(new string[1] { _flag.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).SetFixLookAtTarget(_flag);
			return BehaviorResult.SUCCESS;
		}
	}
}
