using System;

namespace ReluProtocol.Enum
{
	[Flags]
	public enum SkillVictimType
	{
		None = 0,
		Self = 1,
		Player = 2,
		Party = 4,
		Team = 8,
		Ally = 0x10,
		Enemy = 0x20,
		Neutral = 0x40,
		All = int.MaxValue
	}
}
