using System;
using System.Collections;
using OccaSoftware.Buto.Runtime;
using UnityEngine;
using UnityEngine.Rendering;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=5585476605")]
public class SkyAndWeatherSystem : MonoBehaviour
{
	public enum eWeatherPreset
	{
		Sunny = 0,
		Rain = 1,
		HeavyRain = 2,
		Squall = 3
	}

	public enum eWeather
	{
		Sunny = 0,
		Rainy = 1,
		Snowy = 2,
		TurnOff = 3
	}

	public enum eCloudDensity
	{
		Clear = 0,
		Cloudy = 1,
		Overcast = 2,
		TurnOff = 3
	}

	[Serializable]
	public struct WeatherPresetData
	{
		public eWeatherPreset weatherPreset;

		public eWeather weather;

		public eCloudDensity cloudDensity;

		public float rainIntensity;
	}

	[SerializeField]
	[Header("날씨 프리셋")]
	private eWeatherPreset weatherPreset;

	[SerializeField]
	private WeatherPresetData[] weatherPresets = new WeatherPresetData[4]
	{
		new WeatherPresetData
		{
			weatherPreset = eWeatherPreset.Sunny,
			weather = eWeather.Sunny,
			cloudDensity = eCloudDensity.Clear,
			rainIntensity = 0f
		},
		new WeatherPresetData
		{
			weatherPreset = eWeatherPreset.Rain,
			weather = eWeather.Rainy,
			cloudDensity = eCloudDensity.Overcast,
			rainIntensity = 0.2f
		},
		new WeatherPresetData
		{
			weatherPreset = eWeatherPreset.HeavyRain,
			weather = eWeather.Rainy,
			cloudDensity = eCloudDensity.Overcast,
			rainIntensity = 0.5f
		},
		new WeatherPresetData
		{
			weatherPreset = eWeatherPreset.Squall,
			weather = eWeather.Rainy,
			cloudDensity = eCloudDensity.Overcast,
			rainIntensity = 0.8f
		}
	};

	[SerializeField]
	[Header("날씨")]
	private eWeather weather;

	[SerializeField]
	[Header("구름")]
	[InspectorReadOnly]
	private eCloudDensity cloudDensity;

	[Range(0f, 24f)]
	[Header("현재 시각 (0시~24시)")]
	public float hours = 12f;

	[SerializeField]
	[Range(0f, 1f)]
	[Header("비가 올 때, 비의 강도")]
	private float rainIntensity = 0.2f;

	[SerializeField]
	[Header("오염도에 따른 안개강도 최종 보정")]
	private float[] fogDensityFinalMix_SunnyTable = new float[3] { 1f, 1f, 1f };

	[SerializeField]
	private float[] fogDensityFinalMix_RainyTable = new float[3] { 1f, 1f, 1f };

	[SerializeField]
	[Header("오염도에 따른 안개색밝기 최종 보정")]
	private float[] fogColorFinalMix_SunnyTable = new float[3] { 1f, 1f, 1f };

	[SerializeField]
	private float[] fogColorFinalMix_RainyTable = new float[3] { 1f, 1f, 1f };

	[Tooltip("0시~24시에 따른 태양광의 색을 지정")]
	[Header("Sun & lighting")]
	public Gradient sunLightColor;

	[GradientUsage(true)]
	[Tooltip("0시~24시에 따른 태양의 색을 지정 (HDR)")]
	public Gradient sunSphereColor;

	[GradientUsage(true)]
	[Tooltip("0시~24시에 따른 ambient light 를 지정 (HDR)")]
	public Gradient ambientSkyColor;

	[Tooltip("skybox 회전 속도")]
	public float skyboxRotationSpeed = 1f;

	[Header("일몰/일출 관련")]
	[Tooltip("일출 시각 (단위=시)")]
	[Range(0f, 24f)]
	public float sunRiseTime = 6f;

	[Tooltip("일몰 시각 (단위=시)")]
	[Range(0f, 24f)]
	public float sunSetTime = 18f;

	[Tooltip("애니메이션 길이 (단위=시간)")]
	[Range(0.1f, 2f)]
	public float sunSetBlendingTime = 0.25f;

	[InspectorReadOnly]
	[SerializeField]
	[Tooltip("현재 시각 (0~1)")]
	private float debug_gradientPercent;

	[Tooltip("테스트용, 시간이 흐르는 속도")]
	public float timescale = 0.1f;

	[SerializeField]
	[Tooltip("테스트용, 자동으로 시간 흐름")]
	private bool animateTime;

	[SerializeField]
	[Tooltip("서울 위도 = 37.5665")]
	private float latitude = 37.5665f;

