using System.Collections.Generic;
using DunGen.Tags;
using UnityEngine;
using UnityEngine.Serialization;

namespace DunGen
{
	[AddComponentMenu("DunGen/Doorway")]
	public class Doorway : MonoBehaviour, ISerializationCallbackReceiver
	{
		public const int CurrentFileVersion = 1;

		public int DoorPrefabPriority;

		public List<GameObjectWeight> ConnectorPrefabWeights = new List<GameObjectWeight>();

		public List<GameObjectWeight> BlockerPrefabWeights = new List<GameObjectWeight>();

		public bool AvoidRotatingDoorPrefab;

		public bool AvoidRotatingBlockerPrefab;

		[FormerlySerializedAs("AddWhenInUse")]
		public List<GameObject> ConnectorSceneObjects = new List<GameObject>();

		[FormerlySerializedAs("AddWhenNotInUse")]
		public List<GameObject> BlockerSceneObjects = new List<GameObject>();

		public TagContainer Tags = new TagContainer();

		public int? LockID;

		[SerializeField]
		[FormerlySerializedAs("SocketGroup")]
		private DoorwaySocketType socketGroup_obsolete = (DoorwaySocketType)(-1);

		[SerializeField]
		[FormerlySerializedAs("DoorPrefabs")]
		private List<GameObject> doorPrefabs_obsolete = new List<GameObject>();

		[SerializeField]
		[FormerlySerializedAs("BlockerPrefabs")]
		private List<GameObject> blockerPrefabs_obsolete = new List<GameObject>();

		[SerializeField]
		private DoorwaySocket socket;

		[SerializeField]
		private GameObject doorPrefabInstance;

		[SerializeField]
		private Door doorComponent;

		[SerializeField]
		private Tile tile;

		[SerializeField]
		private Doorway connectedDoorway;

		[SerializeField]
		private bool hideConditionalObjects;

		[SerializeField]
		public GameObject spawnedBlockerPrefab;

		[SerializeField]
		private int fileVersion;

		internal bool placedByGenerator;

		public bool HasSocketAssigned => socket != null;

		public DoorwaySocket Socket
		{
			get
			{
				if (!(socket != null))
				{
					return DunGenSettings.Instance.DefaultSocket;
				}
				return socket;
			}
		}

		public Tile Tile
		{
			get
			{
				return tile;
			}
			internal set
			{
				tile = value;
			}
		}

		public bool IsLocked => LockID.HasValue;

		public bool HasDoorPrefabInstance => doorPrefabInstance != null;

		public GameObject UsedDoorPrefabInstance => doorPrefabInstance;

		public Door DoorComponent => doorComponent;

		public Dungeon Dungeon { get; internal set; }

		public Doorway ConnectedDoorway
		{
			get
			{
				return connectedDoorway;
			}
			internal set
			{
				connectedDoorway = value;
			}
		}

		public bool HideConditionalObjects
		{
			get
			{
				return hideConditionalObjects;
			}
			set
			{
				hideConditionalObjects = value;
				foreach (GameObject connectorSceneObject in ConnectorSceneObjects)
				{
					if (connectorSceneObject != null)
					{
						connectorSceneObject.SetActive(!hideConditionalObjects);
					}
				}
				foreach (GameObject blockerSceneObject in BlockerSceneObjects)
				{
					if (blockerSceneObject != null)
					{
						blockerSceneObject.SetActive(!hideConditionalObjects);
					}
				}
			}
		}

		internal void SetUsedPrefab(GameObject doorPrefab)
		{
			doorPrefabInstance = doorPrefab;
			if (doorPrefab != null)
			{
				doorComponent = doorPrefab.GetComponent<Door>();
			}
		}

		internal void RemoveUsedPrefab()
		{
			if (doorPrefabInstance != null)
			{
				UnityUtil.Destroy(doorPrefabInstance);
			}
			doorPrefabInstance = null;
		}

		private void GetTileRoot(out GameObject tileRoot, out Tile tileComponent)
		{
			tileComponent = GetComponentInParent<Tile>();
			if (tileComponent != null)
			{
				tileRoot = tileComponent.gameObject;
			}
			else
			{
				tileRoot = base.transform.root.gameObject;
			}
		}

