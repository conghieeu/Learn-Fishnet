using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MMActorPrefabTable", menuName = "_Mimic/MMActorPrefabTable", order = 0)]
internal class MMActorPrefabTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		public string id;

		public GameObject prefab;
	}

	public Row[] rows = Array.Empty<Row>();

	public GameObject Get(string id)
	{
		Row row = rows.Where((Row rows) => rows.id == id).FirstOrDefault();
		if (row == null)
		{
			Logger.RError("[MMActorPrefabTable] Get(" + id + ") failed");
			return null;
		}
		return row.prefab;
	}
}
