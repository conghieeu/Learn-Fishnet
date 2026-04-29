using System;
using System.Collections.Generic;
using DunGen.Graph;
using UnityEngine;

[CreateAssetMenu(fileName = "DungenFlowTable", menuName = "_Mimic/DungenFlowTable")]
public class DungenFlowTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		public string id = string.Empty;

		public DungeonFlow flow;
	}

	public string[] prefabFolders = new string[1] { "Assets/_testdata/testDungen" };

	[SerializeField]
	private List<Row> rows;

	public bool TryGet(string id, out Row? row)
	{
		foreach (Row row2 in rows)
		{
			if (row2.id == id)
			{
				row = row2;
				return true;
			}
		}
		row = null;
		return false;
	}
}
