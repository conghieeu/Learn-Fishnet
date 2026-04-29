using UnityEngine;

namespace FS_Atmo
{
	public class SimplePlayerUse : MonoBehaviour
	{
		public GameObject mainCamera;

		private GameObject objectClicked;

		public GameObject flashlight;

		public KeyCode OpenClose;

		public KeyCode Flashlight;

		private void Start()
		{
		}

		private void Update()
		{
			if (Input.GetKeyDown(OpenClose))
			{
				RaycastCheck();
			}
			if (Input.GetKeyDown(Flashlight))
			{
				if (flashlight.activeSelf)
				{
					flashlight.SetActive(value: false);
				}
				else
				{
					flashlight.SetActive(value: true);
				}
			}
		}

		private void RaycastCheck()
		{
			if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out var hitInfo, 2.3f) && (bool)hitInfo.collider.gameObject.GetComponent<SimpleOpenClose>())
			{
				hitInfo.collider.gameObject.BroadcastMessage("ObjectClicked");
			}
		}
	}
}
