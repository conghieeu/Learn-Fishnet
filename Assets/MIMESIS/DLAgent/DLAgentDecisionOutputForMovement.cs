using System;
using UnityEngine;

namespace DLAgent
{
	public class DLAgentDecisionOutputForMovement : IEquatable<DLAgentDecisionOutputForMovement>
	{
		public DLDecisionType Decision;

		public long DecisionTime;

		public Vector3 TargetPos = Vector3.zero;

		public void Reset()
		{
			Decision = DLDecisionType.None;
			TargetPos = Vector3.zero;
		}

		public DLAgentDecisionOutputForMovement Clone()
		{
			return new DLAgentDecisionOutputForMovement
			{
				Decision = Decision,
				DecisionTime = DecisionTime,
				TargetPos = TargetPos
			};
		}

		public bool Equals(DLAgentDecisionOutputForMovement other)
		{
			if (other == null)
			{
				return false;
			}
			return Decision == other.Decision;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as DLAgentDecisionOutputForMovement);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Decision);
		}
	}
}
