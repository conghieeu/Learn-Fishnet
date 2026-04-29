using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DunGen.Tags;
using UnityEngine;

namespace DunGen
{
	public sealed class TileProxy
	{
		private List<DoorwayProxy> doorways = new List<DoorwayProxy>();

		public GameObject Prefab { get; private set; }

		public Tile PrefabTile { get; private set; }

		public TilePlacementData Placement { get; internal set; }

		public List<DoorwayProxy> Entrances { get; private set; }

		public List<DoorwayProxy> Exits { get; private set; }

		public ReadOnlyCollection<DoorwayProxy> Doorways { get; private set; }

		public IEnumerable<DoorwayProxy> UsedDoorways => doorways.Where((DoorwayProxy d) => d.Used);

		public IEnumerable<DoorwayProxy> UnusedDoorways => doorways.Where((DoorwayProxy d) => !d.Used);

		public TagContainer Tags { get; private set; }

		public TileProxy(TileProxy existingTile)
		{
			Prefab = existingTile.Prefab;
			PrefabTile = existingTile.PrefabTile;
			Placement = new TilePlacementData(existingTile.Placement);
			Tags = new TagContainer(existingTile.Tags);
			Doorways = new ReadOnlyCollection<DoorwayProxy>(doorways);
			Entrances = new List<DoorwayProxy>();
			Exits = new List<DoorwayProxy>();
			foreach (DoorwayProxy doorway in existingTile.doorways)
			{
				DoorwayProxy item = new DoorwayProxy(this, doorway);
				doorways.Add(item);
				if (existingTile.Entrances.Contains(doorway))
				{
					Entrances.Add(item);
				}
				if (existingTile.Exits.Contains(doorway))
				{
					Exits.Add(item);
				}
			}
		}

		public TileProxy(GameObject prefab, bool ignoreSpriteRendererBounds, Vector3 upVector, Func<Doorway, int, bool> allowedDoorwayPredicate = null)
		{
			prefab.transform.localPosition = Vector3.zero;
			prefab.transform.localRotation = Quaternion.identity;
			Prefab = prefab;
			PrefabTile = prefab.GetComponent<Tile>();
			if (PrefabTile == null)
			{
				PrefabTile = prefab.AddComponent<Tile>();
			}
			Placement = new TilePlacementData();
			Tags = new TagContainer(PrefabTile.Tags);
			Doorways = new ReadOnlyCollection<DoorwayProxy>(doorways);
			Entrances = new List<DoorwayProxy>();
			Exits = new List<DoorwayProxy>();
			Doorway[] componentsInChildren = prefab.GetComponentsInChildren<Doorway>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Doorway doorway = componentsInChildren[i];
				Vector3 position = doorway.transform.position;
				Quaternion rotation = doorway.transform.rotation;
				DoorwayProxy doorwayProxy = new DoorwayProxy(this, i, doorway, position, rotation);
				doorways.Add(doorwayProxy);
				if (PrefabTile.Entrances.Contains(doorway))
				{
					Entrances.Add(doorwayProxy);
				}
				if (PrefabTile.Exits.Contains(doorway))
				{
					Exits.Add(doorwayProxy);
				}
				if (allowedDoorwayPredicate != null && !allowedDoorwayPredicate(doorway, i))
				{
					doorwayProxy.IsDisabled = true;
				}
			}
			Bounds bounds = ((!(PrefabTile != null) || !PrefabTile.OverrideAutomaticTileBounds) ? UnityUtil.CalculateProxyBounds(Prefab, ignoreSpriteRendererBounds, upVector) : PrefabTile.TileBoundsOverride);
			if (bounds.size.x <= 0f || bounds.size.y <= 0f || bounds.size.z <= 0f)
			{
				Debug.LogError($"Tile prefab '{prefab}' has automatic bounds that are zero or negative in size. The bounding volume for this tile will need to be manually defined.", prefab);
			}
			Placement.LocalBounds = UnityUtil.CondenseBounds(bounds, Prefab.GetComponentsInChildren<Doorway>());
		}

		public void PositionBySocket(DoorwayProxy myDoorway, DoorwayProxy otherDoorway)
		{
			Quaternion quaternion = Quaternion.LookRotation(-otherDoorway.Forward, otherDoorway.Up);
			Placement.Rotation = quaternion * Quaternion.Inverse(Quaternion.Inverse(Placement.Rotation) * (Placement.Rotation * myDoorway.LocalRotation));
			Vector3 position = otherDoorway.Position;
			Placement.Position = position - (myDoorway.Position - Placement.Position);
		}

		public bool IsOverlapping(TileProxy other, float maxOverlap)
		{
			return UnityUtil.AreBoundsOverlapping(Placement.Bounds, other.Placement.Bounds, maxOverlap);
		}

		public bool IsOverlappingOrOverhanging(TileProxy other, AxisDirection upDirection, float maxOverlap)
		{
			return UnityUtil.AreBoundsOverlappingOrOverhanging(Placement.Bounds, other.Placement.Bounds, upDirection, maxOverlap);
		}
	}
}
