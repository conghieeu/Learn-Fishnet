using ReluProtocol;
using UnityEngine;

public abstract class SpawnedActorData
{
	public readonly int Index;

	public readonly SpawnType SpawnType;

	public readonly long SpawnWaitTime;

	public readonly int MaxRespawnCount;

	public readonly bool EnableReset;

	public readonly string AIName = string.Empty;

	public readonly string BTName = string.Empty;

	public readonly int MasterID;

	public readonly MapMarkerType MarkerType;

	public readonly PosWithRot Pos;

	public readonly int StackCount;

	public readonly int Durability;

	public readonly int DefaultGauge;

	public readonly string Name;

	public readonly bool IsIndoor;

	public readonly Vector3 SurfaceNormalVector;

	public readonly Vector3 PosVector;

	public int ActorID { get; private set; }

	public int CurrentSpawnCount { get; private set; }

	public long SpawnWaitStartTime { get; private set; }

	public long LastSpawnTime { get; private set; }

	public SpawnedActorData(MapMarker_SpawnPoint spawnPointData)
	{
		Index = spawnPointData.ID;
		MasterID = spawnPointData.masterID;
		SpawnType = spawnPointData.spawnType;
		SpawnWaitTime = spawnPointData.spawnWaitTime;
		MaxRespawnCount = spawnPointData.MaxRespawnCount;
		EnableReset = spawnPointData.enableReset;
		CurrentSpawnCount = 0;
		SpawnWaitStartTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		LastSpawnTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		Pos = spawnPointData.pos.Clone();
		PosVector = Pos.toVector3();
		Name = spawnPointData.Name;
		IsIndoor = spawnPointData.IsIndoor;
		if (spawnPointData is MapMarker_CreatureSpawnPoint mapMarker_CreatureSpawnPoint)
		{
			MarkerType = MapMarkerType.Creature;
			AIName = mapMarker_CreatureSpawnPoint.aiName;
			BTName = mapMarker_CreatureSpawnPoint.btName;
		}
		else if (spawnPointData is MapMarker_LootingObjectSpawnPoint mapMarker_LootingObjectSpawnPoint)
		{
			MarkerType = MapMarkerType.LootingObject;
			StackCount = mapMarker_LootingObjectSpawnPoint.stackCount;
			Durability = mapMarker_LootingObjectSpawnPoint.durability;
			DefaultGauge = mapMarker_LootingObjectSpawnPoint.defaultGauge;
		}
		else if (spawnPointData is MapMarker_FieldSkillSpawnPoint mapMarker_FieldSkillSpawnPoint)
		{
			MarkerType = MapMarkerType.FieldSkill;
			Pos.pitch = mapMarker_FieldSkillSpawnPoint.DecalDirectionVector.x;
			Pos.yaw = mapMarker_FieldSkillSpawnPoint.DecalDirectionVector.y;
			Pos.roll = mapMarker_FieldSkillSpawnPoint.DecalDirectionVector.z;
			SurfaceNormalVector = new Vector3(mapMarker_FieldSkillSpawnPoint.SurfaceNormalVector.x, mapMarker_FieldSkillSpawnPoint.SurfaceNormalVector.y, mapMarker_FieldSkillSpawnPoint.SurfaceNormalVector.z);
		}
		else
		{
			Logger.RError("Unknown SpawnPoint Type. Type : " + spawnPointData.GetType());
			MarkerType = MapMarkerType.Creature;
		}
		if (SpawnType == SpawnType.OnStartMap && MaxRespawnCount >= 1)
		{
			Logger.RWarn("[SpawnActorData] SpawnType.OnStartMap is not affected by MaxRespawnCount. Only once.");
		}
	}

	public void SetActorID(int actorID)
	{
		if (ActorID != 0 && ActorID != 999999)
		{
			Logger.RError("ActorID is already set. ActorID : " + ActorID);
			return;
		}
		ActorID = actorID;
		if (ActorID != 999999)
		{
			if (ActorID > 0)
			{
				CurrentSpawnCount++;
			}
			SpawnWaitStartTime = 0L;
		}
	}

	public virtual void OnActorDead()
	{
		SpawnWaitStartTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		ActorID = 0;
	}

	public bool CanSpawn()
	{
		if (ActorID != 0 && ActorID != 999999)
		{
			return false;
		}
		switch (SpawnType)
		{
		case SpawnType.OnStartMap:
		case SpawnType.EventAction:
			if (!CheckRespawnCount())
			{
				return false;
			}
			if (SpawnWaitStartTime + SpawnWaitTime >= Hub.s.timeutil.GetCurrentTickMilliSec())
			{
				return false;
			}
			break;
		}
		return true;
	}

	private bool CheckRespawnCount()
	{
		if (SpawnType == SpawnType.OnStartMap && CurrentSpawnCount >= 1)
		{
			return false;
		}
		if (CurrentSpawnCount > MaxRespawnCount)
		{
			if (EnableReset)
			{
				CurrentSpawnCount = 0;
				return true;
			}
			return false;
		}
		return true;
	}
}
