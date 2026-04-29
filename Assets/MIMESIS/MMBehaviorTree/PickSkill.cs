using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;

namespace MMBehaviorTree
{
	public class PickSkill : BehaviorAction
	{
		private BTPickSkillRule _pickRule;

		private int _skillMasterID;

		private bool _override;

		public PickSkill(string[] param)
			: base(param)
		{
			if (param.Length != 3)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			if (!Enum.TryParse<BTPickSkillRule>(param[0], out _pickRule))
			{
				_pickRule = BTPickSkillRule.Target;
			}
			if (!int.TryParse(param[1], out _skillMasterID))
			{
				_skillMasterID = 0;
			}
			if (!bool.TryParse(param[2], out _override))
			{
				_override = false;
			}
		}

		public override IComposite Clone()
		{
			return new PickSkill(new string[3]
			{
				_pickRule.ToString(),
				_skillMasterID.ToString(),
				_override.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			_ = behaviorTreeState.BTAIController;
			if (_skillMasterID != 0 && behaviorTreeState.UsableSkillMasterID == _skillMasterID)
			{
				return BehaviorResult.SUCCESS;
			}
			if (behaviorTreeState.UsableSkillMasterID != 0 && !_override)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.Target == null)
			{
				return BehaviorResult.FAILURE;
			}
			switch (_pickRule)
			{
			case BTPickSkillRule.ItemSkill:
			{
				int nonDefaultSkillMasterID = behaviorTreeState.BTSkillController.GetNonDefaultSkillMasterID();
				if (nonDefaultSkillMasterID == 0)
				{
					return BehaviorResult.FAILURE;
				}
				if (!behaviorTreeState.SetUsableSkillMasterID(nonDefaultSkillMasterID))
				{
					return BehaviorResult.FAILURE;
				}
				return BehaviorResult.SUCCESS;
			}
			case BTPickSkillRule.Target:
				if (_skillMasterID == 0)
				{
					return BehaviorResult.FAILURE;
				}
				if (!behaviorTreeState.BTSkillController.GetSkillSlots().Contains(_skillMasterID))
				{
					return BehaviorResult.FAILURE;
				}
				if (!behaviorTreeState.SetUsableSkillMasterID(_skillMasterID))
				{
					return BehaviorResult.FAILURE;
				}
				return BehaviorResult.SUCCESS;
			case BTPickSkillRule.Usable:
			{
				HashSet<int> skillSlots2 = behaviorTreeState.BTSkillController.GetSkillSlots();
				if (skillSlots2.Count == 0)
				{
					return BehaviorResult.FAILURE;
				}
				foreach (int item in skillSlots2)
				{
					SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(item);
					if (skillInfo != null && VWorldUtil.Distance(behaviorTreeState.Target.Position, behaviorTreeState.Self.Position) < (double)skillInfo.AllowRange)
					{
						return behaviorTreeState.SetUsableSkillMasterID(item) ? BehaviorResult.SUCCESS : BehaviorResult.FAILURE;
					}
				}
				return BehaviorResult.FAILURE;
			}
			case BTPickSkillRule.Random:
			{
				HashSet<int> skillSlots = behaviorTreeState.BTSkillController.GetSkillSlots();
				if (skillSlots.Count == 0)
				{
					return BehaviorResult.FAILURE;
				}
				int usableSkillMasterID = skillSlots.ElementAt(SimpleRandUtil.Next(0, skillSlots.Count));
				if (!behaviorTreeState.SetUsableSkillMasterID(usableSkillMasterID))
				{
					return BehaviorResult.FAILURE;
				}
				return BehaviorResult.SUCCESS;
			}
			default:
				throw new ArgumentException($"Invalid rule {_pickRule}");
			}
		}
	}
}
