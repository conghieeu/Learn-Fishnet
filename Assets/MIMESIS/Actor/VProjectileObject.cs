using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;
using UnityEngine.AI;

public class VProjectileObject : VActor
{
	private readonly ProjectileInfo _projectileInfo;

	private ISkillContext? _parentSkillContext;

	private long _startTick;

	private bool _active;

	private long _currentTick;

	private long _lastTick;

	private int _selectedHitFieldSkillMasterID;

	private VCreature? _parentActor;

	private float _speed;

	private Vector3 _gravity;

	private Vector3 _initialVector;

	private Vector3 _gVelocity = Vector3.zero;

	private IHitCheck _hitCheck;

	private ItemElement? _attachedElement;

	private float _radius => _projectileInfo.Radius;

	private ProjectileType _projectileType => _projectileInfo.ProjectileType;

	public VProjectileObject(int actorID, int masterID, string actorName, PosWithRot position, bool isIndoor, IVroom room, ISkillContext? skillContext, ReasonOfSpawn reasonOfSpawn)
		: base(ActorType.Projectile, actorID, masterID, actorName, position, isIndoor, room, 0L, reasonOfSpawn)
	{
		ProjectileInfo projectileInfo = Hub.s.dataman.ExcelDataManager.GetProjectileInfo(masterID);
		if (projectileInfo == null)
		{
			throw new Exception("VProjectileObject : _projectileInfo is null");
		}
		_speed = projectileInfo.PhysicsSpeed;
		_gravity = Vector3.up * projectileInfo.Gravity;
		_projectileInfo = projectileInfo;
		_parentSkillContext = skillContext;
		_startTick = Hub.s.timeutil.GetCurrentTickMilliSec();
		_active = true;
		_currentTick = _startTick;
		_lastTick = _startTick;
		_initialVector = Quaternion.Euler(position.pitch, position.yaw, position.roll) * Vector3.forward * _speed;
		_hitCheck = new SphereMutableHitCheck(Vector3.zero, _radius);
		_parentActor = _parentSkillContext?.ContextInfo.Creature ?? null;
	}

	private (bool isHit, Vector3 hitPos, Vector3 normal) CheckWallHit(Vector3 startPos, Vector3 endPos)
	{
		Vector3 vector = new Rotator(base.Position.rot).ToQuaternion() * Vector3.forward;
		_ = base.PositionVector + vector * (_radius * 1.5f);
		(bool, Vector3, Vector3) wallHitPos = VWorldUtil.GetWallHitPos(startPos, endPos);
		if (wallHitPos.Item1)
		{
			return (isHit: true, hitPos: wallHitPos.Item2, normal: wallHitPos.Item3);
		}
		return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
	}

	private void HandleRaycastProj()
	{
		(bool isHit, Vector3 hitPos, Vector3 normal) tuple = CheckWallHit(base.PositionVector, Vector3.zero);
		bool item = tuple.isHit;
		Vector3 vector = tuple.hitPos;
		Vector3 surfaceNormal = tuple.normal;
		float maxRange = _radius;
		if (item)
		{
			maxRange = Vector3.Distance(base.PositionVector, vector);
		}
		List<VCreature> list = (from c in VRoom.GetCreaturesInRange(base.Position.pos, 0f, maxRange, checkHeight: false, _parentActor)
			orderby Vector3.Distance(c.PositionVector, base.PositionVector)
			select c).ToList();
		int num = _projectileInfo.PenetrateCount;
		List<int> list2 = new List<int>();
		foreach (VCreature item2 in list)
		{
			if (item2 == null || !(item2.HitCheck is CapsuleHitCheck capsule))
			{
				continue;
			}
			(item, vector, surfaceNormal) = CalcCollision.IsSegmentIntersectCapsule(base.PositionVector, _initialVector, capsule);
			if (item)
			{
				num--;
				list2.Add(item2.ObjectID);
				if (num < 0)
				{
					break;
				}
			}
		}
		if (list2.Count > 0)
		{
			OnHit(list2, vector, surfaceNormal, DamageCauseType.Collision, VWorldCombatUtil.DefaultDamageImmuneChecker, VWorldCombatUtil.DefaultSkillPush(this));
		}
		_active = false;
		VRoom.PendRemoveActor(base.ObjectID);
	}

