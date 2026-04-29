using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Bifrost;
using Bifrost.AbnormalData;
using Bifrost.AuraSkillData;
using Bifrost.BattleActionData;
using Bifrost.CharacterSkillData;
using Bifrost.CharacterSkillSequenceData;
using Bifrost.Const;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using Bifrost.CycleCutsceneData;
using Bifrost.DefAbnormal;
using Bifrost.DefStats;
using Bifrost.Dungeon;
using Bifrost.Faction;
using Bifrost.FieldSkillData;
using Bifrost.FieldSkillSequenceData;
using Bifrost.GrabData;
using Bifrost.ItemConsumable;
using Bifrost.ItemDrop;
using Bifrost.ItemEquipment;
using Bifrost.ItemMiscellany;
using Bifrost.LocalizationData;
using Bifrost.MapData;
using Bifrost.MonsterData;
using Bifrost.MonsterSkillData;
using Bifrost.MonsterSkillSequenceData;
using Bifrost.PlayerData;
using Bifrost.ProjectileData;
using Bifrost.PromotionRewardData;
using Bifrost.ShopGroup;
using Bifrost.SkillTargetEffectData;
using Bifrost.SpawnableMiscGroup;
using Bifrost.SpawnableMonsterGroup;
using Bifrost.TramupgradeData;
using Bifrost.WeatherData;

public class ExcelDataManager
{
	private ResourceDataHandler? _dataHandler;

	private static object syncRoot = new object();

	public DataConsts Consts;

	public ImmutableDictionary<int, MonsterInfo> MonsterInfoDict { get; private set; } = ImmutableDictionary<int, MonsterInfo>.Empty;

	public ImmutableDictionary<int, MapMasterInfo> RoomInfoDict { get; private set; } = ImmutableDictionary<int, MapMasterInfo>.Empty;

	public ImmutableDictionary<int, PlayerMasterInfo> PlayerInfoDict { get; private set; } = ImmutableDictionary<int, PlayerMasterInfo>.Empty;

	public ImmutableDictionary<int, SkillInfo> SkillInfoDict { get; private set; } = ImmutableDictionary<int, SkillInfo>.Empty;

	public ImmutableDictionary<int, SkillSequenceInfo> SkillSequenceInfoDict { get; private set; } = ImmutableDictionary<int, SkillSequenceInfo>.Empty;

	public ImmutableDictionary<int, DefAbnormal_MasterData> DefAbnormalDict { get; private set; } = ImmutableDictionary<int, DefAbnormal_MasterData>.Empty;

	public ImmutableDictionary<CCType, DefAbnormal_MasterData> CCAbnormalDict { get; private set; } = ImmutableDictionary<CCType, DefAbnormal_MasterData>.Empty;

	public ImmutableDictionary<AbnormalStatsCategory, DefAbnormal_MasterData> StatsAbnormalDict { get; private set; } = ImmutableDictionary<AbnormalStatsCategory, DefAbnormal_MasterData>.Empty;

	public ImmutableDictionary<DispelType, DefAbnormal_MasterData> DispelAbnormalDict { get; private set; } = ImmutableDictionary<DispelType, DefAbnormal_MasterData>.Empty;

	public ImmutableDictionary<ImmuneType, DefAbnormal_MasterData> ImmuneAbnormalDict { get; private set; } = ImmutableDictionary<ImmuneType, DefAbnormal_MasterData>.Empty;

	public ImmutableDictionary<StatType, MutableStatInfo> MutableStatInfos { get; private set; } = ImmutableDictionary<StatType, MutableStatInfo>.Empty;

	public ImmutableHashSet<StatType> OtherActorSyncStats { get; private set; } = ImmutableHashSet<StatType>.Empty;

	public ImmutableHashSet<MutableStatType> OtherActorSyncMutableStats { get; private set; } = ImmutableHashSet<MutableStatType>.Empty;

	public ImmutableHashSet<int> FactionCategory { get; private set; } = ImmutableHashSet<int>.Empty;

	public ImmutableDictionary<int, Faction_MasterData> Factions { get; private set; } = ImmutableDictionary<int, Faction_MasterData>.Empty;

	public ImmutableDictionary<int, WeatherInfo> Weathers { get; private set; } = ImmutableDictionary<int, WeatherInfo>.Empty;

	public ImmutableDictionary<int, ItemMasterInfo> ItemInfoDict { get; private set; } = ImmutableDictionary<int, ItemMasterInfo>.Empty;

