using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MMVfxTable", menuName = "_Mimic/MMVfxTable", order = 0)]
public class MMVfxTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		[FormerlySerializedAs("name")]
		public string id;

		public GameObject prefab;

		public GameObject Instantiate(Transform parent)
		{
			return UnityEngine.Object.Instantiate(prefab, parent);
		}

		public GameObject Instantiate(Vector3 position)
		{
			return UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
		}
	}

	public string[] prefabFolders = new string[1] { "Assets/_mimic/prefabs/VFX" };

	public Row[] rows;
}
