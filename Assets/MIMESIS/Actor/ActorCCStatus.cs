using System;

[Flags]
public enum ActorCCStatus
{
	NORMAL = 0,
	STOP_ON_POS = 1,
	MOVE_BLOCKED = 2,
	DOWN = 4,
	AIRBORNE = 8,
	SKILL_BLOCKED = 0x10,
	AI_PAUSE = 0x20,
	DETACH = 0x40,
	CANT_HOLD_ITEM = 0x80
}