	public ImmutableDictionary<int, DungeonMasterInfo> DungeonInfoDict { get; private set; } = ImmutableDictionary<int, DungeonMasterInfo>.Empty;

	public ImmutableDictionary<int, ItemDropInfo> ItemDropDict { get; private set; } = ImmutableDictionary<int, ItemDropInfo>.Empty;

	public ImmutableDictionary<int, ShopGroup_MasterData> ShopGroupDict { get; private set; } = ImmutableDictionary<int, ShopGroup_MasterData>.Empty;

	public ImmutableDictionary<int, AbnormalInfo> AbnormalDict { get; private set; } = ImmutableDictionary<int, AbnormalInfo>.Empty;

	public ImmutableDictionary<int, SkillTargetEffectDataInfo> SkillTargetEffecDict { get; private set; } = ImmutableDictionary<int, SkillTargetEffectDataInfo>.Empty;

	public ImmutableDictionary<string, LocalizationData_MasterData> LocalizationDict { get; private set; } = ImmutableDictionary<string, LocalizationData_MasterData>.Empty;

	public ImmutableDictionary<int, AttachMasterInfo> AttachDataDict { get; private set; } = ImmutableDictionary<int, AttachMasterInfo>.Empty;

	public ImmutableDictionary<int, ProjectileInfo> ProjectileInfoDict { get; private set; } = ImmutableDictionary<int, ProjectileInfo>.Empty;

	public ImmutableDictionary<int, FieldSkillInfo> FieldSkillDict { get; private set; } = ImmutableDictionary<int, FieldSkillInfo>.Empty;

	public ImmutableDictionary<int, CycleMasterInfo> CycleDict { get; private set; } = ImmutableDictionary<int, CycleMasterInfo>.Empty;

	public ImmutableDictionary<int, AuraInfo> AuraDict { get; private set; } = ImmutableDictionary<int, AuraInfo>.Empty;

	public ImmutableDictionary<string, BattleActionInfo> BattleActionDict { get; private set; } = ImmutableDictionary<string, BattleActionInfo>.Empty;

	public ImmutableDictionary<int, SpawnableItemInfo> SpawnableItemDict { get; private set; } = ImmutableDictionary<int, SpawnableItemInfo>.Empty;

	public ImmutableDictionary<int, SpawnableMonsterInfo> SpawnableMonsterDict { get; private set; } = ImmutableDictionary<int, SpawnableMonsterInfo>.Empty;

	public ImmutableDictionary<int, PromotionRewardInfo> PromotionRewardDict { get; private set; } = ImmutableDictionary<int, PromotionRewardInfo>.Empty;

	public ImmutableDictionary<int, PromotionRewardInfo> PromotionRewardDictByItemDefId { get; private set; } = ImmutableDictionary<int, PromotionRewardInfo>.Empty;

	public ImmutableDictionary<int, TramupgradeData_MasterData> TramupgradeDataDict { get; private set; } = ImmutableDictionary<int, TramupgradeData_MasterData>.Empty;

	public int MaxSessionCount { get; private set; }

	public ExcelDataManager(ResourceDataHandler dataHandler)
	{
		_dataHandler = dataHandler;
	}

	public bool Initialize()
	{
		if (!LoadDataFrom())
		{
			Logger.RError("[LoadData] LoadDataFrom failed");
			return false;
		}
		return true;
	}

	public void Dispose()
	{
		MonsterInfoDict.Clear();
		RoomInfoDict.Clear();
	}

	public bool LoadDataFrom()
	{
		try
		{
			return LoadDataInternal();
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return false;
		}
	}

	private void LoadDataFile<T>(OnLoadCallback<T> callback) where T : ISchema, new()
	{
		string text = string.Empty;
		try
		{
			text = typeof(T).Name;
			if (!text.EndsWith("_MasterDataHolder"))
			{
				throw new InvalidOperationException();
			}
			text = text.Substring(0, text.Length - "_MasterDataHolder".Length);
			Stopwatch.StartNew();
			using Stream stream = _dataHandler.GetStream("masterdata/" + text + ".json");
			using StreamReader streamReader = new StreamReader(stream);
			string jsonData = streamReader.ReadToEnd();
			T val = new T();
			if (!val.LoadFromJson<T>(jsonData))
			{
				throw new InvalidDataException();
			}
			callback(val);
		}
		catch (Exception arg)
		{
			Logger.RError($"MasterData {text} Loading Failed. {arg}");
			throw;
		}
	}

