using System.Collections.Generic;
using UnityEngine;

namespace PrefabSwapper
{
	[AddComponentMenu("PrefabSwapper/SwapPrefab")]
	[ExecuteInEditMode]
	public class SwapPrefab : MonoBehaviour
	{
		public static List<string> categoryList;

		public static List<string> prefabList;

		public static List<string> variationList;

		public bool placed;

		public string category;

		public string prefab;

		public string variation;

		public string[] quickSwaps;

		public string[] quickSwapsBtns;

		public string[] additionals;

		public string[] additionalsBtns;

		public Vector3[] additionalsPos;

		public Vector3[] additionalsRot;

		public string[] spawnAlongs;

		public Vector3[] spawnAlongsPos;

		public Vector3[] spawnAlongsRot;

		private string exCategory;

		private string exPrefab;

		private string exVariation;

		public void CreateCategoryList()
		{
			categoryList = new List<string>();
			SwapPrefabList.SwapPrefabProperties[] swapPrefabProperties = SwapPrefabList.swapPrefabProperties;
			foreach (SwapPrefabList.SwapPrefabProperties swapPrefabProperties2 in swapPrefabProperties)
			{
				if (!categoryList.Contains(swapPrefabProperties2.category))
				{
					categoryList.Add(swapPrefabProperties2.category);
				}
			}
		}

		public void CreatePrefabList()
		{
			prefabList = new List<string>();
			SwapPrefabList.SwapPrefabProperties[] swapPrefabProperties = SwapPrefabList.swapPrefabProperties;
			foreach (SwapPrefabList.SwapPrefabProperties swapPrefabProperties2 in swapPrefabProperties)
			{
				if (swapPrefabProperties2.category == category && !prefabList.Contains(swapPrefabProperties2.prefabName))
				{
					prefabList.Add(swapPrefabProperties2.prefabName);
				}
			}
		}

		public void CreateVariationList()
		{
			variationList = new List<string>();
			variationList.Add(prefab);
			SwapPrefabList.SwapPrefabProperties[] swapPrefabProperties = SwapPrefabList.swapPrefabProperties;
			foreach (SwapPrefabList.SwapPrefabProperties swapPrefabProperties2 in swapPrefabProperties)
			{
				if (!(swapPrefabProperties2.category == category) || !(swapPrefabProperties2.prefabName == prefab))
				{
					continue;
				}
				string[] variations = swapPrefabProperties2.variations;
				foreach (string item in variations)
				{
					if (!variationList.Contains(item))
					{
						variationList.Add(item);
					}
				}
			}
		}

		public void FindPrefabProperties()
		{
			SwapPrefabList.SwapPrefabProperties[] swapPrefabProperties = SwapPrefabList.swapPrefabProperties;
			foreach (SwapPrefabList.SwapPrefabProperties swapPrefabProperties2 in swapPrefabProperties)
			{
				if (swapPrefabProperties2.prefabName == base.transform.name)
				{
					variation = swapPrefabProperties2.prefabName;
					GetPrefabProperties(swapPrefabProperties2);
					return;
				}
			}
			swapPrefabProperties = SwapPrefabList.swapPrefabProperties;
			foreach (SwapPrefabList.SwapPrefabProperties swapPrefabProperties3 in swapPrefabProperties)
			{
				string[] variations = swapPrefabProperties3.variations;
				foreach (string text in variations)
				{
					if (text == base.transform.name)
					{
						variation = text;
						GetPrefabProperties(swapPrefabProperties3);
						return;
					}
				}
			}
		}

		public void CleanPrefabProperties()
		{
			category = "";
			prefab = "";
			quickSwaps = new string[0];
			quickSwapsBtns = new string[0];
			additionals = new string[0];
			additionalsBtns = new string[0];
			additionalsPos = new Vector3[0];
			additionalsRot = new Vector3[0];
			spawnAlongs = new string[0];
			spawnAlongsPos = new Vector3[0];
			spawnAlongsRot = new Vector3[0];
			exCategory = "";
			exPrefab = "";
			exVariation = "";
		}

		public void GetPrefabProperties(SwapPrefabList.SwapPrefabProperties _prefab)
		{
			category = _prefab.category;
			prefab = _prefab.prefabName;
			quickSwaps = _prefab.swapToPrefab;
			quickSwapsBtns = _prefab.swapToPrefabButtonName;
			additionals = _prefab.addAdditional;
			additionalsBtns = _prefab.addAdditionalButtonName;
			additionalsPos = _prefab.addAdditionalPos;
			additionalsRot = _prefab.addAdditionalRot;
			spawnAlongs = _prefab.spawnAlong;
			spawnAlongsPos = _prefab.spawnAlongPos;
			spawnAlongsRot = _prefab.spawnAlongRot;
			exCategory = category;
			exPrefab = prefab;
			exVariation = variation;
		}

		public void Initialize()
		{
			FindPrefabProperties();
			CreateCategoryList();
			CreatePrefabList();
			CreateVariationList();
		}
	}
}
