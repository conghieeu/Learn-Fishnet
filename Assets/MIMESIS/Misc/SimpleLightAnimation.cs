using System;
using UnityEngine;

[RequireComponent(typeof(Light))]
[HelpURL("https://krafton.atlassian.net/wiki/x/KgVtHg")]
public class SimpleLightAnimation : MonoBehaviour
{
	[SerializeField]
	private Vector2 IntensityMul = Vector2.one;

	[SerializeField]
	private Vector2 RangeMul = Vector2.one;

	[SerializeField]
	private float Color_H_var;

	[SerializeField]
	private float frequency = 1f;

	[SerializeField]
	private float randomize;

	[SerializeField]
	private bool useThreshold;

	[SerializeField]
	[Range(0f, 1f)]
	private float threadhold = 0.5f;

	private Light _light;

	private float _initialIntensity;

	private float _initialRange;

	private Vector3 _initialColorHSV;

	private void Awake()
	{
		_light = GetComponent<Light>();
		_initialIntensity = _light.intensity;
		_initialRange = _light.range;
		Color.RGBToHSV(_light.color, out _initialColorHSV.x, out _initialColorHSV.y, out _initialColorHSV.z);
	}

	private void Update()
	{
		if (!(_light == null) && _light.enabled)
		{
			if (frequency < 0.001f)
			{
				frequency = 0.001f;
			}
			float num = Mathf.Sin(MathF.PI * 2f * Time.time * frequency + randomize);
			float num2 = num * 0.5f + 0.5f;
			if (useThreshold)
			{
				num2 = ((!(num2 > threadhold)) ? 0f : 1f);
			}
			_light.intensity = _initialIntensity * Mathf.Lerp(IntensityMul.x, IntensityMul.y, num2);
			_light.range = _initialRange * Mathf.Lerp(RangeMul.x, RangeMul.y, num2);
			float num3 = _initialColorHSV.x + Color_H_var * 0.5f / 360f * num;
			float y = _initialColorHSV.y;
			float z = _initialColorHSV.z;
			if (num3 > 1f)
			{
				num3 -= 1f;
			}
			if (num3 < 0f)
			{
				num3 += 1f;
			}
			_light.color = Color.HSVToRGB(num3, y, z);
		}
	}
}
