using System;
using System.Collections.Generic;
using System.Text;

namespace StringExtension
{
	public static class StringAddins
	{
		public static string format(this string fmt, params object[] args)
		{
			return string.Format(fmt, args);
		}

		public static string join(this string seperator, string[] args)
		{
			return string.Join(seperator, args);
		}

		public static string join(this string seperator, List<string> args)
		{
			return string.Join(seperator, args.ToArray());
		}

		public static List<string> split(this string source, string seperator)
		{
			List<string> list = new List<string>();
			int num = 0;
			do
			{
				int num2 = source.IndexOf(seperator, num);
				if (num2 == -1)
				{
					break;
				}
				list.Add(source.Substring(num, num2 - num));
				num = num2 + seperator.Length;
			}
			while (num < source.Length);
			if (num <= source.Length)
			{
				list.Add(source.Substring(num));
			}
			return list;
		}

		public static string repeat(this string s, int n)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < n; i++)
			{
				stringBuilder.Append(s);
			}
			return stringBuilder.ToString();
		}

		public static int countOccurence(this string s, string pattern)
		{
			int num = 0;
			int startIndex = 0;
			do
			{
				int num2 = s.IndexOf(pattern, startIndex);
				if (num2 == -1)
				{
					return num;
				}
				num++;
				startIndex = num2 + pattern.Length;
			}
			while (num <= 100000);
			throw new Exception();
		}
	}
}