	private ImmutableDictionary<int, DataT> LoadDataFileAsDict<HolderT, DataT>(OnValidateCallback<DataT>? validate = null) where HolderT : ISchema, new() where DataT : ISchema, new()
	{
		ImmutableDictionary<int, DataT>.Builder builder = ImmutableDictionary.CreateBuilder<int, DataT>();
		FieldInfo listProperty = typeof(HolderT).GetField("dataHolder");
		FieldInfo idProperty = typeof(DataT).GetField("id");
		if (listProperty == null || idProperty == null)
		{
			throw new Exception("invalid argument");
		}
		LoadDataFile(delegate(HolderT holder)
		{
			foreach (DataT item in (listProperty.GetValue(holder) as List<DataT>) ?? throw new Exception("invalid argument"))
			{
				int key = Convert.ToInt32(idProperty.GetValue(item));
				OnValidateCallback<DataT>? onValidateCallback = validate;
				if (onValidateCallback == null || onValidateCallback(item))
				{
					builder.Add(key, item);
				}
			}
		});
		return builder.ToImmutable();
	}

	private ImmutableDictionary<int, CookedT> LoadDataFileAsCooked<HolderT, DataT, CookedT>(OnValidateCallback<DataT>? validate = null) where HolderT : ISchema, new() where DataT : ISchema, new() where CookedT : class
	{
		ImmutableDictionary<int, CookedT>.Builder builder = ImmutableDictionary.CreateBuilder<int, CookedT>();
		FieldInfo listProperty = typeof(HolderT).GetField("dataHolder");
		FieldInfo idProperty = typeof(DataT).GetField("id");
		if (listProperty == null || idProperty == null)
		{
			throw new Exception("invalid argument");
		}
		LoadDataFile(delegate(HolderT holder)
		{
			foreach (DataT item in (listProperty.GetValue(holder) as List<DataT>) ?? throw new Exception("invalid argument"))
			{
				int key = Convert.ToInt32(idProperty.GetValue(item));
				OnValidateCallback<DataT>? onValidateCallback = validate;
				if (onValidateCallback == null || onValidateCallback(item))
				{
					try
					{
						ParameterInfo[] parameters = typeof(CookedT).GetConstructors()[0].GetParameters();
						CookedT val = ((parameters.Length == 0 || !(parameters[0].ParameterType == typeof(ExcelDataManager))) ? (Activator.CreateInstance(typeof(CookedT), item) as CookedT) : (Activator.CreateInstance(typeof(CookedT), this, item) as CookedT));
						if (val == null)
						{
							throw new Exception("invalid argument");
						}
						builder.Add(key, val);
					}
					catch (Exception arg)
					{
						Logger.RError($"MasterData {typeof(DataT).Name} Loading Failed. {arg}");
					}
				}
			}
		});
		return builder.ToImmutable();
	}

