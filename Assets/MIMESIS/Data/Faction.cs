using System.Collections.Generic;

public class Faction
{
	public int id;

	public int category;

	public List<int> ally_faction_id = new List<int>();

	public List<int> neutral_faction_id = new List<int>();

	public List<int> enemy_faction_id = new List<int>();
}
