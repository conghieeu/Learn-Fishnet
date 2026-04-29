using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldDecal : MonoBehaviour
{
	public string decalId;

	public Color decalColor = Color.white;

	public long lifetimeMSec = 5000L;

	public long fadeoutMSec = 1000L;

	public GameObject rootGameObject;

	public float DistanceFromSpawnPoint;

	private DecalProjector[] decalProjectors;

	private static int decalPriority = 0;

	private static List<WorldDecal> instances = new List<WorldDecal>();

	public static void TurnOffDecal()
	{
		foreach (WorldDecal instance in instances)
		{
			if (instance != null)
			{
				instance.gameObject.SetActive(value: false);
			}
		}
	}

	public static void TurnOnDecal()
	{
		foreach (WorldDecal instance in instances)
		{
			if (instance != null)
			{
				instance.gameObject.SetActive(value: true);
			}
		}
	}

	private void Start()
	{
	}

	private int GeneratePriorityValue()
	{
		decalPriority = decalPriority % 50 + 1;
		return decalPriority;
	}

	public void Activate()
	{
		int value = GeneratePriorityValue();
		decalProjectors = GetComponentsInChildren<DecalProjector>();
		DecalProjector[] array = decalProjectors;
		foreach (DecalProjector decalProjector in array)
		{
			if (decalProjector.material != null)
			{
				decalProjector.material = new Material(decalProjector.material);
				decalProjector.material.SetInt("_DrawOrder", value);
			}
		}
		SetColor(decalColor);
		instances.Add(this);
		StartCoroutine(CorRunDecalFadeout());
	}

	private IEnumerator CorRunDecalFadeout()
	{
		yield return new WaitForSeconds((float)lifetimeMSec / 1000f);
		float elapsedTime = 0f;
		while (elapsedTime < (float)fadeoutMSec / 1000f)
		{
			elapsedTime += Time.deltaTime;
			float alpha = Mathf.Lerp(1f, 0f, elapsedTime / ((float)fadeoutMSec / 1000f));
			SetAlpha(alpha);
			yield return null;
		}
		instances.Remove(this);
		Object.Destroy(rootGameObject);
	}

	private void SetColor(Color color)
	{
		DecalProjector[] array = decalProjectors;
		foreach (DecalProjector decalProjector in array)
		{
			if (decalProjector.material.HasProperty("_MainColor"))
			{
				decalProjector.material.SetColor("_MainColor", color);
			}
		}
	}

	private void SetAlpha(float alpha)
	{
		DecalProjector[] array = decalProjectors;
		foreach (DecalProjector decalProjector in array)
		{
			if (decalProjector.material.HasProperty("_MainColor"))
			{
				Color value = decalColor;
				value.a = alpha * value.a;
				decalProjector.material.SetColor("_MainColor", value);
			}
		}
	}
}
