using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MMGameTipTable", menuName = "_Mimic/MMGameTipTable", order = 0)]
public class MMGameTipTable : ScriptableObject
{
	[Serializable]
	public class Mapper
	{
		public long masterId;

		public string tableId;
	}

	[Serializable]
	public class Row
	{
		public string id;

		public List<string> formattedTips;
	}

	public Mapper[] mappers;

	public Row[] rows;

	public bool TryGetFormattedTips(long itemMasterId, out List<string> formattedTips)
	{
		Mapper foundMapper = mappers.FirstOrDefault((Mapper m) => m.masterId == itemMasterId);
		if (foundMapper != null)
		{
			Row row = rows.FirstOrDefault((Row r) => r.id == foundMapper.tableId);
			if (row != null)
			{
				formattedTips = row.formattedTips;
				return true;
			}
		}
		formattedTips = null;
		return false;
	}
}