	[SerializeField]
	[Tooltip("테스트용, 자동으로 비의 강도가 바뀜")]
	private bool debug_animateRain;

	[SerializeField]
	[Range(0f, 1f)]
	[Header("날씨에 따른 태양광 감쇠 (0=완전 어두움, 1=최대 밝기)")]
	private float sunLightEvalMultiplier_Sunny = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float sunLightEvalMultiplier_SunnyOvercast = 0.8f;

	[SerializeField]
	[Range(0f, 1f)]
	private float sunLightEvalMultiplier_Rainy = 0.7f;

	[SerializeField]
	[Range(0f, 1f)]
	private float sunLightEvalMultiplier_RainyOvercast = 0.4f;

	[SerializeField]
	[Header("(임시) 값이 클수록 밝아짐")]
	private float sunLightBaseIntensity = 2f;

	[InspectorReadOnly]
	[SerializeField]
	private float sunLightEvalMultiplier = 1f;

	[SerializeField]
	[Header("(임시) 날씨가 바뀔 때 cross fade 되는 시간")]
	private float sunLightValueInterpolationInterval = 3f;

	[SerializeField]
	[Header("날씨에 따른 buto fog 값 영향")]
	private float butoFogDensity_Sunny = 2f;

	[SerializeField]
	private AnimationCurve butoFogColorInfluence_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	private Gradient butoFogColorInfluenceLit_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	private Gradient butoFogColorInfluenceShadow_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	private Gradient butoFogColorInfluenceEmission_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	[Header("구 데이터. 이제 사용하지 않음")]
	private Gradient butoFogColorInfluenceDicrectionalLightingForward_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	[Header("3개 만들고, 오염도에 따라 선택해서 사용")]
	private Gradient[] newButoFogColorInfluenceDicrectionalLightingForward_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	[Header("구 데이터. 이제 사용하지 않음")]
	private Gradient butoFogColorInfluenceDicrectionalLightingBack_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	[Header("3개 만들고, 오염도에 따라 선택해서 사용")]
	private Gradient[] newButoFogColorInfluenceDicrectionalLightingBack_Sunny;

	[SerializeField]
	private AnimationCurve butoFogColorInfluenceDicrectionalLightingBalance_Sunny;

	[SerializeField]
	private AnimationCurve butoFogColorIntensityOverridesLightIntensity_Sunny;

	[SerializeField]
	[Header("오염도에 따른 light intensity 최종 보정")]
	private float[] butoFogColorIntensityOverridesLightIntensityFinalMix_Sunny = new float[3] { 1f, 1f, 1f };

	[SerializeField]
	private AnimationCurve butoFogColorIntensityOverridesDensityInLight_Sunny;

	[SerializeField]
	private AnimationCurve butoFogColorIntensityOverridesDensityInShadow_Sunny;

	[SerializeField]
	private AnimationCurve butoFogDensity_Rainy;

	[SerializeField]
	private float butoFogDensity_RainyMultiplier = 15f;

	[SerializeField]
	private AnimationCurve butoFogColorInfluence_Rainy;

	[GradientUsage(true)]
	[SerializeField]
	private Gradient butoFogColorInfluenceLit_Rainy;

	[GradientUsage(true)]
	[SerializeField]
	private Gradient butoFogColorInfluenceShadow_Rainy;

	[GradientUsage(true)]
	[SerializeField]
	private Gradient butoFogColorInfluenceEmission_Rainy;

	[GradientUsage(true)]
	[SerializeField]
	[Header("구 데이터. 이제 사용하지 않음")]
	private Gradient butoFogColorInfluenceDicrectionalLightingForward_Rainy;

	[GradientUsage(true)]
	[SerializeField]
	[Header("3개 만들고, 오염도에 따라 선택해서 사용")]
	private Gradient[] newButoFogColorInfluenceDicrectionalLightingForward_Rainy;

	[GradientUsage(true)]
	[SerializeField]
	[Header("구 데이터. 이제 사용하지 않음")]
	private Gradient butoFogColorInfluenceDicrectionalLightingBack_Rainy;

	[GradientUsage(true)]
	[SerializeField]
	[Header("3개 만들고, 오염도에 따라 선택해서 사용")]
	private Gradient[] newButoFogColorInfluenceDicrectionalLightingBack_Rainy;

	[SerializeField]
	private AnimationCurve butoFogColorInfluenceDicrectionalLightingBalance_Rainy;

	[SerializeField]
	private AnimationCurve butoFogColorIntensityOverridesLightIntensity_Rainy;

