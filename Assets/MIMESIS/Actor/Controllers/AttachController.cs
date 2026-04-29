using System;
using System.Collections.Generic;
using System.Linq;
using ReluProtocol.Enum;
using UnityEngine;

public class AttachController : IVActorController, IDisposable
{
	private VCreature _creature;

	private AttachMasterInfo? _attachInfo;

	private Dictionary<int, (int targetActorID, long attachTime)?> _attachingActorIDs = new Dictionary<int, (int, long)?>();

	private int _attachingActorIDByRequest;

	private (int, int) _attachedActor;

	private List<long> _abnormalImmuneSyncIDs = new List<long>();

	public VActorControllerType type => VActorControllerType.Attach;

	public int AttachedActorID => _attachedActor.Item2;

	public int AttachingActorID
	{
		get
		{
			Dictionary<int, (int targetActorID, long attachTime)?> attachingActorIDs = _attachingActorIDs;
			if (attachingActorIDs != null && attachingActorIDs.Count > 0)
			{
				return _attachingActorIDs.First().Value.Value.targetActorID;
			}
			return 0;
		}
	}

	public AttachController(VCreature creture)
	{
		_creature = creture;
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		switch (actionType)
		{
		case VActorActionType.MoveSkill:
		case VActorActionType.Skill:
		case VActorActionType.ChangeInvenSlot:
		case VActorActionType.UseItem:
		case VActorActionType.Move:
		case VActorActionType.Looting:
		case VActorActionType.UseLevelObject:
			if (IsAttached())
			{
				return MsgErrorCode.CantAction;
			}
			return MsgErrorCode.Success;
		case VActorActionType.Emotion:
		case VActorActionType.ScrapMotion:
		case VActorActionType.Jump:
			if (IsAttached() || IsAttaching())
			{
				return MsgErrorCode.CantAction;
			}
			return MsgErrorCode.Success;
		default:
			return MsgErrorCode.Success;
		}
	}

	public void Dispose()
	{
		if (IsAttaching())
		{
			DetachByRequest(_creature.ObjectID, 0, DetachReason.ByDispose);
		}
		else if (IsAttached())
		{
			DetachByRequest(_creature.ObjectID, 0, DetachReason.ByDispose);
		}
	}

	private void InitCandidateAttachSlot(int grabMasterID, bool attached = false)
	{
		if (_attachInfo != null)
		{
			return;
		}
		_attachInfo = Hub.s.dataman.ExcelDataManager.GetAttachInfo(grabMasterID);
		if (_attachInfo != null)
		{
			_attachingActorIDs.Clear();
			for (int i = 1; i <= _attachInfo.SocketCount; i++)
			{
				_attachingActorIDs.Add(i, null);
			}
		}
	}

	private int AllocateAttachSlot(int targetActorID)
	{
		if (_attachInfo == null)
		{
			return 0;
		}
		foreach (int key in _attachingActorIDs.Keys)
		{
			if (!_attachingActorIDs[key].HasValue)
			{
				_attachingActorIDs[key] = (targetActorID, Hub.s.timeutil.GetCurrentTickMilliSec());
				return key;
			}
		}
		return 0;
	}

	private void AttachedBySlot(int slotIndex, int attacherActorID)
	{
		if (_attachedActor.Item1 != 0 || _attachedActor.Item2 != 0)
		{
			Logger.RWarn("AttachedBySlot failed : already attached");
		}
		else
		{
			_attachedActor = (slotIndex, attacherActorID);
		}
	}

