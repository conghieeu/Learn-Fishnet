using System.Collections.Generic;
using Bifrost.Cooked;
using ReluProtocol;

public class HitInputData
{
	public List<TargetInfo> Targets { get; private set; } = new List<TargetInfo>();

	public SkillInfo SkillInfo { get; private set; }

	public SkillSequenceInfo SkillSequenceInfo { get; private set; }

	public HitInputData(SkillInfo skillInfo, SkillSequenceInfo seqInfo, List<TargetInfo> targetInfos)
	{
		Targets.AddRange(targetInfos);
		SkillInfo = skillInfo;
		SkillSequenceInfo = seqInfo;
	}
}
