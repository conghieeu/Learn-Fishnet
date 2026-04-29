using System;

namespace MMBehaviorTree
{
	public class RemoveBlackBoardParam : BehaviorAction
	{
		private string _key;

		public RemoveBlackBoardParam(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new Exception("Invalid parameter count");
			}
			_key = param[0];
		}

		public override IComposite Clone()
		{
			return new RemoveBlackBoardParam(new string[1] { _key });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BlackBoard.Remove(_key);
			return BehaviorResult.SUCCESS;
		}
	}
}
