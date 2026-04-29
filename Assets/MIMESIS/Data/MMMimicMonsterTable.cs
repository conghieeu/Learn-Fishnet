using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MMMimicMonsterTable", menuName = "_Mimic/MMMimicMonsterTable", order = 0)]
public class MMMimicMonsterTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		public int monsterMasterID;

		public bool muteLocalPlayerVoice;

		[Range(0f, 1f)]
		public float mimickingPlayerChangeProb;

		public MinMaxFloatRange InitializeInterval;

		public MinMaxFloatRange interval;

		public MinMaxFloatRange deathMatchInterval;

		public Row()
		{
			monsterMasterID = -1;
			muteLocalPlayerVoice = true;
			mimickingPlayerChangeProb = 1f;
			InitializeInterval = new MinMaxFloatRange(4f, 7f, 0f, 60f);
			interval = new MinMaxFloatRange(2f, 8f, 0f, 60f);
			deathMatchInterval = new MinMaxFloatRange(2f, 8f, 0f, 60f);
		}
	}

	[SerializeField]
	private List<Row> rows = new List<Row>();

	public bool TryGetRow(int monsterMasterID, out Row? row)
	{
		row = rows.FirstOrDefault((Row r) => r.monsterMasterID == monsterMasterID);
		return row != null;
	}
}
