using System.Collections.Immutable;
using Bifrost.AbnormalData;
using Bifrost.ConstEnum;

public class ImmuneElementInfo : IAbnormalElementInfo
{
	public readonly ImmuneType ImmuneType;

	public readonly StatType ImmutableStatType;

	public readonly MutableStatType MutableStatType;

	public readonly CCType CCType;

	public readonly ImmutableArray<int> TargetAbnormalMasterIDs;

	public ImmuneElementInfo(AbnormalData_element element)
		: base(element)
	{
		ImmuneType = AbnormalType.GetImmuneTypeFromString(element.type);
		switch (ImmuneType)
		{
		case ImmuneType.TargetImmutableStat:
			ImmutableStatType = StatUtil.GetStatTypeFromString(element.sub_type);
			break;
		case ImmuneType.TargetCC:
			CCType = AbnormalType.GetCCFromString(element.sub_type);
			break;
		case ImmuneType.TargetAbnormalID:
			TargetAbnormalMasterIDs = element.target_abnormal_id.ToImmutableArray();
			break;
		case ImmuneType.TargetMutableStat:
			MutableStatType = StatUtil.GetMutableStatTypeFromString(element.sub_type);
			break;
		}
	}

	public ImmuneElementInfo(string immuneStr)
		: base(AbnormalCategory.Immune)
	{
		string[] array = immuneStr.Split('|');
		if (array.Length != 2)
		{
			Logger.RError("Invalid ImmuneElementInfo. " + immuneStr);
			return;
		}
		ImmuneType = AbnormalType.GetImmuneTypeFromString(array[0]);
		switch (ImmuneType)
		{
		case ImmuneType.TargetImmutableStat:
			ImmutableStatType = StatUtil.GetStatTypeFromString(array[1]);
			break;
		case ImmuneType.TargetCC:
			CCType = AbnormalType.GetCCFromString(array[1]);
			break;
		case ImmuneType.TargetMutableStat:
			MutableStatType = StatUtil.GetMutableStatTypeFromString(array[1]);
			break;
		}
	}
}
