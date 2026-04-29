using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BlackBoard
{
	private Dictionary<string, (BlackBoardDataType, string)> _kvPairs = new Dictionary<string, (BlackBoardDataType, string)>();

	public (BlackBoardDataType, string)? GetValue(string key)
	{
		if (_kvPairs.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public bool Check(string dataTypeStr, string key, string compareTypeStr, string value)
	{
		if (!Enum.TryParse<BlackBoardDataType>(dataTypeStr, ignoreCase: true, out var result))
		{
			return false;
		}
		if (!Enum.TryParse<BTParamCompareType>(compareTypeStr, ignoreCase: true, out var result2))
		{
			return false;
		}
		if (!_kvPairs.TryGetValue(key, out var value2))
		{
			return false;
		}
		var (blackBoardDataType, text) = value2;
		if (blackBoardDataType != result)
		{
			return false;
		}
		switch (result)
		{
		case BlackBoardDataType.Time:
		{
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (!long.TryParse(value, out var result5))
			{
				return false;
			}
			if (!long.TryParse(text, out var result6))
			{
				return false;
			}
			result5 += currentTickMilliSec;
			return BTStateUtil.CompareLong(result2, result6, result5);
		}
		case BlackBoardDataType.Bool:
		{
			if (!bool.TryParse(value, out var result9))
			{
				return false;
			}
			if (!bool.TryParse(text, out var result10))
			{
				return false;
			}
			return BTStateUtil.CompareBool(result2, result10, result9);
		}
		case BlackBoardDataType.Int:
		{
			if (!int.TryParse(value, out var result7))
			{
				return false;
			}
			if (!int.TryParse(text, out var result8))
			{
				return false;
			}
			return BTStateUtil.CompareInt(result2, result8, result7);
		}
		case BlackBoardDataType.Float:
		{
			if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result3))
			{
				return false;
			}
			if (!float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result4))
			{
				return false;
			}
			return BTStateUtil.CompareFloat(result2, result4, result3);
		}
		case BlackBoardDataType.String:
			return BTStateUtil.CompareString(result2, text, value);
		case BlackBoardDataType.Vector3:
		{
			Vector3 valueSrc = BTStateUtil.ParseVector3(text);
			Vector3 compareValue = BTStateUtil.ParseVector3(value);
			return BTStateUtil.CompareVector3(result2, valueSrc, compareValue);
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public bool SetValue(string key, string value, string dataTypeStr, bool overwrite = false)
	{
		if (!overwrite && _kvPairs.ContainsKey(key))
		{
			return false;
		}
		if (!Enum.TryParse<BlackBoardDataType>(dataTypeStr, ignoreCase: true, out var result))
		{
			return false;
		}
		if (result == BlackBoardDataType.Time)
		{
			if (!long.TryParse(value, out var result2))
			{
				return false;
			}
			value = (Hub.s.timeutil.GetCurrentTickMilliSec() + result2).ToString();
		}
		_kvPairs[key] = (result, value);
		return true;
	}

	public bool Remove(string key)
	{
		return _kvPairs.Remove(key);
	}

	public void Clear()
	{
		_kvPairs.Clear();
	}
}