	private bool LoadDataInternal()
	{
		try
		{
			LoadDataFile(delegate(Const_MasterDataHolder holder)
			{
				Consts = new DataConsts(holder);
			});
			LoadDataFile(delegate(LocalizationData_MasterDataHolder holder)
			{
				ImmutableDictionary<string, LocalizationData_MasterData>.Builder builder4 = ImmutableDictionary.CreateBuilder<string, LocalizationData_MasterData>();
				foreach (LocalizationData_MasterData item in holder.dataHolder)
				{
					builder4.Add(item.key, item);
				}
				LocalizationDict = builder4.ToImmutable();
			});
			LoadDataFile(delegate(BattleActionData_MasterDataHolder holder)
			{
				ImmutableDictionary<string, BattleActionInfo>.Builder builder4 = ImmutableDictionary.CreateBuilder<string, BattleActionInfo>();
				foreach (BattleActionData_MasterData item2 in holder.dataHolder)
				{
					builder4.Add(item2.key, new BattleActionInfo(item2));
				}
				BattleActionDict = builder4.ToImmutable();
			});
			TramupgradeDataDict = LoadDataFileAsDict<TramupgradeData_MasterDataHolder, TramupgradeData_MasterData>();
			SpawnableItemDict = LoadDataFileAsCooked<SpawnableMiscGroup_MasterDataHolder, SpawnableMiscGroup_MasterData, SpawnableItemInfo>();
			SpawnableMonsterDict = LoadDataFileAsCooked<SpawnableMonsterGroup_MasterDataHolder, SpawnableMonsterGroup_MasterData, SpawnableMonsterInfo>();
			MonsterInfoDict = LoadDataFileAsCooked<MonsterData_MasterDataHolder, MonsterData_MasterData, MonsterInfo>();
			RoomInfoDict = LoadDataFileAsCooked<MapData_MasterDataHolder, MapData_MasterData, MapMasterInfo>();
			PlayerInfoDict = LoadDataFileAsCooked<PlayerData_MasterDataHolder, PlayerData_MasterData, PlayerMasterInfo>();
			DungeonInfoDict = LoadDataFileAsCooked<Dungeon_MasterDataHolder, Dungeon_MasterData, DungeonMasterInfo>();
			Factions = LoadDataFileAsDict<Faction_MasterDataHolder, Faction_MasterData>();
			ItemDropDict = LoadDataFileAsCooked<ItemDrop_MasterDataHolder, ItemDrop_MasterData, ItemDropInfo>();
			ShopGroupDict = LoadDataFileAsDict<ShopGroup_MasterDataHolder, ShopGroup_MasterData>();
			AttachDataDict = LoadDataFileAsCooked<GrabData_MasterDataHolder, GrabData_MasterData, AttachMasterInfo>();
			ProjectileInfoDict = LoadDataFileAsCooked<ProjectileData_MasterDataHolder, ProjectileData_MasterData, ProjectileInfo>();
			Weathers = LoadDataFileAsCooked<WeatherData_MasterDataHolder, WeatherData_MasterData, WeatherInfo>();
			CycleDict = LoadDataFileAsCooked<CycleCutsceneData_MasterDataHolder, CycleCutsceneData_MasterData, CycleMasterInfo>();
			LoadDataFile(delegate(DefStats_MasterDataHolder holder)
			{
				ImmutableDictionary<StatType, MutableStatInfo>.Builder builder4 = ImmutableDictionary.CreateBuilder<StatType, MutableStatInfo>();
				ImmutableHashSet<StatType>.Builder builder5 = ImmutableHashSet.CreateBuilder<StatType>();
				ImmutableHashSet<MutableStatType>.Builder builder6 = ImmutableHashSet.CreateBuilder<MutableStatType>();
				foreach (DefStats_MasterData item3 in holder.dataHolder)
				{
					if (StringUtil.ConvertStringToEnum<StatType>(item3.key, out var result))
					{
						if (item3.mutable_key != "none")
						{
							if (StringUtil.ConvertStringToEnum<MutableStatType>(item3.mutable_key, out var result2))
							{
								builder4.Add(result, new MutableStatInfo(result, result2));
								if (item3.sync_mutable)
								{
									builder6.Add(result2);
								}
							}
							else
							{
								Logger.RWarn("Invalid MutableStatType : " + item3.mutable_key);
							}
						}
						else
						{
							builder4.Add(result, MutableStatInfo.InvalidMutableStat);
						}
						if (item3.sync_immutable)
						{
							builder5.Add(result);
						}
					}
				}
				MutableStatInfos = builder4.ToImmutable();
				OtherActorSyncStats = builder5.ToImmutable();
				OtherActorSyncMutableStats = builder6.ToImmutable();
			});
			LoadDataFile(delegate(DefAbnormal_MasterDataHolder holder)
			{
				MakeAbnormalDefDict(holder);
			});
			AbnormalDict = LoadDataFileAsCooked<AbnormalData_MasterDataHolder, AbnormalData_MasterData, AbnormalInfo>();
			SkillTargetEffecDict = LoadDataFileAsCooked<SkillTargetEffectData_MasterDataHolder, SkillTargetEffectData_MasterData, SkillTargetEffectDataInfo>();
			AuraDict = LoadDataFileAsCooked<AuraSkillData_MasterDataHolder, AuraSkillData_MasterData, AuraInfo>();
			ImmutableDictionary<int, SkillSequenceInfo> immutableDictionary = LoadDataFileAsCooked<CharacterSkillSequenceData_MasterDataHolder, CharacterSkillSequenceData_MasterData, SkillSequenceInfo>();
			ImmutableDictionary<int, SkillSequenceInfo> immutableDictionary2 = LoadDataFileAsCooked<MonsterSkillSequenceData_MasterDataHolder, MonsterSkillSequenceData_MasterData, SkillSequenceInfo>();
			ImmutableDictionary<int, SkillSequenceInfo> immutableDictionary3 = LoadDataFileAsCooked<FieldSkillSequenceData_MasterDataHolder, FieldSkillSequenceData_MasterData, SkillSequenceInfo>();
			ImmutableDictionary<int, SkillSequenceInfo>.Builder builder = ImmutableDictionary.CreateBuilder<int, SkillSequenceInfo>();
			foreach (KeyValuePair<int, SkillSequenceInfo> item4 in immutableDictionary)
			{
				builder.Add(item4.Key, item4.Value);
			}
			foreach (KeyValuePair<int, SkillSequenceInfo> item5 in immutableDictionary2)
			{
				builder.Add(item5.Key, item5.Value);
			}
			foreach (KeyValuePair<int, SkillSequenceInfo> item6 in immutableDictionary3)
			{
				builder.Add(item6.Key, item6.Value);
			}
			SkillSequenceInfoDict = builder.ToImmutable();
			ImmutableDictionary<int, SkillInfo> immutableDictionary4 = LoadDataFileAsCooked<CharacterSkillData_MasterDataHolder, CharacterSkillData_MasterData, SkillInfo>();
			ImmutableDictionary<int, SkillInfo> immutableDictionary5 = LoadDataFileAsCooked<MonsterSkillData_MasterDataHolder, MonsterSkillData_MasterData, SkillInfo>();
			FieldSkillDict = LoadDataFileAsCooked<FieldSkillData_MasterDataHolder, FieldSkillData_MasterData, FieldSkillInfo>();
			ImmutableDictionary<int, SkillInfo>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, SkillInfo>();
			foreach (KeyValuePair<int, SkillInfo> item7 in immutableDictionary4)
			{
				builder2.Add(item7.Key, item7.Value);
			}
			foreach (KeyValuePair<int, SkillInfo> item8 in immutableDictionary5)
			{
				builder2.Add(item8.Key, item8.Value);
			}
			SkillInfoDict = builder2.ToImmutable();
			ImmutableDictionary<int, ItemConsumableInfo> immutableDictionary6 = LoadDataFileAsCooked<ItemConsumable_MasterDataHolder, ItemConsumable_MasterData, ItemConsumableInfo>();
			ImmutableDictionary<int, ItemEquipmentInfo> immutableDictionary7 = LoadDataFileAsCooked<ItemEquipment_MasterDataHolder, ItemEquipment_MasterData, ItemEquipmentInfo>();
			ImmutableDictionary<int, ItemMiscellanyInfo> immutableDictionary8 = LoadDataFileAsCooked<ItemMiscellany_MasterDataHolder, ItemMiscellany_MasterData, ItemMiscellanyInfo>();
			ImmutableDictionary<int, ItemMasterInfo>.Builder builder3 = ImmutableDictionary.CreateBuilder<int, ItemMasterInfo>();
			foreach (KeyValuePair<int, ItemConsumableInfo> item9 in immutableDictionary6)
			{
				builder3.Add(item9.Key, item9.Value);
			}
			foreach (KeyValuePair<int, ItemEquipmentInfo> item10 in immutableDictionary7)
			{
				builder3.Add(item10.Key, item10.Value);
			}
			foreach (KeyValuePair<int, ItemMiscellanyInfo> item11 in immutableDictionary8)
			{
				builder3.Add(item11.Key, item11.Value);
			}
			ItemInfoDict = builder3.ToImmutable();
			foreach (Faction_MasterData value in Factions.Values)
			{
				if (!FactionCategory.Contains(value.group))
				{
					FactionCategory = FactionCategory.Add(value.group);
				}
			}
			PromotionRewardDict = LoadDataFileAsCooked<PromotionRewardData_MasterDataHolder, PromotionRewardData_MasterData, PromotionRewardInfo>();
			PromotionRewardDictByItemDefId = PromotionRewardDict.Values.ToImmutableDictionary((PromotionRewardInfo x) => x.ItemDefID, (PromotionRewardInfo x) => x);
			WaitInitDone();
			return true;
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return false;
		}
	}

