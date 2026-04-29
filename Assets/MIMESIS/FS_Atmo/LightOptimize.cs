using UnityEngine;

namespace FS_Atmo
{
	public class LightOptimize : MonoBehaviour
	{
		public GameObject DistanceFromObject;

		public float VisibilityDistance;

		public float FloorHeight;

		public bool UseFloorHeight;

		private float Distance;

		private Light Lightcomponent;

		private GameObject MainCamera;

		private float PlayerFloor;

		private void Start()
		{
			Lightcomponent = base.gameObject.GetComponent<Light>();
			if (DistanceFromObject == null)
			{
				DistanceFromObject = base.gameObject;
			}
			if (GameObject.FindGameObjectWithTag("MainCamera") != null)
			{
				MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			else
			{
				Object.Destroy(GetComponent("LightOptimize"));
			}
		}

		private void Update()
		{
			if (UseFloorHeight)
			{
				PlayerFloor = MainCamera.transform.position.y;
				if (FloorHeight <= PlayerFloor - 2f || FloorHeight >= PlayerFloor + 2f)
				{
					Lightcomponent.enabled = false;
					return;
				}
				Distance = Vector3.Distance(MainCamera.transform.position, DistanceFromObject.transform.position);
				if (Distance < VisibilityDistance)
				{
					Lightcomponent.enabled = true;
				}
				if (Distance > VisibilityDistance)
				{
					Lightcomponent.enabled = false;
				}
			}
			else
			{
				Distance = Vector3.Distance(MainCamera.transform.position, DistanceFromObject.transform.position);
				if (Distance < VisibilityDistance)
				{
					Lightcomponent.enabled = true;
				}
				if (Distance > VisibilityDistance)
				{
					Lightcomponent.enabled = false;
				}
			}
		}
	}
}
