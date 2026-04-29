using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;
using UnityEngine;

public class AuraContext
{
	public readonly bool DefaultFlag;

	public readonly long StartTime = Hub.s.timeutil.GetCurrentTickMilliSec();

	public readonly AuraInfo AuraInfo;

	private VCreature _parent;

	private List<int> _appliedActorIDs = new List<int>();

	private IMutableHitCheck _hitCheck;

	private HitCheckPos _hitCheckPos => new HitCheckPos
	{
		Start = _parent.PositionVector,
		End = _parent.PositionVector,
		AngleRad = _parent.Position.yaw.toRadian()
	};

	public AuraContext(VCreature creature, AuraInfo auraInfo, bool defaultFlag)
	{
		AuraInfo = auraInfo;
		_parent = creature;
		DefaultFlag = defaultFlag;
		_hitCheck = AuraInfo.HitCheck.Clone();
		_hitCheck.ApplyOffset(AuraInfo.AuraOffset);
	}

	public void Update()
	{
		List<VActor> actorsInRange = _parent.VRoom.GetActorsInRange(_parent, _hitCheck.CheckRadius, includeSelf: true);
		List<int> list = new List<int>();
		foreach (VActor item in actorsInRange)
		{
			if (item is VCreature vCreature && _parent.IsValidTarget(AuraInfo.HitTargetTypes, vCreature) && vCreature.IsAliveStatus())
			{
				HitCheckPos posTarget = new HitCheckPos
				{
					Start = vCreature.PositionVector,
					End = vCreature.PositionVector,
					AngleRad = 0f
				};
				if (_hitCheck.IsHit(_hitCheckPos, vCreature.HitCheck, posTarget).isHit)
				{
					list.Add(vCreature.ObjectID);
				}
			}
		}
		List<int> list2 = _appliedActorIDs.Except(list).ToList();
		List<int> list3 = list.Except(_appliedActorIDs).ToList();
		foreach (int item2 in list3)
		{
			AddAura(item2);
		}
		foreach (int item3 in list2)
		{
			RemoveAura(item3);
		}
		foreach (int item4 in list2)
		{
			_appliedActorIDs.Remove(item4);
		}
		foreach (int item5 in list3)
		{
			_appliedActorIDs.Add(item5);
		}
	}

	private void AddAura(int actorID)
	{
		VActor vActor = _parent.VRoom.FindActorByObjectID(actorID);
		if (vActor is VCreature)
		{
			vActor.AuraControlUnit?.ApplyAura(_parent.ObjectID, AuraInfo.MasterID);
		}
	}

	private void RemoveAura(int actorID)
	{
		if (_parent.VRoom.FindActorByObjectID(actorID) is VCreature vCreature)
		{
			vCreature.AuraControlUnit?.EscapeFromAura(AuraInfo.MasterID, _parent.ObjectID);
		}
	}

	public void Dispose()
	{
		foreach (int appliedActorID in _appliedActorIDs)
		{
			if (_parent.VRoom.FindActorByObjectID(appliedActorID) is VCreature vCreature)
			{
				vCreature.AuraControlUnit?.EscapeFromAura(AuraInfo.MasterID, _parent.ObjectID);
			}
		}
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
		if (AuraInfo == null)
		{
			return;
		}
		switch (_hitCheck.ShapeType)
		{
		case HitCheckShapeType.Cube:
			if (_hitCheck is CubeHitCheck cubeHitCheck)
			{
				sig.hitCheckDrawInfos.Add(new CubeHitCheckDrawInfo
				{
					actorID = _parent.ObjectID,
					center = _parent.PositionVector + _hitCheck.Center,
					rotation = new Rotator(_hitCheck.Rotation.ToQuaternion() * Quaternion.Euler(0f, _parent.Position.yaw, 0f)),
					shapeType = HitCheckShapeType.Cube,
					Extent = cubeHitCheck.Extent
				});
			}
			break;
		case HitCheckShapeType.Capsule:
			if (_hitCheck is CapsuleHitCheck capsuleHitCheck)
			{
				sig.hitCheckDrawInfos.Add(new CapsuleHitCheckDrawInfo
				{
					actorID = _parent.ObjectID,
					center = _parent.PositionVector + _hitCheck.Center,
					rotation = new Rotator(_hitCheck.Rotation.ToQuaternion() * Quaternion.Euler(0f, _parent.Position.yaw, 0f)),
					shapeType = HitCheckShapeType.Capsule,
					Radius = capsuleHitCheck.Rad,
					Length = capsuleHitCheck.Length
				});
			}
			break;
		}
	}
}
