using System;
using UnityEngine;

[Serializable]
public struct MinMaxFloatRange
{
	public float minValue;

	public float maxValue;

	public float minLimit;

	public float maxLimit;

	public float RandomValue => GetRandom();

	public MinMaxFloatRange(float minValue, float maxValue, float minLimit, float maxLimit)
	{
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
	}

	private float GetRandom()
	{
		return Math.Clamp(UnityEngine.Random.Range(minValue, maxValue), minLimit, maxLimit);
	}
}
