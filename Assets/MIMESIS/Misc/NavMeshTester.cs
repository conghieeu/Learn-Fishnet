using UnityEngine;

public class NavMeshTester : MonoBehaviour
{
	[SerializeField]
	private float rayLength = 5f;

	[ContextMenu("RayCast Down")]
	public void RayCast_Down()
	{
		Vector3 position = base.transform.position;
		Vector3 end = position + Vector3.down * rayLength;
		Vector3 vector = Hub.s.navman.RayCastWithPhysics(position, end);
		if (vector != Vector3.zero)
		{
			Debug.Log($"RayCast hit: {vector}");
			Debug.DrawLine(position, vector, Color.red, 5f);
		}
		else
		{
			Debug.Log("RayCast miss");
		}
	}
}
