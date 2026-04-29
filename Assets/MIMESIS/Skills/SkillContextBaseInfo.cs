using System;
using System.Collections.Generic;
using Bifrost.Cooked;
using ReluProtocol;
using UnityEngine;

public class SkillContextBaseInfo : IDisposable
{
	public readonly VCreature Creature;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public SkillFlags Flags { get; }

	public SkillInfo SkillInfo { get; }

	public long SkillSyncID { get; }

	public int TargetObjectID { get; }

	public List<int> MultiTargetObjectIDs { get; } = new List<int>();

	public List<Vector3> TargetPositions { get; } = new List<Vector3>();

	public PosWithRot SkillBasedStartPosition { get; } = new PosWithRot();

	public PosWithRot SkillBasedEndPosition { get; } = new PosWithRot();

	public float ProjectileFinalSpeed { get; }

	public float ProjectileLifeTime { get; }

	public SkillAnimInfo? AnimInfo { get; }

	public int HitNotifyCount { get; }

	public float SkillTime { get; }

	public Dictionary<int, SkillSequenceInfo> SequenceInfos { get; } = new Dictionary<int, SkillSequenceInfo>();

	public Dictionary<int, ProjectileInfo> ProjectileInfos { get; } = new Dictionary<int, ProjectileInfo>();

	public Dictionary<int, FieldSkillInfo> FieldSkillInfos { get; } = new Dictionary<int, FieldSkillInfo>();

	public SkillContextBaseInfo(VCreature actor, SkillInfo skillInfo, long skillSyncID, int targetObjectID, List<Vector3>? targetPositions, PosWithRot endPosition, SkillFlags flags = SkillFlags.None, List<int>? multiTargetIDs = null, SkillAnimInfo? animInfo = null)
	{
		Creature = actor;
		Flags = flags;
		SkillInfo = skillInfo;
		SkillSyncID = skillSyncID;
		TargetObjectID = targetObjectID;
		AnimInfo = animInfo;
		SkillBasedStartPosition = Creature.Position.Clone();
		SkillBasedEndPosition = endPosition.Clone();
		if (multiTargetIDs != null)
		{
			MultiTargetObjectIDs.AddRange(multiTargetIDs);
		}
		if (targetPositions != null)
		{
			foreach (Vector3 targetPosition in targetPositions)
			{
				TargetPositions.Add(targetPosition);
			}
		}
		foreach (KeyValuePair<int, int> item in skillInfo.SkillHitboxSequenceDictionary)
		{
			SkillSequenceInfo skillSequenceInfo = Hub.s.dataman.ExcelDataManager.GetSkillSequenceInfo(item.Value);
			if (skillSequenceInfo != null)
			{
				SequenceInfos.Add(item.Key, skillSequenceInfo);
			}
		}
		foreach (KeyValuePair<int, int> item2 in skillInfo.SkillProjectileEventDictionary)
		{
			ProjectileInfo projectileInfo = Hub.s.dataman.ExcelDataManager.GetProjectileInfo(item2.Value);
			if (projectileInfo != null)
			{
				ProjectileInfos.Add(item2.Key, projectileInfo);
			}
		}
		foreach (KeyValuePair<int, int> item3 in skillInfo.SkillFieldSkillEventDictionary)
		{
			FieldSkillInfo fieldSkillData = Hub.s.dataman.ExcelDataManager.GetFieldSkillData(item3.Value);
			if (fieldSkillData != null)
			{
				FieldSkillInfos.Add(item3.Key, fieldSkillData);
			}
		}
		if (AnimInfo != null)
		{
			SkillTime = (float)AnimInfo.Length;
			HitNotifyCount = AnimInfo.HitChecks.Length;
		}
	}

	public SkillContextBaseInfo Clone()
	{
		return new SkillContextBaseInfo(this);
	}

	public SkillContextBaseInfo(SkillContextBaseInfo other)
	{
		Creature = other.Creature;
		Flags = other.Flags;
		SkillInfo = other.SkillInfo;
		SkillSyncID = other.SkillSyncID;
		TargetObjectID = other.TargetObjectID;
		MultiTargetObjectIDs.Clear();
		MultiTargetObjectIDs.AddRange(other.MultiTargetObjectIDs);
		TargetPositions.Clear();
		foreach (Vector3 targetPosition in other.TargetPositions)
		{
			TargetPositions.Add(targetPosition);
		}
		SkillBasedStartPosition = other.SkillBasedStartPosition.Clone();
		SkillBasedEndPosition = other.SkillBasedEndPosition.Clone();
		ProjectileFinalSpeed = other.ProjectileFinalSpeed;
		ProjectileLifeTime = other.ProjectileLifeTime;
		HitNotifyCount = other.HitNotifyCount;
		SkillTime = other.SkillTime;
		AnimInfo = other.AnimInfo;
		SequenceInfos.Clear();
		foreach (KeyValuePair<int, SkillSequenceInfo> sequenceInfo in other.SequenceInfos)
		{
			SequenceInfos.Add(sequenceInfo.Key, sequenceInfo.Value);
		}
		ProjectileInfos.Clear();
		foreach (KeyValuePair<int, ProjectileInfo> projectileInfo in other.ProjectileInfos)
		{
			ProjectileInfos.Add(projectileInfo.Key, projectileInfo.Value);
		}
		FieldSkillInfos.Clear();
		foreach (KeyValuePair<int, FieldSkillInfo> fieldSkillInfo in other.FieldSkillInfos)
		{
			FieldSkillInfos.Add(fieldSkillInfo.Key, fieldSkillInfo.Value);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && _disposed.On())
		{
			SequenceInfos.Clear();
			ProjectileInfos.Clear();
			FieldSkillInfos.Clear();
			TargetPositions.Clear();
			MultiTargetObjectIDs.Clear();
		}
	}
}
