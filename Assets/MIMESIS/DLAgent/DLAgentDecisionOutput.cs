using System;

namespace DLAgent
{
	public class DLAgentDecisionOutput : IEquatable<DLAgentDecisionOutput>
	{
		public DLDecisionType Decision;

		public long DecisionTime;

		public int EmoteMasterID;

		public long EmoteTime;

		public VCreature? TargetActor;

		public bool EmoteInfoChanged { get; private set; }

		public void Reset()
		{
			Decision = DLDecisionType.None;
			DecisionTime = 0L;
			EmoteMasterID = 0;
			EmoteTime = 0L;
			TargetActor = null;
			EmoteInfoChanged = false;
		}

		public void ResetEmoteInfoChanged()
		{
			EmoteInfoChanged = false;
		}

		public void SetEmoteInfoChanged()
		{
			EmoteInfoChanged = true;
		}

		public DLAgentDecisionOutput Clone()
		{
			return new DLAgentDecisionOutput
			{
				Decision = Decision,
				DecisionTime = DecisionTime,
				EmoteMasterID = EmoteMasterID,
				EmoteTime = EmoteTime,
				TargetActor = TargetActor,
				EmoteInfoChanged = EmoteInfoChanged
			};
		}

		public bool Equals(DLAgentDecisionOutput other)
		{
			if (other == null)
			{
				return false;
			}
			if (Decision == other.Decision && TargetActor == other.TargetActor && EmoteMasterID == other.EmoteMasterID)
			{
				return EmoteTime == other.EmoteTime;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as DLAgentDecisionOutput);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Decision, TargetActor, EmoteMasterID, EmoteTime);
		}
	}
}
