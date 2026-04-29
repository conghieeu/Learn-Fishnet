using System;

[Flags]
public enum UntargetReason
{
	None = 0,
	Normal = 1,
	Invisible = 2,
	AI = 4,
	UntargetableObject = 8,
	Spawn = 0x20,
	SkillAnimation = 0x40
}
