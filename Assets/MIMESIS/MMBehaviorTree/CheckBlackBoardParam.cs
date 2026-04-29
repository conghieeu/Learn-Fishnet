using System;

namespace MMBehaviorTree
{
	public class CheckBlackBoardParam : Conditional
	{
		private string _dataType;

		private string _key;

		private string _comparisonType;

		private string _value;

		public CheckBlackBoardParam(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 4)
			{
				throw new Exception("Invalid parameter count");
			}
			_dataType = param[0];
			_key = param[1];
			_comparisonType = param[2];
			_value = param[3];
		}

		public override IComposite Clone()
		{
			return new CheckBlackBoardParam(this, new string[4] { _dataType, _key, _comparisonType, _value });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if (!(state as BehaviorTreeState).BlackBoard.Check(_dataType, _key, _comparisonType, _value))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
