using UnityEngine;

namespace Demo_Project
{
	public class Head : MonoBehaviour
	{
		private float minimumAngle;

		private float maximumAngle;

		public float dividerRate = 3f;

		private void Start()
		{
			SceneManager.listOfHeads.Add(base.gameObject);
		}

		public void MoveHead()
		{
			Quaternion rotation = default(Quaternion);
			float num = GetAngle() * 57.29578f;
			if (num > maximumAngle)
			{
				num = maximumAngle;
			}
			if (num < minimumAngle)
			{
				num = minimumAngle;
			}
			num /= dividerRate;
			rotation.eulerAngles = new Vector3(0f, 0f, num);
			base.transform.rotation = rotation;
		}

		public float GetAngle()
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			return Mathf.Atan2(vector.y - base.transform.position.y, vector.x - base.transform.position.x);
		}

		public void SetAngles(float min, float max)
		{
			minimumAngle = min;
			maximumAngle = max;
		}

		private void Update()
		{
			MoveHead();
		}
	}
}