	private void WaitInitDone()
	{
		foreach (CycleMasterInfo value in CycleDict.Values)
		{
			if (MaxSessionCount < value.CycleCount)
			{
				MaxSessionCount = value.CycleCount;
			}
		}
	}

	private void MakeAbnormalDefDict(DefAbnormal_MasterDataHolder holder)
	{
		ImmutableDictionary<int, DefAbnormal_MasterData>.Builder builder = ImmutableDictionary.CreateBuilder<int, DefAbnormal_MasterData>();
		ImmutableDictionary<CCType, DefAbnormal_MasterData>.Builder builder2 = ImmutableDictionary.CreateBuilder<CCType, DefAbnormal_MasterData>();
		ImmutableDictionary<AbnormalStatsCategory, DefAbnormal_MasterData>.Builder builder3 = ImmutableDictionary.CreateBuilder<AbnormalStatsCategory, DefAbnormal_MasterData>();
		ImmutableDictionary<DispelType, DefAbnormal_MasterData>.Builder builder4 = ImmutableDictionary.CreateBuilder<DispelType, DefAbnormal_MasterData>();
		ImmutableDictionary<ImmuneType, DefAbnormal_MasterData>.Builder builder5 = ImmutableDictionary.CreateBuilder<ImmuneType, DefAbnormal_MasterData>();
		ImmutableArray.CreateBuilder<CCType>();
		foreach (DefAbnormal_MasterData item in holder.dataHolder)
		{
			builder.Add(item.id, item);
			switch ((AbnormalCategory)item.category)
			{
			case AbnormalCategory.CC:
			{
				CCType cCFromString = AbnormalType.GetCCFromString(item.name);
				if (builder2.ContainsKey(cCFromString))
				{
					Logger.RError($"Duplicated CCType {cCFromString}");
				}
				else
				{
					builder2.Add(cCFromString, item);
				}
				break;
			}
			case AbnormalCategory.Stats:
			{
				AbnormalStatsCategory abnormalStatsCategoryFromString = AbnormalType.GetAbnormalStatsCategoryFromString(item.name);
				if (builder3.ContainsKey(abnormalStatsCategoryFromString))
				{
					Logger.RError("Duplicated StatsAbnormal " + item.key);
				}
				else
				{
					builder3.Add(abnormalStatsCategoryFromString, item);
				}
				break;
			}
			case AbnormalCategory.Dispel:
			{
				DispelType dispelTypeFromString = AbnormalType.GetDispelTypeFromString(item.name);
				if (builder4.ContainsKey(dispelTypeFromString))
				{
					Logger.RError("Duplicated DispelAbnormal " + item.key);
				}
				else
				{
					builder4.Add(dispelTypeFromString, item);
				}
				break;
			}
			case AbnormalCategory.Immune:
			{
				ImmuneType immuneTypeFromString = AbnormalType.GetImmuneTypeFromString(item.name);
				if (builder5.ContainsKey(immuneTypeFromString))
				{
					Logger.RError("Duplicated ImmuneAbnormal " + item.key);
				}
				else
				{
					builder5.Add(immuneTypeFromString, item);
				}
				break;
			}
			}
		}
		DefAbnormalDict = builder.ToImmutable();
		CCAbnormalDict = builder2.ToImmutable();
		StatsAbnormalDict = builder3.ToImmutable();
		DispelAbnormalDict = builder4.ToImmutable();
		ImmuneAbnormalDict = builder5.ToImmutable();
	}

