using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=5077601729")]
public class PathFindManager : MonoBehaviour
{
	private AstarPath? astarPathObject;

	[SerializeField]
	private GameObject prefab_recastConfig;

	private RecastGraph? recastGraph;

	private Seeker? seeker;

	private SimpleSmoothModifier? smoothModifier;

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] PathFindManager.Awake ->");
		InitializeAstarPath();
		Logger.RLog("[AwakeLogs] PathFindManager.Awake <-");
	}

	private void InitializeAstarPath()
	{
		if ((Object)(object)astarPathObject != null)
		{
			FindRecastGraph();
			return;
		}
		GameObject gameObject = Object.Instantiate(prefab_recastConfig, base.transform);
		astarPathObject = gameObject.GetComponent<AstarPath>();
		seeker = gameObject.GetComponent<Seeker>();
	}

	public void EditorMode_InitializeAstarPath(AstarPath _astarPathObject)
	{
		astarPathObject = _astarPathObject;
		FindRecastGraph();
	}

	private void FindRecastGraph()
	{
		if ((Object)(object)AstarPath.active == null)
		{
			return;
		}
		NavGraph[] graphs = AstarPath.active.data.graphs;
		for (int i = 0; i < graphs.Length; i++)
		{
			if (graphs[i] is RecastGraph recastGraph)
			{
				this.recastGraph = recastGraph;
				break;
			}
		}
	}

	public bool Build()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if ((Object)(object)astarPathObject == null)
		{
			Logger.RError("[PathFindManager] AstarPath not initialized!");
			return false;
		}
		recastGraph = astarPathObject.data.recastGraph;
		if (recastGraph == null)
		{
			Logger.RError("[PathFindManager] RecastGraph not found!");
			return false;
		}
		recastGraph.SnapBoundsToScene();
		astarPathObject.Scan();
		float realtimeSinceStartup2 = Time.realtimeSinceStartup;
		Logger.RError($"[PathFindManager] Build completed in {realtimeSinceStartup2 - realtimeSinceStartup}s");
		return true;
	}

	public bool Rebuild()
	{
		if ((Object)(object)AstarPath.active == null)
		{
			return false;
		}
		if (recastGraph != null)
		{
			AstarPath.active.UpdateGraphs(recastGraph.bounds);
		}
		else
		{
			AstarPath.active.Scan();
		}
		return true;
	}

	public void Clear()
	{
		if ((Object)(object)AstarPath.active != null)
		{
			_ = recastGraph;
		}
	}

	public PathFindResult GetRoute(Vector3 _start, Vector3 _end)
	{
		PathFindResult pathFindResult = new PathFindResult();
		if ((Object)(object)AstarPath.active == null || (Object)(object)seeker == null)
		{
			pathFindResult.Success = false;
			return pathFindResult;
		}
		Vector3 vector = GetNearestPointOnNavMesh(_start, 0.5f);
		Vector3 vector2 = GetNearestPointOnNavMesh(_end, 0.5f);
		if (vector == Vector3.zero && vector2 == Vector3.zero)
		{
			pathFindResult.Success = false;
			return pathFindResult;
		}
		if (vector == Vector3.zero)
		{
			vector = _start;
		}
		if (vector2 == Vector3.zero)
		{
			vector2 = _end;
		}
		if (Vector3.Distance(vector, vector2) < 0.3f)
		{
			pathFindResult.Success = true;
			pathFindResult.Length = 0f;
			pathFindResult.PathPoints = new List<Vector3>();
			return pathFindResult;
		}
		Path path = seeker.StartPath(vector, vector2, null);
		path.BlockUntilCalculated();
		if (path.error || path.vectorPath == null || path.vectorPath.Count < 2)
		{
			pathFindResult.Success = false;
			return pathFindResult;
		}
		pathFindResult.Success = true;
		pathFindResult.PathPoints = path.vectorPath;
		pathFindResult.Length = path.GetTotalLength();
		return pathFindResult;
	}

	private List<Vector3> GenerateDetailedPath(List<Vector3> originalPath, float segmentInterval)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < originalPath.Count - 1; i++)
		{
			Vector3 vector = originalPath[i];
			Vector3 vector2 = originalPath[i + 1];
			list.Add(vector);
			float num = Vector3.Distance(vector, vector2);
			if (num > 2f)
			{
				Vector3 normalized = (vector2 - vector).normalized;
				int num2 = Mathf.FloorToInt(num / segmentInterval);
				for (int j = 1; j < num2; j++)
				{
					Vector3 vector3 = vector + normalized * (segmentInterval * (float)j);
					Vector3 nearestPointOnNavMesh = GetNearestPointOnNavMesh(vector3, 1f);
					list.Add((nearestPointOnNavMesh == Vector3.zero) ? vector3 : nearestPointOnNavMesh);
				}
			}
		}
		list.Add(originalPath[originalPath.Count - 1]);
		return list;
	}

	public Vector3 GetNearestPointOnNavMesh(Vector3 position, float maxDistance)
	{
		if ((Object)(object)AstarPath.active == null)
		{
			return Vector3.zero;
		}
		NNInfo nearest = AstarPath.active.GetNearest(position, NearestNodeConstraint.None);
		if (nearest.node != null)
		{
			Vector3 position2 = nearest.position;
			if (Vector3.Distance(position2, position) <= maxDistance)
			{
				return position2;
			}
		}
		return Vector3.zero;
	}

	public Vector3 RayCast(Vector3 start, Vector3 end)
	{
		_ = Vector3.zero;
		if ((Object)(object)AstarPath.active == null)
		{
			return Vector3.zero;
		}
		if (AstarPath.active.GetNearest(end, NearestNodeConstraint.None).node == null)
		{
			return FindNavMeshBoundary(start, end);
		}
		ABPath aBPath = ABPath.Construct(start, end);
		AstarPath.StartPath(aBPath);
		AstarPath.BlockUntilCalculated(aBPath);
		if (aBPath.error)
		{
			return Vector3.zero;
		}
		if (aBPath.vectorPath.Count <= 2)
		{
			return end;
		}
		return aBPath.vectorPath[0];
	}

	private Vector3 FindNavMeshBoundary(Vector3 start, Vector3 end)
	{
		Vector3 zero = Vector3.zero;
		if ((Object)(object)AstarPath.active == null)
		{
			return zero;
		}
		Vector3 normalized = (end - start).normalized;
		float num = Vector3.Distance(start, end);
		float num2 = 0f;
		float num3 = num;
		Vector3 result = start;
		for (int i = 0; i < 10; i++)
		{
			if (!(num3 - num2 > 0.1f))
			{
				break;
			}
			float num4 = (num2 + num3) * 0.5f;
			Vector3 vector = start + normalized * num4;
			NNInfo nearest = AstarPath.active.GetNearest(vector, NearestNodeConstraint.None);
			if (nearest.node != null && Vector3.Distance(nearest.position, vector) < 1f)
			{
				result = nearest.position;
				num2 = num4;
			}
			else
			{
				num3 = num4;
			}
		}
		return result;
	}

	public bool IsHitWall(Vector3 startPos, Vector3 endPos, out Vector3 hitPos, float turnAngleDegThreshold = 30f, float minSegmentDistance = 1f, float sampleRadius = 1f)
	{
		if ((Object)(object)AstarPath.active == null)
		{
			hitPos = startPos;
			return true;
		}
		Vector3 nearestPointOnNavMesh = GetNearestPointOnNavMesh(startPos, sampleRadius);
		Vector3 nearestPointOnNavMesh2 = GetNearestPointOnNavMesh(endPos, sampleRadius);
		if (nearestPointOnNavMesh == Vector3.zero || nearestPointOnNavMesh2 == Vector3.zero)
		{
			hitPos = startPos;
			return true;
		}
		PathFindResult route = GetRoute(nearestPointOnNavMesh, nearestPointOnNavMesh2);
		if (!route.Success)
		{
			hitPos = nearestPointOnNavMesh2;
			return false;
		}
		List<Vector3> pathPoints = route.PathPoints;
		for (int i = 1; i < pathPoints.Count - 1; i++)
		{
			Vector3 lhs = pathPoints[i] - pathPoints[i - 1];
			Vector3 rhs = pathPoints[i + 1] - pathPoints[i];
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
					hitPos = pathPoints[i];
					return true;
				}
			}
		}
		hitPos = nearestPointOnNavMesh2;
		return false;
	}

	public bool RayCastWithPhysics(Vector3 start, Vector3 end, out Vector3 hitPoint)
	{
		float magnitude = (end - start).magnitude;
		Ray ray = new Ray(start, end - start);
		Vector3 position = start;
		if (Physics.Raycast(ray, out var hitInfo, magnitude, LayerMask.GetMask("Default")))
		{
			position = hitInfo.point;
		}
		hitPoint = GetNearestPointOnNavMesh(position, magnitude);
		if (hitPoint == Vector3.zero)
		{
			hitPoint = start;
			return false;
		}
		return true;
	}
}
