using DG.Tweening;
using OccaSoftware.Buto.Runtime;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class SkyAndWeatherLightSwitch : MonoBehaviour
{
	[SerializeField]
	private Vector2[] SwitchOnHours = new Vector2[2]
	{
		new Vector2(0f, 6f),
		new Vector2(18f, 24f)
	};

	[SerializeField]
	private float transitionTime = 1f;

	private float _initialIntensity;

	private SkyAndWeatherSystem _weather;

	private bool _animating;

	private bool old_shouldBeOn;

	private ButoLight _butoLight;

	public Light _light { get; private set; }

	private void Awake()
	{
		_light = GetComponent<Light>();
		_butoLight = GetComponent<ButoLight>();
		_initialIntensity = _light.intensity;
		_weather = Object.FindFirstObjectByType<SkyAndWeatherSystem>();
	}

	public void Update()
	{
		if (_weather == null || _animating)
		{
			return;
		}
		bool flag = false;
		Vector2[] switchOnHours = SwitchOnHours;
		for (int i = 0; i < switchOnHours.Length; i++)
		{
			Vector2 vector = switchOnHours[i];
			if (vector.x <= _weather.hours && _weather.hours <= vector.y)
			{
				flag = true;
			}
		}
		if (!flag && old_shouldBeOn)
		{
			_animating = true;
			_light.DOIntensity(0f, transitionTime).OnComplete(delegate
			{
				_animating = false;
				if (_butoLight != null)
				{
					_butoLight.enabled = false;
				}
			});
		}
		if (flag && !old_shouldBeOn)
		{
			if (_butoLight != null)
			{
				_butoLight.enabled = true;
			}
			_animating = true;
			_light.DOIntensity(_initialIntensity, transitionTime).OnComplete(delegate
			{
				_animating = false;
			});
		}
		old_shouldBeOn = flag;
	}
}
