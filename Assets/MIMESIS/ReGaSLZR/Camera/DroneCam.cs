using UnityEngine;

namespace ReGaSLZR.Camera
{
	[RequireComponent(typeof(SphereCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class DroneCam : MonoBehaviour
	{
		[Header("Controls (apart from WASD / Arrow Keys)")]
		[SerializeField]
		private KeyCode keyCodeHoverUp = KeyCode.Q;

		[SerializeField]
		private KeyCode keyCodeHoverDown = KeyCode.E;

		[Space]
		[SerializeField]
		private KeyCode keyCodeMoveFaster = KeyCode.LeftShift;

		[SerializeField]
		private KeyCode keyCodeMoveSlower = KeyCode.LeftControl;

		[Header("Config")]
		[SerializeField]
		private float cameraSensitivity = 3f;

		[SerializeField]
		private float climbSpeed = 4f;

		[SerializeField]
		private float normalMoveSpeed = 10f;

		[SerializeField]
		private float slowMoveFactor = 0.25f;

		[SerializeField]
		private float fastMoveFactor = 3f;

		private float rotationX;

		private float rotationY;

		private Rigidbody rigidBody;

		private void Start()
		{
			ConfigureRigidbody();
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			Rotate();
		}

		private void FixedUpdate()
		{
			Move();
		}

		private void ConfigureRigidbody()
		{
			rigidBody = GetComponent<Rigidbody>();
			rigidBody.isKinematic = false;
			rigidBody.useGravity = false;
		}

		private void UpdateCaches()
		{
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity;
			rotationY = Mathf.Clamp(rotationY, -90f, 90f);
		}

		private void Rotate()
		{
			UpdateCaches();
			if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0f)
			{
				Debug.Log(GetType().Name + ".Rotate(): Updating Rotation...", base.gameObject);
				base.transform.rotation = Quaternion.AngleAxis(rotationX, Vector3.up);
				base.transform.rotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
			}
		}

		private void Move()
		{
			if (!Input.anyKey)
			{
				rigidBody.linearVelocity = Vector3.zero;
				rigidBody.angularVelocity = Vector3.zero;
				return;
			}
			if (Mathf.Abs(Input.GetAxis("Vertical")) > 0f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0f)
			{
				rigidBody.position += base.transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.fixedDeltaTime;
				rigidBody.position += base.transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
			}
			else if (Input.GetKey(keyCodeHoverUp))
			{
				rigidBody.position += base.transform.up * climbSpeed * Time.fixedDeltaTime;
			}
			else if (Input.GetKey(keyCodeHoverDown))
			{
				rigidBody.position -= base.transform.up * climbSpeed * Time.fixedDeltaTime;
			}
			if (Input.GetKey(keyCodeMoveFaster))
			{
				rigidBody.position += base.transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.fixedDeltaTime;
			}
			else if (Input.GetKey(keyCodeMoveSlower))
			{
				rigidBody.position -= base.transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.fixedDeltaTime;
			}
		}
	}
}
