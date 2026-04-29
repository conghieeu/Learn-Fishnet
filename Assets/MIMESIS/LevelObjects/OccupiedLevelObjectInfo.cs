using ReluProtocol;

public class OccupiedLevelObjectInfo : StateLevelObjectInfo
{
	public int OccupiedActorID { get; private set; }

	public OccupiedLevelObjectInfo(LevelObject origin)
		: base(LevelObjectType.Occupied, origin)
	{
		OccupiedActorID = 0;
	}

	public override LevelObjectInfo toLevelObjectInfo()
	{
		return new LevelObjectInfo
		{
			levelObjectID = ID,
			prevState = base.PrevState,
			CurrentState = base.CurrentState,
			OccupiedActorID = OccupiedActorID,
			position = Pos.toPosWithRot(0f)
		};
	}

	public bool CanChangeState(int state, int actorID)
	{
		if (!CanChangeState(state))
		{
			return false;
		}
		if (OccupiedActorID != 0 && OccupiedActorID != actorID)
		{
			return false;
		}
		return true;
	}

	public void ChangeOccupy(int actorID)
	{
		OccupiedActorID = actorID;
	}
}
