using System;

[Flags]
public enum SkillFlags
{
	None = 1,
	IgnoreCoolTime = 2,
	IgnoreCC = 4,
	UseNextSkill = 8,
	IgnoreValidation = 0x10
}
