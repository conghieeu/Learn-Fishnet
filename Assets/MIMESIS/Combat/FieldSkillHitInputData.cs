using System.Collections.Generic;
using Bifrost.Cooked;
using ReluProtocol;

public class FieldSkillHitInputData
{
	public FieldSkillInfo FieldSkillInfo { get; private set; }

	public int FieldSkillMemberIndex { get; private set; }

	public SkillSequenceInfo SkillSequenceInfo { get; private set; }

	public List<TargetInfo> Targets { get; private set; } = new List<TargetInfo>();

	public FieldSkillHitInputData(FieldSkillInfo fieldSkillInfo, int fieldSkillMemberIndex, SkillSequenceInfo seqInfo)
	{
		FieldSkillInfo = fieldSkillInfo;
		FieldSkillMemberIndex = fieldSkillMemberIndex;
		SkillSequenceInfo = seqInfo;
	}
}