	public bool AttachBySkill(int grabMasterID, int attachedActorID)
	{
		if (!CanAttach(grabMasterID, attachedActorID))
		{
			return false;
		}
		InitCandidateAttachSlot(grabMasterID);
		VActor vActor = _creature.VRoom.FindActorByObjectID(attachedActorID);
		if (vActor == null)
		{
			return false;
		}
		int num = AllocateAttachSlot(attachedActorID);
		if (num == 0)
		{
			return false;
		}
		if (!vActor.AttachControlUnit.AttachedBySkill(grabMasterID, num, _creature.ObjectID))
		{
			return false;
		}
		if (_attachInfo.CasterStartTimeAbnormalID != 0)
		{
			_creature.AbnormalControlUnit.AppendAbnormal(_creature.ObjectID, _attachInfo.CasterStartTimeAbnormalID);
		}
		if (_creature.AbnormalControlUnit != null)
		{
			foreach (ImmuneElementInfo casterImmune in _attachInfo.CasterImmunes)
			{
				long newAbnormalSyncID = _creature.AbnormalControlUnit.GetNewAbnormalSyncID();
				_abnormalImmuneSyncIDs.Add(newAbnormalSyncID);
				_creature.AbnormalControlUnit.AddImmune(casterImmune.ImmuneType, new ImmuneInputArgs
				{
					SyncID = newAbnormalSyncID,
					Duration = Hub.s.timeutil.ChangeTimeSec2Milli(_attachInfo.Duration),
					CasterObjectID = _creature.ObjectID,
					CCType = casterImmune.CCType,
					Dispelable = false,
					ImmuneType = casterImmune.ImmuneType,
					ImmutableStatType = casterImmune.ImmutableStatType,
					MutableStatType = casterImmune.MutableStatType
				});
			}
		}
		_creature.VRoom.IterateAllMonster(delegate(VActor monster)
		{
			monster.AIControlUnit?.OnAttaching(_attachInfo.IsReverseGrab ? attachedActorID : _creature.ObjectID, _attachInfo.IsReverseGrab ? _creature.ObjectID : attachedActorID);
		});
		if (_creature is VPlayer)
		{
			_creature.EmotionControlUnit.OnAttach();
			_creature.ScrapMotionController.OnAttach();
		}
		_creature.SendInSight(new AttachActorSig
		{
			actorID = _creature.ObjectID,
			targetID = attachedActorID,
			state = AttachState.Attached,
			socketIndex = num,
			grabMasterID = grabMasterID
		}, includeSelf: true);
		OnMove();
		return true;
	}

	public bool AttachByRequest(int targetActorID)
	{
		return false;
	}

