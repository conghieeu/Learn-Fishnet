using UnityEngine;

public class BilboardWorldGUI : MonoBehaviour
{
	private void LateUpdate()
	{
		if (Camera.main != null)
		{
			base.transform.LookAt(base.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
		}
	}
}