	public DefAbnormal_MasterData? GetDefAbnormal_MasterData(CCType ccType)
	{
		if (CCAbnormalDict.TryGetValue(ccType, out DefAbnormal_MasterData value))
		{
			return value;
		}
		return null;
	}

	public MapMasterInfo? GetMapInfo(int id)
	{
		if (RoomInfoDict.TryGetValue(id, out MapMasterInfo value))
		{
			return value;
		}
		return null;
	}

	public MonsterInfo? GetMonsterInfo(int id)
	{
		if (MonsterInfoDict.TryGetValue(id, out MonsterInfo value))
		{
			return value;
		}
		return null;
	}

	public PlayerMasterInfo? GetPlayerInfo(int id)
	{
		if (PlayerInfoDict.TryGetValue(id, out PlayerMasterInfo value))
		{
			return value;
		}
		return null;
	}

	public static MutableStatType toMutableStatType(StatType type)
	{
		return type switch
		{
			StatType.HP => MutableStatType.HP, 
			StatType.Stamina => MutableStatType.Stamina, 
			StatType.Conta => MutableStatType.Conta, 
			_ => MutableStatType.Invalid, 
		};
	}

	public StatType toImmutableStatType(MutableStatType type)
	{
		return type switch
		{
			MutableStatType.HP => StatType.HP, 
			MutableStatType.Stamina => StatType.Stamina, 
			MutableStatType.Conta => StatType.Conta, 
			_ => StatType.Invalid, 
		};
	}

