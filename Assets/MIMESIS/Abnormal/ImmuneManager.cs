using System;
using System.Collections.Generic;
using Bifrost.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class ImmuneManager : IAbnormalManager
{
	private Dictionary<ImmuneGrade, HashSet<long>> _gradeImmuneDict = new Dictionary<ImmuneGrade, HashSet<long>>();

	private Dictionary<StatType, HashSet<long>> _statImmuneDict = new Dictionary<StatType, HashSet<long>>();

	private Dictionary<MutableStatType, HashSet<long>> _mutableStatImmuneDict = new Dictionary<MutableStatType, HashSet<long>>();

	private Dictionary<CCType, HashSet<long>> _ccImmuneDict = new Dictionary<CCType, HashSet<long>>();

	private Dictionary<int, HashSet<long>> _abnormalImmuneDict = new Dictionary<int, HashSet<long>>();

	public ImmuneManager(AbnormalController abnormalController)
		: base(abnormalController)
	{
	}

	public bool Initialize()
	{
		foreach (ImmuneGrade value in Enum.GetValues(typeof(ImmuneGrade)))
		{
			_gradeImmuneDict.Add(value, new HashSet<long>());
		}
		foreach (StatType value2 in Enum.GetValues(typeof(StatType)))
		{
			_statImmuneDict.Add(value2, new HashSet<long>());
		}
		foreach (MutableStatType value3 in Enum.GetValues(typeof(MutableStatType)))
		{
			_mutableStatImmuneDict.Add(value3, new HashSet<long>());
		}
		foreach (CCType value4 in Enum.GetValues(typeof(CCType)))
		{
			_ccImmuneDict.Add(value4, new HashSet<long>());
		}
		return true;
	}

	protected override void RefineInfo()
	{
	}

	protected override void DisposeOnList()
	{
		foreach (long trashBucket in _trashBuckets)
		{
			if (!GetAbnormalElement(trashBucket, out IAbnormalElement abnormalElement))
			{
				Logger.RWarn($"GetAbnormalElement failed. SyncID : {trashBucket}");
			}
			else if (!(abnormalElement is ImmuneElement immuneElement))
			{
				Logger.RWarn($"Element is not ImmuneElement. SyncID : {trashBucket}");
			}
			else if (immuneElement.Grade == ImmuneGrade.Invalid)
			{
				switch (immuneElement.ImmuneCategory)
				{
				case ImmuneCategory.CC:
					if (!_ccImmuneDict[immuneElement.CCType].Remove(trashBucket))
					{
						Logger.RWarn($"Remove CC Immune failed. SyncID : {trashBucket}");
					}
					break;
				case ImmuneCategory.ImmutableStats:
					if (!_statImmuneDict[immuneElement.ImmutableStatType].Remove(trashBucket))
					{
						Logger.RWarn($"Remove Negative Stat Immune failed. SyncID : {trashBucket}");
					}
					break;
				case ImmuneCategory.AbnormalID:
					foreach (int targetAbnormalMasterID in immuneElement.TargetAbnormalMasterIDs)
					{
						if (!_abnormalImmuneDict.TryGetValue(targetAbnormalMasterID, out var value) || value.Count == 0)
						{
							Logger.RWarn($"AbnormalTemplate Immune Dict is not exist. AbnormalTemplateMasterID : {targetAbnormalMasterID}");
							continue;
						}
						value.Remove(trashBucket);
						if (value.Count == 0)
						{
							_abnormalImmuneDict.Remove(targetAbnormalMasterID);
						}
					}
					break;
				case ImmuneCategory.MutableStats:
					if (!_mutableStatImmuneDict[immuneElement.MutableStatType].Remove(trashBucket))
					{
						Logger.RWarn($"Remove Negative Mutable Stat Immune failed. SyncID : {trashBucket}");
					}
					break;
				default:
					Logger.RWarn($"Invalid ImmuneCategory. SyncID : {trashBucket}");
					break;
				}
			}
			else if (_gradeImmuneDict[immuneElement.Grade].Count == 0 || !_gradeImmuneDict[immuneElement.Grade].Contains(trashBucket))
			{
				Logger.RWarn($"Remove Grade Immune failed. SyncID : {trashBucket}");
			}
			else
			{
				_gradeImmuneDict[immuneElement.Grade].Remove(trashBucket);
			}
		}
	}

	public AddImmuneResult AddImmune(IAbnormalElement abnormalElement)
	{
		if (!(abnormalElement is ImmuneElement immuneElement))
		{
			return AddImmuneResult.Fail;
		}
		switch (AddAbnormal(immuneElement))
		{
		case AbnormalHandleResult.Failed:
			return AddImmuneResult.Fail;
		case AbnormalHandleResult.Defered:
			return AddImmuneResult.Defered;
		default:
		{
			AddImmuneResult addImmuneResult = ((immuneElement.Grade != ImmuneGrade.Invalid) ? ApplyGradeImmune(immuneElement) : (immuneElement.ImmuneCategory switch
			{
				ImmuneCategory.CC => ApplyCCImmune(immuneElement), 
				ImmuneCategory.ImmutableStats => ApplyStatImmune(immuneElement), 
				ImmuneCategory.AbnormalID => ApplyAbnormalImmune(immuneElement), 
				ImmuneCategory.MutableStats => ApplyMutableStatImmune(immuneElement), 
				_ => AddImmuneResult.Fail, 
			}));
			if (addImmuneResult != AddImmuneResult.Success)
			{
				return addImmuneResult;
			}
			ApplyDispelByImmune(immuneElement);
			return AddImmuneResult.Success;
		}
		}
	}

	private AddImmuneResult ApplyGradeImmune(ImmuneElement element)
	{
		if (!_gradeImmuneDict[element.Grade].Add(element.FixedValue.SyncID))
		{
			return AddImmuneResult.AlreadyExist;
		}
		AddAbnormal(element);
		return AddImmuneResult.Success;
	}

	private AddImmuneResult ApplyCCImmune(ImmuneElement element)
	{
		if (!_ccImmuneDict[element.CCType].Add(element.FixedValue.SyncID))
		{
			return AddImmuneResult.AlreadyExist;
		}
		AddAbnormal(element);
		return AddImmuneResult.Success;
	}

	private AddImmuneResult ApplyStatImmune(ImmuneElement element)
	{
		if (!_statImmuneDict[element.ImmutableStatType].Add(element.FixedValue.SyncID))
		{
			return AddImmuneResult.AlreadyExist;
		}
		AddAbnormal(element);
		return AddImmuneResult.Success;
	}

	private AddImmuneResult ApplyMutableStatImmune(ImmuneElement element)
	{
		if (!_mutableStatImmuneDict[element.MutableStatType].Add(element.FixedValue.SyncID))
		{
			return AddImmuneResult.AlreadyExist;
		}
		AddAbnormal(element);
		return AddImmuneResult.Success;
	}

	private AddImmuneResult ApplyAbnormalImmune(ImmuneElement element)
	{
		foreach (int targetAbnormalMasterID in element.TargetAbnormalMasterIDs)
		{
			if (!_abnormalImmuneDict.TryGetValue(targetAbnormalMasterID, out var value))
			{
				value = new HashSet<long>();
				_abnormalImmuneDict.Add(targetAbnormalMasterID, value);
			}
			if (!value.Add(element.FixedValue.SyncID))
			{
				return AddImmuneResult.AlreadyExist;
			}
		}
		AddAbnormal(element);
		return AddImmuneResult.Success;
	}

	public override void Clear()
	{
		base.Clear();
		_abnormalImmuneDict.Clear();
		_gradeImmuneDict.Clear();
		_ccImmuneDict.Clear();
		_statImmuneDict.Clear();
		_mutableStatImmuneDict.Clear();
	}

	public bool IsCCImmune(CCType ccType)
	{
		if (_gradeImmuneDict[ImmuneGrade.AllCC].Count <= 0 && _ccImmuneDict[ccType].Count <= 0)
		{
			return _gradeImmuneDict[ImmuneGrade.Immortal].Count > 0;
		}
		return true;
	}

	public bool IsStatsImmune(StatType type)
	{
		if (_gradeImmuneDict[ImmuneGrade.Immortal].Count > 0 || _gradeImmuneDict[ImmuneGrade.AllStats].Count > 0)
		{
			return true;
		}
		return _statImmuneDict[type].Count > 0;
	}

	public bool IsStatsImmune(MutableStatType type)
	{
		if (_gradeImmuneDict[ImmuneGrade.Immortal].Count > 0 || _gradeImmuneDict[ImmuneGrade.AllStats].Count > 0)
		{
			return true;
		}
		return _mutableStatImmuneDict[type].Count > 0;
	}

	public bool IsImmortal()
	{
		return _gradeImmuneDict[ImmuneGrade.Immortal].Count > 0;
	}

	public bool IsAbnormalImmune(int abnormalMasterID)
	{
		return _abnormalImmuneDict.ContainsKey(abnormalMasterID);
	}

	protected override bool CollectInfo(long syncID, AbnormalDataSyncType syncType, ref AbnormalSig sig)
	{
		if (!GetAbnormalElement(syncID, out IAbnormalElement abnormalElement))
		{
			return false;
		}
		if (!(abnormalElement is ImmuneElement immuneElement))
		{
			return false;
		}
		sig.immuneList.Add(new AbnormalImmuneInfo
		{
			changeType = syncType,
			abnormalMasterID = immuneElement.AbnormalMasterID,
			abnormalSyncID = immuneElement.FixedValue.AbnormalObjectID,
			syncID = syncID,
			immuneType = immuneElement.Type,
			remainTime = immuneElement.GetRemainTime(),
			duration = immuneElement.Duration
		});
		return true;
	}

	public void GetAllInfo(ref List<AbnormalImmuneInfo> info)
	{
		foreach (IAbnormalElement value in _abnormals.Values)
		{
			if (!value.DeferToDelete && value is ImmuneElement immuneElement)
			{
				info.Add(new AbnormalImmuneInfo
				{
					changeType = AbnormalDataSyncType.Add,
					abnormalMasterID = immuneElement.AbnormalMasterID,
					syncID = immuneElement.FixedValue.SyncID,
					immuneType = immuneElement.Type,
					remainTime = immuneElement.GetRemainTime(),
					duration = immuneElement.Duration
				});
			}
		}
	}

	private void ApplyDispelByImmune(ImmuneElement element)
	{
		DispelType dispelType = DefAbnormalUtil.ChangeImmuneType2DispelType(element.Type);
		if (dispelType != DispelType.None)
		{
			_abnormalController.DispelAbnormal(dispelType, element.ImmutableStatType, element.MutableStatType, element.CCType, element.TargetAbnormalMasterIDs);
		}
	}
}
