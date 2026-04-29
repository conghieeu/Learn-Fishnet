using System;
using UnityEngine;

[Serializable]
public class EffectData
{
	public string effectName;

	public GameObject effectObject;

	[Range(0f, 1f)]
	public float minRatio;

	[Range(0f, 1f)]
	public float maxRatio;

	public EffectData(string effectName, GameObject effectObject, float minRatio, float maxRatio)
	{
		this.effectName = effectName;
		this.effectObject = effectObject;
		this.minRatio = minRatio;
		this.maxRatio = maxRatio;
	}
}