	[SerializeField]
	[Header("오염도에 따른 light intensity 최종 보정")]
	private float[] butoFogColorIntensityOverridesLightIntensityFinalMix_Rainy = new float[3] { 1f, 1f, 1f };

	[SerializeField]
	private AnimationCurve butoFogColorIntensityOverridesDensityInLight_Rainy;

	[SerializeField]
	private AnimationCurve butoFogColorIntensityOverridesDensityInShadow_Rainy;

	[InspectorReadOnly]
	[SerializeField]
	private float butoFogIntensity = 1f;

	[GradientUsage(true)]
	[SerializeField]
	[Header("날씨/시간에 따른 구름 색 설정")]
	private Gradient cloudFakeAmbient_Sunny;

	[GradientUsage(true)]
	[SerializeField]
	private Gradient cloudFakeAmbient_Rainy;

	[SerializeField]
	[Header("Object link")]
	private Transform skyPivot;

	[SerializeField]
	private CloudZone cloudZone_Clear;

	[SerializeField]
	private CloudZone cloudZone_Overcast;

	[SerializeField]
	private Light sun;

	[SerializeField]
	private MeshRenderer sunMeshRenderer;

	[SerializeField]
	private MeshRenderer moonMeshRenderer;

	[SerializeField]
	private ParticleSystem[] rainMakers;

	[SerializeField]
	private Material matRain;

	[SerializeField]
	private Volume targetVolume;

	[SerializeField]
	private bool debug_turnOnAtStart = true;

	private bool mainSwitch;

	private Color sunLightEval;

	private Color ambientSkyColorEval;

	private eWeather lastWeather;

	private eCloudDensity lastCloudDensity;

	private float lastRainIntensity = 1f;

	private float rainIntensityAnimatingValue;

	[SerializeField]
	private float rainIntensityAnimationSpeed = 0.1f;

	private Coroutine sunLightEvalMultiplierAnimationRunner;

	[SerializeField]
	private Camera targetCamera;

	[SerializeField]
	private Material cloudMaterial;

	[SerializeField]
	private Material cloudAlphaMaterial;

	private int pollutionTableIndex;

	private float fogDensityFinalMix_Sunny = 1f;

	private float fogColorFinalMix_Sunny = 1f;

	private float fogDensityFinalMix_Rainy = 1f;

	private float fogColorFinalMix_Rainy = 1f;

	private bool turnedOnThisFrame;

	private ParticleSystem[] rainScreenVFX;

	private bool isRainScreenVFXPlaying;

	private bool suppressRainScreenVFX;

	private void Awake()
	{
		if (debug_turnOnAtStart)
		{
			TurnOn(eWeather.Sunny, eCloudDensity.Clear, 0.2f);
		}
		ParticleSystem[] array = rainMakers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
		if (Hub.s != null && Hub.s.uiman != null)
		{
			Transform rainScreenVFXNode = Hub.s.uiman.GetRainScreenVFXNode();
			if (rainScreenVFXNode != null)
			{
				rainScreenVFX = rainScreenVFXNode.GetComponentsInChildren<ParticleSystem>();
				StopRainScreenVFX(forceStop: true);
			}
		}
	}

	public void OnSceneLoadComplete()
	{
	}

	public void TurnOn(eWeatherPreset weatherPreset)
	{
		WeatherPresetData weatherPresetData = Array.Find(weatherPresets, (WeatherPresetData x) => x.weatherPreset == weatherPreset);
		TurnOn(weatherPresetData.weather, weatherPresetData.cloudDensity, weatherPresetData.rainIntensity);
	}

