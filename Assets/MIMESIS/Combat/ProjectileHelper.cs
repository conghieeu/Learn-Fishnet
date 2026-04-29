using System.Collections.Generic;
using Bifrost.ConstEnum;
using Mimic.Actors;
using UnityEngine;

public sealed class ProjectileHelper
{
	private Dictionary<int, ProjectileActor> idToProjectileActorDict = new Dictionary<int, ProjectileActor>();

	public ProjectileActor? Spawn(int projectileMasterID, int projectileActorID, Vector3 position, Quaternion rotation)
	{
		ProjectileInfo projectileInfo = Hub.s.dataman.ExcelDataManager.GetProjectileInfo(projectileMasterID);
		if (projectileInfo == null)
		{
			Logger.RError($"Projectile data not found. projectileMasterID = {projectileMasterID}");
			return null;
		}
		switch (projectileInfo.ProjectileType)
		{
		case ProjectileType.Physics:
		{
			if (!Hub.s.tableman.projectile.TryGet(projectileInfo.PhysicsPrefabId, out MMProjectileTable.Row row) || row == null)
			{
				Logger.RError("Projectile prefab not found. id = " + projectileInfo.PhysicsPrefabId);
				break;
			}
			if (row.prefab == null)
			{
				Logger.RError("Projectile prefab is null. id = " + projectileInfo.PhysicsPrefabId);
				break;
			}
			GameObject gameObject = Object.Instantiate(row.prefab, position, rotation);
			if (gameObject == null)
			{
				Logger.RError("instantiate projectile failed");
				break;
			}
			if (!gameObject.TryGetComponent<PhysicsProjectileActor>(out var component) || component == null)
			{
				Logger.RError("PhysicsProjectile component not found");
				break;
			}
			idToProjectileActorDict.Add(projectileActorID, component);
			component.Initialize(projectileActorID, projectileInfo.PhysicsSpeed, projectileInfo.Gravity, projectileInfo.PhysicsLifetime, projectileInfo.PhysicsProjectileDestroy);
			component.StartMove();
			return component;
		}
		case ProjectileType.Raycast:
			return null;
		default:
			Logger.RError($"Unhandled projectile type: {projectileInfo.ProjectileType}");
			break;
		case ProjectileType.None:
			break;
		}
		return null;
	}

	public bool TryDespawn(int actorID)
	{
		if (idToProjectileActorDict.TryGetValue(actorID, out ProjectileActor value))
		{
			idToProjectileActorDict.Remove(actorID);
			value.StartDestroy();
			return true;
		}
		return false;
	}
}
