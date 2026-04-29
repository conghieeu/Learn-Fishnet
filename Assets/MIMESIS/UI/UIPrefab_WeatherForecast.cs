using System.Collections;
using UnityEngine;
using UnityEngine.UI;

internal class UIPrefab_WeatherForecast : UIPrefabScript
{
	public const string UEID_weather = "weather";

	public const string UEID_Squall = "Squall";

	private Transform _UE_weather;

	private Image _UE_Squall;

	public float onTime = 1f;

	public float offTime = 1f;

	public float totalTime = 10f;

	private float dTime;

	public Transform UE_weather => _UE_weather ?? (_UE_weather = PickTransform("weather"));

	public Image UE_Squall => _UE_Squall ?? (_UE_Squall = PickImage("Squall"));

	private void OnEnable()
	{
		StartCoroutine(Forecast());
	}

	private IEnumerator Forecast()
	{
		for (dTime = 0f; dTime < totalTime; dTime += offTime)
		{
			UE_weather.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(onTime);
			dTime += onTime;
			if (dTime >= totalTime)
			{
				break;
			}
			UE_weather.gameObject.SetActive(value: false);
			yield return new WaitForSeconds(offTime);
		}
		Hide();
	}
}
