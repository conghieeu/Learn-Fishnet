using System;
using System.Collections.Generic;

public class AdminCommandArgs
{
	private Dictionary<string, string> args = new Dictionary<string, string>();

	public string? this[string key]
	{
		get
		{
			if (args.TryGetValue(key, out var value))
			{
				return value;
			}
			return null;
		}
	}

	public AdminCommandArgs(string arg)
	{
		string[] array = arg.Split('^');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[1] { '=' }, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length == 2)
			{
				args[array2[0]] = array2[1];
			}
		}
	}

	public int AsInt32(string key, int defaultValue)
	{
		string text = this[key];
		try
		{
			if (text == null)
			{
				return defaultValue;
			}
			return Convert.ToInt32(text);
		}
		catch
		{
			Logger.RError("Invalid Argument. input : " + text);
			return defaultValue;
		}
	}

	public long AsInt64(string key, int defaultValue)
	{
		string text = this[key];
		try
		{
			if (text == null)
			{
				return defaultValue;
			}
			return Convert.ToInt64(text);
		}
		catch
		{
			Logger.RError("Invalid Argument. input : " + text);
			return defaultValue;
		}
	}

	public bool AsBool(string key, bool defaultValue)
	{
		if (!bool.TryParse(this[key], out var result))
		{
			return defaultValue;
		}
		return result;
	}

	public string AsString(string key, string defaultValue)
	{
		string text = this[key];
		if (string.IsNullOrEmpty(text))
		{
			return defaultValue;
		}
		return text;
	}

	public float AsFloat(string key, float defaultValue)
	{
		string text = this[key];
		try
		{
			if (text == null)
			{
				return defaultValue;
			}
			return Convert.ToSingle(text);
		}
		catch
		{
			Logger.RError("Invalid Argument. input : " + text);
			return defaultValue;
		}
	}

	public T AsEnum<T>(string key, T defaultValue) where T : struct
	{
		if (!Enum.TryParse<T>(this[key], out var result))
		{
			return defaultValue;
		}
		return result;
	}
}
