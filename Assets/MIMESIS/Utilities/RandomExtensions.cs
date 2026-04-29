using System;
using System.Collections.Generic;

public static class RandomExtensions
{
	public static bool TryPickRandom<T>(this T[] array, out T pick) where T : class
	{
		if (array.Length == 0)
		{
			pick = null;
			return false;
		}
		int num = new Random().Next(array.Length);
		pick = array[num];
		return true;
	}

	public static bool TryPickRandom<T>(this IList<T> list, out T pick) where T : class
	{
		if (list.Count == 0)
		{
			pick = null;
			return false;
		}
		int index = new Random().Next(list.Count);
		pick = list[index];
		return true;
	}

	public static bool TryPickRandom<T>(this T[] array, Func<T, int> probSelector, out T pick) where T : class
	{
		pick = null;
		if (array.Length == 0)
		{
			return false;
		}
		int num = 0;
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			int val;
			try
			{
				val = probSelector(array[i]);
			}
			catch
			{
				Logger.RError($"Failed to select probability for element at index {i} in array of type {typeof(T)}.");
				val = 1;
			}
			num = (array2[i] = num + Math.Max(1, val));
		}
		int num2 = new Random().Next(num);
		for (int j = 0; j < array2.Length; j++)
		{
			if (num2 < array2[j])
			{
				pick = array[j];
				break;
			}
		}
		return pick != null;
	}
}
