using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMBehaviorTree
{
	public class PickLevelObject : BehaviorAction
	{
		private LevelObjectClientType _type;

		private BTTargetPickRule _pickRule;

		private AIRangeType _rangeType;

		private bool _checkHeight;

		private bool _reset;

		public PickLevelObject(string[] param)
			: base(param)
		{
			if (param.Length != 5)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			string text = param[0];
			if (!Enum.TryParse<LevelObjectClientType>(text, ignoreCase: true, out _type))
			{
				throw new ArgumentException("Invalid type " + text);
			}
			string text2 = param[1];
			if (!Enum.TryParse<BTTargetPickRule>(text2, out _pickRule))
			{
				throw new ArgumentException("Invalid rule " + text2);
			}
			if (!Enum.TryParse<AIRangeType>(param[2], ignoreCase: true, out _rangeType))
			{
				throw new ArgumentException("Invalid range type " + param[2]);
			}
			if (!bool.TryParse(param[3], out _checkHeight))
			{
				throw new ArgumentException("Invalid check height " + param[3]);
			}
			if (!bool.TryParse(param[4], out _reset))
			{
				throw new ArgumentException("Invalid reset " + param[4]);
			}
		}

		public override IComposite Clone()
		{
			return new PickLevelObject(new string[5]
			{
				_type.ToString(),
				_pickRule.ToString(),
				_rangeType.ToString(),
				_checkHeight.ToString(),
				_reset.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (!_reset && behaviorTreeState.TargetLevelObject != null)
			{
				return BehaviorResult.SUCCESS;
			}
			List<ILevelObjectInfo> list = behaviorTreeState.Self.VRoom.FindLevelObjectsByType(behaviorTreeState.Self.PositionVector, _type, _checkHeight, _rangeType);
			if (list.Count == 0)
			{
				return BehaviorResult.FAILURE;
			}
			ILevelObjectInfo levelObjectInfo = null;
			levelObjectInfo = _pickRule switch
			{
				BTTargetPickRule.Random => list[SimpleRandUtil.Next(0, list.Count)], 
				BTTargetPickRule.MinDistance => list[0], 
				BTTargetPickRule.MaxDistance => list[list.Count - 1], 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			if (!behaviorTreeState.Self.VRoom.FindNearestPoly(levelObjectInfo.Pos, out var nearestPos))
			{
				behaviorTreeState.SetTargetLevelObject(null);
				return BehaviorResult.FAILURE;
			}
			if (!behaviorTreeState.Self.VRoom.FindPath(behaviorTreeState.Self.PositionVector, nearestPos, out List<Vector3> _))
			{
				behaviorTreeState.SetTargetLevelObject(null);
				return BehaviorResult.FAILURE;
			}
			behaviorTreeState.SetTargetLevelObject(levelObjectInfo);
			return BehaviorResult.SUCCESS;
		}
	}
}
