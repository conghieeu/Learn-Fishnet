using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MMSpriteTable", menuName = "_Mimic/MMSpriteTable", order = 0)]
public class MMSpriteTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		[FormerlySerializedAs("name")]
		public string id;

		public Sprite sprite;
	}

	[Header("Collect Rows 메뉴 실행시 Texture2D 애셋을 수집할 폴더들")]
	public string[] textureFolders = new string[1] { "Assets/_mimic/art/UI/Icons" };

	[SerializeField]
	private Sprite defaultSprite;

	[SerializeField]
	private Row[] rows = Array.Empty<Row>();

	private Row? FindRow(string rowId)
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

	public Sprite? GetSprite(string spriteId)
	{
		Row row = FindRow(spriteId);
		if (row == null)
		{
			return defaultSprite;
		}
		return row.sprite;
	}
}
