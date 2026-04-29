using UnityEngine;

namespace Beautify.Universal
{
	public class SphereAnimator : MonoBehaviour
	{
		private Rigidbody rb;

		private const float SPEED = 4f;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			Application.targetFrameRate = 60;
		}

		private void FixedUpdate()
		{
			if (base.transform.position.z < 0.5f)
			{
				rb.linearVelocity = Vector3.forward * 4f;
			}
			else if (base.transform.position.z > 8f)
			{
				rb.linearVelocity = Vector3.back * 4f;
			}
		}
	}
}
