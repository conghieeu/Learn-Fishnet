using System;
using ReluProtocol.Enum;

public class GELActorDead : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int StageCount;

	public ActorType ActorType;

	public ReasonOfDeath reason;

	public int SkillMasterID;

	public DateTime Timestamp;

	public GELActorDead()
		: base(GELType.ActorDead)
	{
	}
}
