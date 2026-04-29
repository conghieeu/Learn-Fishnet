using System;
using System.Collections.Generic;
using Bifrost.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class AbnormalStatsManager : IAbnormalManager
{
	public struct AbnormalStatsKey : IEquatable<AbnormalStatsKey>
	{
		public AbnormalStatsCategory Category;

		public override bool Equals(object? obj)
		{
			if (obj is AbnormalStatsKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(AbnormalStatsKey other)
		{
			return Category == other.Category;
		}

		public override int GetHashCode()
		{
			return Category.GetHashCode();
		}
	}

	public Dictionary<AbnormalStatsKey, HashSet<long>> StatsElementDict { get; private set; } = new Dictionary<AbnormalStatsKey, HashSet<long>>();

	public AbnormalStatsManager(AbnormalController abnormalController)
		: base(abnormalController)
	{
	}

	public bool Initialize()
	{
		foreach (AbnormalStatsCategory value in Enum.GetValues(typeof(AbnormalStatsCategory)))
		{
			if (value != AbnormalStatsCategory.None)
			{
				AbnormalStatsKey key = new AbnormalStatsKey
				{
					Category = value
				};
				StatsElementDict.Add(key, new HashSet<long>());
			}
		}
		return true;
	}

	protected override void DisposeOnList()
	{
		foreach (long trashBucket in _trashBuckets)
		{
			if (GetAbnormalElement(trashBucket, out IAbnormalElement abnormalElement))
			{
				AbnormalStatsKey key = default(AbnormalStatsKey);
				if (abnormalElement is StaticStatsElement staticStatsElement)
				{
					key.Category = staticStatsElement.StaticStatsFixedValue.Category;
				}
				else if (abnormalElement is DotElement dotElement)
				{
					key.Category = dotElement.DotFixedValue.Category;
				}
				if (StatsElementDict.TryGetValue(key, out var value) && value.Contains(trashBucket))
				{
					value.Remove(trashBucket);
				}
			}
		}
	}

	public bool AddStaticStatsAbnormal(IAbnormalElement element)
	{
		switch (AddAbnormal(element))
		{
		case AbnormalHandleResult.Failed:
			return false;
		case AbnormalHandleResult.Defered:
			return true;
		default:
		{
			AbnormalStatsKey key = default(AbnormalStatsKey);
			if (element is StaticStatsElement staticStatsElement)
			{
				key.Category = staticStatsElement.StaticStatsFixedValue.Category;
			}
			else if (element is DotElement dotElement)
			{
				key.Category = dotElement.DotFixedValue.Category;
			}
			if (!StatsElementDict.TryGetValue(key, out var value))
			{
				Logger.RWarn($"Invalid abnormal element. syncID: {element.FixedValue.SyncID}");
				return false;
			}
			if (value.Contains(element.FixedValue.SyncID))
			{
				Logger.RWarn($"Already exist abnormal element. syncID: {element.FixedValue.SyncID}");
				return false;
			}
			value.Add(element.FixedValue.SyncID);
			return true;
		}
		}
	}

	public bool DispelAbnormal(DispelType type, StatType statType = StatType.Invalid, long syncID = 0L)
	{
		switch (type)
		{
		case DispelType.ALL:
			DispelAll();
			break;
		case DispelType.AllStats:
			return DispelImmutableAll();
		case DispelType.TargetImmutableStat:
			return DispelImmutableStatsAbnormal(statType);
		case DispelType.AbnormalID:
			return DispelAbnormal(syncID);
		}
		return true;
	}

	public bool DispelImmutableAll()
	{
		AbnormalStatsKey key = new AbnormalStatsKey
		{
			Category = AbnormalStatsCategory.ImmutableStat
		};
		if (!DispelElementInList(key))
		{
			return false;
		}
		return true;
	}

	public bool DispelElementInList(AbnormalStatsKey key)
	{
		if (!StatsElementDict.TryGetValue(key, out var value))
		{
			return false;
		}
		List<long> list = new List<long>();
		foreach (long item in value)
		{
			if (!GetAbnormalElement(item, out IAbnormalElement abnormalElement) || abnormalElement == null)
			{
				list.Add(item);
			}
			else if (abnormalElement.FixedValue.Dispelable)
			{
				abnormalElement.SetDelete();
			}
		}
		if (list.Count > 0)
		{
			foreach (long item2 in list)
			{
				value.Remove(item2);
			}
		}
		return true;
	}

	public bool DispelImmutableStatsAbnormal(StatType type)
	{
		AbnormalStatsKey key = new AbnormalStatsKey
		{
			Category = AbnormalStatsCategory.ImmutableStat
		};
		if (!StatsElementDict.TryGetValue(key, out var value))
		{
			return false;
		}
		foreach (long item in value)
		{
			if (!GetAbnormalElement(item, out IAbnormalElement abnormalElement) || abnormalElement == null || !abnormalElement.FixedValue.Dispelable)
			{
				continue;
			}
			if (abnormalElement is StaticStatsElement staticStatsElement)
			{
				if (staticStatsElement.StaticStatsFixedValue.ImmutableStatType == type)
				{
					abnormalElement.SetDelete();
				}
			}
			else if (abnormalElement is DotElement dotElement && dotElement.DotFixedValue.ImmutableStatType == type)
			{
				abnormalElement.SetDelete();
			}
		}
		return true;
	}

	public bool DispelMutableStatsAbnormal(MutableStatType type)
	{
		AbnormalStatsKey key = new AbnormalStatsKey
		{
			Category = AbnormalStatsCategory.MutableStat
		};
		if (!StatsElementDict.TryGetValue(key, out var value))
		{
			return false;
		}
		foreach (long item in value)
		{
			if (!GetAbnormalElement(item, out IAbnormalElement abnormalElement) || abnormalElement == null || !abnormalElement.FixedValue.Dispelable)
			{
				continue;
			}
			if (abnormalElement is StaticStatsElement staticStatsElement)
			{
				if (staticStatsElement.StaticStatsFixedValue.MutableStatType == type)
				{
					abnormalElement.SetDelete();
				}
			}
			else if (abnormalElement is DotElement dotElement && dotElement.DotFixedValue.MutableStatType == type)
			{
				abnormalElement.SetDelete();
			}
		}
		return true;
	}

	protected override void RefineInfo()
	{
	}

	public override void Clear()
	{
		base.Clear();
		StatsElementDict.Clear();
	}

	protected override bool CollectInfo(long syncID, AbnormalDataSyncType syncType, ref AbnormalSig sig)
	{
		if (!GetAbnormalElement(syncID, out IAbnormalElement abnormalElement) || abnormalElement == null)
		{
			return false;
		}
		if (abnormalElement.AbnormalMasterID == 0)
		{
			return false;
		}
		sig.statsList.Add(new AbnormalStatsInfo
		{
			changeType = syncType,
			abnormalMasterID = abnormalElement.AbnormalMasterID,
			abnormalSyncID = abnormalElement.FixedValue.AbnormalObjectID,
			syncID = syncID,
			index = abnormalElement.Index,
			remainTime = abnormalElement.GetRemainTime(),
			duration = abnormalElement.Duration
		});
		return true;
	}

	public void GetAllInfo(ref List<AbnormalStatsInfo> list)
	{
		foreach (IAbnormalElement value in _abnormals.Values)
		{
			if (!value.DeferToDelete && value.AbnormalMasterID != 0)
			{
				list.Add(new AbnormalStatsInfo
				{
					changeType = AbnormalDataSyncType.Add,
					abnormalMasterID = value.AbnormalMasterID,
					syncID = value.FixedValue.SyncID,
					index = value.Index,
					remainTime = value.GetRemainTime(),
					duration = value.Duration
				});
			}
		}
	}
}
