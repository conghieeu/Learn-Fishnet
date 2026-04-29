using System.Collections.Immutable;
using Bifrost.AbnormalData;
using Bifrost.ConstEnum;

public class DispelElementInfo : IAbnormalElementInfo
{
	public readonly DispelType DispelType;

	public readonly StatType ImmutableStatType;

	public readonly MutableStatType MutableStatType;

	public readonly CCType CCType;

	public readonly ImmutableArray<int> AbnormalMasterIDs;

	public DispelElementInfo(AbnormalData_element element)
		: base(element)
	{
		DispelType = AbnormalType.GetDispelTypeFromString(element.type);
		switch (DispelType)
		{
		case DispelType.TargetImmutableStat:
			ImmutableStatType = StatUtil.GetStatTypeFromString(element.sub_type);
			break;
		case DispelType.TargetCC:
			CCType = AbnormalType.GetCCFromString(element.sub_type);
			break;
		case DispelType.Dot:
			MutableStatType = StatUtil.GetMutableStatTypeFromString(element.sub_type);
			break;
		case DispelType.AbnormalID:
			AbnormalMasterIDs = element.target_abnormal_id.ToImmutableArray();
			break;
		default:
			Logger.RError($"Invalid DispelType. {DispelType}");
			break;
		case DispelType.ALL:
		case DispelType.AllCC:
		case DispelType.AllStats:
			break;
		}
	}
}
