using UnityEngine;

public static class LevelObjectUtility
{
	public static bool TryPick4(TilesByXZ<LevelObject> levelObjectTiles, Vector3 from, Vector3 dir, float maxDistance, out LevelObject? pickedLevelObject)
	{
		pickedLevelObject = null;
		float bestSqrDistance = float.MaxValue;
		bool isBlocker = false;
		LevelObject _pickedLevelObject = null;
		levelObjectTiles.ForeachItems(from, delegate(LevelObject lo)
		{
			if (!(lo == null) && !(lo.gameObject == null) && lo.gameObject.activeSelf)
			{
				float num = lo.maxInteractionDistance * lo.maxInteractionDistance;
				LevelObjectBoundElement[] bounds = lo.bounds;
				foreach (LevelObjectBoundElement levelObjectBoundElement in bounds)
				{
					if (!(levelObjectBoundElement == null) && (levelObjectBoundElement.pickable || levelObjectBoundElement.blockable))
					{
						Vector3 localFrom = levelObjectBoundElement.transform.InverseTransformPoint(from);
						Vector3 normalized = levelObjectBoundElement.transform.InverseTransformDirection(dir).normalized;
						Bounds bound = levelObjectBoundElement.bound;
						float t = 0f;
						float num2 = float.MaxValue;
						bool flag = true;
						if (levelObjectBoundElement.onlyUseFront)
						{
							if (IntersectAABB_FrontOnly(bound, localFrom, normalized, out t, out var _))
							{
								float sqrMagnitude = (dir * t).sqrMagnitude;
								if (sqrMagnitude <= num && sqrMagnitude < bestSqrDistance)
								{
									bestSqrDistance = sqrMagnitude;
									_pickedLevelObject = lo;
									isBlocker = levelObjectBoundElement.blockable;
								}
							}
						}
						else
						{
							for (int j = 0; j < 3; j++)
							{
								float num3 = normalized[j];
								if (Mathf.Abs(num3) < 1E-06f)
								{
									if (localFrom[j] < bound.min[j] || localFrom[j] > bound.max[j])
									{
										flag = false;
										break;
									}
								}
								else
								{
									float num4 = 1f / num3;
									float num5 = (bound.min[j] - localFrom[j]) * num4;
									float num6 = (bound.max[j] - localFrom[j]) * num4;
									if (num5 > num6)
									{
										float num7 = num5;
										num5 = num6;
										num6 = num7;
									}
									t = Mathf.Max(t, num5);
									num2 = Mathf.Min(num2, num6);
									if (num2 < t)
									{
										flag = false;
										break;
									}
								}
							}
							if (flag && !(t < 0f))
							{
								float sqrMagnitude2 = (dir * t).sqrMagnitude;
								if (sqrMagnitude2 <= num && sqrMagnitude2 < bestSqrDistance)
								{
									bestSqrDistance = sqrMagnitude2;
									_pickedLevelObject = lo;
									isBlocker = levelObjectBoundElement.blockable;
								}
							}
						}
					}
				}
			}
		});
		if (_pickedLevelObject != null)
		{
			int layerMask = 1 << LayerMask.NameToLayer("PickingBlocker");
			float maxDistance2 = Mathf.Sqrt(bestSqrDistance);
			if (Physics.Raycast(from, dir, out var _, maxDistance2, layerMask))
			{
				pickedLevelObject = null;
				return false;
			}
			pickedLevelObject = _pickedLevelObject;
		}
		if (isBlocker)
		{
			pickedLevelObject = null;
		}
		return pickedLevelObject != null;
	}

	private static bool IntersectAABB_FrontOnly(Bounds bound, Vector3 localFrom, Vector3 localDir, out float t, out Vector3 normal)
	{
		t = 0f;
		normal = Vector3.zero;
		float num = 0f;
		float num2 = float.MaxValue;
		int num3 = -1;
		for (int i = 0; i < 3; i++)
		{
			float num4 = localFrom[i];
			float num5 = localDir[i];
			float num6 = bound.min[i];
			float num7 = bound.max[i];
			if (Mathf.Abs(num5) < 1E-06f)
			{
				if (num4 < num6 || num4 > num7)
				{
					return false;
				}
				continue;
			}
			float num8 = 1f / num5;
			float num9 = (num6 - num4) * num8;
			float num10 = (num7 - num4) * num8;
			if (num9 > num10)
			{
				float num11 = num9;
				num9 = num10;
				num10 = num11;
			}
			if (num9 > num)
			{
				num = num9;
				num3 = i;
			}
			num2 = Mathf.Min(num2, num10);
			if (num2 < num)
			{
				return false;
			}
		}
		if (num < 0f)
		{
			return false;
		}
		t = num;
		switch (num3)
		{
		case 0:
			normal = new Vector3((localDir.x > 0f) ? (-1f) : 1f, 0f, 0f);
			break;
		case 1:
			normal = new Vector3(0f, (localDir.y > 0f) ? (-1f) : 1f, 0f);
			break;
		case 2:
			normal = new Vector3(0f, 0f, (localDir.z > 0f) ? (-1f) : 1f);
			break;
		}
		if (Vector3.Dot(normal, localDir) >= 0f)
		{
			return false;
		}
		if (Vector3.Dot(normal, Vector3.forward) < 0.999f)
		{
			return false;
		}
		return true;
	}
}
