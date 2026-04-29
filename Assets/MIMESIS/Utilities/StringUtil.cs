using System;
using System.Globalization;

public static class StringUtil
{
	public static string ConvertSnakeToPascal(string snakeCase)
	{
		return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(snakeCase.Replace("_", " ").ToLower()).Replace(" ", "");
	}

	public static bool ConvertStringToEnum<T>(string input, out T result) where T : struct, Enum
	{
		if (Enum.TryParse<T>(ConvertSnakeToPascal(input), ignoreCase: true, out result))
		{
			return true;
		}
		return false;
	}
}
