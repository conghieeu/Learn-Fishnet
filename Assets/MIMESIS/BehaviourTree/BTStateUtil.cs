using System;
using System.Globalization;
using UnityEngine;

public static class BTStateUtil
{
	public static Vector3 ParseVector3(string value)
	{
		string[] array = value.Split(',');
		if (array.Length != 3)
		{
			return Vector3.zero;
		}
		if (!float.TryParse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return Vector3.zero;
		}
		if (!float.TryParse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2))
		{
			return Vector3.zero;
		}
		if (!float.TryParse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var result3))
		{
			return Vector3.zero;
		}
		return new Vector3(result, result2, result3);
	}

	public static bool CompareLong(BTParamCompareType compareType, long valueSrc, long compareValue)
	{
		return compareType switch
		{
			BTParamCompareType.EQ => valueSrc == compareValue, 
			BTParamCompareType.NEQ => valueSrc != compareValue, 
			BTParamCompareType.GT => valueSrc > compareValue, 
			BTParamCompareType.LT => valueSrc < compareValue, 
			BTParamCompareType.GE => valueSrc >= compareValue, 
			BTParamCompareType.LE => valueSrc <= compareValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static bool CompareInt(BTParamCompareType compareType, int valueSrc, int compareValue)
	{
		return compareType switch
		{
			BTParamCompareType.EQ => valueSrc == compareValue, 
			BTParamCompareType.NEQ => valueSrc != compareValue, 
			BTParamCompareType.GT => valueSrc > compareValue, 
			BTParamCompareType.LT => valueSrc < compareValue, 
			BTParamCompareType.GE => valueSrc >= compareValue, 
			BTParamCompareType.LE => valueSrc <= compareValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static bool CompareDouble(BTParamCompareType compareType, double valueSrc, double compareValue)
	{
		return compareType switch
		{
			BTParamCompareType.EQ => valueSrc == compareValue, 
			BTParamCompareType.NEQ => valueSrc != compareValue, 
			BTParamCompareType.GT => valueSrc > compareValue, 
			BTParamCompareType.LT => valueSrc < compareValue, 
			BTParamCompareType.GE => valueSrc >= compareValue, 
			BTParamCompareType.LE => valueSrc <= compareValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static bool CompareFloat(BTParamCompareType compareType, float valueSrc, float compareValue)
	{
		return compareType switch
		{
			BTParamCompareType.EQ => valueSrc == compareValue, 
			BTParamCompareType.NEQ => valueSrc != compareValue, 
			BTParamCompareType.GT => valueSrc > compareValue, 
			BTParamCompareType.LT => valueSrc < compareValue, 
			BTParamCompareType.GE => valueSrc >= compareValue, 
			BTParamCompareType.LE => valueSrc <= compareValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static bool CompareBool(BTParamCompareType compareType, bool valueSrc, bool compareValue)
	{
		return compareType switch
		{
			BTParamCompareType.EQ => valueSrc == compareValue, 
			BTParamCompareType.NEQ => valueSrc != compareValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static bool CompareString(BTParamCompareType compareType, string valueSrc, string compareValue)
	{
		return compareType switch
		{
			BTParamCompareType.EQ => valueSrc == compareValue, 
			BTParamCompareType.NEQ => valueSrc != compareValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static bool CompareVector3(BTParamCompareType compareType, Vector3 valueSrc, Vector3 compareValue)
	{
		return compareType switch
		{
			BTParamCompareType.EQ => valueSrc == compareValue, 
			BTParamCompareType.NEQ => valueSrc != compareValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
