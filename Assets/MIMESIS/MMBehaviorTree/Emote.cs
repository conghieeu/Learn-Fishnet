using System;
using System.Globalization;
using DLAgent;

namespace MMBehaviorTree
{
	public class Emote : BehaviorBlockingAction
	{
		private BTEmoteType _btEmoteType;

		private float _btEmoteMinDuration;

		private float _btEmoteMaxDuration;

		public Emote(string[] param)
			: base(param)
		{
			if (param.Length != 3)
			{
				throw new ArgumentException("PickTarget needs a rule to Emote");
			}
			if (!Enum.TryParse<BTEmoteType>(param[0], out _btEmoteType))
			{
				throw new ArgumentException("Invalid rule " + param[0]);
			}
			if (!float.TryParse(param[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _btEmoteMinDuration))
			{
				throw new ArgumentException("Invalid min duration " + param[1]);
			}
			if (!float.TryParse(param[2], NumberStyles.Any, CultureInfo.InvariantCulture, out _btEmoteMaxDuration))
			{
				throw new ArgumentException("Invalid max duration " + param[2]);
			}
		}

		public override IComposite Clone()
		{
			return new Emote(new string[3]
			{
				_btEmoteType.ToString(),
				_btEmoteMinDuration.ToString(),
				_btEmoteMaxDuration.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			AIController bTAIController = behaviorTreeState.BTAIController;
			switch (_btEmoteType)
			{
			case BTEmoteType.Invalid:
				return BehaviorResult.FAILURE;
			case BTEmoteType.Random:
			{
				int randomEmoteMasterID = Hub.s.tableman.emote.GetRandomEmoteMasterID();
				if (randomEmoteMasterID == 0)
				{
					return BehaviorResult.FAILURE;
				}
				behaviorTreeState.Self.EmotionControlUnit.OnEmotion(randomEmoteMasterID, behaviorTreeState.Self.Position, 0);
				return BehaviorResult.RUNNING;
			}
			case BTEmoteType.DL:
			{
				bTAIController.ToggleDLAgent(flag: true);
				if (!bTAIController.GetDLAgentDecisionResult(out DLAgentDecisionOutput output))
				{
					return BehaviorResult.FAILURE;
				}
				if (output == null)
				{
					return BehaviorResult.FAILURE;
				}
				if (output.Decision != DLDecisionType.EmoteRespond)
				{
					return BehaviorResult.FAILURE;
				}
				if (output.EmoteMasterID == 0)
				{
					return BehaviorResult.FAILURE;
				}
				if (output.TargetActor != null)
				{
					behaviorTreeState.BTMovementController.TurnToTargetPos(output.TargetActor.PositionVector);
				}
				int emoteMasterID = output.EmoteMasterID;
				if (Hub.s.tableman.emote.TryGetRandomEmoteInSameGroup(emoteMasterID, out MMEmoteTable.Emote emote) && emote != null)
				{
					emoteMasterID = emote.emoteMasterID;
				}
				behaviorTreeState.Self.EmotionControlUnit.OnEmotion(emoteMasterID, behaviorTreeState.Self.Position, 0);
				output.ResetEmoteInfoChanged();
				return BehaviorResult.RUNNING;
			}
			default:
				return BehaviorResult.FAILURE;
			}
		}

		public override bool IsEnd(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			AIController bTAIController = behaviorTreeState.BTAIController;
			if (_btEmoteType == BTEmoteType.Random)
			{
				if (behaviorTreeState.Self.EmotionControlUnit.PlayedEmotionMasterID == 0)
				{
					return true;
				}
			}
			else if (_btEmoteType == BTEmoteType.DL)
			{
				if (!bTAIController.GetDLAgentDecisionResult(out DLAgentDecisionOutput output))
				{
					return true;
				}
				if (output == null)
				{
					return true;
				}
				if (output.Decision != DLDecisionType.EmoteRespond)
				{
					return true;
				}
				if (output.EmoteInfoChanged)
				{
					if (output.EmoteMasterID == 0 || output.TargetActor == null)
					{
						return true;
					}
					behaviorTreeState.BTMovementController.TurnToTargetPos(output.TargetActor.PositionVector);
					behaviorTreeState.Self.EmotionControlUnit.OnEmotion(output.EmoteMasterID, behaviorTreeState.Self.Position, 0);
					output.ResetEmoteInfoChanged();
				}
			}
			if ((float)(Hub.s.timeutil.GetCurrentTickMilliSec() - behaviorTreeState.Self.EmotionControlUnit.EmotionStartTime) < _btEmoteMinDuration * 1000f)
			{
				return false;
			}
			if ((float)(Hub.s.timeutil.GetCurrentTickMilliSec() - behaviorTreeState.Self.EmotionControlUnit.EmotionStartTime) > _btEmoteMaxDuration * 1000f)
			{
				return true;
			}
			return SimpleRandUtil.Next(0, 101) < 50;
		}

		public override BehaviorResult OnEnd(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).Self.EmotionControlUnit.OnCancelEmotion();
			return base.OnEnd(state);
		}
	}
}
