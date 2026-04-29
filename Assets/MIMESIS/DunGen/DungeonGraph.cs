using System.Collections.Generic;
using System.Linq;

namespace DunGen
{
	public class DungeonGraph
	{
		public readonly List<DungeonGraphNode> Nodes = new List<DungeonGraphNode>();

		public readonly List<DungeonGraphConnection> Connections = new List<DungeonGraphConnection>();

		public DungeonGraph(Dungeon dungeon, IEnumerable<Tile> additionalTiles)
		{
			Dictionary<Tile, DungeonGraphNode> dictionary = new Dictionary<Tile, DungeonGraphNode>();
			foreach (Tile item3 in dungeon.AllTiles.Concat(additionalTiles))
			{
				DungeonGraphNode item = (dictionary[item3] = new DungeonGraphNode(item3));
				Nodes.Add(item);
			}
			foreach (DoorwayConnection connection in dungeon.Connections)
			{
				DungeonGraphConnection item2 = new DungeonGraphConnection(dictionary[connection.A.Tile], dictionary[connection.B.Tile], connection.A, connection.B);
				Connections.Add(item2);
			}
		}
	}
}
