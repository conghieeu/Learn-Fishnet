using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DunGen
{
	[DefaultExecutionOrder(-1)]
	[AddComponentMenu("DunGen/Culling/Adjacent Room Culling")]
	public class AdjacentRoomCulling : MonoBehaviour
	{
		public delegate void VisibilityChangedDelegate(Tile tile, bool visible);

		public int AdjacentTileDepth = 1;

		public bool CullBehindClosedDoors = true;

		public Transform TargetOverride;

		public bool IncludeDisabledComponents;

		[NonSerialized]
		public Dictionary<Renderer, bool> OverrideRendererVisibilities = new Dictionary<Renderer, bool>();

		[NonSerialized]
		public Dictionary<Light, bool> OverrideLightVisibilities = new Dictionary<Light, bool>();

		protected List<Tile> allTiles;

		protected List<Door> allDoors;

		protected List<Tile> oldVisibleTiles;

		protected List<Tile> visibleTiles;

		protected Dictionary<Tile, bool> tileVisibilities;

		protected Dictionary<Tile, List<Renderer>> tileRenderers;

		protected Dictionary<Tile, List<Light>> lightSources;

		protected Dictionary<Tile, List<ReflectionProbe>> reflectionProbes;

		protected Dictionary<Tile, List<SimpleLightAnimation>> relumod_lightAnimations;

		protected Dictionary<Tile, List<DecalProjector>> relumod_decals;

		protected Dictionary<Door, List<Renderer>> doorRenderers;

		private bool dirty;

		private DungeonGenerator generator;

		private Tile currentTile;

		private Queue<Tile> tilesToSearch;

		private List<Tile> searchedTiles;

		private Dungeon dungeon;

		private Bounds relumod_allBounds;

		private List<List<List<Tile>>> relumod_tilesByXZ;

		private float relumod_tilesByXWidth;

		private float relumod_tilesByZWidth;

		public bool Ready { get; protected set; }

		public bool relumod_Suppressed { get; protected set; }

		protected Transform targetTransform
		{
			get
			{
				if (!(TargetOverride != null))
				{
					return base.transform;
				}
				return TargetOverride;
			}
		}

		public event VisibilityChangedDelegate TileVisibilityChanged;

		public bool IsCameraTargetOutdoor()
		{
			if (Hub.s == null)
			{
				return false;
			}
			if (Hub.s.pdata.main is GamePlayScene gamePlayScene)
			{
				return gamePlayScene.IsCameraTargetOutdoor();
			}
			return false;
		}

		protected virtual void OnEnable()
		{
			RuntimeDungeon runtimeDungeon = UnityUtil.FindObjectByType<RuntimeDungeon>();
			if (runtimeDungeon != null)
			{
				generator = runtimeDungeon.Generator;
				generator.OnGenerationStatusChanged += OnDungeonGenerationStatusChanged;
				if (generator.Status == GenerationStatus.Complete)
				{
					SetDungeon(generator.CurrentDungeon);
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (generator != null)
			{
				generator.OnGenerationStatusChanged -= OnDungeonGenerationStatusChanged;
			}
			ClearDungeon();
		}

		public virtual void SetDungeon(Dungeon newDungeon)
		{
			if (Ready)
			{
				ClearDungeon();
			}
			dungeon = newDungeon;
			if (dungeon == null)
			{
				return;
			}
			allTiles = new List<Tile>(dungeon.AllTiles);
			allDoors = new List<Door>(GetAllDoorsInDungeon(dungeon));
			oldVisibleTiles = new List<Tile>(allTiles.Count);
			visibleTiles = new List<Tile>(allTiles.Count);
			tileVisibilities = new Dictionary<Tile, bool>();
			tileRenderers = new Dictionary<Tile, List<Renderer>>();
			lightSources = new Dictionary<Tile, List<Light>>();
			reflectionProbes = new Dictionary<Tile, List<ReflectionProbe>>();
			doorRenderers = new Dictionary<Door, List<Renderer>>();
			relumod_lightAnimations = new Dictionary<Tile, List<SimpleLightAnimation>>();
			relumod_decals = new Dictionary<Tile, List<DecalProjector>>();
			UpdateRendererLists();
			foreach (Tile allTile in allTiles)
			{
				SetTileVisibility(allTile, visible: false);
			}
			foreach (Door allDoor in allDoors)
			{
				allDoor.OnDoorStateChanged += OnDoorStateChanged;
				SetDoorVisibility(allDoor, visible: false);
			}
			relumod_InspectTiles();
			Ready = true;
			dirty = true;
		}

		private void relumod_InspectTiles()
		{
			relumod_allBounds = default(Bounds);
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			foreach (Tile allTile in allTiles)
			{
				if (allTile.Bounds.size.x < num)
				{
					num = allTile.Bounds.size.x;
				}
				if (allTile.Bounds.size.z < num2)
				{
					num2 = allTile.Bounds.size.z;
				}
				relumod_allBounds.Encapsulate(allTile.Bounds);
			}
			relumod_tilesByXWidth = num;
			relumod_tilesByZWidth = num2;
			relumod_tilesByXZ = new List<List<List<Tile>>>();
			int num3 = Mathf.CeilToInt(relumod_allBounds.size.x / relumod_tilesByXWidth);
			int num4 = Mathf.CeilToInt(relumod_allBounds.size.z / relumod_tilesByZWidth);
			for (int i = 0; i < num3; i++)
			{
				List<List<Tile>> list = new List<List<Tile>>();
				relumod_tilesByXZ.Add(list);
				for (int j = 0; j < num4; j++)
				{
					list.Add(new List<Tile>());
				}
			}
			foreach (Tile allTile2 in allTiles)
			{
				float num5 = 0.01f;
				int value = Mathf.FloorToInt((allTile2.Bounds.min.x + num5 - relumod_allBounds.min.x) / relumod_tilesByXWidth);
				int value2 = Mathf.FloorToInt((allTile2.Bounds.max.x - num5 - relumod_allBounds.min.x) / relumod_tilesByXWidth);
				int value3 = Mathf.FloorToInt((allTile2.Bounds.min.z + num5 - relumod_allBounds.min.z) / relumod_tilesByZWidth);
				int value4 = Mathf.FloorToInt((allTile2.Bounds.max.z - num5 - relumod_allBounds.min.z) / relumod_tilesByZWidth);
				int num6 = Mathf.Clamp(value, 0, num3 - 1);
				value2 = Mathf.Clamp(value2, 0, num3 - 1);
				value3 = Mathf.Clamp(value3, 0, num4 - 1);
				value4 = Mathf.Clamp(value4, 0, num4 - 1);
				for (int k = num6; k <= value2; k++)
				{
					for (int l = value3; l <= value4; l++)
					{
						relumod_tilesByXZ[k][l].Add(allTile2);
					}
				}
			}
		}

		public virtual bool IsTileVisible(Tile tile)
		{
			if (tileVisibilities.TryGetValue(tile, out var value))
			{
				return value;
			}
			return false;
		}

		protected IEnumerable<Door> GetAllDoorsInDungeon(Dungeon dungeon)
		{
			foreach (GameObject door in dungeon.Doors)
			{
				if (!(door == null))
				{
					Door component = door.GetComponent<Door>();
					if (component != null)
					{
						yield return component;
					}
				}
			}
		}

		protected virtual void ClearDungeon()
		{
			if (!Ready)
			{
				return;
			}
			foreach (Door allDoor in allDoors)
			{
				SetDoorVisibility(allDoor, visible: true);
				allDoor.OnDoorStateChanged -= OnDoorStateChanged;
			}
			foreach (Tile allTile in allTiles)
			{
				SetTileVisibility(allTile, visible: true);
			}
			Ready = false;
		}

		protected virtual void OnDoorStateChanged(Door door, bool isOpen)
		{
			dirty = true;
		}

		protected virtual void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			switch (status)
			{
			case GenerationStatus.Complete:
				SetDungeon(generator.CurrentDungeon);
				break;
			case GenerationStatus.Failed:
				ClearDungeon();
				break;
			}
		}

		protected virtual void LateUpdate()
		{
			if (Ready && !relumod_Suppressed)
			{
				Tile tile = currentTile;
				Tile tile2 = null;
				if (currentTile == null)
				{
					tile2 = relumod_FastFindCurrentTile();
				}
				else if (!currentTile.Bounds.Contains(targetTransform.position))
				{
					tile2 = SearchForNewCurrentTile();
				}
				if (tile2 != null && tile2 != tile)
				{
					currentTile = tile2;
					RefreshVisibility();
				}
			}
		}

		protected virtual void RefreshVisibility()
		{
			List<Tile> list = visibleTiles;
			visibleTiles = oldVisibleTiles;
			oldVisibleTiles = list;
			UpdateVisibleTiles();
			foreach (Tile oldVisibleTile in oldVisibleTiles)
			{
				if (!visibleTiles.Contains(oldVisibleTile))
				{
					SetTileVisibility(oldVisibleTile, visible: false);
				}
			}
			foreach (Tile visibleTile in visibleTiles)
			{
				if (!oldVisibleTiles.Contains(visibleTile))
				{
					SetTileVisibility(visibleTile, visible: true);
				}
			}
			oldVisibleTiles.Clear();
			RefreshDoorVisibilities();
		}

		protected virtual void RefreshDoorVisibilities()
		{
			foreach (Door allDoor in allDoors)
			{
				bool visible = visibleTiles.Contains(allDoor.DoorwayA.Tile) || visibleTiles.Contains(allDoor.DoorwayB.Tile);
				SetDoorVisibility(allDoor, visible);
			}
		}

		protected virtual void SetDoorVisibility(Door door, bool visible)
		{
			if (!doorRenderers.TryGetValue(door, out var value))
			{
				return;
			}
			for (int num = value.Count - 1; num >= 0; num--)
			{
				Renderer renderer = value[num];
				bool value2;
				if (renderer == null)
				{
					value.RemoveAt(num);
				}
				else if (OverrideRendererVisibilities.TryGetValue(renderer, out value2))
				{
					renderer.enabled = value2;
				}
				else
				{
					renderer.enabled = visible;
				}
			}
		}

		protected virtual void UpdateVisibleTiles()
		{
			visibleTiles.Clear();
			if (currentTile != null)
			{
				visibleTiles.Add(currentTile);
			}
			int num = 0;
			for (int i = 0; i < AdjacentTileDepth; i++)
			{
				int count = visibleTiles.Count;
				for (int j = num; j < count; j++)
				{
					foreach (Doorway usedDoorway in visibleTiles[j].UsedDoorways)
					{
						Tile tile = usedDoorway.ConnectedDoorway.Tile;
						if (visibleTiles.Contains(tile))
						{
							continue;
						}
						if (CullBehindClosedDoors)
						{
							Door doorComponent = usedDoorway.DoorComponent;
							if (doorComponent != null && doorComponent.ShouldCullBehind)
							{
								continue;
							}
						}
						visibleTiles.Add(tile);
					}
				}
				num = count;
			}
		}

		protected virtual void SetTileVisibility(Tile tile, bool visible)
		{
			tileVisibilities[tile] = visible;
			if (tileRenderers.TryGetValue(tile, out var value))
			{
				for (int num = value.Count - 1; num >= 0; num--)
				{
					Renderer renderer = value[num];
					bool value2;
					if (renderer == null)
					{
						value.RemoveAt(num);
					}
					else if (OverrideRendererVisibilities.TryGetValue(renderer, out value2))
					{
						renderer.enabled = value2;
					}
					else
					{
						renderer.enabled = visible;
					}
				}
			}
			if (lightSources.TryGetValue(tile, out var value3))
			{
				for (int num2 = value3.Count - 1; num2 >= 0; num2--)
				{
					Light light = value3[num2];
					bool value4;
					if (light == null)
					{
						value3.RemoveAt(num2);
					}
					else if (OverrideLightVisibilities.TryGetValue(light, out value4))
					{
						light.enabled = value4;
					}
					else
					{
						light.enabled = visible;
					}
				}
			}
			if (relumod_lightAnimations.TryGetValue(tile, out var value5))
			{
				for (int num3 = value5.Count - 1; num3 >= 0; num3--)
				{
					SimpleLightAnimation simpleLightAnimation = value5[num3];
					if (simpleLightAnimation == null)
					{
						value5.RemoveAt(num3);
					}
					else
					{
						simpleLightAnimation.enabled = visible;
					}
				}
			}
			if (relumod_decals.TryGetValue(tile, out var value6))
			{
				for (int num4 = value6.Count - 1; num4 >= 0; num4--)
				{
					DecalProjector decalProjector = value6[num4];
					if (decalProjector == null)
					{
						value6.RemoveAt(num4);
					}
					else
					{
						decalProjector.enabled = visible;
					}
				}
			}
			if (reflectionProbes.TryGetValue(tile, out var value7))
			{
				for (int num5 = value7.Count - 1; num5 >= 0; num5--)
				{
					ReflectionProbe reflectionProbe = value7[num5];
					if (reflectionProbe == null)
					{
						value7.RemoveAt(num5);
					}
					else
					{
						reflectionProbe.enabled = visible;
					}
				}
			}
			if (this.TileVisibilityChanged != null)
			{
				this.TileVisibilityChanged(tile, visible);
			}
		}

		public virtual void UpdateRendererLists()
		{
			foreach (Tile allTile in allTiles)
			{
				if (!tileRenderers.TryGetValue(allTile, out var value))
				{
					value = (tileRenderers[allTile] = new List<Renderer>());
				}
				Renderer[] componentsInChildren = allTile.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					if (IncludeDisabledComponents || (renderer.enabled && renderer.gameObject.activeInHierarchy))
					{
						value.Add(renderer);
					}
				}
				if (!lightSources.TryGetValue(allTile, out var value2))
				{
					value2 = (lightSources[allTile] = new List<Light>());
				}
				Light[] componentsInChildren2 = allTile.GetComponentsInChildren<Light>();
				foreach (Light light in componentsInChildren2)
				{
					if (IncludeDisabledComponents || (light.enabled && light.gameObject.activeInHierarchy))
					{
						value2.Add(light);
					}
				}
				if (!relumod_lightAnimations.TryGetValue(allTile, out var value3))
				{
					value3 = (relumod_lightAnimations[allTile] = new List<SimpleLightAnimation>());
				}
				SimpleLightAnimation[] componentsInChildren3 = allTile.GetComponentsInChildren<SimpleLightAnimation>();
				foreach (SimpleLightAnimation simpleLightAnimation in componentsInChildren3)
				{
					if (IncludeDisabledComponents || (simpleLightAnimation.enabled && simpleLightAnimation.gameObject.activeInHierarchy))
					{
						value3.Add(simpleLightAnimation);
					}
				}
				if (!relumod_decals.TryGetValue(allTile, out var value4))
				{
					value4 = (relumod_decals[allTile] = new List<DecalProjector>());
				}
				DecalProjector[] componentsInChildren4 = allTile.GetComponentsInChildren<DecalProjector>();
				foreach (DecalProjector decalProjector in componentsInChildren4)
				{
					if (IncludeDisabledComponents || (decalProjector.enabled && decalProjector.gameObject.activeInHierarchy))
					{
						value4.Add(decalProjector);
					}
				}
				if (!reflectionProbes.TryGetValue(allTile, out var value5))
				{
					value5 = (reflectionProbes[allTile] = new List<ReflectionProbe>());
				}
				ReflectionProbe[] componentsInChildren5 = allTile.GetComponentsInChildren<ReflectionProbe>();
				foreach (ReflectionProbe reflectionProbe in componentsInChildren5)
				{
					if (IncludeDisabledComponents || (reflectionProbe.enabled && reflectionProbe.gameObject.activeInHierarchy))
					{
						value5.Add(reflectionProbe);
					}
				}
			}
			foreach (Door allDoor in allDoors)
			{
				List<Renderer> list6 = new List<Renderer>();
				doorRenderers[allDoor] = list6;
				Renderer[] componentsInChildren = allDoor.GetComponentsInChildren<Renderer>(includeInactive: true);
				foreach (Renderer renderer2 in componentsInChildren)
				{
					if (IncludeDisabledComponents || (renderer2.enabled && renderer2.gameObject.activeInHierarchy))
					{
						list6.Add(renderer2);
					}
				}
			}
		}

		protected Tile FindCurrentTile()
		{
			if (dungeon == null)
			{
				return null;
			}
			foreach (Tile allTile in dungeon.AllTiles)
			{
				if (allTile.Bounds.Contains(targetTransform.position))
				{
					return allTile;
				}
			}
			return null;
		}

		public Tile relumod_FastFindCurrentTile(Transform? externalTransform = null, float levitation = 0f)
		{
			if (!Ready)
			{
				return null;
			}
			if (dungeon == null || relumod_tilesByXZ == null || relumod_tilesByXZ.Count == 0)
			{
				return null;
			}
			Vector3 position = (externalTransform ?? targetTransform).position;
			return relumod_FastFindCurrentTile(position, levitation);
		}

		public Tile relumod_FastFindCurrentTile(Vector3 position, float levitation = 0f)
		{
			if (!Ready)
			{
				return null;
			}
			if (dungeon == null || relumod_tilesByXZ == null || relumod_tilesByXZ.Count == 0)
			{
				return null;
			}
			int value = Mathf.FloorToInt((position.x - relumod_allBounds.min.x) / relumod_tilesByXWidth);
			int value2 = Mathf.FloorToInt((position.z - relumod_allBounds.min.z) / relumod_tilesByZWidth);
			value = Mathf.Clamp(value, 0, relumod_tilesByXZ.Count - 1);
			if (relumod_tilesByXZ[value].Count == 0)
			{
				return null;
			}
			value2 = Mathf.Clamp(value2, 0, relumod_tilesByXZ[value].Count - 1);
			foreach (Tile item in relumod_tilesByXZ[value][value2])
			{
				if (item.Bounds.Contains(position + Vector3.up * levitation))
				{
					return item;
				}
			}
			return null;
		}

		protected Tile SearchForNewCurrentTile()
		{
			if (tilesToSearch == null)
			{
				tilesToSearch = new Queue<Tile>();
			}
			if (searchedTiles == null)
			{
				searchedTiles = new List<Tile>();
			}
			foreach (Doorway usedDoorway in currentTile.UsedDoorways)
			{
				Tile tile = usedDoorway.ConnectedDoorway.Tile;
				if (!tilesToSearch.Contains(tile))
				{
					tilesToSearch.Enqueue(tile);
				}
			}
			while (tilesToSearch.Count > 0)
			{
				Tile tile2 = tilesToSearch.Dequeue();
				if (tile2.Bounds.Contains(targetTransform.position))
				{
					tilesToSearch.Clear();
					searchedTiles.Clear();
					return tile2;
				}
				searchedTiles.Add(tile2);
				foreach (Doorway usedDoorway2 in tile2.UsedDoorways)
				{
					Tile tile3 = usedDoorway2.ConnectedDoorway.Tile;
					if (!tilesToSearch.Contains(tile3) && !searchedTiles.Contains(tile3))
					{
						tilesToSearch.Enqueue(tile3);
					}
				}
			}
			searchedTiles.Clear();
			return null;
		}

		public void relumod_AddRenderersToTile(Tile tile, Transform nodeRoot)
		{
			if (!Ready || !tileRenderers.TryGetValue(tile, out var value))
			{
				return;
			}
			Renderer[] componentsInChildren = nodeRoot.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (!IsTileVisible(tile))
				{
					renderer.enabled = false;
				}
				value.Add(renderer);
			}
		}

		public void relumod_RemoveRenderersFromTile(Tile tile, Transform nodeRoot)
		{
			if (!Ready || !tileRenderers.TryGetValue(tile, out var value))
			{
				return;
			}
			Renderer[] componentsInChildren = nodeRoot.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer item in componentsInChildren)
			{
				if (value.Contains(item))
				{
					value.Remove(item);
				}
			}
		}

		public void relumod_Suppress()
		{
			if (Ready)
			{
				relumod_Suppressed = true;
				currentTile = null;
				RefreshVisibility();
			}
		}

		public void relumod_Unsuppress()
		{
			if (Ready)
			{
				relumod_Suppressed = false;
			}
		}

		public void OnTeleported()
		{
		}
	}
}
