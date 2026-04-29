using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MMDecalTable", menuName = "_Mimic/MMDecalTable", order = 0)]
public class MMDecalTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		[FormerlySerializedAs("name")]
		public string id;

		public GameObject prefab;

		public GameObject Instantiate(Transform parent, Vector3 rotation)
		{
			return UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.Euler(rotation), parent);
		}

		public GameObject Instantiate(Vector3 position)
		{
			return UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
		}
	}

	public string[] prefabFolders = new string[1] { "Assets/_mimic/art/BG/BG_Decal/prefabs" };

	public Row[] rows;
}
