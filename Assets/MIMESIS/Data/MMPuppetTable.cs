using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MMPuppetTable", menuName = "_Mimic/MMPuppetTable", order = 0)]
public class MMPuppetTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		public string id = string.Empty;

		public GameObject? prefab;
	}

	public string[] prefabFolders = new string[1] { "Assets/_mimic/prefabs/Actor/Puppet" };

	[SerializeField]
	private List<Row> rows = new List<Row>();

	public Row? FindRow(string id)
	{
		return rows.Find((Row p) => p.id == id);
	}
}
