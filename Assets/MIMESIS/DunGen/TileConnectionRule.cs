using System;

namespace DunGen
{
	public sealed class TileConnectionRule
	{
		public enum ConnectionResult
		{
			Allow = 0,
			Deny = 1,
			Passthrough = 2
		}

		public delegate ConnectionResult CanTilesConnectDelegate(Tile previousTile, Tile nextTile, Doorway previousDoorway, Doorway nextDoorway);

		public delegate ConnectionResult TileConnectionDelegate(ProposedConnection connection);

		public int Priority;

		[Obsolete("Use ConnectionDelegate instead")]
		public CanTilesConnectDelegate Delegate;

		public TileConnectionDelegate ConnectionDelegate;

		[Obsolete("Use the constructor that takes a delegate of type 'TileConnectionDelegate' instead")]
		public TileConnectionRule(CanTilesConnectDelegate connectionDelegate, int priority = 0)
		{
			Delegate = connectionDelegate;
			Priority = priority;
		}

		public TileConnectionRule(TileConnectionDelegate connectionDelegate, int priority = 0)
		{
			ConnectionDelegate = connectionDelegate;
			Priority = priority;
		}
	}
}