	public void TurnOn(eWeather newWeather, eCloudDensity newCloud, float newRainIntensity)
	{
		if (!mainSwitch)
		{
			turnedOnThisFrame = true;
			mainSwitch = true;
			ParticleSystem[] array = rainMakers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: true);
			}
			sun.gameObject.SetActive(value: true);
			switch (newCloud)
			{
			case eCloudDensity.Clear:
				RenderSettings.skybox.SetFloat("_Blend", 0f);
				break;
			case eCloudDensity.Overcast:
				RenderSettings.skybox.SetFloat("_Blend", 1f);
				break;
			}
			rainIntensity = newRainIntensity;
			rainIntensityAnimatingValue = newRainIntensity;
			weather = newWeather;
			cloudDensity = newCloud;
			sunMeshRenderer.enabled = true;
			moonMeshRenderer.enabled = true;
		}
	}

	public void TurnOff()
	{
		if (!mainSwitch)
		{
			return;
		}
		mainSwitch = false;
		ParticleSystem[] array = rainMakers;
		foreach (ParticleSystem particleSystem in array)
		{
			if (particleSystem != null)
			{
				particleSystem.gameObject.SetActive(value: false);
			}
		}
		StopRainScreenVFX();
		weather = eWeather.TurnOff;
		lastWeather = eWeather.TurnOff;
		cloudDensity = eCloudDensity.TurnOff;
		lastCloudDensity = eCloudDensity.TurnOff;
		sun.gameObject.SetActive(value: false);
		RenderSettings.ambientSkyColor = Color.black;
		RenderSettings.skybox.SetColor("_Tint", Color.black);
		RenderSettings.skybox.SetColor("_SkyTint", Color.black);
		sunMeshRenderer.enabled = false;
		moonMeshRenderer.enabled = false;
	}

	public void SetWeather(eWeather newWeather, eCloudDensity newCloudDensity, float newRainIntensity)
	{
		weather = newWeather;
		cloudDensity = newCloudDensity;
		rainIntensity = newRainIntensity;
		if (newWeather == eWeather.Sunny)
		{
			rainIntensityAnimatingValue = 0f;
		}
		if (weather == eWeather.Sunny)
		{
			cloudDensity = eCloudDensity.Clear;
		}
		if (weather == eWeather.Rainy)
		{
			cloudDensity = eCloudDensity.Overcast;
		}
	}

	public void SetWeatherPreset(eWeatherPreset newWeatherPreset)
	{
		WeatherPresetData weatherPresetData = Array.Find(weatherPresets, (WeatherPresetData x) => x.weatherPreset == newWeatherPreset);
		SetWeather(weatherPresetData.weather, weatherPresetData.cloudDensity, weatherPresetData.rainIntensity);
	}

	public void SetTime(float newHours)
	{
		if (newHours < 0f)
		{
			newHours = 0f;
		}
		if (newHours > 24f)
		{
			newHours = 24f;
		}
		hours = newHours;
	}

	public void SetPollutionLevel(int pollutionLevel)
	{
		pollutionTableIndex = pollutionLevel - 1;
		if (pollutionTableIndex > 2)
		{
			pollutionTableIndex = 2;
		}
		if (pollutionTableIndex < 0)
		{
			pollutionTableIndex = 0;
		}
		fogDensityFinalMix_Sunny = fogDensityFinalMix_SunnyTable[pollutionTableIndex];
		fogColorFinalMix_Sunny = fogColorFinalMix_SunnyTable[pollutionTableIndex];
		fogDensityFinalMix_Rainy = fogDensityFinalMix_RainyTable[pollutionTableIndex];
		fogColorFinalMix_Rainy = fogColorFinalMix_RainyTable[pollutionTableIndex];
	}

	public void SetAnimateTime(bool newAnimateTime)
	{
		animateTime = newAnimateTime;
	}

	private void Update()
	{
		if (!mainSwitch)
		{
			return;
		}
		if (animateTime)
		{
			hours += Time.deltaTime * timescale;
			hours %= 24f;
			debug_gradientPercent = hours / 24f * 100f;
		}
		if (debug_animateRain)
		{
			rainIntensity = (Mathf.Sin(Time.time * 0.3f) * 0.5f + 0.5f) * 0.5f;
		}
		if (targetCamera != null)
		{
			if (rainMakers.Length == 5)
			{
				Transform transform = targetCamera.transform;
				Vector3 position = transform.position;
				Vector3 vector = new Vector3(transform.forward.x, 0f, transform.forward.z);
				if (vector.sqrMagnitude < 0.001f)
				{
					vector = Vector3.forward;
				}
				vector.Normalize();
				Vector3 vector2 = new Vector3(transform.right.x, 0f, transform.right.z);
				if (vector2.sqrMagnitude < 0.001f)
				{
					vector2 = Vector3.right;
				}
				vector2.Normalize();
				float num = 12f;
				rainMakers[0].transform.position = position + vector * -5f + vector2 * -5f + Vector3.up * num;
				rainMakers[1].transform.position = position + vector * -5f + vector2 * 5f + Vector3.up * num;
				rainMakers[2].transform.position = position + vector * 5f + vector2 * -5f + Vector3.up * num;
				rainMakers[3].transform.position = position + vector * 5f + vector2 * 5f + Vector3.up * num;
				rainMakers[4].transform.position = position + vector * 5f + vector2 * 0f + Vector3.up * num;
			}
			else
			{
				Logger.RError("반드시 5 개의 rain maker 가 설정되어야 한다.");
			}
		}
		UpdateSunAndMoon();
		UpdateWeather();
		UpdateCloud();
		UpdateButo();
		turnedOnThisFrame = false;
	}

	private void UpdateSunAndMoon()
	{
		float num = 0f;
		float num2 = (hours - 12f) * 15f;
		float num3 = Mathf.Asin(Mathf.Sin(latitude * (MathF.PI / 180f)) * Mathf.Sin(num * (MathF.PI / 180f)) + Mathf.Cos(latitude * (MathF.PI / 180f)) * Mathf.Cos(num * (MathF.PI / 180f)) * Mathf.Cos(num2 * (MathF.PI / 180f)));
		float num4 = Mathf.Atan2(0f - Mathf.Sin(num2 * (MathF.PI / 180f)), Mathf.Cos(num2 * (MathF.PI / 180f)) * Mathf.Sin(latitude * (MathF.PI / 180f)) - Mathf.Tan(num * (MathF.PI / 180f)) * Mathf.Cos(latitude * (MathF.PI / 180f)));
		sunLightEval = sunLightColor.Evaluate(hours / 24f) * sunLightEvalMultiplier;
		Color color = sunSphereColor.Evaluate(hours / 24f) * sunLightEvalMultiplier * 0.005f;
		ambientSkyColorEval = ambientSkyColor.Evaluate(hours / 24f) * sunLightEvalMultiplier;
		sun.color = sunLightEval;
		sun.intensity = sunLightEval.grayscale * sunLightBaseIntensity;
		if (hours < 6f || hours > 18f)
		{
			float value = 1f;
			if (hours < sunRiseTime)
			{
				value = (hours - (sunRiseTime - sunSetBlendingTime)) * (1f / sunSetBlendingTime);
			}
			else if (hours > sunSetTime)
			{
				value = (sunSetTime + sunSetBlendingTime - hours) * (1f / sunSetBlendingTime);
			}
			value = Mathf.Clamp01(value);
			sun.intensity *= value;
		}
		Material skybox = RenderSettings.skybox;
		if (skybox != null)
		{
			skybox.SetColor("_Tint", sunLightEval);
			skybox.SetColor("_SkyTint", sunLightEval);
			skybox.SetFloat("_Rotation", hours / 24f * 360f * skyboxRotationSpeed);
		}
		if (sunMeshRenderer.material != null)
		{
			sunMeshRenderer.material.color = color;
			sunMeshRenderer.material.SetColor("_EmissionColor", color);
		}
		RenderSettings.ambientSkyColor = ambientSkyColorEval;
		Quaternion localRotation = Quaternion.Euler(num3 * 57.29578f, num4 * 57.29578f, 0f);
		skyPivot.localRotation = localRotation;
	}

	private void UpdateWeather()
	{
		if (lastWeather != weather || turnedOnThisFrame)
		{
			ApplySunLightMultiplier();
			if (weather == eWeather.Rainy)
			{
				ParticleSystem[] array = rainMakers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Play();
				}
				PlayRainScreenVFX();
			}
			else
			{
				ParticleSystem[] array = rainMakers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Stop();
				}
				StopRainScreenVFX();
			}
			_ = weather;
			lastWeather = weather;
		}
		if (weather == eWeather.Rainy)
		{
			if (Mathf.Abs(lastRainIntensity - rainIntensity) > 0.05f || turnedOnThisFrame)
			{
				ApplySunLightMultiplier();
				lastRainIntensity = rainIntensity;
			}
			Renderer component = rainMakers[0].GetComponent<ParticleSystemRenderer>();
			if (component != null)
			{
				Color rgbColor = sunLightEval;
				Color.RGBToHSV(rgbColor, out var H, out var S, out var V);
				S *= 0.5f;
				rgbColor = Color.HSVToRGB(H, S, V);
				rainIntensityAnimatingValue = Mathf.Lerp(rainIntensityAnimatingValue, rainIntensity, Time.deltaTime * rainIntensityAnimationSpeed);
				rgbColor.a = rainIntensityAnimatingValue;
				component.sharedMaterial.SetColor("_BaseColor", rgbColor);
			}
			if (Physics.Raycast(Camera.main.transform.position, Vector3.up, out var _, 10f, Hub.DefaultOnlyLayerMask))
			{
				StopRainScreenVFX();
			}
			else
			{
				PlayRainScreenVFX();
			}
		}
		if (weather == eWeather.Sunny)
		{
			cloudDensity = eCloudDensity.Clear;
		}
		if (weather == eWeather.Rainy)
		{
			cloudDensity = eCloudDensity.Overcast;
		}
	}

	private void UpdateCloud()
	{
		if (lastCloudDensity != cloudDensity || turnedOnThisFrame)
		{
			ApplySunLightMultiplier();
			switch (cloudDensity)
			{
			case eCloudDensity.Clear:
				cloudZone_Clear.gameObject.SetActive(value: true);
				cloudZone_Overcast.gameObject.SetActive(value: false);
				StartCoroutine(AnimateCloud(0f, 1f));
				break;
			case eCloudDensity.Overcast:
				StartCoroutine(AnimateCloud(1f, 1f));
				break;
			case eCloudDensity.TurnOff:
				cloudZone_Clear.gameObject.SetActive(value: false);
				cloudZone_Overcast.gameObject.SetActive(value: false);
				break;
			}
			lastCloudDensity = cloudDensity;
		}
		Color fakeAmbientColor = weather switch
		{
			eWeather.Sunny => cloudFakeAmbient_Sunny.Evaluate(hours / 24f), 
			eWeather.Rainy => cloudFakeAmbient_Rainy.Evaluate(hours / 24f), 
			_ => Color.black, 
		};
		switch (cloudDensity)
		{
		case eCloudDensity.Clear:
			cloudZone_Clear.ApplySkyAndWeather(ambientSkyColorEval, fakeAmbientColor);
			break;
		case eCloudDensity.Overcast:
			cloudZone_Overcast.ApplySkyAndWeather(ambientSkyColorEval, fakeAmbientColor);
			break;
		}
	}

	private IEnumerator AnimateCloud(float skyBlendFactor, float duration)
	{
		float startBlendFactor = RenderSettings.skybox.GetFloat("_Blend");
		_ = skyBlendFactor;
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			float value = Mathf.Lerp(startBlendFactor, skyBlendFactor, t / duration);
			RenderSettings.skybox.SetFloat("_Blend", value);
			yield return null;
		}
		if (skyBlendFactor > 0.9f)
		{
			cloudZone_Clear.gameObject.SetActive(value: false);
		}
	}

	private void UpdateButo()
	{
		if (targetVolume == null)
		{
			Logger.RError("You need to set the target volume.");
			return;
		}
		targetVolume.profile.TryGet<ButoVolumetricFog>(out var component);
		if (component == null)
		{
			Logger.RError("Cannot find ButoFog settings.");
			return;
		}
		float time = hours / 24f;
		AnimationCurve animationCurve = weather switch
		{
			eWeather.Sunny => butoFogColorInfluence_Sunny, 
			eWeather.Rainy => butoFogColorInfluence_Rainy, 
			eWeather.Snowy => butoFogColorInfluence_Sunny, 
			_ => butoFogColorInfluence_Sunny, 
		};
		Gradient gradient = weather switch
		{
			eWeather.Sunny => butoFogColorInfluenceLit_Sunny, 
			eWeather.Rainy => butoFogColorInfluenceLit_Rainy, 
			eWeather.Snowy => butoFogColorInfluenceLit_Sunny, 
			_ => butoFogColorInfluenceLit_Sunny, 
		};
		Gradient gradient2 = weather switch
		{
			eWeather.Sunny => butoFogColorInfluenceShadow_Sunny, 
			eWeather.Rainy => butoFogColorInfluenceShadow_Rainy, 
			eWeather.Snowy => butoFogColorInfluenceShadow_Sunny, 
			_ => butoFogColorInfluenceShadow_Sunny, 
		};
		Gradient gradient3 = weather switch
		{
			eWeather.Sunny => butoFogColorInfluenceEmission_Sunny, 
			eWeather.Rainy => butoFogColorInfluenceEmission_Rainy, 
			eWeather.Snowy => butoFogColorInfluenceEmission_Sunny, 
			_ => butoFogColorInfluenceEmission_Sunny, 
		};
		Gradient gradient4 = weather switch
		{
			eWeather.Sunny => newButoFogColorInfluenceDicrectionalLightingForward_Sunny[pollutionTableIndex], 
			eWeather.Rainy => newButoFogColorInfluenceDicrectionalLightingForward_Rainy[pollutionTableIndex], 
			eWeather.Snowy => newButoFogColorInfluenceDicrectionalLightingForward_Sunny[pollutionTableIndex], 
			_ => butoFogColorInfluenceDicrectionalLightingForward_Sunny, 
		};
		Gradient gradient5 = weather switch
		{
			eWeather.Sunny => newButoFogColorInfluenceDicrectionalLightingBack_Sunny[pollutionTableIndex], 
			eWeather.Rainy => newButoFogColorInfluenceDicrectionalLightingBack_Rainy[pollutionTableIndex], 
			eWeather.Snowy => newButoFogColorInfluenceDicrectionalLightingBack_Sunny[pollutionTableIndex], 
			_ => butoFogColorInfluenceDicrectionalLightingBack_Sunny, 
		};
		AnimationCurve animationCurve2 = weather switch
		{
			eWeather.Sunny => butoFogColorInfluenceDicrectionalLightingBalance_Sunny, 
			eWeather.Rainy => butoFogColorInfluenceDicrectionalLightingBalance_Rainy, 
			eWeather.Snowy => butoFogColorInfluenceDicrectionalLightingBalance_Sunny, 
			_ => butoFogColorInfluenceDicrectionalLightingBalance_Sunny, 
		};
		AnimationCurve animationCurve3 = weather switch
		{
			eWeather.Sunny => butoFogColorIntensityOverridesLightIntensity_Sunny, 
			eWeather.Rainy => butoFogColorIntensityOverridesLightIntensity_Rainy, 
			eWeather.Snowy => butoFogColorIntensityOverridesLightIntensity_Sunny, 
			_ => butoFogColorIntensityOverridesLightIntensity_Sunny, 
		};
		AnimationCurve animationCurve4 = weather switch
		{
			eWeather.Sunny => butoFogColorIntensityOverridesDensityInLight_Sunny, 
			eWeather.Rainy => butoFogColorIntensityOverridesDensityInLight_Rainy, 
			eWeather.Snowy => butoFogColorIntensityOverridesDensityInLight_Sunny, 
			_ => butoFogColorIntensityOverridesDensityInLight_Sunny, 
		};
		object obj = weather switch
		{
			eWeather.Sunny => butoFogColorIntensityOverridesDensityInShadow_Sunny, 
			eWeather.Rainy => butoFogColorIntensityOverridesDensityInShadow_Rainy, 
			eWeather.Snowy => butoFogColorIntensityOverridesDensityInShadow_Sunny, 
			_ => butoFogColorIntensityOverridesDensityInShadow_Sunny, 
		};
		float num = animationCurve.Evaluate(time);
		Color color = gradient.Evaluate(time);
		Color color2 = gradient2.Evaluate(time);
		Color color3 = gradient3.Evaluate(time);
		Color newValue = gradient4.Evaluate(time);
		Color newValue2 = gradient5.Evaluate(time);
		float newValue3 = animationCurve2.Evaluate(time);
		float num2 = animationCurve3.Evaluate(time);
		float newValue4 = animationCurve4.Evaluate(time);
		float newValue5 = ((AnimationCurve)obj).Evaluate(time);
		float num3 = weather switch
		{
			eWeather.Sunny => fogColorFinalMix_Sunny, 
			eWeather.Rainy => fogColorFinalMix_Rainy, 
			_ => 1f, 
		};
		float num4 = weather switch
		{
			eWeather.Sunny => butoFogColorIntensityOverridesLightIntensityFinalMix_Sunny[pollutionTableIndex], 
			eWeather.Rainy => butoFogColorIntensityOverridesLightIntensityFinalMix_Rainy[pollutionTableIndex], 
			_ => 1f, 
		};
		FindAndSetButoFogValueFloat(component, "colorInfluence", num * num3);
		FindAndSetButoFogValueColor(component, "litColor", color * num3);
		FindAndSetButoFogValueColor(component, "shadowedColor", color2 * num3);
		FindAndSetButoFogValueColor(component, "emitColor", color3 * num3);
		FindAndSetButoFogValueColor(component, "directionalForward", newValue);
		FindAndSetButoFogValueColor(component, "directionalBack", newValue2);
		FindAndSetButoFogValueFloat(component, "directionalRatio", newValue3);
		FindAndSetButoFogValueFloat(component, "lightIntensity", num2 * num4);
		FindAndSetButoFogValueFloat(component, "densityInLight", newValue4);
		FindAndSetButoFogValueFloat(component, "densityInShadow", newValue5);
	}

	private void FindAndSetButoFogValueFloat(ButoVolumetricFog fogSetting, string valueName, float newValue)
	{
		FloatParameter floatParameter = valueName switch
		{
			"colorInfluence" => fogSetting.colorInfluence, 
			"directionalRatio" => fogSetting.directionalRatio, 
			"lightIntensity" => fogSetting.lightIntensity, 
			"densityInLight" => fogSetting.densityInLight, 
			"densityInShadow" => fogSetting.densityInShadow, 
			_ => null, 
		};
		if (floatParameter == null)
		{
			Logger.RError("Cannot find value holder for " + valueName);
			return;
		}
		_ = floatParameter.value;
		floatParameter.value = newValue;
	}

	private void FindAndSetButoFogValueColor(ButoVolumetricFog fogSetting, string valueName, Color newValue)
	{
		ColorParameter colorParameter = valueName switch
		{
			"litColor" => fogSetting.litColor, 
			"shadowedColor" => fogSetting.shadowedColor, 
			"emitColor" => fogSetting.emitColor, 
			"directionalForward" => fogSetting.directionalForward, 
			"directionalBack" => fogSetting.directionalBack, 
			_ => null, 
		};
		if (colorParameter == null)
		{
			Logger.RError("Cannot find value holder for " + valueName);
			return;
		}
		_ = colorParameter.value;
		colorParameter.value = newValue;
	}

	private void ApplySunLightMultiplier()
	{
		float targetSun = 1f;
		float num = 1f;
		switch (weather)
		{
		case eWeather.Sunny:
			num = butoFogDensity_Sunny;
			switch (cloudDensity)
			{
			case eCloudDensity.Clear:
				targetSun = sunLightEvalMultiplier_Sunny;
				break;
			case eCloudDensity.Overcast:
				targetSun = sunLightEvalMultiplier_SunnyOvercast;
				break;
			}
			break;
		case eWeather.Rainy:
			num = butoFogDensity_Rainy.Evaluate(rainIntensity) * butoFogDensity_RainyMultiplier;
			switch (cloudDensity)
			{
			case eCloudDensity.Clear:
				targetSun = sunLightEvalMultiplier_Rainy;
				break;
			case eCloudDensity.Overcast:
				targetSun = sunLightEvalMultiplier_RainyOvercast;
				break;
			}
			break;
		}
		num *= weather switch
		{
			eWeather.Sunny => fogDensityFinalMix_Sunny, 
			eWeather.Rainy => fogDensityFinalMix_Rainy, 
			_ => 1f, 
		};
		if (sunLightEvalMultiplierAnimationRunner != null)
		{
			StopCoroutine(sunLightEvalMultiplierAnimationRunner);
		}
		sunLightEvalMultiplierAnimationRunner = StartCoroutine(AnimateSunLightEvalMultiplier(targetSun, num, sunLightValueInterpolationInterval));
	}

	private IEnumerator AnimateSunLightEvalMultiplier(float targetSun, float targetButo, float duration)
	{
		float startSun = sunLightEvalMultiplier;
		float startButo = butoFogIntensity;
		DateTime lastTime = DateTime.Now;
		DateTime startTime = DateTime.Now;
		DateTime TargetTime = startTime + TimeSpan.FromMilliseconds(duration * 1000f);
		while (lastTime < TargetTime)
		{
			DateTime now = DateTime.Now;
			float t = (float)((now - startTime).TotalSeconds / (double)duration);
			lastTime = now;
			sunLightEvalMultiplier = Mathf.Lerp(startSun, targetSun, t);
			butoFogIntensity = Mathf.Lerp(startButo, targetButo, t);
			SetButoFogIntensityValue(butoFogIntensity);
			yield return null;
		}
		sunLightEvalMultiplier = targetSun;
		sunLightEvalMultiplierAnimationRunner = null;
	}

	private void SetButoFogIntensityValue(float butoFogIntensity)
	{
		if (!(targetVolume == null) && targetVolume.profile.TryGet<ButoVolumetricFog>(out var component))
		{
			component.fogDensity.value = butoFogIntensity;
		}
	}

	private void PlayRainScreenVFX(bool forcePlay = false)
	{
		if (rainScreenVFX == null || (isRainScreenVFXPlaying && !forcePlay))
		{
			return;
		}
		ParticleSystem[] array = rainScreenVFX;
		foreach (ParticleSystem particleSystem in array)
		{
			if (!suppressRainScreenVFX && particleSystem != null)
			{
				particleSystem.Play();
			}
			isRainScreenVFXPlaying = true;
		}
	}

	private void StopRainScreenVFX(bool forceStop = false)
	{
		if (rainScreenVFX == null || (!isRainScreenVFXPlaying && !forceStop))
		{
			return;
		}
		ParticleSystem[] array = rainScreenVFX;
		foreach (ParticleSystem particleSystem in array)
		{
			if (particleSystem != null)
			{
				particleSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
			}
			isRainScreenVFXPlaying = false;
		}
	}

	public void SuppressRainScreenVFX()
	{
		suppressRainScreenVFX = true;
		StopRainScreenVFX();
	}

	public void UnsuppressRainScreenVFX()
	{
		suppressRainScreenVFX = false;
		PlayRainScreenVFX(forcePlay: true);
	}
}
