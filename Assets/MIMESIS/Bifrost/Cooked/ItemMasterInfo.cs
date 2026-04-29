using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bifrost.ConstEnum;

namespace Bifrost.Cooked
{
	public abstract class ItemMasterInfo
	{
		public ItemType ItemType;

		public readonly int MasterID;

		public readonly string Name;

		public readonly int Weight;

		public readonly string LootingObjectID;

		public readonly int ItemDropID;

		public readonly string AttachSocketName;

		public readonly int PriceForSellMin;

		public readonly int PriceForSellMax;

		public readonly PuppetHandheldState PuppetHandheldState;

		public readonly List<string> ToolTip;

		public readonly string VendingMachineTooltip;

		public readonly bool IsVendingMachineExchange;

		public readonly bool ForbidChange;

		public readonly bool PreserveOnWipe;

		public readonly bool VisibleGaugeCount;

		public readonly bool VisibleDurabilityCount;

		public readonly int HandheldAuraSkillID;

		public readonly int HandheldAbnormalID;

		public readonly int SpawnFieldSkillID;

		public readonly ItemSpawnFieldSkillEnvConditionType SpawnFieldSkillEnvConditionType;

		public readonly ItemSpawnFieldSkillEnclosureConditionType SpawnFieldSkillEnclosureConditionType;

		public readonly int SpawnFieldSkillTimeMin;

		public readonly int SpawnFieldSkillTimeMax;

		public readonly int SpawnFieldSkillWaitTime;

		public ImmutableArray<string> SpawnFieldSkillWaitEffects = ImmutableArray<string>.Empty;

		public string SpawnFieldSkillWaitEffectName;

		public string SpawnFieldSkillWaitEffectSocket;

		public int SpawnFieldSkillWaitEffectDurationMSec;

		public readonly int SpawnFieldSkillRate;

		public readonly string PropertyKey;

		public readonly bool IsBoostItemCandidate;

		public readonly int SoundAggroPerUse;

		public readonly int SoundAggroInHandPerTick;

		public readonly int SoundAggroInHandToggleOnPerTick;

		public readonly bool HideItemByEmote;

		public readonly bool IsPromotionItem;

		public readonly bool IsPromotionItemHidden;

		public ItemMasterInfo(ItemType type, int masterID, string name, int weight, string lootingObjectID, int crowShopInputGroup, string attachSocketName, int puppetHandheldState, int priceForSellMin, int priceForSellMax, List<string> toolTip, string vendingMachineTooltip, bool isVendingMachineExchange, bool forbidChange, bool preserveOnWipe, bool visibleGaugeCount, bool visibleDurabilityCount, int handheldAuraSkillID, int handheldAbnormalID, int spawnFieldSkillID, int spawnFieldSkillEnvCondition, int spawnFieldSkillEnclosureCondition, int spawnFieldSkillTimeMin, int spawnFieldSkillTimeMax, int spawnFieldSkillWaitTime, List<string> spawnFieldSkillWaitEffectList, int spawnFieldSkillRate, string propertyKey, bool isBoostItemCandidate, int soundAggroPerUse, int soundAggroInHandPerTick, int soundAggroInHandToggleOnPerTick, bool hideItemByEmote, bool isPromotionItem, bool isPromotionItemHidden)
		{
			ItemType = type;
			MasterID = masterID;
			Name = name;
			Weight = weight;
			LootingObjectID = lootingObjectID;
			ItemDropID = crowShopInputGroup;
			AttachSocketName = attachSocketName;
			PuppetHandheldState = (PuppetHandheldState)puppetHandheldState;
			PriceForSellMin = priceForSellMin;
			PriceForSellMax = priceForSellMax;
			ToolTip = toolTip;
			VendingMachineTooltip = vendingMachineTooltip;
			IsVendingMachineExchange = isVendingMachineExchange;
			ForbidChange = forbidChange;
			PreserveOnWipe = preserveOnWipe;
			VisibleGaugeCount = visibleGaugeCount;
			VisibleDurabilityCount = visibleDurabilityCount;
			HandheldAuraSkillID = handheldAuraSkillID;
			HandheldAbnormalID = handheldAbnormalID;
			SpawnFieldSkillID = spawnFieldSkillID;
			if (Enum.IsDefined(typeof(ItemSpawnFieldSkillEnvConditionType), spawnFieldSkillEnvCondition))
			{
				SpawnFieldSkillEnvConditionType = (ItemSpawnFieldSkillEnvConditionType)spawnFieldSkillEnvCondition;
			}
			else
			{
				Logger.RError($"[ItemMasterInfo] Invalid SpawnFieldSkillEnvCondition {spawnFieldSkillEnvCondition}");
			}
			if (Enum.IsDefined(typeof(ItemSpawnFieldSkillEnclosureConditionType), spawnFieldSkillEnclosureCondition))
			{
				SpawnFieldSkillEnclosureConditionType = (ItemSpawnFieldSkillEnclosureConditionType)spawnFieldSkillEnclosureCondition;
			}
			else
			{
				Logger.RError($"[ItemMasterInfo] Invalid SpawnFieldSkillEnclosureCondition {spawnFieldSkillEnclosureCondition}");
			}
			SpawnFieldSkillTimeMin = spawnFieldSkillTimeMin;
			SpawnFieldSkillTimeMax = spawnFieldSkillTimeMax;
			SpawnFieldSkillWaitTime = spawnFieldSkillWaitTime;
			if (spawnFieldSkillWaitEffectList.Count > 0)
			{
				SpawnFieldSkillWaitEffectName = spawnFieldSkillWaitEffectList[0].Trim();
				SpawnFieldSkillWaitEffectSocket = spawnFieldSkillWaitEffectList[1].Trim();
				SpawnFieldSkillWaitEffectDurationMSec = int.Parse(spawnFieldSkillWaitEffectList[2].Trim());
			}
			else
			{
				SpawnFieldSkillWaitEffectName = string.Empty;
				SpawnFieldSkillWaitEffectSocket = string.Empty;
				SpawnFieldSkillWaitEffectDurationMSec = 0;
			}
			SpawnFieldSkillRate = spawnFieldSkillRate;
			PropertyKey = propertyKey;
			IsBoostItemCandidate = isBoostItemCandidate;
			SoundAggroPerUse = soundAggroPerUse;
			SoundAggroInHandPerTick = soundAggroInHandPerTick;
			SoundAggroInHandToggleOnPerTick = soundAggroInHandToggleOnPerTick;
			HideItemByEmote = hideItemByEmote;
			IsPromotionItem = isPromotionItem;
			IsPromotionItemHidden = isPromotionItemHidden;
		}

		public int GetPrice()
		{
			return SimpleRandUtil.Next(PriceForSellMin, PriceForSellMax + 1);
		}

		public int GetMeanPrice()
		{
			return (PriceForSellMin + PriceForSellMax) / 2;
		}
	}
}
