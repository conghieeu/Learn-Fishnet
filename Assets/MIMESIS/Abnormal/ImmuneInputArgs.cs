using System.Collections.Generic;
using Bifrost.ConstEnum;

public class ImmuneInputArgs : AbnormalCommonInputArgs
{
	public ImmuneType ImmuneType;

	public StatType ImmutableStatType;

	public MutableStatType MutableStatType;

	public CCType CCType;

	public List<int>? TargetSkillMasterIDs;
}
