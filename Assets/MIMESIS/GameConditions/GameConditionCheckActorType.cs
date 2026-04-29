using Bifrost.ConstEnum;
using ReluProtocol.Enum;

public class GameConditionCheckActorType : BaseGameCondition
{
	public ActorType ActorType { get; private set; }

	public GameConditionCheckActorType(ActorType actorType)
		: base(DefCondition.CHECK_ACTOR_TYPE)
	{
		ActorType = actorType;
	}

	public override bool Correct(IGameCondition info)
	{
		if (info is GameConditionCheckActorType gameConditionCheckActorType)
		{
			return gameConditionCheckActorType.ActorType == ActorType;
		}
		return false;
	}

	public override bool IsComplete()
	{
		return false;
	}

	public override bool IsFailed()
	{
		return false;
	}

	public override bool Progress(int accumulateCount)
	{
		return true;
	}

	public override int GetCurrentCount()
	{
		return 0;
	}

	public override void Clone(ref IGameCondition? info)
	{
		info = new GameConditionCheckActorType(ActorType);
		info.SetIndex(base.ConditionIndex);
	}

	public override IGameCondition Clone()
	{
		GameConditionCheckActorType gameConditionCheckActorType = new GameConditionCheckActorType(ActorType);
		gameConditionCheckActorType.SetIndex(base.ConditionIndex);
		return gameConditionCheckActorType;
	}

	public override void ApplyCurrentCondition(int conditionValue)
	{
	}

	public override void ForceComplete()
	{
	}

	public override string ToString()
	{
		return $"{base.ObjType}:{ActorType}";
	}

	public override GameConditionParamType GetLinkedParamType()
	{
		return GameConditionParamType.Actors;
	}
}
