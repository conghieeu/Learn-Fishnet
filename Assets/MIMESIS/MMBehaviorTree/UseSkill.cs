using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;
using ReluProtocol.Enum;

namespace MMBehaviorTree
{
	public class UseSkill : BehaviorBlockingAction
	{
		public UseSkill(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new UseSkill(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.UsableSkillMasterID == 0)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.Self.CanAction(VActorActionType.Skill) != MsgErrorCode.Success)
			{
				return BehaviorResult.FAILURE;
			}
			SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(behaviorTreeState.UsableSkillMasterID);
			if (skillInfo == null)
			{
				return BehaviorResult.FAILURE;
			}
			List<VCreature> targetActors = new List<VCreature>();
			targetActors.Add(behaviorTreeState.Target);
			if (skillInfo.TargetCount > 1)
			{
				behaviorTreeState.BTAIController.IterateAggroActorOrderByPoint(delegate(VCreature actor)
				{
					if (targetActors.Count < skillInfo.TargetCount && !targetActors.Contains(actor))
					{
						targetActors.Add(actor);
					}
				});
			}
			long skillSyncID = 0L;
			bool num = behaviorTreeState.BTSkillController.UseSkillReqByAI(behaviorTreeState.Target, (targetActors.Count > 1) ? targetActors.Select((VCreature x) => x.PositionVector).ToList() : null, behaviorTreeState.UsableSkillMasterID, ref skillSyncID, SkillFlags.None);
			behaviorTreeState.SetUsingSkillSyncID(skillSyncID);
			if (!num)
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}

		public override bool IsEnd(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.BTSkillController.IsUsingSkillNow())
			{
				if (behaviorTreeState.GetUsingSkillSyncID() == behaviorTreeState.BTSkillController.GetCurrentSkillSyncID())
				{
					return false;
				}
				Logger.RError($"SkillSyncID is not matched on useSkill. monster MasterID : {behaviorTreeState.Self.MasterID}, skillSyncID : {behaviorTreeState.GetUsingSkillSyncID()}, currentSkillSyncID : {behaviorTreeState.BTSkillController.GetCurrentSkillSyncID()}");
				behaviorTreeState.SetUsableSkillMasterID(0);
				behaviorTreeState.SetUsingSkillSyncID(0L);
				return true;
			}
			behaviorTreeState.SetUsableSkillMasterID(0);
			behaviorTreeState.SetUsingSkillSyncID(0L);
			return true;
		}
	}
}