	public bool DetachByRequest(int requestActorID, int targetActorID, DetachReason reason)
	{
		if (_attachedActor.Item1 != 0)
		{
			return false;
		}
		List<int> list = new List<int>();
		switch (reason)
		{
		case DetachReason.ActiveByCaster:
		case DetachReason.ForceCheat:
			if (requestActorID != _creature.ObjectID)
			{
				return false;
			}
			if (targetActorID > 0)
			{
				if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue && x.Value.Value.targetActorID == targetActorID).Count() == 0)
				{
					return false;
				}
				list.Add(targetActorID);
			}
			else if (targetActorID == 0)
			{
				list.AddRange(from x in _attachingActorIDs
					where x.Value.HasValue
					select x.Value.Value.targetActorID);
			}
			break;
		case DetachReason.ActiveByVictim:
			if (requestActorID != targetActorID)
			{
				return false;
			}
			if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue && x.Value.Value.targetActorID == targetActorID).Count() == 0)
			{
				return false;
			}
			list.Add(targetActorID);
			break;
		case DetachReason.AttacherDead:
		case DetachReason.AbnormalOnCaster:
			if (requestActorID != _creature.ObjectID)
			{
				return false;
			}
			list.AddRange(from x in _attachingActorIDs
				where x.Value.HasValue
				select x.Value.Value.targetActorID);
			break;
		case DetachReason.AttachedDead:
		case DetachReason.AbnormalOnVictim:
			if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue && x.Value.Value.targetActorID == targetActorID).Count() == 0)
			{
				return false;
			}
			if (requestActorID != targetActorID)
			{
				return false;
			}
			list.Add(targetActorID);
			break;
		case DetachReason.TimeOver:
			if (requestActorID != _creature.ObjectID)
			{
				return false;
			}
			if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue && x.Value.Value.targetActorID == targetActorID).Count() == 0)
			{
				return false;
			}
			list.Add(targetActorID);
			break;
		case DetachReason.ByDispose:
			if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue && x.Value.Value.targetActorID == targetActorID).Count() == 0)
			{
				return false;
			}
			if (requestActorID == _creature.ObjectID)
			{
				list.AddRange(from x in _attachingActorIDs
					where x.Value.HasValue
					select x.Value.Value.targetActorID);
				break;
			}
			if (requestActorID == targetActorID)
			{
				if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue && x.Value.Value.targetActorID == targetActorID).Count() == 0)
				{
					return false;
				}
				list.Add(targetActorID);
				break;
			}
			return false;
		default:
			return false;
		}
		if (list.Count > 0)
		{
			switch (reason)
			{
			case DetachReason.TimeOver:
				if (_attachInfo.CasterEndTimeAbnormalID != 0)
				{
					_creature.AbnormalControlUnit.AppendAbnormal(_creature.ObjectID, _attachInfo.CasterEndTimeAbnormalID);
				}
				break;
			case DetachReason.ActiveByCaster:
			case DetachReason.ActiveByVictim:
			case DetachReason.AttacherDead:
			case DetachReason.AttachedDead:
			case DetachReason.AbnormalOnCaster:
			case DetachReason.AbnormalOnVictim:
			case DetachReason.ForceCheat:
				if (_attachInfo.CasterForceDetachAbnormalID != 0)
				{
					_creature.AbnormalControlUnit.AppendAbnormal(_creature.ObjectID, _attachInfo.CasterForceDetachAbnormalID);
				}
				break;
			}
		}
		foreach (int actorID in list)
		{
			VActor vActor = _creature.VRoom.FindActorByObjectID(actorID);
			if (vActor == null)
			{
				continue;
			}
			int num = 0;
			foreach (KeyValuePair<int, (int, long)?> attachingActorID in _attachingActorIDs)
			{
				if (attachingActorID.Value.HasValue && attachingActorID.Value.Value.Item1 == actorID)
				{
					num = attachingActorID.Key;
					break;
				}
			}
			if (num == 0)
			{
				Logger.RWarn("DetachByRequest failed at targetActorID : " + actorID);
				continue;
			}
			if (!vActor.AttachControlUnit.DetachedByRequest(_creature, num, reason))
			{
				Logger.RWarn("DetachByRequest failed at targetActorID : " + actorID);
				continue;
			}
			_attachingActorIDs[num] = null;
			_creature.SendInSight(new AttachActorSig
			{
				actorID = _creature.ObjectID,
				targetID = actorID,
				state = AttachState.Detached,
				socketIndex = num,
				grabMasterID = _attachInfo.MasterID
			}, includeSelf: true);
			_creature.VRoom.IterateAllMonster(delegate(VActor monster)
			{
				monster.AIControlUnit?.OnDetaching(_attachInfo.IsReverseGrab ? _creature.ObjectID : actorID);
			});
		}
		if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue).Count() == 0)
		{
			_attachInfo = null;
			foreach (long abnormalImmuneSyncID in _abnormalImmuneSyncIDs)
			{
				_creature.AbnormalControlUnit?.ImmuneManager.DispelAbnormal(abnormalImmuneSyncID, force: true);
			}
		}
		return true;
	}

	private bool AttachedBySkill(int grabMasterID, int slotIndex, int attachingActorID)
	{
		if (IsAttached() || IsAttaching())
		{
			return false;
		}
		_attachInfo = Hub.s.dataman.ExcelDataManager.GetAttachInfo(grabMasterID);
		if (_attachInfo == null)
		{
			return false;
		}
		_attachedActor = (slotIndex, attachingActorID);
		OnAttached();
		return true;
	}

	public bool AttachedByRequest(int targetActorID)
	{
		return false;
	}

	private bool DetachedByRequest(VCreature? caster, int slotIndex, DetachReason reason)
	{
		if (_attachInfo == null)
		{
			return false;
		}
		if (IsAttaching())
		{
			Logger.RWarn("DetachedByRequest is called while attaching");
			return false;
		}
		if (!IsAttached(slotIndex))
		{
			return false;
		}
		OnDetached(reason);
		VCreature vCreature = ((caster == null) ? _creature : caster);
		Vector3? dropPosition = Misc.GetDropPosition(vCreature.PositionVector, vCreature.Angle, !_attachInfo.FrontDrop, _attachInfo.DropDistance * 0.01f);
		if (!dropPosition.HasValue)
		{
			_creature.Teleport(vCreature.Position, TeleportReason.Detach);
		}
		else
		{
			_creature.Teleport(dropPosition.Value.toPosWithRot(Misc.GetDirectionAngle(vCreature.Position.yaw, 180f)), TeleportReason.Detach);
		}
		_attachedActor = (0, 0);
		_attachInfo = null;
		return true;
	}

	public bool IsAttaching()
	{
		if (_attachInfo != null)
		{
			return _attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue).Count() > 0;
		}
		return false;
	}

	public bool IsAttached(int slotIndex = 0)
	{
		if (_attachInfo == null)
		{
			return false;
		}
		if (_attachedActor.Item2 == 0)
		{
			return false;
		}
		if (slotIndex == 0 && _attachedActor.Item1 != 0)
		{
			return true;
		}
		if (slotIndex == _attachedActor.Item1)
		{
			return true;
		}
		return false;
	}

	public bool CanAttach(int grabMasterID, int targetActorID)
	{
		if (_attachInfo != null && _attachInfo.MasterID != grabMasterID)
		{
			return false;
		}
		VActor vActor = _creature.VRoom.FindActorByObjectID(targetActorID);
		if (vActor == null)
		{
			return false;
		}
		if (vActor.AttachControlUnit.IsAttached())
		{
			return false;
		}
		return _attachingActorIDs.Values.Where(((int targetActorID, long attachTime)? x) => x.HasValue && x.Value.targetActorID == targetActorID).Count() == 0;
	}

	public void Initialize()
	{
	}

	public void Update(long delta)
	{
		if (_attachInfo == null)
		{
			return;
		}
		DetachReason reason = DetachReason.Invalid;
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		List<(int, DetachReason)> list = new List<(int, DetachReason)>();
		if (_creature.AbnormalControlUnit.IsDetach())
		{
			if (_attachedActor.Item2 != 0)
			{
				reason = DetachReason.AbnormalOnVictim;
				VActor vActor = _creature.VRoom.FindActorByObjectID(_attachedActor.Item2);
				if (vActor == null)
				{
					return;
				}
				vActor.AttachControlUnit.DetachByRequest(_creature.ObjectID, _creature.ObjectID, reason);
			}
			else if (_attachingActorIDs.Where((KeyValuePair<int, (int targetActorID, long attachTime)?> x) => x.Value.HasValue).Count() > 0)
			{
				reason = DetachReason.AbnormalOnCaster;
				list.AddRange(from x in _attachingActorIDs
					where x.Value.HasValue
					select (x.Value.Value.targetActorID, reason: reason));
			}
		}
		else
		{
			reason = DetachReason.TimeOver;
			foreach (KeyValuePair<int, (int, long)?> attachingActorID in _attachingActorIDs)
			{
				if (attachingActorID.Value.HasValue && currentTickMilliSec - attachingActorID.Value.Value.Item2 > _attachInfo.Duration)
				{
					list.Add((attachingActorID.Value.Value.Item1, reason));
				}
			}
		}
		foreach (var (targetActorID, reason2) in list)
		{
			DetachByRequest(_creature.ObjectID, targetActorID, reason2);
		}
	}

	private void OnAttached()
	{
		_creature.SkillControlUnit.CancelSkill(SkillCancelType.All, 0L);
		_creature.MovementControlUnit.StopMove(sync: true, needToCancel: true);
		_creature.EmotionControlUnit.OnAttach();
		_creature.ScrapMotionController.OnAttach();
		if (_attachInfo.VictimStartTimeAbnormalID != 0)
		{
			_creature.AbnormalControlUnit.AppendAbnormal(_attachedActor.Item2, _attachInfo.VictimStartTimeAbnormalID);
		}
		if (_creature.AbnormalControlUnit == null)
		{
			return;
		}
		foreach (ImmuneElementInfo victimImmune in _attachInfo.VictimImmunes)
		{
			long newAbnormalSyncID = _creature.AbnormalControlUnit.GetNewAbnormalSyncID();
			_abnormalImmuneSyncIDs.Add(newAbnormalSyncID);
			_creature.AbnormalControlUnit.AddImmune(victimImmune.ImmuneType, new ImmuneInputArgs
			{
				SyncID = newAbnormalSyncID,
				Duration = Hub.s.timeutil.ChangeTimeSec2Milli(_attachInfo.Duration),
				CasterObjectID = _creature.ObjectID,
				CCType = victimImmune.CCType,
				Dispelable = false,
				ImmuneType = victimImmune.ImmuneType,
				ImmutableStatType = victimImmune.ImmutableStatType,
				MutableStatType = victimImmune.MutableStatType
			});
		}
	}

	private void OnDetached(DetachReason reason)
	{
		if ((reason == DetachReason.ActiveByCaster || reason == DetachReason.ActiveByVictim || reason == DetachReason.AbnormalOnCaster || reason == DetachReason.AbnormalOnVictim || reason == DetachReason.AttacherDead || reason == DetachReason.AttachedDead || reason == DetachReason.ForceCheat) && _attachInfo.VictimForceDetachAbnormalID != 0)
		{
			_creature.AbnormalControlUnit.AppendAbnormal(_creature.ObjectID, _attachInfo.VictimForceDetachAbnormalID);
		}
		else if (reason == DetachReason.TimeOver && _attachInfo.VictimEndTimeAbnormalID != 0)
		{
			_creature.AbnormalControlUnit.AppendAbnormal(_attachedActor.Item2, _attachInfo.VictimEndTimeAbnormalID);
		}
		foreach (long abnormalImmuneSyncID in _abnormalImmuneSyncIDs)
		{
			_creature.AbnormalControlUnit?.ImmuneManager.DispelAbnormal(abnormalImmuneSyncID, force: true);
		}
		if (Hub.s.CheckPosIsIndoor(_creature.PositionVector))
		{
			_creature.SetIsIndoor(isIndoor: true);
		}
		else
		{
			_creature.SetIsIndoor(isIndoor: false);
		}
	}

	public void OnDead()
	{
		if (IsAttaching())
		{
			DetachByRequest(_creature.ObjectID, 0, DetachReason.AttacherDead);
		}
		else if (IsAttached())
		{
			Logger.RWarn("OnDead : attached");
			_creature.VRoom.FindActorByObjectID(_attachedActor.Item2)?.AttachControlUnit.DetachByRequest(_creature.ObjectID, _creature.ObjectID, DetachReason.AttachedDead);
		}
	}

	public void OnMove()
	{
		if (!IsAttaching())
		{
			return;
		}
		foreach (KeyValuePair<int, (int, long)?> attachingActorID in _attachingActorIDs)
		{
			if (attachingActorID.Value.HasValue)
			{
				_creature.VRoom.FindActorByObjectID(attachingActorID.Value.Value.Item1)?.MovementControlUnit?.SyncMoveForce(_creature.Position, ActorMoveType.Attached);
			}
		}
	}

	public void WaitInitDone()
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
