using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_Scene_Loading : UIPrefabScript
{
	[Serializable]
	public class SceneLoadingRule
	{
		public string loadingSceneKey;

		public List<string> loadingSceneNameList;
	}

	[Serializable]
	public class LoadingSceneData
	{
		public string loadingSceneName;

		public List<Transform> loadingScenes;
	}

	public const string UEID_rootNode = "rootNode";

	public const string UEID_LoadingPercentText = "LoadingPercentText";

	public const string UEID_contents = "contents";

	private Image _UE_rootNode;

	private TMP_Text _UE_LoadingPercentText;

	private TMP_Text _UE_contents;

	[SerializeField]
	private List<LoadingSceneData> loadingSceneDataList = new List<LoadingSceneData>();

	[SerializeField]
	private List<SceneLoadingRule> sceneLoadingRules = new List<SceneLoadingRule>();

	private LoadingSceneData currentLoadingSceneData;

	[SerializeField]
	public float LoadingSceneChangeTimeInSec = 2.5f;

	private float loadingStartTime;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_LoadingPercentText => _UE_LoadingPercentText ?? (_UE_LoadingPercentText = PickText("LoadingPercentText"));

	public TMP_Text UE_contents => _UE_contents ?? (_UE_contents = PickText("contents"));

	private void OnEnable()
	{
		SetLoadingText("STRING_LOADING");
	}

	protected override void OnShow()
	{
		base.OnShow();
		loadingStartTime = Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	public IEnumerator WaitForMinimumLoadingTime()
	{
		if ((float)Hub.s.timeutil.GetCurrentTickMilliSec() - loadingStartTime < LoadingSceneChangeTimeInSec * 1000f)
		{
			yield return new WaitForSeconds(LoadingSceneChangeTimeInSec - ((float)Hub.s.timeutil.GetCurrentTickMilliSec() - loadingStartTime) / 1000f);
		}
	}

	public void SetLoadingScene(string loadingSceneKey)
	{
		if (currentLoadingSceneData != null && currentLoadingSceneData.loadingScenes != null)
		{
			for (int i = 0; i < currentLoadingSceneData.loadingScenes.Count; i++)
			{
				if (currentLoadingSceneData.loadingScenes[i] != null)
				{
					currentLoadingSceneData.loadingScenes[i].gameObject.SetActive(value: false);
				}
			}
		}
		SceneLoadingRule sceneLoadingRule = sceneLoadingRules.Find((SceneLoadingRule r) => r.loadingSceneKey == loadingSceneKey);
		if (sceneLoadingRule == null)
		{
			Debug.LogWarning("Can't find '" + loadingSceneKey + "' in sceneLoadingRules");
			sceneLoadingRule = sceneLoadingRules.Find((SceneLoadingRule r) => r.loadingSceneKey == "Default");
		}
		if (sceneLoadingRule == null)
		{
			Debug.LogWarning("Can't find 'Default' rule in sceneLoadingRules");
			return;
		}
		if (sceneLoadingRule.loadingSceneNameList != null && sceneLoadingRule.loadingSceneNameList.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, sceneLoadingRule.loadingSceneNameList.Count);
			string selectedSceneName = sceneLoadingRule.loadingSceneNameList[index];
			currentLoadingSceneData = loadingSceneDataList.Find((LoadingSceneData data) => data.loadingSceneName == selectedSceneName);
			if (currentLoadingSceneData == null)
			{
				Debug.LogWarning("Can't find '" + selectedSceneName + "' in loadingSceneDataList");
			}
		}
		else
		{
			currentLoadingSceneData = null;
			Debug.LogWarning("No loading scene name list found for '" + loadingSceneKey + "'");
		}
		if (currentLoadingSceneData == null || currentLoadingSceneData.loadingScenes == null)
		{
			return;
		}
		for (int num = 0; num < currentLoadingSceneData.loadingScenes.Count; num++)
		{
			if (currentLoadingSceneData.loadingScenes[num] != null)
			{
				currentLoadingSceneData.loadingScenes[num].gameObject.SetActive(value: true);
			}
		}
	}

	public void SetLoadingText(string str)
	{
		UE_LoadingPercentText.text = Hub.GetL10NText(str);
	}
}
