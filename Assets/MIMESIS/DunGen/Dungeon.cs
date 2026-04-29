using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DunGen.Graph;
using UnityEngine;

namespace DunGen
{
	public class Dungeon : MonoBehaviour
	{
		public bool DebugRender;

		[SerializeField]
		private DungeonFlow dungeonFlow;

		[SerializeField]
		private List<Tile> allTiles = new List<Tile>();

		[SerializeField]
		private List<Tile> mainPathTiles = new List<Tile>();

		[SerializeField]
		private List<Tile> branchPathTiles = new List<Tile>();

		[SerializeField]
		private List<GameObject> doors = new List<GameObject>();

		[SerializeField]
		private List<DoorwayConnection> connections = new List<DoorwayConnection>();

		[SerializeField]
		private Tile attachmentTile;

		public Bounds Bounds { get; protected set; }

		public DungeonFlow DungeonFlow
		{
			get
			{
				return dungeonFlow;
			}
			set
			{
				dungeonFlow = value;
			}
		}

		public ReadOnlyCollection<Tile> AllTiles { get; private set; }

		public ReadOnlyCollection<Tile> MainPathTiles { get; private set; }

		public ReadOnlyCollection<Tile> BranchPathTiles { get; private set; }

		public ReadOnlyCollection<GameObject> Doors { get; private set; }

		public ReadOnlyCollection<DoorwayConnection> Connections { get; private set; }

		public DungeonGraph ConnectionGraph { get; private set; }

		public Dungeon()
		{
			AllTiles = new ReadOnlyCollection<Tile>(allTiles);
			MainPathTiles = new ReadOnlyCollection<Tile>(mainPathTiles);
			BranchPathTiles = new ReadOnlyCollection<Tile>(branchPathTiles);
			Doors = new ReadOnlyCollection<GameObject>(doors);
			Connections = new ReadOnlyCollection<DoorwayConnection>(connections);
		}

		private void Start()
		{
			if (allTiles.Count > 0 && ConnectionGraph == null)
			{
				FinaliseDungeonInfo();
			}
		}

		internal void AddAdditionalDoor(Door door)
		{
			if (door != null)
			{
				doors.Add(door.gameObject);
			}
		}

		internal void PreGenerateDungeon(DungeonGenerator dungeonGenerator)
		{
			DungeonFlow = dungeonGenerator.DungeonFlow;
		}

		internal void PostGenerateDungeon(DungeonGenerator dungeonGenerator)
		{
			FinaliseDungeonInfo();
		}

		private void FinaliseDungeonInfo()
		{
			List<Tile> list = new List<Tile>();
			if (attachmentTile != null)
			{
				list.Add(attachmentTile);
			}
			ConnectionGraph = new DungeonGraph(this, list);
			Bounds = UnityUtil.CombineBounds(allTiles.Select((Tile x) => x.Placement.Bounds).ToArray());
		}

		public void Clear()
		{
			foreach (Tile allTile in allTiles)
			{
				foreach (Doorway usedDoorway in allTile.UsedDoorways)
				{
					if (usedDoorway.UsedDoorPrefabInstance != null)
					{
						UnityUtil.Destroy(usedDoorway.UsedDoorPrefabInstance);
					}
				}
				UnityUtil.Destroy(allTile.gameObject);
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				UnityUtil.Destroy(base.transform.GetChild(i).gameObject);
			}
			allTiles.Clear();
			mainPathTiles.Clear();
			branchPathTiles.Clear();
			doors.Clear();
			connections.Clear();
			attachmentTile = null;
		}

		public Doorway GetConnectedDoorway(Doorway doorway)
		{
			foreach (DoorwayConnection connection in connections)
			{
				if (connection.A == doorway)
				{
					return connection.B;
				}
				if (connection.B == doorway)
				{
					return connection.A;
				}
			}
			return null;
		}

