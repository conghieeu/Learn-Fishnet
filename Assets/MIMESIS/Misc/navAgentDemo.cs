using UnityEngine;
using UnityEngine.AI;

public class navAgentDemo : MonoBehaviour
{
	private NavMeshAgent navagent;

	[SerializeField]
	private Vector3[] debug_corners;

	private void Start()
	{
		navagent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if (Hub.s == null || Hub.s.inputman == null || Hub.s.inputman.mouse == null)
		{
			return;
		}
		if (Hub.s.inputman.mouse.leftButton.wasPressedThisFrame)
		{
			RaycastHit hitInfo;
			Vector3 vector = (Physics.Raycast(Camera.main.ScreenPointToRay(Hub.s.inputman.mouse.position.value), out hitInfo, 100f, LayerMask.GetMask("Default")) ? hitInfo.point : base.transform.position);
			navagent.SetDestination(vector);
			Vector3 nearestPointOnNavMesh = Hub.s.navman.GetNearestPointOnNavMesh(vector, 100f);
			if (nearestPointOnNavMesh != Vector3.zero)
			{
				PathFindResult route = Hub.s.navman.GetRoute(base.transform.position, nearestPointOnNavMesh);
				if (route.Success)
				{
					debug_corners = route.PathPoints.ToArray();
				}
				else
				{
					Debug.LogError("Path not found");
				}
			}
			else
			{
				Debug.LogError("No navmesh hit");
			}
		}
		if (Hub.s.dynamicDataMan.CheckAny(base.transform.position, out MapTrigger trigger) && trigger != null)
		{
			Debug.Log(trigger.name);
		}
	}
}
