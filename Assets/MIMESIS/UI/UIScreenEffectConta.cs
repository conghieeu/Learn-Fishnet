using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenEffectConta : UIScreenEffectBase
{
	[Serializable]
	public class ContaEffectInfo
	{
		public int id;

		public int triggerRatio;

		public float fadeDuration;

		public List<GameObject> triggerObjects;
	}

	[SerializeField]
	private List<ContaEffectInfo> contaEffectInfos;

	private Dictionary<int, ContaEffectInfo> contaEffectDictionary = new Dictionary<int, ContaEffectInfo>();

	private Dictionary<int, List<Graphic>> contaGraphicleDictionary = new Dictionary<int, List<Graphic>>();

	private List<int> currentTriggeredEffectIds = new List<int>();

	private bool isInitialized;

	public void Initialize()
	{
		contaEffectDictionary.Clear();
		currentTriggeredEffectIds.Clear();
		foreach (ContaEffectInfo contaEffectInfo in contaEffectInfos)
		{
			contaEffectDictionary.Add(contaEffectInfo.id, contaEffectInfo);
			foreach (GameObject triggerObject in contaEffectInfo.triggerObjects)
			{
				Graphic[] componentsInChildren = triggerObject.GetComponentsInChildren<Graphic>(includeInactive: true);
				foreach (Graphic item in componentsInChildren)
				{
					if (contaGraphicleDictionary.TryGetValue(contaEffectInfo.id, out var value))
					{
						value.Add(item);
						continue;
					}
					contaGraphicleDictionary.Add(contaEffectInfo.id, new List<Graphic> { item });
				}
			}
		}
		contaEffectInfos.Sort((ContaEffectInfo a, ContaEffectInfo b) => b.triggerRatio.CompareTo(a.triggerRatio));
		isInitialized = true;
	}

	public void PlayContaScreenEffect(int conta, int maxConta)
	{
		if (!isInitialized)
		{
			return;
		}
		List<int> list = new List<int>();
		int num = (int)((float)conta / (float)maxConta * 100f);
		foreach (ContaEffectInfo contaEffectInfo in contaEffectInfos)
		{
			if (contaEffectInfo.triggerRatio <= num)
			{
				list.Add(contaEffectInfo.id);
			}
		}
		if (list.Count > 0)
		{
			if (!base.gameObject.activeSelf)
			{
				Show();
				return;
			}
			foreach (int currentTriggeredEffectId in currentTriggeredEffectIds)
			{
				if (!list.Contains(currentTriggeredEffectId))
				{
					StopScreenEffect(currentTriggeredEffectId);
				}
			}
			foreach (int item in list)
			{
				if (!currentTriggeredEffectIds.Contains(item))
				{
					PlayScreenEffect(item, contaEffectDictionary[item].fadeDuration);
				}
			}
			currentTriggeredEffectIds = list;
			return;
		}
		foreach (int currentTriggeredEffectId2 in currentTriggeredEffectIds)
		{
			StopScreenEffect(currentTriggeredEffectId2);
		}
		if (base.gameObject.activeSelf)
		{
			Hide();
		}
		currentTriggeredEffectIds.Clear();
	}

	public void StopAllScreenEffect()
	{
		if (!isInitialized)
		{
			return;
		}
		foreach (int currentTriggeredEffectId in currentTriggeredEffectIds)
		{
			StopScreenEffect(currentTriggeredEffectId);
		}
	}

	private void PlayScreenEffect(int id, float fadeDuration)
	{
		if (contaGraphicleDictionary.TryGetValue(id, out var value))
		{
			foreach (Graphic item in value)
			{
				item.canvasRenderer.SetAlpha(0f);
			}
		}
		if (contaEffectDictionary.TryGetValue(id, out var value2))
		{
			foreach (GameObject triggerObject in value2.triggerObjects)
			{
				triggerObject.SetActive(value: true);
			}
		}
		foreach (Graphic item2 in value)
		{
			item2.CrossFadeAlpha(1f, fadeDuration, ignoreTimeScale: false);
		}
	}

	private void StopScreenEffect(int id)
	{
		if (!contaEffectDictionary.TryGetValue(id, out var value))
		{
			return;
		}
		foreach (GameObject triggerObject in value.triggerObjects)
		{
			triggerObject.SetActive(value: false);
		}
	}
}