	private void HandlePhysicsProj()
	{
		bool flag = false;
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		long num = currentTickMilliSec - _lastTick;
		_lastTick = currentTickMilliSec;
		var (posWithRot, basePositionFuture) = GetNextPos((float)num / 1000f);
		SendInSight(new MoveStartSig
		{
			actorID = base.ObjectID,
			targetID = 0,
			actorMoveType = ActorMoveType.Projectile,
			basePositionPrev = base.Position.Clone(),
			basePositionCurr = posWithRot,
			basePositionFuture = basePositionFuture,
			futureTime = num
		});
		Vector3 positionVector = base.PositionVector;
		SetPosition(posWithRot, ActorMoveCause.Move);
		bool flag2 = false;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		List<int> list = new List<int>();
		List<VCreature> creaturesInRange = VRoom.GetCreaturesInRange(base.Position.pos, 0f, 2.5f, checkHeight: false, _parentActor, collectDeadActor: true);
		if (creaturesInRange.Count > 0)
		{
			foreach (VCreature item in creaturesInRange)
			{
				if (item == null)
				{
					continue;
				}
				VCreature vCreature = item;
				HitCheckPos posOrigin = new HitCheckPos
				{
					Start = base.PositionVector,
					End = base.PositionVector,
					AngleRad = 0f
				};
				HitCheckPos posTarget = new HitCheckPos
				{
					Start = item.PositionVector,
					End = item.PositionVector,
					AngleRad = 0f
				};
				(bool, Vector3, Vector3) tuple2 = _hitCheck.IsHit(posOrigin, vCreature.HitCheck, posTarget);
				if (tuple2.Item1)
				{
					if (!flag2)
					{
						flag2 = true;
						vector = tuple2.Item2;
						vector2 = tuple2.Item3;
					}
					list.Add(item.ObjectID);
					flag = true;
				}
			}
		}
		if (!flag2)
		{
			(flag2, vector, vector2) = CheckWallHit(positionVector, posWithRot.pos);
		}
		if (!flag2)
		{
			if (!NavMesh.SamplePosition(base.PositionVector, out var _, 10f, -1))
			{
				_active = false;
				VRoom.PendRemoveActor(base.ObjectID);
			}
			return;
		}
		_active = false;
		VRoom.PendRemoveActor(base.ObjectID);
		if (_projectileInfo.HitFieldSkillMasterIDCandidates.Count > 0)
		{
			_selectedHitFieldSkillMasterID = _projectileInfo.HitFieldSkillMasterIDCandidates[SimpleRandUtil.Next(0, _projectileInfo.HitFieldSkillMasterIDCandidates.Count)];
		}
		PosWithRot posWithRot2 = new PosWithRot
		{
			pos = vector
		};
		if (!VRoom.FindNearestPoly(vector, out var nearestPos, 5f))
		{
			Logger.RWarn($"VProjectileObject FindNearestPoly fail. pos:{vector}");
		}
		else
		{
			posWithRot2.pos = nearestPos;
		}
		if (vector != Vector3.zero)
		{
			OnHit(list, vector, vector2, DamageCauseType.Collision, VWorldCombatUtil.DefaultDamageImmuneChecker, VWorldCombatUtil.DefaultSkillPush(this));
		}
		if (_attachedElement != null)
		{
			ItemElement attachedElement = _attachedElement;
			_attachedElement = null;
			if (attachedElement is EquipmentItemElement equipmentItemElement)
			{
				int num2 = equipmentItemElement.RemainDurability - _projectileInfo.DecreaseDurabilityOnCollision;
				if (num2 > 0)
				{
					equipmentItemElement.SetDurability(num2);
					VRoom.SpawnLootingObject(attachedElement, posWithRot2, base.IsIndoor, ReasonOfSpawn.Skill);
				}
			}
		}
		if (_selectedHitFieldSkillMasterID > 0 && VRoom.SpawnFieldSkill(_selectedHitFieldSkillMasterID, flag ? posWithRot2 : vector.toPosWithRot(0f), _isIndoor, vector2, null, _parentSkillContext, ReasonOfSpawn.Linked) == 0)
		{
			Logger.RError($"VProjectileObject SpawnFieldSkill fail. ProjectileID:{_projectileInfo.MasterID}, HitFieldSkillMasterID:{_selectedHitFieldSkillMasterID}");
		}
	}