	public SkillInfo? GetSkillInfo(int id)
	{
		if (SkillInfoDict.TryGetValue(id, out SkillInfo value))
		{
			return value;
		}
		return null;
	}

	public SkillSequenceInfo? GetSkillSequenceInfo(int id)
	{
		if (SkillSequenceInfoDict.TryGetValue(id, out SkillSequenceInfo value))
		{
			return value;
		}
		return null;
	}

	public Faction_MasterData? GetFaction(int id)
	{
		if (Factions.TryGetValue(id, out Faction_MasterData value))
		{
			return value;
		}
		return null;
	}

	public List<int> GetFactionCategory()
	{
		return FactionCategory.ToList();
	}

	public ItemMasterInfo? GetItemInfo(int id)
	{
		if (ItemInfoDict.TryGetValue(id, out ItemMasterInfo value))
		{
			return value;
		}
		return null;
	}

	public DungeonMasterInfo? GetDungeonInfo(int id)
	{
		if (DungeonInfoDict.TryGetValue(id, out DungeonMasterInfo value))
		{
			return value;
		}
		return null;
	}

	public ItemDropInfo? GetItemDropInfo(int id)
	{
		if (ItemDropDict.TryGetValue(id, out ItemDropInfo value))
		{
			return value;
		}
		return null;
	}

	public List<int> GetDungeonCandidateMasterID(int sessionCount)
	{
		List<int> list = new List<int>();
		foreach (DungeonMasterInfo value in DungeonInfoDict.Values)
		{
			if (value.IsActive && (value.MinSessionCount == 0 || value.MinSessionCount <= sessionCount) && (value.MaxSessionCount == 0 || value.MaxSessionCount >= sessionCount))
			{
				list.Add(value.ID);
			}
		}
		return list;
	}

