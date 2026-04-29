using UnityEngine;

public class AUT_Unproject : MonoBehaviour
{
	public Camera cam;

	private void Update()
	{
		if (Input.GetMouseButton(0) && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hitInfo))
		{
			base.transform.position = hitInfo.point + hitInfo.normal * 0.5f;
		}
	}
}
