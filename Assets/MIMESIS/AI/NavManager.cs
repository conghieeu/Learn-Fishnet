using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=5077601729")]
public class NavManager : MonoBehaviour
{
	private NavMeshPath? navPath;

	[SerializeField]
	private GameObject prefab_navMeshSurface_bigmap;

	[SerializeField]
	private GameObject prefab_navMeshSurface_tramonly;

	private NavMeshSurface current_navMeshSurface;

	public bool Build(bool isTramOnly = false)
	{
		_ = Time.realtimeSinceStartup;
		NavMesh.RemoveAllNavMeshData();
		if (current_navMeshSurface != null)
		{
			Object.DestroyImmediate(current_navMeshSurface.gameObject);
			current_navMeshSurface = null;
		}
		GameObject gameObject = ((!isTramOnly) ? Object.Instantiate(prefab_navMeshSurface_bigmap, base.transform) : Object.Instantiate(prefab_navMeshSurface_tramonly, base.transform));
		current_navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
		current_navMeshSurface.BuildNavMesh();
		_ = Time.realtimeSinceStartup;
		return true;
	}

	public bool Rebuild()
	{
		current_navMeshSurface?.UpdateNavMesh(current_navMeshSurface.navMeshData);
		return true;
	}

	public void Clear()
	{
		NavMesh.RemoveAllNavMeshData();
	}

	public PathFindResult GetRoute(Vector3 _start, Vector3 _end, bool needToSplit = false)
	{
		bool flag = true;
		bool flag2 = true;
		Vector3 vector = GetNearestPointOnNavMesh(_start, 0.3f);
		if (vector == Vector3.zero)
		{
			vector = _start;
			flag = false;
		}
		Vector3 vector2 = GetNearestPointOnNavMesh(_end, 0.3f);
		if (vector2 == Vector3.zero)
		{
			vector2 = _end;
			flag2 = false;
		}
		PathFindResult pathFindResult = new PathFindResult();
		if (!flag && !flag2)
		{
			pathFindResult.Success = false;
			return pathFindResult;
		}
		if (Vector3.Distance(vector, vector2) < 0.1f)
		{
			pathFindResult.Success = true;
			return pathFindResult;
		}
		if (navPath == null)
		{
			navPath = new NavMeshPath();
		}
		if (!NavMesh.CalculatePath(vector, vector2, -1, navPath))
		{
			(bool, List<Vector3>) twoPointBumpPath = GetTwoPointBumpPath(vector, vector2);
			if (twoPointBumpPath.Item1)
			{
				pathFindResult.PathPoints = twoPointBumpPath.Item2;
				pathFindResult.Success = true;
				return pathFindResult;
			}
		}
		List<Vector3> list = navPath.corners.ToList();
		switch (list.Count)
		{
		case 0:
			pathFindResult.Success = false;
			return pathFindResult;
		case 1:
			pathFindResult.Success = false;
			return pathFindResult;
		case 2:
		{
			(bool, List<Vector3>) twoPointBumpPath2 = GetTwoPointBumpPath(list[0], list[1]);
			if (twoPointBumpPath2.Item1)
			{
				pathFindResult.PathPoints = twoPointBumpPath2.Item2;
				pathFindResult.Success = true;
				return pathFindResult;
			}
			break;
		}
		}
		if (Vector3.Distance(list[list.Count - 1], vector2) > 0.4f)
		{
			pathFindResult.Success = false;
			return pathFindResult;
		}
		if (needToSplit)
		{
			pathFindResult.PathPoints = GenerateDetailedPath(vector2, 1f);
		}
		else
		{
			pathFindResult.PathPoints = navPath.corners.ToList();
		}
		if (pathFindResult.PathPoints.Count == 0)
		{
			pathFindResult.Success = false;
			return pathFindResult;
		}
		pathFindResult.Length = GetPathLength(pathFindResult.PathPoints);
		pathFindResult.Success = true;
		return pathFindResult;
	}

	private float GetPathLength(List<Vector3> paths)
	{
		float num = 0f;
		for (int i = 1; i < paths.Count; i++)
		{
			num += Vector3.Distance(paths[i - 1], paths[i]);
		}
		return num;
	}

	private (bool, List<Vector3>) GetTwoPointBumpPath(Vector3 start, Vector3 end)
	{
		if (Vector3.Distance(start, end) < 1.4f)
		{
			if (RayCast(start, end, ignore: false) == Vector3.zero)
			{
				return (false, new List<Vector3>());
			}
			return (true, new List<Vector3> { start, end });
		}
		return (false, new List<Vector3>());
	}