	public List<(int masterid, int price)>? GetShopGroupPriceList(int shopGroupID)
	{
		List<(int, int)> list = new List<(int, int)>();
		if (!ShopGroupDict.TryGetValue(shopGroupID, out ShopGroup_MasterData value))
		{
			return null;
		}
		int num = SimpleRandUtil.Next(0, 10001);
		int num2 = 0;
		int item1_masterid = value.item1_masterid;
		int item1_price = value.item1_price;
		foreach (ShopGroup_item1_val item in value.ShopGroup_item1_valval)
		{
			num2 += item.item1_rate;
			if (num <= num2)
			{
				int item1_discount_rate = item.item1_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item1_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item2_masterid;
		item1_price = value.item2_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item2_val item2 in value.ShopGroup_item2_valval)
		{
			num2 += item2.item2_rate;
			if (num <= num2)
			{
				int item2_discount_rate = item2.item2_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item2_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item3_masterid;
		item1_price = value.item3_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item3_val item3 in value.ShopGroup_item3_valval)
		{
			num2 += item3.item3_rate;
			if (num <= num2)
			{
				int item3_discount_rate = item3.item3_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item3_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item4_masterid;
		item1_price = value.item4_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item4_val item4 in value.ShopGroup_item4_valval)
		{
			num2 += item4.item4_rate;
			if (num <= num2)
			{
				int item4_discount_rate = item4.item4_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item4_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item5_masterid;
		item1_price = value.item5_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item5_val item5 in value.ShopGroup_item5_valval)
		{
			num2 += item5.item5_rate;
			if (num <= num2)
			{
				int item5_discount_rate = item5.item5_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item5_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item6_masterid;
		item1_price = value.item6_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item6_val item6 in value.ShopGroup_item6_valval)
		{
			num2 += item6.item6_rate;
			if (num <= num2)
			{
				int item6_discount_rate = item6.item6_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item6_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item7_masterid;
		item1_price = value.item7_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item7_val item7 in value.ShopGroup_item7_valval)
		{
			num2 += item7.item7_rate;
			if (num <= num2)
			{
				int item7_discount_rate = item7.item7_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item7_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item8_masterid;
		item1_price = value.item8_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item8_val item8 in value.ShopGroup_item8_valval)
		{
			num2 += item8.item8_rate;
			if (num <= num2)
			{
				int item8_discount_rate = item8.item8_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item8_discount_rate) * 0.0001)));
				break;
			}
		}
		item1_masterid = value.item9_masterid;
		item1_price = value.item9_price;
		num2 = 0;
		num = SimpleRandUtil.Next(0, 10001);
		foreach (ShopGroup_item9_val item9 in value.ShopGroup_item9_valval)
		{
			num2 += item9.item9_rate;
			if (num <= num2)
			{
				int item9_discount_rate = item9.item9_discount_rate;
				list.Add((item1_masterid, item1_price - (int)((double)(item1_price * item9_discount_rate) * 0.0001)));
				break;
			}
		}
		return list;
	}

	public AbnormalInfo? GetAbnormalInfo(int masterID)
	{
		if (AbnormalDict.TryGetValue(masterID, out AbnormalInfo value))
		{
			return value;
		}
		return null;
	}

	public SkillTargetEffectDataInfo? GetSkillTargetEffectDataInfo(int masterID)
	{
		if (SkillTargetEffecDict.TryGetValue(masterID, out SkillTargetEffectDataInfo value))
		{
			return value;
		}
		return null;
	}

	public LocalizationData_MasterData? GetLocalizationData(string key)
	{
		if (LocalizationDict.TryGetValue(key, out LocalizationData_MasterData value))
		{
			return value;
		}
		return null;
	}

	public AttachMasterInfo? GetAttachInfo(int masterID)
	{
		if (AttachDataDict.TryGetValue(masterID, out AttachMasterInfo value))
		{
			return value;
		}
		return null;
	}

	public ProjectileInfo? GetProjectileInfo(int masterID)
	{
		if (ProjectileInfoDict.TryGetValue(masterID, out ProjectileInfo value))
		{
			return value;
		}
		return null;
	}

	public WeatherInfo? GetWeatherInfo(int masterID)
	{
		if (Weathers.TryGetValue(masterID, out WeatherInfo value))
		{
			return value;
		}
		return null;
	}

	public FieldSkillInfo? GetFieldSkillData(int masterID)
	{
		if (FieldSkillDict.TryGetValue(masterID, out FieldSkillInfo value))
		{
			return value;
		}
		return null;
	}

	public CycleMasterInfo? GetCycleMasterInfo(int sessionCount)
	{
		if (sessionCount > MaxSessionCount)
		{
			return (from x in CycleDict
				where x.Value.CycleCount == MaxSessionCount
				select x.Value).FirstOrDefault();
		}
		foreach (CycleMasterInfo value in CycleDict.Values)
		{
			if (value.CycleCount == sessionCount)
			{
				return value;
			}
		}
		return null;
	}

	public AuraInfo? GetAuraInfo(int masterID)
	{
		if (AuraDict.TryGetValue(masterID, out AuraInfo value))
		{
			return value;
		}
		return null;
	}

	public BattleActionInfo? GetBattleActionData(string key)
	{
		if (BattleActionDict.TryGetValue(key, out BattleActionInfo value))
		{
			return value;
		}
		return null;
	}

	public SpawnableItemInfo? GetSpawnableItemData(int masterID)
	{
		if (SpawnableItemDict.TryGetValue(masterID, out SpawnableItemInfo value))
		{
			return value;
		}
		return null;
	}

	public SpawnableMonsterInfo? GetSpawnableMonsterData(int masterID)
	{
		if (SpawnableMonsterDict.TryGetValue(masterID, out SpawnableMonsterInfo value))
		{
			return value;
		}
		return null;
	}

	public int GetRandomBoostItemMasterID()
	{
		ItemMasterInfo itemMasterInfo = (from x in ItemInfoDict.Values
			where x.IsBoostItemCandidate
			orderby Guid.NewGuid()
			select x).FirstOrDefault();
		if (itemMasterInfo != null)
		{
			return itemMasterInfo.MasterID;
		}
		Logger.RError("No Boost Item Candidate Found");
		return 0;
	}

	public TramupgradeData_MasterData? GetTramupgradeData(int masterID)
	{
		if (TramupgradeDataDict.TryGetValue(masterID, out TramupgradeData_MasterData value))
		{
			return value;
		}
		return null;
	}

	public List<int> GetTramUpgradeMasterIDs()
	{
		return TramupgradeDataDict.Keys.ToList();
	}
}
