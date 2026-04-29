using UnityEngine;

namespace DunGen
{
	public class DungeonAttachmentSettings
	{
		public Doorway AttachmentDoorway { get; private set; }

		public Tile AttachmentTile { get; private set; }

		public TileProxy TileProxy { get; private set; }

		public DungeonAttachmentSettings(Doorway attachmentDoorway)
		{
			AttachmentDoorway = attachmentDoorway;
			if (AttachmentDoorway.Tile.UsedDoorways.Contains(AttachmentDoorway))
			{
				Debug.LogError("Cannot attach dungeon to doorway '" + attachmentDoorway.name + "' as it is already in use");
			}
		}

		public DungeonAttachmentSettings(Tile attachmentTile)
		{
			AttachmentTile = attachmentTile;
		}

		public TileProxy GenerateAttachmentProxy(bool ignoreSpriteRendererBounds, Vector3 upVector, RandomStream randomStream)
		{
			if (AttachmentTile != null)
			{
				if (AttachmentTile.Prefab == null)
				{
					PrepareManuallyPlacedTile(ignoreSpriteRendererBounds, upVector, randomStream);
				}
				TileProxy = new TileProxy(AttachmentTile.Prefab, ignoreSpriteRendererBounds, upVector, (Doorway doorway, int index) => AttachmentTile.UnusedDoorways.Contains(AttachmentTile.AllDoorways[index]));
				TileProxy.Placement.Position = AttachmentTile.transform.localPosition;
				TileProxy.Placement.Rotation = AttachmentTile.transform.localRotation;
			}
			else if (AttachmentDoorway != null)
			{
				Tile attachmentTile = AttachmentDoorway.Tile;
				TileProxy = new TileProxy(AttachmentDoorway.Tile.Prefab, ignoreSpriteRendererBounds, upVector, (Doorway doorway, int index) => index == attachmentTile.AllDoorways.IndexOf(AttachmentDoorway));
				TileProxy.Placement.Position = AttachmentDoorway.Tile.transform.localPosition;
				TileProxy.Placement.Rotation = AttachmentDoorway.Tile.transform.localRotation;
			}
			return TileProxy;
		}

		private void PrepareManuallyPlacedTile(bool ignoreSpriteRendererBounds, Vector3 upVector, RandomStream randomStream)
		{
			AttachmentTile.Prefab = AttachmentTile.gameObject;
			Doorway[] componentsInChildren = AttachmentTile.GetComponentsInChildren<Doorway>();
			foreach (Doorway doorway in componentsInChildren)
			{
				doorway.Tile = AttachmentTile;
				AttachmentTile.AllDoorways.Add(doorway);
				AttachmentTile.UnusedDoorways.Add(doorway);
				doorway.ProcessDoorwayObjects(isDoorwayInUse: false, randomStream);
			}
			Bounds bounds = ((!AttachmentTile.OverrideAutomaticTileBounds) ? UnityUtil.CalculateProxyBounds(AttachmentTile.gameObject, ignoreSpriteRendererBounds, upVector) : AttachmentTile.TileBoundsOverride);
			AttachmentTile.Placement.LocalBounds = UnityUtil.CondenseBounds(bounds, AttachmentTile.AllDoorways);
		}

		public Tile GetAttachmentTile()
		{
			Tile result = null;
			if (AttachmentTile != null)
			{
				result = AttachmentTile;
			}
			else if (AttachmentDoorway != null)
			{
				result = AttachmentDoorway.Tile;
			}
			return result;
		}
	}
}
