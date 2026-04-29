using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MMLootingObjectTable", menuName = "_Mimic/MMLootingObjectTable", order = 0)]
public class MMLootingObjectTable : ScriptableObject
{
	[Serializable]
	public class SkinInfo
	{
		public string skinName = string.Empty;

		public GameObject? prefab;

		public string iconSpriteId = string.Empty;

		public string posterIconSpriteId = "BonusScrabDummy512512";

		public string pickAudioClipId = string.Empty;

		public string dropAudioClipId = string.Empty;
	}

	[Serializable]
	public class Row
	{
		[FormerlySerializedAs("name")]
		public string id = string.Empty;

		public GameObject? prefab;

		[Tooltip("MMSpriteTable에서 아이콘 스프라이트를 찾기 위한 id")]
		public string iconSpriteId = string.Empty;

		[Tooltip("MMSpriteTable에서 포스터 아이콘 스프라이트를 찾기 위한 id")]
		public string posterIconSpriteId = "BonusScrabDummy512512";

		[Tooltip("MMAudioClipTable에서 루팅 오브젝트를 주울 때 재생할 오디오 클립을 찾기 위한 id")]
		public string pickAudioClipId = string.Empty;

		[Tooltip("MMAudioClipTable에서 루팅 오브젝트를 버릴 때 재생할 오디오 클립을 찾기 위한 id")]
		public string dropAudioClipId = string.Empty;

		public List<SkinInfo> SkinList = new List<SkinInfo>();
	}

	public class SkinnedItemInfo
	{
		public int itemMasterID;

		public string skinName = string.Empty;

		public GameObject? prefab;

		[Tooltip("MMSpriteTable에서 아이콘 스프라이트를 찾기 위한 id")]
		public string iconSpriteId = string.Empty;

		[Tooltip("MMSpriteTable에서 포스터 아이콘 스프라이트를 찾기 위한 id")]
		public string posterIconSpriteId = "BonusScrabDummy512512";

		[Tooltip("MMAudioClipTable에서 루팅 오브젝트를 주울 때 재생할 오디오 클립을 찾기 위한 id")]
		public string pickAudioClipId = string.Empty;

		[Tooltip("MMAudioClipTable에서 루팅 오브젝트를 버릴 때 재생할 오디오 클립을 찾기 위한 id")]
		public string dropAudioClipId = string.Empty;
	}

	public string[] prefabFolders = new string[1] { "Assets/_mimic/prefabs/LootingObject" };

	private Dictionary<int, (bool isSkinned, SkinnedItemInfo skinnedItemInfo)> skinnedItemInfos = new Dictionary<int, (bool, SkinnedItemInfo)>();

	public List<Row> rows = new List<Row>();

	public void ClearSkinnedItemInfos()
	{
		skinnedItemInfos.Clear();
	}

	public (bool isSkinned, SkinnedItemInfo? skinnedItemInfo) FindSkinnedItemInfo(int itemMasterID)
	{
		if (skinnedItemInfos.TryGetValue(itemMasterID, out (bool, SkinnedItemInfo) value))
		{
			return (isSkinned: value.Item1, skinnedItemInfo: value.Item2);
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
		if (itemInfo == null)
		{
			Logger.RError("itemMasterInfo is null");
			return (isSkinned: false, skinnedItemInfo: null);
		}
		Row row = FindRow(itemInfo.LootingObjectID);
		if (row == null)
		{
			Logger.RError("foundRow is null");
			return (isSkinned: false, skinnedItemInfo: null);
		}
		Hub.s.pdata.AppliedItemSkinDictionary.TryGetValue(itemMasterID, out string skinName);
		bool item = false;
		SkinnedItemInfo skinnedItemInfo = new SkinnedItemInfo();
		if (string.IsNullOrEmpty(skinName))
		{
			skinnedItemInfo.itemMasterID = itemMasterID;
			skinnedItemInfo.skinName = string.Empty;
			skinnedItemInfo.prefab = row.prefab;
			skinnedItemInfo.iconSpriteId = row.iconSpriteId;
			skinnedItemInfo.posterIconSpriteId = row.posterIconSpriteId;
			skinnedItemInfo.pickAudioClipId = row.pickAudioClipId;
			skinnedItemInfo.dropAudioClipId = row.dropAudioClipId;
		}
		else
		{
			SkinInfo skinInfo = row.SkinList.FirstOrDefault((SkinInfo skin) => skin.skinName == skinName);
			if (skinInfo == null)
			{
				Logger.RError("Can't find " + skinName + " in " + row.id + ", using default skin");
				skinnedItemInfo.itemMasterID = itemMasterID;
				skinnedItemInfo.skinName = string.Empty;
				skinnedItemInfo.prefab = row.prefab;
				skinnedItemInfo.iconSpriteId = row.iconSpriteId;
				skinnedItemInfo.posterIconSpriteId = row.posterIconSpriteId;
				skinnedItemInfo.pickAudioClipId = row.pickAudioClipId;
				skinnedItemInfo.dropAudioClipId = row.dropAudioClipId;
			}
			else
			{
				item = true;
				skinnedItemInfo.itemMasterID = itemMasterID;
				skinnedItemInfo.skinName = skinName;
				skinnedItemInfo.prefab = skinInfo.prefab;
				skinnedItemInfo.iconSpriteId = ((skinInfo.iconSpriteId == string.Empty) ? row.iconSpriteId : skinInfo.iconSpriteId);
				skinnedItemInfo.posterIconSpriteId = ((skinInfo.posterIconSpriteId == string.Empty) ? row.posterIconSpriteId : skinInfo.posterIconSpriteId);
				skinnedItemInfo.pickAudioClipId = ((skinInfo.pickAudioClipId == string.Empty) ? row.pickAudioClipId : skinInfo.pickAudioClipId);
				skinnedItemInfo.dropAudioClipId = ((skinInfo.dropAudioClipId == string.Empty) ? row.dropAudioClipId : skinInfo.dropAudioClipId);
			}
		}
		skinnedItemInfos[itemMasterID] = (item, skinnedItemInfo);
		return (isSkinned: item, skinnedItemInfo: skinnedItemInfo);
	}

	private Row? FindRow(string rowId)
	{
		foreach (Row row in rows)
		{
			if (row != null && row.id == rowId)
			{
				return row;
			}
		}
		return null;
	}

	private Row? FindRow(int itemMasterID)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
		if (itemInfo == null)
		{
			Logger.RError("itemInfo is null");
			return null;
		}
		return FindRow(itemInfo.LootingObjectID);
	}
}
