using Bifrost.Cooked;
using ReluProtocol;

namespace MMBehaviorTree
{
	public class IsSkillUsableRange : Conditional
	{
		public IsSkillUsableRange(IComposite composite, string[] param)
			: base(composite, param)
		{
		}

		public override IComposite Clone()
		{
			return new IsSkillUsableRange(this, new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Target == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.UsableSkillMasterID == 0)
			{
				return BehaviorResult.FAILURE;
			}
			PosWithRot position = behaviorTreeState.Target.Position;
			SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(behaviorTreeState.UsableSkillMasterID);
			if (skillInfo == null)
			{
				return BehaviorResult.FAILURE;
			}
			float allowRange = skillInfo.AllowRange;
			if (VWorldUtil.Distance(position, behaviorTreeState.Self.Position) < (double)allowRange)
			{
				return BehaviorResult.SUCCESS;
			}
			return BehaviorResult.FAILURE;
		}
	}
}