	public override void Update(long deltaTick)
	{
		base.Update(deltaTick);
		if (!_active)
		{
			return;
		}
		long physicsLifetimeMillisec = _projectileInfo.PhysicsLifetimeMillisec;
		if (physicsLifetimeMillisec > 0 && _startTick + physicsLifetimeMillisec < Hub.s.timeutil.GetCurrentTickMilliSec() && _active)
		{
			_active = false;
			VRoom.PendRemoveActor(base.ObjectID);
			return;
		}
		_currentTick += deltaTick;
		if (_projectileType == ProjectileType.Physics)
		{
			HandlePhysicsProj();
		}
		else if (_projectileType == ProjectileType.Raycast)
		{
			HandleRaycastProj();
		}
	}

	private (PosWithRot nextPos, PosWithRot futurePos) GetNextPos(float deltaTime)
	{
		_gVelocity += _gravity * deltaTime;
		Vector3 vector = (_initialVector + _gVelocity) * deltaTime;
		Vector3 vector2 = base.PositionVector + vector;
		Vector3 vector3 = _gVelocity + _gravity * deltaTime;
		Vector3 vector4 = (_initialVector + vector3) * deltaTime;
		Vector3 vector5 = vector2 + vector4;
		Vector3 rot = (vector5 - vector2).normalized.toRotatorVector();
		rot.y = base.Position.yaw;
		rot.z = 0f;
		PosWithRot item = new PosWithRot
		{
			pos = vector2,
			rot = rot
		};
		PosWithRot item2 = new PosWithRot
		{
			pos = vector5,
			rot = rot
		};
		return (nextPos: item, futurePos: item2);
	}

	public bool OnHit(List<int> hitTargetActorIDs, Vector3 hitPos, Vector3 surfaceNormal, DamageCauseType causeType, IsDamageImmuned isDamageImmuned, SkillPushDelegate skillPush)
	{
		List<TargetHitInfo> list = new List<TargetHitInfo>();
		if (_projectileInfo.MutableStatType == MutableStatType.HP)
		{
			list = OnDamageImpl(hitTargetActorIDs, _projectileInfo.MutableValue * -1, causeType, isDamageImmuned, skillPush);
		}
		else if (_projectileInfo.MutableStatType == MutableStatType.Conta)
		{
			foreach (int hitTargetActorID in hitTargetActorIDs)
			{
				VActor vActor = VRoom.FindActorByObjectID(hitTargetActorID);
				if (vActor != null && vActor is VCreature && IsValidTarget(_projectileInfo.HitTargetTypes, vActor) && vActor.IsAliveStatus())
				{
					vActor.StatControlUnit?.IncreaseConta(_projectileInfo.MutableValue);
					list.Add(new TargetHitInfo
					{
						targetID = hitTargetActorID,
						basePosition = vActor.Position
					});
				}
			}
		}
		ProjectileHitTargetSig projectileHitTargetSig = new ProjectileHitTargetSig
		{
			projectileActorID = base.ObjectID,
			projectileMasterID = MasterID,
			hitPos = hitPos,
			surfaceNormal = surfaceNormal
		};
		if (list.Count > 0)
		{
			projectileHitTargetSig.targetHitInfos.AddRange(list);
		}
		SendInSight(projectileHitTargetSig);
		return true;
	}

