using UnityEngine;

public class navtester : MonoBehaviour
{
	public Transform target;

	[ContextMenu("test")]
	private void Test()
	{
		PathFindResult route = Hub.s.navman.GetRoute(base.transform.position, target.position);
		if (route.Success)
		{
			foreach (Vector3 pathPoint in route.PathPoints)
			{
				Debug.Log($"[FindPath] path: {pathPoint}");
			}
			Hub.DebugDraw_Path(route.PathPoints, Color.red, 10f);
		}
		else
		{
			Debug.LogError("[FindPath] failed");
		}
	}

	[ContextMenu("testDrop")]
	private void TestDrop()
	{
		Hub.DebugDraw_Octahedron(Hub.s.navman.GetNearestPointOnNavMesh(base.transform.position, 0.5f), 0.5f, Color.white, 1f);
	}
}
