using System.Diagnostics;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class MeasureTimeForBuildingNavmesh : MonoBehaviour
{
	[ContextMenu("Bake NavMesh")]
	public void Bake()
	{
		NavMeshSurface component = GetComponent<NavMeshSurface>();
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Restart();
		component.BuildNavMesh();
		stopwatch.Stop();
		UnityEngine.Debug.Log($"NavMesh built in {stopwatch.ElapsedMilliseconds} ms");
	}
}
