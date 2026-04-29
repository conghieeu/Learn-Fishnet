using System;
using Bifrost.ConstEnum;

public class AbnormalType
{
	public static CCType GetCCFromString(string typeString)
	{
		CCType result = CCType.None;
		Enum.TryParse<CCType>(typeString, ignoreCase: true, out result);
		return result;
	}

	public static AbnormalStatsCategory GetAbnormalStatsCategoryFromString(string typeString)
	{
		AbnormalStatsCategory result = AbnormalStatsCategory.None;
		Enum.TryParse<AbnormalStatsCategory>(typeString, ignoreCase: true, out result);
		return result;
	}

	public static DispelType GetDispelTypeFromString(string typeString)
	{
		DispelType result = DispelType.None;
		Enum.TryParse<DispelType>(typeString, ignoreCase: true, out result);
		return result;
	}

	public static ImmuneType GetImmuneTypeFromString(string typeString)
	{
		ImmuneType result = ImmuneType.None;
		Enum.TryParse<ImmuneType>(typeString, ignoreCase: true, out result);
		return result;
	}
}
