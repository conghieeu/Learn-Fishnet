using System;
using System.Collections.Generic;
using Bifrost.Cooked;
using DunGen;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class RealWorldRaySensor : MonoBehaviour
{
	public enum eTargetType
	{
		None = 0,
		Wall = 1,
		Player = 2,
		Monster = 3,
		Mimic = 4
	}

	[SerializeField]
	private float forwardRayDistance = 10f;

	[SerializeField]
	private float backwardRayDistance = 3f;

	[SerializeField]
	private float spreadAngle = 60f;

	[SerializeField]
	private Vector3 offset = Vector3.up;

	[SerializeField]
	[Range(1f, 32f)]
	private int forwardRayCount = 17;

	[SerializeField]
	[Range(0f, 10f)]
	private int backwardRayCount = 3;

	public float forwardHeadRadius = 0.2f;

	[SerializeField]
	private float backwardHeadRadius = 0.5f;

	[SerializeField]
	private bool showGizmo = true;

	[SerializeField]
	private bool debug_preview;

	public HashSet<Collider> _doorColliders = new HashSet<Collider>();

	private readonly Color[] debugColors = new Color[7]
	{
		Color.white,
		Color.red,
		Color.green,
		Color.blue,
		Color.yellow,
		Color.cyan,
		Color.magenta
	};

	private RaySensorHitResult[] resultsBuffer;

	private RaycastHit[] _hitsBuffer = new RaycastHit[64];

	public RaySensorHitResult[] Shoot(Vector3 position, float rotation)
	{
		int num = forwardRayCount + backwardRayCount;
		if (resultsBuffer == null || resultsBuffer.Length != num)
		{
			resultsBuffer = new RaySensorHitResult[num];
		}
		(float, float)[] array = new(float, float)[5]
		{
			(0f, 1f),
			(0f, 3f),
			(3f, 5f),
			(5f, 7f),
			(7f, 15f)
		};
		(float, float)[] array2 = new(float, float)[1] { (0f, 1.5f) };
		CollectDoorColliders(position);
		for (int num2 = 0; num2 < num; num2++)
		{
			float num3 = 0f;
			num3 = ((num2 >= forwardRayCount) ? (90f + 180f / (float)(backwardRayCount + 1) * (float)(num2 + 1 - forwardRayCount)) : (-90f + 180f / (float)(forwardRayCount - 1) * (float)num2));
			Vector3 vector = new Vector3(Mathf.Sin(rotation * (MathF.PI / 180f)), 0f, Mathf.Cos(rotation * (MathF.PI / 180f)));
			Vector3 vector2 = Quaternion.Euler(0f, num3, 0f) * vector;
			int num4;
			float num5;
			if (num3 >= -90f)
			{
				num4 = ((num3 <= 90f) ? 1 : 0);
				if (num4 != 0)
				{
					num5 = forwardRayDistance;
					goto IL_0183;
				}
			}
			else
			{
				num4 = 0;
			}
			num5 = backwardRayDistance;
			goto IL_0183;
			IL_0183:
			int num6 = Physics.SphereCastNonAlloc(maxDistance: num5, radius: (num4 != 0) ? forwardHeadRadius : backwardHeadRadius, origin: position + offset, direction: vector2, results: _hitsBuffer, layerMask: Hub.RaySensorLayerMaskForMimic, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			ref RaySensorHitResult reference = ref resultsBuffer[num2];
			reference.hitTypes = new List<int>();
			reference.hitPoints = new List<Vector3>();
			(float, float)[] array3 = ((num4 != 0) ? array : array2);
			for (int i = 0; i < array3.Length; i++)
			{
				(float, float) tuple = array3[i];
				float item = tuple.Item1;
				float item2 = tuple.Item2;
				int item3 = 0;
				Vector3 item4 = position + offset + vector2 * item2;
				float num7 = float.MaxValue;
				int num8 = -1;
				for (int j = 0; j < num6; j++)
				{
					if (!(_hitsBuffer[j].collider == null) && !(_hitsBuffer[j].collider.gameObject == base.gameObject))
					{
						float distance = _hitsBuffer[j].distance;
						if (distance > item && distance <= item2 && distance < num7)
						{
							num7 = distance;
							num8 = j;
						}
					}
				}
				if (num8 >= 0)
				{
					RaycastHit raycastHit = _hitsBuffer[num8];
					item4 = raycastHit.point;
					ProtoActor component = raycastHit.collider.gameObject.GetComponent<ProtoActor>();
					if (component != null)
					{
						if (component.ActorType == ActorType.Player)
						{
							item3 = 2;
						}
						else if (component.ActorType == ActorType.Monster)
						{
							MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(component.monsterMasterID);
							item3 = ((monsterInfo != null && monsterInfo.IsMimic()) ? 4 : 3);
						}
					}
					else
					{
						item3 = ((!raycastHit.collider.CompareTag("Area_SingleStair") && !IsDoorHit(raycastHit.collider)) ? 1 : 0);
					}
				}
				reference.hitTypes.Add(item3);
				reference.hitPoints.Add(item4);
			}
		}
		return resultsBuffer;
	}

	public void CollectDoorColliders(Vector3 position)
	{
		if (_doorColliders == null)
		{
			_doorColliders = new HashSet<Collider>(64);
		}
		else
		{
			_doorColliders.Clear();
		}
		if (Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.main == null)
		{
			return;
		}
		GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
		if (!(gamePlayScene != null))
		{
			return;
		}
		Tile tile = gamePlayScene.FindCurrentTile(in position);
		if (!(tile != null))
		{
			return;
		}
		HashSet<DoorLevelObject> hashSet = new HashSet<DoorLevelObject>();
		foreach (Doorway allDoorway in tile.AllDoorways)
		{
			if ((object)allDoorway == null || !allDoorway.HasDoorPrefabInstance)
			{
				continue;
			}
			DoorLevelObject component = allDoorway.UsedDoorPrefabInstance.GetComponent<DoorLevelObject>();
			if (component != null && component.doorState == DoorState.Closed && hashSet.Add(component))
			{
				Collider[] componentsInChildren = allDoorway.UsedDoorPrefabInstance.GetComponentsInChildren<Collider>();
				foreach (Collider item in componentsInChildren)
				{
					_doorColliders.Add(item);
				}
			}
		}
		DoorLevelObject[] componentsInChildren2 = tile.gameObject.GetComponentsInChildren<DoorLevelObject>();
		foreach (DoorLevelObject doorLevelObject in componentsInChildren2)
		{
			if (!(doorLevelObject as LockerDoorLevelObject != null) && doorLevelObject != null && doorLevelObject.doorState == DoorState.Closed && hashSet.Add(doorLevelObject))
			{
				Collider[] componentsInChildren = doorLevelObject.GetComponentsInChildren<Collider>();
				foreach (Collider item2 in componentsInChildren)
				{
					_doorColliders.Add(item2);
				}
			}
		}
	}

	public bool IsDoorHit(Collider collider)
	{
		if (_doorColliders != null)
		{
			return _doorColliders.Contains(collider);
		}
		return false;
	}
}
