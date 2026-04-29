using System;

public class StatUtil
{
	public static StatType GetStatTypeFromString(string typeString)
	{
		StatType result = StatType.Invalid;
		Enum.TryParse<StatType>(typeString, ignoreCase: true, out result);
		return result;
	}

	public static MutableStatType GetMutableStatTypeFromString(string typeString)
	{
		MutableStatType result = MutableStatType.Invalid;
		Enum.TryParse<MutableStatType>(typeString, ignoreCase: true, out result);
		return result;
	}
}
