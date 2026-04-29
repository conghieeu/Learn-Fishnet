using System.Collections.Generic;
using UnityEngine;

namespace DunGen
{
	[AddComponentMenu("DunGen/Random Props/Random Prefab")]
	public class RandomPrefab : RandomProp
	{
		[AcceptGameObjectTypes(GameObjectFilter.Asset)]
		public GameObjectChanceTable Props = new GameObjectChanceTable();

		public bool ZeroPosition = true;

		public bool ZeroRotation = true;

		public override void Process(RandomStream randomStream, Tile tile, ref List<GameObject> spawnedObjects)
		{
			if (Props.Weights.Count <= 0)
			{
				return;
			}
			GameObjectChance random = Props.GetRandom(randomStream, tile.Placement.IsOnMainPath, tile.Placement.NormalizedDepth, null, allowImmediateRepeats: true, removeFromTable: true, allowNullSelection: true);
			if (random != null && !(random.Value == null))
			{
				GameObject value = random.Value;
				GameObject gameObject = Object.Instantiate(value);
				gameObject.transform.parent = base.transform;
				spawnedObjects.Add(gameObject);
				if (ZeroPosition)
				{
					gameObject.transform.localPosition = Vector3.zero;
				}
				else
				{
					gameObject.transform.localPosition = value.transform.localPosition;
				}
				if (ZeroRotation)
				{
					gameObject.transform.localRotation = Quaternion.identity;
				}
				else
				{
					gameObject.transform.localRotation = value.transform.localRotation;
				}
			}
		}
	}
}
