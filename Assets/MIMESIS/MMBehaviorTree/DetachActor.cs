namespace MMBehaviorTree
{
	public class DetachActor : BehaviorAction
	{
		public DetachActor(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new DetachActor(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (!behaviorTreeState.Self.AttachControlUnit.IsAttaching() && !behaviorTreeState.Self.AttachControlUnit.IsAttached())
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.Self.AttachControlUnit.IsAttaching())
			{
				if (!behaviorTreeState.Self.AttachControlUnit.DetachByRequest(behaviorTreeState.Self.ObjectID, 0, DetachReason.ActiveByCaster))
				{
					return BehaviorResult.FAILURE;
				}
			}
			else if (behaviorTreeState.Self.AttachControlUnit.IsAttached())
			{
				int attachedActorID = behaviorTreeState.Self.AttachControlUnit.AttachedActorID;
				if (attachedActorID == 0)
				{
					return BehaviorResult.FAILURE;
				}
				VActor vActor = behaviorTreeState.Self.VRoom.FindActorByObjectID(attachedActorID);
				if (vActor == null)
				{
					return BehaviorResult.FAILURE;
				}
				if (!vActor.AttachControlUnit.DetachByRequest(behaviorTreeState.Self.ObjectID, behaviorTreeState.Self.ObjectID, DetachReason.ActiveByVictim))
				{
					return BehaviorResult.FAILURE;
				}
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
