using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MMScreenEffectTable", menuName = "_Mimic/MMScreenEffectTable", order = 0)]
internal class MMScreenEffectTable : ScriptableObject
{
	[Serializable]
	public class Row
	{
		[FormerlySerializedAs("name")]
		public string id;

		public GameObject prefab;

		internal GameObject Instantiate(Transform parent)
		{
			return UnityEngine.Object.Instantiate(prefab, parent);
		}

		internal GameObject Instantiate(Vector3 position)
		{
			return UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
		}
	}

	public Row[] rows;
}
