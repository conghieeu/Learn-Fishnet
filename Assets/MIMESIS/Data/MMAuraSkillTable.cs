using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MMAuraSkillTable", menuName = "_Mimic/MMAuraSkillTable", order = 0)]
public class MMAuraSkillTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		public string id = string.Empty;

		public GameObject? prefab;
	}

	[SerializeField]
	private string[] prefabFolders = new string[1] { "Assets/_mimic/prefabs/AuraSkill" };

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
