using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class SimpleRandUtil
{
	private static Random SimpleRandomGenerator = new Random();

	private static long _lastTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

	public static int Next()
	{
		return SimpleRandomGenerator.Next();
	}

	public static int Next(int maxValue)
	{
		return SimpleRandomGenerator.Next(maxValue);
	}

	public static int Next(int minValue, int maxValue)
	{
		return SimpleRandomGenerator.Next(minValue, maxValue);
	}

	public static double Next(double minValue, double maxValue)
	{
		return SimpleRandomGenerator.NextDouble() * (maxValue - minValue) + minValue;
	}

	public static float Next(float minValue, float maxValue)
	{
		return (float)(SimpleRandomGenerator.NextDouble() * (double)(maxValue - minValue) + (double)minValue);
	}

	public static long Next(long minValue, long maxValue)
	{
		return (long)(SimpleRandomGenerator.NextDouble() * (double)(maxValue - minValue) + (double)minValue);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static string GenerateDigits(int length)
	{
		char[] source = new StringBuilder().Insert(0, "0123456789", length).ToString().ToCharArray();
		return string.Join("", source.OrderBy((char o) => Guid.NewGuid()).Take(length));
	}

	public static string GetNewRandString(int length)
	{
		StringBuilder stringBuilder = new StringBuilder(length);
		for (int i = 0; i < length; i++)
		{
			stringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[Next("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Length)]);
		}
		return stringBuilder.ToString();
	}

	public static bool IsSuccessPerTenThousand(int successRate)
	{
		return successRate >= GetRandomRatePerTenThousand();
	}

	public static int GetRandomRatePerTenThousand()
	{
		return Next(0, 10001);
	}
}
