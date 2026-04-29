using System;

namespace MMBehaviorTree
{
	public class SetBlackBoardParam : BehaviorAction
	{
		private string _dataType;

		private string _key;

		private string _value;

		private bool _overwrite;

		public SetBlackBoardParam(string[] param)
			: base(param)
		{
			if (param.Length != 4)
			{
				throw new Exception("Invalid parameter count");
			}
			_key = param[0];
			_value = param[1];
			_dataType = param[2];
			if (!bool.TryParse(param[3], out _overwrite))
			{
				throw new Exception("Invalid parameter count");
			}
		}

		public override IComposite Clone()
		{
			return new SetBlackBoardParam(new string[4]
			{
				_key,
				_value,
				_dataType,
				_overwrite.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if (!(state as BehaviorTreeState).BlackBoard.SetValue(_key, _value, _dataType, _overwrite))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
