using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
	public class DecalData
	{
		public readonly string DecalId;

		public readonly string Socket;

		public readonly Color DecalColor;

		public readonly float PrintIntervalDist;

		public readonly bool IsRandomRotation;

		public readonly long LifetimeMSec;

		public readonly long FadeoutMSec;

		public readonly float DistanceFromSpawnPoint = 1f;

		public readonly Vector3 Direction;

		public readonly Vector3 HitSurfaceNormal;

		public readonly Transform? SpawnBase;

		private DecalData(string pathWithSocket, string colorId, float printIntervalDist, bool isRandomRotation, long lifetimeMSec, long fadeoutMSec, float distanceFromSpawnPoint, Vector3 direction, Vector3 hitSurfaceNormal, Transform spawnBase = null)
		{
			if (!string.IsNullOrEmpty(pathWithSocket) && pathWithSocket.Contains("@"))
			{
				string[] array = pathWithSocket.Split('@');
				DecalId = array[0];
				Socket = ((array.Length > 1) ? array[1] : "");
			}
			else
			{
				DecalId = pathWithSocket;
				Socket = "";
			}
			DecalColor = Hub.s.tableman.color.GetColor(colorId);
			IsRandomRotation = isRandomRotation;
			PrintIntervalDist = printIntervalDist;
			LifetimeMSec = lifetimeMSec;
			FadeoutMSec = fadeoutMSec;
			DistanceFromSpawnPoint = distanceFromSpawnPoint;
			Direction = direction;
			HitSurfaceNormal = hitSurfaceNormal;
			SpawnBase = spawnBase;
		}

		public static DecalData CreateDecalData(string pathWithSocket, string colorId, float printIntervalDist, bool isRandomRotation, long lifetimeMSec, long fadeoutMSec, float distanceFromSpawnPoint, Vector3 direction, Vector3 hitSurfaceNormal, Transform spawnBase = null)
		{
			return new DecalData(pathWithSocket, colorId, printIntervalDist, isRandomRotation, lifetimeMSec, fadeoutMSec, distanceFromSpawnPoint, direction, hitSurfaceNormal, spawnBase);
		}
	}

	public int periodicDecalInstanceId;

	public int decalIntanceId;

	private Dictionary<int, DecalData> periodicDecals = new Dictionary<int, DecalData>();

	private Dictionary<int, Vector3> previousPositionForDecals = new Dictionary<int, Vector3>();

	private Dictionary<int, float> moveDistanceCacheForDecals = new Dictionary<int, float>();

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] DecalManager.Awake ->");
		StartCoroutine(CorPrintPeriodicDecals());
		periodicDecalInstanceId = 0;
		decalIntanceId = 0;
		Logger.RLog("[AwakeLogs] DecalManager.Awake <-");
	}

	private IEnumerator CorPrintPeriodicDecals()
	{
		while (true)
		{
			foreach (KeyValuePair<int, DecalData> item in periodicDecals.ToList())
			{
				int key = item.Key;
				DecalData value = item.Value;
				if (value.SpawnBase == null)
				{
					continue;
				}
				Vector3 position = value.SpawnBase.position;
				moveDistanceCacheForDecals.TryGetValue(key, out var value2);
				if (!previousPositionForDecals.TryGetValue(key, out var value3))
				{
					previousPositionForDecals[key] = position;
					moveDistanceCacheForDecals[key] = 0f;
					continue;
				}
				float num = Vector3.Distance(value3, position);
				value2 += num;
				if (value2 >= value.PrintIntervalDist)
				{
					SpawnPeriodic(value);
					value2 = 0f;
					previousPositionForDecals[key] = position;
				}
				else
				{
					previousPositionForDecals[key] = position;
				}
				moveDistanceCacheForDecals[key] = value2;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void OnDestroy()
	{
		periodicDecals.Clear();
		previousPositionForDecals.Clear();
		moveDistanceCacheForDecals.Clear();
	}

	public int ActivateDecalPeriodically(DecalData decalData)
	{
		if (decalData == null)
		{
			return -1;
		}
		if (decalData.SpawnBase == null)
		{
			Logger.RError("[DecalManager] SpawnBase is null. Cannot spawn decal.");
			return -1;
		}
		periodicDecalInstanceId++;
		periodicDecals.Add(periodicDecalInstanceId, decalData);
		return periodicDecalInstanceId;
	}

	public void DeactivateDecal(int instanceId)
	{
		if (periodicDecals.TryGetValue(instanceId, out var _))
		{
			periodicDecals.Remove(instanceId);
			previousPositionForDecals.Remove(instanceId);
			moveDistanceCacheForDecals.Remove(instanceId);
		}
		else
		{
			Logger.RError($"[DecalManager] No periodic decal found with ID: {instanceId}");
		}
	}

	private void SpawnPeriodic(DecalData decalData)
	{
		if (decalData == null)
		{
			Logger.RError("[DecalManager] DecalData is null. Cannot spawn decal.");
		}
		else if (decalData.SpawnBase == null)
		{
			Logger.RError("[DecalManager] SpawnBase is null. Cannot spawn decal. Maybe onwer is dead.");
		}
		else
		{
			SpawnDecal(decalData, null, decalData.SpawnBase.position);
		}
	}

	public GameObject? SpawnDecal(DecalData decalData, Transform? parent, Vector3? worldPosition)
	{
		if (decalData == null)
		{
			return null;
		}
		MMDecalTable.Row row = Hub.s.tableman.decal.rows.FirstOrDefault((MMDecalTable.Row x) => x.id == decalData.DecalId);
		if (row == null)
		{
			Logger.RError("[DecalManager] Decal not found: " + decalData.DecalId);
			return null;
		}
		Vector3 vector = -decalData.Direction;
		if (Vector3.Angle(vector, decalData.HitSurfaceNormal) > 45f)
		{
			vector = Vector3.RotateTowards(decalData.HitSurfaceNormal, vector, MathF.PI / 4f, 0f);
		}
		vector = -vector;
		Quaternion quaternion = Quaternion.LookRotation(vector);
		GameObject gameObject = null;
		if (parent != null)
		{
			Transform transform = ((decalData.Socket != "") ? SocketNodeMarker.FindFirstInHierarchy(parent, decalData.Socket) : parent);
			gameObject = row.Instantiate(transform.position);
			float num = 1f;
			if (gameObject != null)
			{
				WorldDecal component = gameObject.GetComponent<WorldDecal>();
				if (component != null)
				{
					num = component.DistanceFromSpawnPoint;
				}
			}
			Vector3 vector2 = -(quaternion * Vector3.forward).normalized * num;
			gameObject.transform.position += vector2;
			gameObject.transform.rotation = quaternion;
		}
		else
		{
			if (!worldPosition.HasValue)
			{
				Logger.RError("[DecalManager] Both parent and worldPosition are null.");
				return null;
			}
			gameObject = row.Instantiate(worldPosition.Value);
			float num2 = 1f;
			if (gameObject != null)
			{
				WorldDecal component2 = gameObject.GetComponent<WorldDecal>();
				if (component2 != null)
				{
					num2 = component2.DistanceFromSpawnPoint;
				}
			}
			Vector3 vector3 = -(quaternion * Vector3.forward).normalized * num2;
			gameObject.transform.position += vector3;
			if (decalData.IsRandomRotation)
			{
				gameObject.transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), vector3) * Quaternion.FromToRotation(gameObject.transform.up, vector3) * gameObject.transform.rotation;
			}
			else
			{
				gameObject.transform.rotation = Quaternion.FromToRotation(gameObject.transform.up, vector3) * gameObject.transform.rotation;
			}
			Hub.DebugDraw_Arrow(worldPosition.Value, worldPosition.Value + vector3 * 10f, Color.magenta, 5f);
		}
		if (gameObject != null)
		{
			WorldDecal componentInChildren = gameObject.GetComponentInChildren<WorldDecal>();
			if (componentInChildren == null)
			{
				UnityEngine.Object.Destroy(gameObject);
				Logger.RError("[DecalManager] WorldDecal component not found in the instantiated prefab.");
				return null;
			}
			componentInChildren.decalId = decalData.DecalId;
			componentInChildren.lifetimeMSec = decalData.LifetimeMSec;
			componentInChildren.fadeoutMSec = decalData.FadeoutMSec;
			componentInChildren.rootGameObject = gameObject;
			componentInChildren.Activate();
		}
		return gameObject;
	}

	public void TurnOffDecal()
	{
		WorldDecal.TurnOffDecal();
	}

	public void TurnOnDecal()
	{
		WorldDecal.TurnOnDecal();
	}
}
