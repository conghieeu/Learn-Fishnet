using System;
using ReluProtocol.Enum;

public class GELUseLevelObject : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int StageCount;

	public ActorType ActorType;

	public LevelObjectType LevelObjectType;

	public int FromState;

	public int ToState;

	public DateTime Timestamp;

	public GELUseLevelObject()
		: base(GELType.UseLevelObject)
	{
	}
}