	public List<TargetHitInfo> OnDamageImpl(List<int> targetActorIDs, long damage, DamageCauseType causeType, IsDamageImmuned isDamageImmuned, SkillPushDelegate skillPush)
	{
		Hub.s.timeutil.GetCurrentTickMilliSec();
		List<TargetHitInfo> list = new List<TargetHitInfo>();
		List<VActor> list2 = new List<VActor>();
		foreach (int targetActorID in targetActorIDs)
		{
			VActor vActor = VRoom.FindActorByObjectID(targetActorID);
			if (vActor != null && vActor is VCreature && IsValidTarget(_projectileInfo.HitTargetTypes, vActor) && vActor.IsAliveStatus())
			{
				list2.Add(vActor);
			}
		}
		_ = list2.Count;
		long num = 0L;
		foreach (VActor item in list2)
		{
			int objectID = item.ObjectID;
			bool flag = false;
			TargetHitInfo hitInfo = new TargetHitInfo
			{
				targetID = objectID,
				basePosition = item.Position
			};
			ImmuneCheckResult immuneCheckResult = isDamageImmuned(item);
			flag = immuneCheckResult.Immuned;
			if (flag)
			{
				hitInfo.immuneType = immuneCheckResult.ImmuneType;
				hitInfo.actionAbnormalHitType = CCType.None;
			}
			else
			{
				DamageAttribute damageAttribute = null;
				damageAttribute = (VRoom.DamageAppliable() ? new DamageAttribute(damage, 0L, critical: false) : new DamageAttribute(0L, 0L, critical: false));
				CCType actionAbnormalHitType = CCType.None;
				if (!flag && _projectileInfo.BattleActionKey != "NO_ACTION")
				{
					BattleActionInfo battleActionData = Hub.s.dataman.ExcelDataManager.GetBattleActionData(_projectileInfo.BattleActionKey);
					if (battleActionData != null)
					{
						actionAbnormalHitType = battleActionData.CCType;
						hitInfo.hitDelay = battleActionData.MoveTime + battleActionData.DownTime;
						hitInfo.pushTime = battleActionData.MoveTime;
					}
				}
				hitInfo.actionAbnormalHitType = actionAbnormalHitType;
				skillPush(hitInfo, item, _projectileInfo.BattleActionKey, BattleActionDistanceType.AtVictim);
				if (damageAttribute.Damage > 0)
				{
					item.StatControlUnit?.ApplyDamage(new ApplyDamageArgs(_parentActor, item, MutableStatChangeCause.ActiveAttack, damageAttribute.Damage, damageAttribute.GroggyValue, _parentSkillContext?.SkillMasterID ?? 0));
					num += hitInfo.damage;
				}
				damageAttribute.Convert2HitInfo(ref hitInfo);
				list.Add(hitInfo);
			}
			if (_projectileInfo.AbnormalMasterIDs.Count() <= 0 || flag)
			{
				continue;
			}
			if (_projectileInfo.AbnormalApplyType == AbnormalApplyType.All)
			{
				ImmutableArray<int>.Enumerator enumerator3 = _projectileInfo.AbnormalMasterIDs.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					int current3 = enumerator3.Current;
					if (current3 > 0 && Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(current3) != null)
					{
						item.AbnormalControlUnit?.AppendAbnormal(base.ObjectID, current3);
					}
				}
			}
			else if (_projectileInfo.AbnormalApplyType == AbnormalApplyType.SelectiveRandom)
			{
				int num2 = _projectileInfo.AbnormalMasterIDs[SimpleRandUtil.Next(0, _projectileInfo.AbnormalMasterIDs.Count())];
				if (num2 > 0 && Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(num2) != null)
				{
					item.AbnormalControlUnit?.AppendAbnormal(base.ObjectID, num2);
				}
			}
		}
		return list;
	}

	public override void CollectDebugInfo(ref DebugInfoSig sig)
	{
		DebugInfoSig obj = sig;
		obj.debugInfo = obj.debugInfo + "ProjectileInfo : " + _projectileInfo.ToString();
		CollectHitCheckInfo(_hitCheck, ref sig);
	}

	protected void GetInfo(ref ProjectileObjectInfo info)
	{
		ActorBaseInfo info2 = info;
		GetActorBaseInfo(ref info2);
		info.parentActorID = ((_parentActor != null) ? _parentActor.ObjectID : 0);
		info.projectileMasterID = _projectileInfo.MasterID;
		info.endTime = _startTick + (long)(_projectileInfo.PhysicsLifetime * 1000f);
	}

	public override void FillSightInSig(ref SightInSig sig)
	{
		ProjectileObjectInfo info = new ProjectileObjectInfo();
		GetInfo(ref info);
		sig.projectileObjectInfos.Add(info);
	}

	public override bool IsAliveStatus()
	{
		if (_projectileInfo == null)
		{
			return false;
		}
		return true;
	}

	public override SendResult SendToMe(IMsg msg)
	{
		return SendResult.Success;
	}

	public void AttachItemElement(ItemElement element)
	{
		_attachedElement = element;
	}
}
