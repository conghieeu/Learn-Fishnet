using ReluProtocol;

public class StateLevelObjectInfo : ILevelObjectInfo
{
	public int PrevState { get; private set; }

	public int CurrentState { get; private set; }

	public StateLevelObjectInfo(LevelObject origin)
		: base(LevelObjectType.State, origin)
	{
		PrevState = origin.InitialState;
		CurrentState = origin.InitialState;
	}

	public StateLevelObjectInfo(LevelObjectType type, LevelObject origin)
		: base(type, origin)
	{
		PrevState = origin.InitialState;
		CurrentState = origin.InitialState;
	}

	public override LevelObjectInfo toLevelObjectInfo()
	{
		return new LevelObjectInfo
		{
			levelObjectID = ID,
			prevState = PrevState,
			CurrentState = CurrentState,
			OccupiedActorID = 0,
			position = Pos.toPosWithRot(0f)
		};
	}

	public bool CanChangeState(int state)
	{
		if (DataOrigin.DisableReverseState && state <= CurrentState)
		{
			return false;
		}
		if (!CheckActionRemainingCurrentCount(CurrentState, state))
		{
			return false;
		}
		return true;
	}

	public void ChangeState(int state)
	{
		PrevState = CurrentState;
		CurrentState = state;
	}
}