		public void FromProxy(DungeonProxy proxyDungeon, DungeonGenerator generator)
		{
			Clear();
			Dictionary<TileProxy, Tile> dictionary = new Dictionary<TileProxy, Tile>();
			if (generator.AttachmentSettings != null && generator.AttachmentSettings.TileProxy != null)
			{
				TileProxy tileProxy = generator.AttachmentSettings.TileProxy;
				attachmentTile = generator.AttachmentSettings.GetAttachmentTile();
				dictionary[tileProxy] = attachmentTile;
				DoorwayProxy doorwayProxy = tileProxy.UsedDoorways.First();
				Doorway doorway = attachmentTile.AllDoorways[doorwayProxy.Index];
				doorway.ProcessDoorwayObjects(isDoorwayInUse: true, generator.RandomStream);
				attachmentTile.UsedDoorways.Add(doorway);
				attachmentTile.UnusedDoorways.Remove(doorway);
			}
			foreach (TileProxy allTile in proxyDungeon.AllTiles)
			{
				GameObject gameObject = Object.Instantiate(allTile.Prefab, generator.Root.transform);
				gameObject.transform.localPosition = allTile.Placement.Position;
				gameObject.transform.localRotation = allTile.Placement.Rotation;
				Tile component = gameObject.GetComponent<Tile>();
				component.Dungeon = this;
				component.Placement = new TilePlacementData(allTile.Placement);
				component.Prefab = allTile.Prefab;
				dictionary[allTile] = component;
				allTiles.Add(component);
				component.Placement.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
				if (component.Placement.IsOnMainPath)
				{
					mainPathTiles.Add(component);
				}
				else
				{
					branchPathTiles.Add(component);
				}
				if (generator.PlaceTileTriggers)
				{
					component.AddTriggerVolume();
					component.gameObject.layer = generator.TileTriggerLayer;
				}
				Doorway[] componentsInChildren = gameObject.GetComponentsInChildren<Doorway>();
				Doorway[] array = componentsInChildren;
				foreach (Doorway doorway2 in array)
				{
					doorway2.Tile = component;
					doorway2.placedByGenerator = true;
					doorway2.HideConditionalObjects = false;
					component.AllDoorways.Add(doorway2);
				}
				foreach (DoorwayProxy usedDoorway in allTile.UsedDoorways)
				{
					Doorway doorway3 = componentsInChildren[usedDoorway.Index];
					component.UsedDoorways.Add(doorway3);
					doorway3.ProcessDoorwayObjects(isDoorwayInUse: true, generator.RandomStream);
				}
				foreach (DoorwayProxy unusedDoorway in allTile.UnusedDoorways)
				{
					Doorway doorway4 = componentsInChildren[unusedDoorway.Index];
					component.UnusedDoorways.Add(doorway4);
					doorway4.ProcessDoorwayObjects(isDoorwayInUse: false, generator.RandomStream);
				}
			}
			foreach (ProxyDoorwayConnection connection in proxyDungeon.Connections)
			{
				Tile tile = dictionary[connection.A.TileProxy];
				Tile tile2 = dictionary[connection.B.TileProxy];
				Doorway doorway5 = tile.AllDoorways[connection.A.Index];
				Doorway doorway6 = (doorway5.ConnectedDoorway = tile2.AllDoorways[connection.B.Index]);
				doorway6.ConnectedDoorway = doorway5;
				DoorwayConnection item = new DoorwayConnection(doorway5, doorway6);
				connections.Add(item);
				SpawnDoorPrefab(doorway5, doorway6, generator.RandomStream);
			}
		}

		private void SpawnDoorPrefab(Doorway a, Doorway b, RandomStream randomStream)
		{
			if (a.HasDoorPrefabInstance || b.HasDoorPrefabInstance)
			{
				return;
			}
			bool flag = a.ConnectorPrefabWeights.HasAnyViableEntries();
			bool flag2 = b.ConnectorPrefabWeights.HasAnyViableEntries();
			if (!flag && !flag2)
			{
				return;
			}
			Doorway doorway = ((!(flag && flag2)) ? (flag ? a : b) : ((a.DoorPrefabPriority < b.DoorPrefabPriority) ? b : a));
			GameObject random = doorway.ConnectorPrefabWeights.GetRandom(randomStream);
			if (random != null)
			{
				GameObject gameObject = Object.Instantiate(random, doorway.transform);
				gameObject.transform.localPosition = Vector3.zero;
				if (!doorway.AvoidRotatingDoorPrefab)
				{
					gameObject.transform.localRotation = Quaternion.identity;
				}
				doors.Add(gameObject);
				DungeonUtil.AddAndSetupDoorComponent(this, gameObject, doorway);
				a.SetUsedPrefab(gameObject);
				b.SetUsedPrefab(gameObject);
			}
		}

		public void OnDrawGizmos()
		{
			if (DebugRender)
			{
				DebugDraw();
			}
		}

		public void DebugDraw()
		{
			Color red = Color.red;
			Color green = Color.green;
			Color blue = Color.blue;
			Color b = new Color(0.5f, 0f, 0.5f);
			float a = 0.75f;
			foreach (Tile allTile in allTiles)
			{
				Bounds bounds = allTile.Placement.Bounds;
				bounds.size *= 1.01f;
				Color color = (allTile.Placement.IsOnMainPath ? Color.Lerp(red, green, allTile.Placement.NormalizedDepth) : Color.Lerp(blue, b, allTile.Placement.NormalizedDepth));
				color.a = a;
				Gizmos.color = color;
				Gizmos.DrawCube(bounds.center, bounds.size);
			}
		}
	}
}
