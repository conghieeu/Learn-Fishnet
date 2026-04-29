using System;
using UnityEngine;

[Serializable]
public struct MinMaxIntRange
{
	public int minValue;

	public int maxValue;

	public int minLimit;

	public int maxLimit;

	public int RandomValue => GetRandom();

	public MinMaxIntRange(int minValue, int maxValue, int minLimit, int maxLimit)
	{
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
	}

	private int GetRandom()
	{
		return Math.Clamp(UnityEngine.Random.Range(minValue, maxValue + 1), minLimit, maxLimit);
	}
}