		public bool ValidateTransform(out Bounds tileBounds, out bool isAxisAligned, out bool isEdgePositioned)
		{
			GetTileRoot(out var tileRoot, out var tileComponent);
			isAxisAligned = true;
			isEdgePositioned = true;
			if (tileComponent != null && tileComponent.OverrideAutomaticTileBounds)
			{
				tileBounds = tileComponent.TileBoundsOverride;
				tileBounds.center += tileRoot.transform.position;
			}
			else
			{
				tileBounds = UnityUtil.CalculateObjectBounds(tileRoot, includeInactive: false, ignoreSpriteRenderers: false);
			}
			if (!UnityUtil.IsVectorAxisAligned(base.transform.forward))
			{
				isAxisAligned = false;
			}
			if ((ProjectPositionToTileBounds(tileBounds) - base.transform.position).magnitude > 0.1f)
			{
				isEdgePositioned = false;
			}
			return isAxisAligned & isEdgePositioned;
		}

		public void TrySnapToCorrectedTransform()
		{
			if (!ValidateTransform(out var tileBounds, out var _, out var _))
			{
				float magnitude;
				Vector3 cardinalDirection = UnityUtil.GetCardinalDirection(base.transform.forward, out magnitude);
				base.transform.forward = cardinalDirection;
				base.transform.position = ProjectPositionToTileBounds(tileBounds);
			}
		}

		public Vector3 ProjectPositionToTileBounds(Bounds tileBounds)
		{
			float magnitude;
			Vector3 cardinalDirection = UnityUtil.GetCardinalDirection(base.transform.forward, out magnitude);
			Vector3 rhs = base.transform.position - tileBounds.center;
			float num = Vector3.Dot(cardinalDirection, rhs);
			float num2 = Vector3.Dot((magnitude < 0f) ? (-cardinalDirection) : cardinalDirection, tileBounds.extents) - num;
			return UnityUtil.ClampVector(base.transform.position + cardinalDirection * num2, tileBounds.min, tileBounds.max);
		}

		internal void ProcessDoorwayObjects(bool isDoorwayInUse, RandomStream randomStream)
		{
			foreach (GameObject blockerSceneObject in BlockerSceneObjects)
			{
				if (blockerSceneObject != null)
				{
					blockerSceneObject.SetActive(!isDoorwayInUse);
				}
			}
			foreach (GameObject connectorSceneObject in ConnectorSceneObjects)
			{
				if (connectorSceneObject != null)
				{
					connectorSceneObject.SetActive(isDoorwayInUse);
				}
			}
			if (isDoorwayInUse)
			{
				if (spawnedBlockerPrefab != null)
				{
					Object.DestroyImmediate(spawnedBlockerPrefab);
				}
			}
			else if (BlockerPrefabWeights.HasAnyViableEntries())
			{
				spawnedBlockerPrefab = Object.Instantiate(BlockerPrefabWeights.GetRandom(randomStream));
				spawnedBlockerPrefab.transform.parent = base.gameObject.transform;
				spawnedBlockerPrefab.transform.localPosition = Vector3.zero;
				spawnedBlockerPrefab.transform.localScale = Vector3.one;
				if (!AvoidRotatingBlockerPrefab)
				{
					spawnedBlockerPrefab.transform.localRotation = Quaternion.identity;
				}
			}
		}

		public void OnBeforeSerialize()
		{
			fileVersion = 1;
		}

		public void OnAfterDeserialize()
		{
			if (fileVersion >= 1)
			{
				return;
			}
			foreach (GameObject item in doorPrefabs_obsolete)
			{
				ConnectorPrefabWeights.Add(new GameObjectWeight(item));
			}
			foreach (GameObject item2 in blockerPrefabs_obsolete)
			{
				BlockerPrefabWeights.Add(new GameObjectWeight(item2));
			}
			doorPrefabs_obsolete.Clear();
			blockerPrefabs_obsolete.Clear();
		}
	}
}
