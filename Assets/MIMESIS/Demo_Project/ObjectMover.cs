using UnityEngine;

namespace Demo_Project
{
	public class ObjectMover : MonoBehaviour
	{
		public Vector3 targetPosition = new Vector3(0f, 0f, 0f);

		private void Start()
		{
			MoveObject();
		}

		private void MoveObject()
		{
			base.transform.position = targetPosition;
		}
	}
}
