using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MMColorTable", menuName = "_Mimic/MMColorTable", order = 0)]
public class MMColorTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		[FormerlySerializedAs("name")]
		public string id;

		public Color color;
	}

	[SerializeField]
	private Row[] rows = Array.Empty<Row>();

	public Row? FindRow(string rowId)
	{
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (row != null && row.id == rowId)
			{
				return row;
			}
		}
		return null;
	}

	public Color GetColor(string rowId)
	{
		Row row = FindRow(rowId);
		if (row != null)
		{
			return row.color;
		}
		Debug.LogWarning("MMColorTable: Color with id '" + rowId + "' not found.");
		return Color.white;
	}
}
