using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public sealed class AbnormalController : IVActorController, IDisposable
{
	public struct AbnormalDebugInfo
	{
		public string Name;

		public int MasterID;

		public float RemainTime;

		public AbnormalDebugInfo(AbnormalObject abnormalObject)
		{
			MasterID = abnormalObject.AbnormalMasterInfo.MasterID;
			Name = abnormalObject.AbnormalMasterInfo.Name;
			RemainTime = abnormalObject.GetRemainTime();
		}
	}

	private long _abnormalSyncIDGenerator;

	private long _abnormalObjectIDGenerator;

	public VCreature Self;

	public List<IAbnormalManager> AbnormalManagers = new List<IAbnormalManager>();

	private Queue<AbnormalParamInfo> _deferedAbnormal = new Queue<AbnormalParamInfo>();

	private Dictionary<long, AbnormalObject> _abnormalObjectList = new Dictionary<long, AbnormalObject>();

	public List<long> AddAbnormalObjectIDs = new List<long>();

	public VActorControllerType type => VActorControllerType.Abnormal;

	public CrowdControlManager CrowdControlManager { get; private set; }

	public AbnormalStatsManager AbnormalStatsManager { get; private set; }

	public ImmuneManager ImmuneManager { get; private set; }

	public AbnormalController(VCreature self)
	{
		Self = self;
		CrowdControlManager = new CrowdControlManager(this);
		AbnormalStatsManager = new AbnormalStatsManager(this);
		ImmuneManager = new ImmuneManager(this);
		AbnormalManagers.Add(CrowdControlManager);
		AbnormalManagers.Add(AbnormalStatsManager);
		AbnormalManagers.Add(ImmuneManager);
	}

	public void OnDeath()
	{
		DispelAbnormalByDead();
		CrowdControlManager.DispelCC(CCType.None, all: true);
	}

	private void DispelAbnormalByDead()
	{
		foreach (AbnormalObject value in _abnormalObjectList.Values)
		{
			DispelAbnormalInternal(value, force: true);
		}
	}

	public bool AppendAbnormal(int casterObjectID, int abnormalMasterID, int duration = 0, bool ignoreImmuneCheck = false, int abnormalStack = 0, AbnormalReason reason = AbnormalReason.Skill)
	{
		if (Self.VRoom.FindActorByObjectID(casterObjectID) == null)
		{
			return false;
		}
		if (Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(abnormalMasterID) == null)
		{
			return false;
		}
		_deferedAbnormal.Enqueue(new AbnormalParamInfo(casterObjectID, abnormalMasterID, duration, reason));
		return true;
	}

	public bool DispelAbnormal(int casterObjectID, int abnormalMasterID, bool force)
	{
		if (Self.VRoom.FindActorByObjectID(casterObjectID) == null)
		{
			return false;
		}
		if (Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(abnormalMasterID) == null)
		{
			return false;
		}
		AbnormalObject abnormalObjectByMasterID = GetAbnormalObjectByMasterID(abnormalMasterID);
		if (abnormalObjectByMasterID == null)
		{
			return false;
		}
		DispelAbnormalInternal(abnormalObjectByMasterID, force);
		return true;
	}

	public bool DispelAbnormal(long abnormalObjectID)
	{
		if (!_abnormalObjectList.TryGetValue(abnormalObjectID, out var value))
		{
			return false;
		}
		DispelAbnormalInternal(value, force: true);
		return true;
	}

	private void ExtendAbnormalDuration(AbnormalObject abnormalObject, int duration)
	{
		AbnormalInfo abnormalMasterInfo = abnormalObject.AbnormalMasterInfo;
		int durationMsec = ((duration > 0) ? duration : abnormalMasterInfo.Duration);
		bool flag = false;
		foreach (KeyValuePair<long, AbnormalCategory> abnormalSyncID in abnormalObject.AbnormalSyncIDs)
		{
			IAbnormalManager abnormalManager = GetAbnormalManager(abnormalSyncID.Value);
			if (abnormalManager != null && abnormalManager.GetAbnormalElement(abnormalSyncID.Key, out IAbnormalElement abnormalElement) && abnormalElement != null)
			{
				_ = abnormalMasterInfo.ElementList[abnormalElement.Index];
				abnormalElement.ExtendDuration(durationMsec);
				flag = true;
			}
		}
		if (flag)
		{
			abnormalObject.ExtendDuration(durationMsec);
		}
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		switch (actionType)
		{
		case VActorActionType.Move:
		case VActorActionType.ScrapMotion:
		case VActorActionType.Jump:
		case VActorActionType.UseLevelObject:
			if (!IsMovable())
			{
				return MsgErrorCode.CantMove;
			}
			return MsgErrorCode.Success;
		case VActorActionType.Skill:
			if (!CanUseSkill(masterID))
			{
				return MsgErrorCode.CannotUsingSkill;
			}
			return MsgErrorCode.Success;
		case VActorActionType.ChangeInvenSlot:
		case VActorActionType.UseItem:
		case VActorActionType.Looting:
			if (CrowdControlManager.IsActionAbnormal())
			{
				return MsgErrorCode.CannotHandleItem;
			}
			return MsgErrorCode.Success;
		case VActorActionType.Emotion:
			if (!CrowdControlManager.IsInCC(ActorCCStatus.NORMAL, any: true))
			{
				return MsgErrorCode.Success;
			}
			return MsgErrorCode.CantEmotion;
		default:
			return MsgErrorCode.Success;
		}
	}

	public void Dispose()
	{
		DispelAbnormalByChannelMove();
	}

	public void Initialize()
	{
		InitializeAbnormal();
	}

	public void Update(long deltaTime)
	{
		if (_deferedAbnormal.Count > 0)
		{
			ApplyAbnormal();
		}
		bool flag = false;
		foreach (IAbnormalManager abnormalManager in AbnormalManagers)
		{
			abnormalManager.Update(deltaTime);
			if (abnormalManager.Changed())
			{
				flag = true;
			}
		}
		foreach (AbnormalObject value in _abnormalObjectList.Values)
		{
			value.Update();
			if (value.DeferToDelete)
			{
				flag = true;
			}
		}
		if (flag)
		{
			SyncChangedInfo();
		}
		foreach (IAbnormalManager abnormalManager2 in AbnormalManagers)
		{
			abnormalManager2.OnSyncComplete();
		}
		OnSyncComplete();
	}

	public void WaitInitDone()
	{
	}

	public long GetNewAbnormalSyncID()
	{
		return _abnormalSyncIDGenerator++;
	}

	public long GetNewAbnormalObjectID()
	{
		return _abnormalObjectIDGenerator++;
	}

	public void InitializeAbnormal()
	{
		_abnormalObjectIDGenerator = 1L;
		_abnormalSyncIDGenerator = 1L;
		_deferedAbnormal.Clear();
		_abnormalObjectList.Clear();
		CrowdControlManager.Clear();
		AbnormalStatsManager.Clear();
		ImmuneManager.Clear();
		CrowdControlManager.Initialize();
		AbnormalStatsManager.Initialize();
		ImmuneManager.Initialize();
	}

	public bool CalculatePushActionPos(Vector3 casterAbsolutePos, float casterAbsoluteAngle, BattleActionDistanceType distType, BattleActionInfo battleActionData, ref PosWithRot targetPos)
	{
		return CalculatePushActionPos(casterAbsolutePos, casterAbsoluteAngle, distType, battleActionData.Distance, battleActionData.TurnToAttacker, ref targetPos);
	}

	private bool CalculatePushActionPos(Vector3 casterAbsolutePos, float casterAbsoluteAngle, BattleActionDistanceType distanceType, float pushDistance, bool turnToCaster, ref PosWithRot targetPos)
	{
		if (CrowdControlManager.IsStopOnPos())
		{
			return false;
		}
		PosWithRot posWithRot = Self.Position.Clone();
		Vector3 endPos = default(Vector3);
		float num = ((distanceType == BattleActionDistanceType.AtAttacker) ? casterAbsoluteAngle : Misc.GetDirectionAngle(casterAbsolutePos, Self.PositionVector));
		switch (distanceType)
		{
		case BattleActionDistanceType.AtAttacker:
			endPos = Misc.GetPosWithAngleDistance(casterAbsolutePos, num, pushDistance);
			break;
		case BattleActionDistanceType.AtVictim:
			endPos = Misc.GetPosWithAngleDistance(Self.PositionVector, num, pushDistance);
			break;
		}
		if (!Self.VRoom.MoveAlongSurface(Self.PositionVector, endPos, out var resultPos, ignore: false) && !Self.VRoom.FindNearestPoly(Self.PositionVector, out resultPos))
		{
			resultPos = posWithRot.pos;
		}
		posWithRot.pos = resultPos;
		if (turnToCaster)
		{
			posWithRot.yaw = Misc.GetDirectionAngle(Self.PositionVector, casterAbsolutePos);
		}
		else
		{
			posWithRot.yaw = Self.Position.yaw;
		}
		posWithRot.CopyTo(targetPos);
		return true;
	}

	public IAbnormalManager? GetAbnormalManager(AbnormalCategory category)
	{
		return category switch
		{
			AbnormalCategory.CC => CrowdControlManager, 
			AbnormalCategory.Stats => AbnormalStatsManager, 
			AbnormalCategory.Immune => ImmuneManager, 
			_ => null, 
		};
	}

	private AbnormalObject? ApplyAbnormalInternal(int casterObjectID, long objectID, AbnormalInfo info, int duration, AbnormalReason reason)
	{
		AbnormalObject abnormalObject = new AbnormalObject(casterObjectID, objectID, info);
		bool flag = false;
		AbnormalObject abnormalObjectByMasterID = GetAbnormalObjectByMasterID(info.MasterID);
		if (abnormalObjectByMasterID != null)
		{
			if (info.Overlap)
			{
				ExtendAbnormalDuration(abnormalObjectByMasterID, (duration > 0) ? duration : info.Duration);
				return null;
			}
			return null;
		}
		foreach (KeyValuePair<int, IAbnormalElementInfo> element in info.ElementList)
		{
			long newAbnormalSyncID = GetNewAbnormalSyncID();
			int duration2 = ((duration > 0) ? duration : info.Duration);
			switch (element.Value.Category)
			{
			case AbnormalCategory.CC:
			{
				CCElement newElement2 = null;
				if (!ApplyCC(casterObjectID, newAbnormalSyncID, objectID, info, element.Key, ref newElement2, duration2))
				{
					flag = true;
				}
				else
				{
					abnormalObject.AddElement(newAbnormalSyncID, newElement2);
				}
				break;
			}
			case AbnormalCategory.Stats:
			{
				IAbnormalElement newElement3 = null;
				if (!ApplyStatsAbnormal(casterObjectID, newAbnormalSyncID, objectID, info, element.Key, ref newElement3, duration2))
				{
					flag = true;
				}
				else
				{
					abnormalObject.AddElement(newAbnormalSyncID, newElement3);
				}
				break;
			}
			case AbnormalCategory.Dispel:
				if (!DispelAbnormal(info, element.Key))
				{
					flag = true;
				}
				break;
			case AbnormalCategory.Immune:
			{
				ImmuneElement newElement = null;
				if (ApplyImmune(casterObjectID, newAbnormalSyncID, objectID, info, element.Key, ref newElement, duration2, reason) == AddImmuneResult.Fail)
				{
					flag = true;
				}
				else
				{
					abnormalObject.AddElement(newAbnormalSyncID, newElement);
				}
				break;
			}
			}
			if (flag)
			{
				break;
			}
		}
		if (flag)
		{
			DispelAbnormalInternal(abnormalObject, force: true);
			return null;
		}
		if (abnormalObject.AbnormalSyncIDs.Count > 0)
		{
			_abnormalObjectList.Add(objectID, abnormalObject);
			AddAbnormalObjectIDs.Add(objectID);
		}
		return abnormalObject;
	}

	private void DispelAbnormalInternal(AbnormalObject abnormalObject, bool force = false)
	{
		foreach (KeyValuePair<long, AbnormalCategory> abnormalSyncID in abnormalObject.AbnormalSyncIDs)
		{
			GetAbnormalManager(abnormalSyncID.Value)?.DispelAbnormal(abnormalSyncID.Key, force);
		}
		abnormalObject.Dispel();
	}

	private void ApplyAbnormal()
	{
		AbnormalParamInfo result;
		while (_deferedAbnormal.TryDequeue(out result))
		{
			AbnormalInfo abnormalInfo = Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(result.AbnormalMasterID);
			if (abnormalInfo != null && Self.IsAliveStatus() && !CheckAbnormalImmune(abnormalInfo) && ApplyAbnormalInternal(result.CasterObjectID, GetNewAbnormalObjectID(), abnormalInfo, result.Duration, result.Reason) != null)
			{
			}
		}
	}

	private bool ApplyCC(int casterObjectID, long newSyncID, long newObjectID, AbnormalInfo info, int index, ref CCElement? newElement, int duration)
	{
		if (!(info.ElementList[index] is CCElementInfo cCElementInfo))
		{
			return false;
		}
		if (cCElementInfo.CCType == CCType.None)
		{
			return false;
		}
		if (DefAbnormalUtil.IsActionAbnormal(cCElementInfo.CCType))
		{
			int priority = DefAbnormalUtil.GetPriority(cCElementInfo.CCType);
			var (num, flag) = CrowdControlManager.GetHighestPriority();
			if (priority < num || (priority == num && !flag))
			{
				return false;
			}
		}
		newElement = new CCElement(cCElementInfo.CCType, OnUpdateCrowdControl);
		if (DefAbnormalUtil.IsActionAbnormal(cCElementInfo.CCType))
		{
			BattleActionInfo battleActionInfo = cCElementInfo.BattleActionInfo;
			long duration2 = battleActionInfo.MoveTime + battleActionInfo.DownTime;
			if (!newElement.Initialize(newSyncID, newObjectID, casterObjectID, info, index, 0, duration2, Self.Position))
			{
				return false;
			}
			VActor vActor = Self.VRoom.FindActorByObjectID(casterObjectID);
			if (vActor == null)
			{
				return false;
			}
			PosWithRot targetPos = new PosWithRot();
			if (!CalculatePushActionPos(vActor.PositionVector, vActor.Position.yaw, cCElementInfo.DistanceType, battleActionInfo.Distance, battleActionInfo.TurnToAttacker, ref targetPos))
			{
				return false;
			}
			AbnormalCCInputArgs value = new AbnormalCCInputArgs
			{
				SyncID = newSyncID,
				CCType = cCElementInfo.CCType,
				CasterObjectID = casterObjectID,
				Duration = duration2,
				PushTime = battleActionInfo.MoveTime,
				DownTime = battleActionInfo.DownTime,
				TargetPos = targetPos,
				CurrentPos = Self.Position.Clone()
			};
			newElement.SetValue(value);
		}
		else if (!newElement.Initialize(newSyncID, newObjectID, casterObjectID, info, index, 0, info.Duration, Self.Position))
		{
			return false;
		}
		if (!CrowdControlManager.AddCC(newElement))
		{
			return false;
		}
		return true;
	}

	public bool ApplyStatsAbnormal(int casterObjectID, long newSyncID, long newObjectID, AbnormalInfo info, int index, ref IAbnormalElement? newElement, int duration)
	{
		if (!(info.ElementList[index] is AbnormalStatsElementInfo abnormalStatsElementInfo))
		{
			return false;
		}
		if (abnormalStatsElementInfo.StatsCategory == AbnormalStatsCategory.None)
		{
			return false;
		}
		if (abnormalStatsElementInfo.PeriodType == AbnormalApplyPeriodType.StaticStats)
		{
			newElement = new StaticStatsElement(abnormalStatsElementInfo.StatsCategory, Self.StatControlUnit.OnChangeAbnormalStats);
			if (!newElement.Initialize(newSyncID, newObjectID, casterObjectID, info, index, 0, duration, Self.Position))
			{
				return false;
			}
		}
		else if (abnormalStatsElementInfo.PeriodType == AbnormalApplyPeriodType.Dot)
		{
			newElement = new DotElement(abnormalStatsElementInfo.StatsCategory, Self.StatControlUnit.OnChangeAbnormalStats);
			if (!newElement.Initialize(newSyncID, newObjectID, casterObjectID, info, index, 0, duration, Self.Position))
			{
				return false;
			}
		}
		if (newElement == null)
		{
			return false;
		}
		if (!AbnormalStatsManager.AddStaticStatsAbnormal(newElement))
		{
			return false;
		}
		return true;
	}

	private bool DispelAbnormal(AbnormalInfo info, int index)
	{
		if (!(info.ElementList[index] is DispelElementInfo dispelElementInfo))
		{
			return false;
		}
		return DispelAbnormal(dispelElementInfo.DispelType, dispelElementInfo.ImmutableStatType, dispelElementInfo.MutableStatType, dispelElementInfo.CCType, dispelElementInfo.AbnormalMasterIDs);
	}

	public bool DispelAbnormal(DispelType type, StatType ImmutableStatType, MutableStatType mutableStatType, CCType cCType, IEnumerable<int> abnormalMasterIDs)
	{
		switch (type)
		{
		case DispelType.ALL:
			CrowdControlManager.DispelCC(CCType.NormalPush, all: true);
			AbnormalStatsManager.DispelAbnormal(type, StatType.Invalid, 0L);
			break;
		case DispelType.AllCC:
			CrowdControlManager.DispelCC(CCType.NormalPush, all: true);
			break;
		case DispelType.AllStats:
			AbnormalStatsManager.DispelAbnormal(type, StatType.Invalid, 0L);
			break;
		case DispelType.AbnormalID:
			if (abnormalMasterIDs.Count() == 0)
			{
				return false;
			}
			foreach (int abnormalMasterID in abnormalMasterIDs)
			{
				AbnormalObject abnormalObjectByMasterID = GetAbnormalObjectByMasterID(abnormalMasterID);
				if (abnormalObjectByMasterID != null)
				{
					DispelAbnormalInternal(abnormalObjectByMasterID);
				}
			}
			break;
		case DispelType.TargetCC:
			return CrowdControlManager.DispelCC(cCType);
		case DispelType.TargetImmutableStat:
			return AbnormalStatsManager.DispelAbnormal(type, ImmutableStatType, 0L);
		}
		return true;
	}

	public AddImmuneResult ApplyImmune(int casterObjectID, long newSyncID, long newObjectID, AbnormalInfo info, int index, ref ImmuneElement? newElement, int duration, AbnormalReason reason = AbnormalReason.Skill)
	{
		if (!(info.ElementList[index] is ImmuneElementInfo immuneElementInfo))
		{
			return AddImmuneResult.Fail;
		}
		newElement = new ImmuneElement(reason, immuneElementInfo.ImmuneType, duration);
		if (!newElement.Initialize(newSyncID, newObjectID, casterObjectID, info, index, 0, duration, Self.Position))
		{
			return AddImmuneResult.Fail;
		}
		return ImmuneManager.AddImmune(newElement);
	}

	public bool DispelCC(CCType ccType)
	{
		return CrowdControlManager.DispelCC(ccType);
	}

	public bool AddCC(AbnormalCCInputArgs args)
	{
		if (args.CCType == CCType.None)
		{
			return false;
		}
		if (DefAbnormalUtil.IsActionAbnormal(args.CCType))
		{
			int priority = DefAbnormalUtil.GetPriority(args.CCType);
			var (num, flag) = CrowdControlManager.GetHighestPriority();
			if (priority < num || (priority == num && !flag))
			{
				return false;
			}
		}
		CCElement cCElement = new CCElement(args.CCType, OnUpdateCrowdControl);
		if (!cCElement.SetValue(args))
		{
			return false;
		}
		CrowdControlManager.AddCC(cCElement);
		return true;
	}

	public bool AddStaticStatsAbnormal(AbnormalStatsCategory category, AbnormalStatsInputArgs args)
	{
		StaticStatsElement staticStatsElement = new StaticStatsElement(category, Self.StatControlUnit.OnChangeAbnormalStats);
		if (!staticStatsElement.SetValue(args))
		{
			return false;
		}
		AbnormalStatsManager.AddStaticStatsAbnormal(staticStatsElement);
		return true;
	}

	public bool AddDOT(AbnormalStatsCategory category, AbnormalDOTInputArgs args)
	{
		DotElement dotElement = new DotElement(category, Self.StatControlUnit.OnChangeAbnormalStats);
		if (!dotElement.SetValue(args))
		{
			return false;
		}
		AbnormalStatsManager.AddStaticStatsAbnormal(dotElement);
		return true;
	}

	public AddImmuneResult AddImmune(ImmuneType immuneType, ImmuneInputArgs args)
	{
		ImmuneElement immuneElement = new ImmuneElement(AbnormalReason.System, args.ImmuneType);
		if (!immuneElement.SetValue(args))
		{
			return AddImmuneResult.Fail;
		}
		return ImmuneManager.AddImmune(immuneElement);
	}

	private void OnUpdateCrowdControl(VActorEventArgs? args)
	{
		if (args is UpdateCrowdControlArgs updateCrowdControlArgs)
		{
			PosWithRot targetPosition = updateCrowdControlArgs.TargetPosition;
			PosWithRot registerPosition = updateCrowdControlArgs.RegisterPosition;
			long pushTime = updateCrowdControlArgs.PushTime;
			long elapsedTimeTotal = updateCrowdControlArgs.ElapsedTimeTotal;
			if (elapsedTimeTotal < pushTime)
			{
				Vector3 a = registerPosition.toVector3();
				Vector3 b = targetPosition.toVector3();
				Vector3 vector = Vector3.Lerp(a, b, Math.Clamp((float)elapsedTimeTotal / (float)pushTime, 0f, 1f));
				PosWithRot posWithRot = Self.Position.Clone();
				posWithRot.x = vector.x;
				posWithRot.y = vector.y;
				posWithRot.z = vector.z;
				Self.SetPosition(posWithRot, ActorMoveCause.Move);
				Self.MovementControlUnit?.OnChangePosition();
			}
			else if (!Misc.IsSamePos(Self.Position.toVector3(), targetPosition.toVector3()))
			{
				Self.SetPosition(targetPosition, ActorMoveCause.Move);
				Self.MovementControlUnit?.OnChangePosition();
			}
		}
	}

	public void DisposeElement(long abnormalObjectID, long syncID)
	{
		if (_abnormalObjectList.TryGetValue(abnormalObjectID, out var value))
		{
			value.RemoveElement(syncID);
		}
	}

	private void OnSyncComplete()
	{
		List<long> list = new List<long>();
		foreach (KeyValuePair<long, AbnormalObject> abnormalObject in _abnormalObjectList)
		{
			if (abnormalObject.Value.DeferToDelete)
			{
				list.Add(abnormalObject.Key);
			}
		}
		foreach (long item in list)
		{
			if (!_abnormalObjectList.TryGetValue(item, out var value))
			{
				continue;
			}
			if (value.AbnormalMasterInfo.ChainingAbnormalID != 0)
			{
				if (Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(value.AbnormalMasterInfo.ChainingAbnormalID) == null)
				{
					Logger.RWarn($"chain abnormal not found. abnormalMasterID: {value.AbnormalMasterInfo.MasterID}");
					continue;
				}
				AppendAbnormal(value.CasterObjectID, value.AbnormalMasterInfo.ChainingAbnormalID);
			}
			_abnormalObjectList.Remove(item);
		}
		AddAbnormalObjectIDs.Clear();
	}

	public void GetAbnormalIconInfo(ref List<AbnormalObjectInfo> list)
	{
		list.AddRange(from x in _abnormalObjectList.Values
			where !x.DeferToDelete
			select x.Serialize());
	}

	public void GetCCInfo(ref List<AbnormalCCInfo> list)
	{
		CrowdControlManager.GetAllInfo(ref list);
	}

	public void GetStatsInfo(ref List<AbnormalStatsInfo> list)
	{
		AbnormalStatsManager.GetAllInfo(ref list);
	}

	public void GetImmuneInfo(ref List<AbnormalImmuneInfo> list)
	{
		ImmuneManager.GetAllInfo(ref list);
	}

	private void SyncChangedInfo()
	{
		AbnormalSig sig = new AbnormalSig
		{
			actorID = Self.ObjectID
		};
		foreach (long addAbnormalObjectID in AddAbnormalObjectIDs)
		{
			if (_abnormalObjectList.TryGetValue(addAbnormalObjectID, out var value) && !value.DeferToDelete)
			{
				sig.abnormalIcons.Add(value.Serialize());
			}
		}
		foreach (AbnormalObject value2 in _abnormalObjectList.Values)
		{
			if (value2.DeferToDelete)
			{
				sig.abnormalIcons.Add(new AbnormalObjectInfo
				{
					abnormalMasterID = value2.AbnormalMasterInfo.MasterID,
					syncType = AbnormalDataSyncType.Remove,
					abnormalSyncID = value2.AbnormalSyncID
				});
			}
			else if (value2.Changed)
			{
				sig.abnormalIcons.Add(value2.Serialize(AbnormalDataSyncType.Change));
			}
		}
		foreach (IAbnormalManager abnormalManager in AbnormalManagers)
		{
			if (abnormalManager.Changed())
			{
				abnormalManager.CollectChangedInfo(ref sig);
			}
		}
		if (sig.abnormalIcons.Count > 0 || sig.ccList.Count > 0 || sig.statsList.Count > 0 || sig.immuneList.Count > 0)
		{
			Self.SendInSight(sig, includeSelf: true);
		}
	}

	public bool IsMovable()
	{
		return CrowdControlManager.IsMovable();
	}

	public bool IsActionPossible()
	{
		return CrowdControlManager.IsActionPossible();
	}

	public bool IsActionPossibleStatus(ActorCCStatus status)
	{
		return !CrowdControlManager.IsInCC(status);
	}

	public bool IsGetup()
	{
		return CrowdControlManager.IsGetup();
	}

	public bool IsStatsImmune(StatType type)
	{
		return ImmuneManager.IsStatsImmune(type);
	}

	public bool IsStatsImmune(MutableStatType type)
	{
		return ImmuneManager.IsStatsImmune(type);
	}

	public bool CanUseSkill(int skillMasterID)
	{
		if (skillMasterID == 0)
		{
			return CrowdControlManager.CanUseSkill();
		}
		if (Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID) == null)
		{
			return false;
		}
		return CrowdControlManager.CanUseSkill();
	}

	public bool IsAIPaused()
	{
		return CrowdControlManager.IsAIPause();
	}

	public bool IsDetach()
	{
		return CrowdControlManager.IsDetach();
	}

	public bool IsCantHoldItem()
	{
		return CrowdControlManager.IsCantHoldItem();
	}

	public bool IsCCImmune(CCType ccType)
	{
		return ImmuneManager.IsCCImmune(ccType);
	}

	public bool IsImmortal()
	{
		return ImmuneManager.IsImmortal();
	}

	private bool CheckAbnormalImmune(AbnormalInfo info)
	{
		if (ImmuneManager.IsAbnormalImmune(info.MasterID))
		{
			return true;
		}
		foreach (KeyValuePair<int, IAbnormalElementInfo> element in info.ElementList)
		{
			switch (element.Value.Category)
			{
			case AbnormalCategory.CC:
				if (!(element.Value is CCElementInfo cCElementInfo))
				{
					Logger.RError($"AbnormalController.CheckAbnormalImmune: abnormal element is not CCElementInfo. abnormalMasterID: {info.MasterID}");
				}
				else if (ImmuneManager.IsCCImmune(cCElementInfo.CCType))
				{
					return true;
				}
				break;
			case AbnormalCategory.Stats:
				if (!(element.Value is AbnormalStatsElementInfo abnormalStatsElementInfo))
				{
					Logger.RError($"AbnormalController.CheckAbnormalImmune: abnormal element is not StatsElementInfo. abnormalMasterID: {info.MasterID}");
				}
				else if (abnormalStatsElementInfo.StatsCategory == AbnormalStatsCategory.MutableStat)
				{
					if (ImmuneManager.IsStatsImmune(abnormalStatsElementInfo.MutableStatType))
					{
						return true;
					}
				}
				else if (ImmuneManager.IsStatsImmune(abnormalStatsElementInfo.ImmutableStatType))
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	private void DispelAbnormalByChannelMove()
	{
		foreach (AbnormalObject value in _abnormalObjectList.Values)
		{
			DispelAbnormalInternal(value, force: true);
		}
	}

	public int GetCCCasterObjectID(CCType ccType)
	{
		return CrowdControlManager.GetCCCasterObjectID(ccType);
	}

	public AbnormalObject? GetAbnormalObjectByMasterID(int abnormalMasterID)
	{
		return _abnormalObjectList.Values.FirstOrDefault((AbnormalObject x) => x.AbnormalMasterInfo.MasterID == abnormalMasterID);
	}

	public List<AbnormalDebugInfo> GetAbnormalDebugInfo()
	{
		return _abnormalObjectList.Values.Select((AbnormalObject x) => new AbnormalDebugInfo(x)).ToList();
	}

	public void PostUpdate(long deltaTime)
	{
	}

	public string GetDebugString()
	{
		return string.Empty;
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}
}