	private List<Vector3> GenerateDetailedPath(Vector3 end, float segmentInterval)
	{
		List<Vector3> list = new List<Vector3>();
		if (navPath == null)
		{
			return list;
		}
		List<Vector3> list2 = navPath.corners.ToList();
		for (int i = 0; i < list2.Count - 1; i++)
		{
			Vector3 vector = list2[i];
			Vector3 vector2 = list2[i + 1];
			list.Add(vector);
			float num = Vector3.Distance(vector, vector2);
			if (!(num > 2f))
			{
				continue;
			}
			Vector3 normalized = (vector2 - vector).normalized;
			for (float num2 = segmentInterval; num2 < num; num2 += segmentInterval)
			{
				Vector3 vector3 = vector + normalized * num2;
				if (NavMesh.SamplePosition(vector3, out var hit, 2f, -1))
				{
					list.Add(hit.position);
				}
				else
				{
					list.Add(vector3);
				}
			}
		}
		list.Add(list2[list2.Count - 1]);
		return list;
	}

	public float GetRouteLength(Vector3 from, Vector3 to)
	{
		PathFindResult route = GetRoute(from, to);
		if (!route.Success)
		{
			return -1f;
		}
		float num = 0f;
		for (int i = 1; i < route.PathPoints.Count; i++)
		{
			num += Vector3.Distance(route.PathPoints[i - 1], route.PathPoints[i]);
		}
		float num2 = 0.001f;
		if (0f - num2 < num && num < num2)
		{
			num = 0f;
		}
		return num;
	}

	public bool IsHitWall(Vector3 startPos, Vector3 endPos, out Vector3 hitPos, float turnAngleDegThreshold = 30f, float minSegmentDistance = 1f, float sampleRadius = 1f)
	{
		hitPos = Vector3.zero;
		if (!NavMesh.SamplePosition(startPos, out var hit, sampleRadius, -1) || !NavMesh.SamplePosition(endPos, out var hit2, sampleRadius, -1))
		{
			hitPos = startPos;
			return true;
		}
		if (navPath == null)
		{
			navPath = new NavMeshPath();
		}
		if (!NavMesh.CalculatePath(hit.position, hit2.position, -1, navPath) || navPath.status != NavMeshPathStatus.PathComplete)
		{
			hitPos = hit.position;
			return true;
		}
		Vector3[] corners = navPath.corners;
		if (corners == null || corners.Length <= 2)
		{
			hitPos = hit2.position;
			return false;
		}
		for (int i = 1; i < corners.Length - 1; i++)
		{
			Vector3 vector = corners[i - 1];
			Vector3 vector2 = corners[i];
			Vector3 vector3 = corners[i + 1];
			Vector3 lhs = vector2 - vector;
			Vector3 rhs = vector3 - vector2;
			lhs.y = 0f;
			rhs.y = 0f;
			if (!(lhs.sqrMagnitude < 1E-06f) && !(rhs.sqrMagnitude < 1E-06f))
			{
				float magnitude = lhs.magnitude;
				float magnitude2 = rhs.magnitude;
				lhs.Normalize();
				rhs.Normalize();
				if (Mathf.Acos(Mathf.Clamp(Vector3.Dot(lhs, rhs), -1f, 1f)) * 57.29578f >= turnAngleDegThreshold && (magnitude >= minSegmentDistance || magnitude2 >= minSegmentDistance))
				{
					hitPos = vector2;
					return true;
				}
			}
		}
		hitPos = hit2.position;
		return false;
	}

	public Vector3 GetNearestPointOnNavMesh(Vector3 position, float maxDistance)
	{
		if (NavMesh.SamplePosition(position, out var hit, 0.5f, -1))
		{
			return hit.position;
		}
		if (NavMesh.FindClosestEdge(position, out var hit2, -1) && Vector3.Distance(hit2.position, position) <= maxDistance)
		{
			return hit2.position;
		}
		if (NavMesh.SamplePosition(position, out var hit3, maxDistance, -1))
		{
			return hit3.position;
		}
		return Vector3.zero;
	}

	public Vector3 RayCast(Vector3 start, Vector3 end, bool ignore = true)
	{
		if (NavMesh.Raycast(start, end, out var hit, -1))
		{
			return hit.position;
		}
		if (!ignore)
		{
			return Vector3.zero;
		}
		return end;
	}

	public Vector3 RayCastWithPhysics(Vector3 start, Vector3 end)
	{
		float magnitude = (end - start).magnitude;
		Ray ray = new Ray(start, end - start);
		Vector3 position = start;
		if (Physics.Raycast(ray, out var hitInfo, magnitude, LayerMask.GetMask("Default")))
		{
			position = hitInfo.point;
		}
		Vector3 nearestPointOnNavMesh = GetNearestPointOnNavMesh(position, magnitude);
		if (nearestPointOnNavMesh == Vector3.zero)
		{
			return start;
		}
		return nearestPointOnNavMesh;
	}
}
