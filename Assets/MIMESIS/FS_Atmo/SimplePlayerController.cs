using UnityEngine;

namespace FS_Atmo
{
	[RequireComponent(typeof(CharacterController))]
	public class SimplePlayerController : MonoBehaviour
	{
		public Camera playerCamera;

		public float walkSpeed = 1.15f;

		public float runSpeed = 4f;

		public float lookSpeed = 2f;

		public float lookXLimit = 60f;

		public float gravity = 150f;

		private CharacterController characterController;

		private Vector3 moveDirection = Vector3.zero;

		private float rotationX;

		private bool canMove = true;

		private void Start()
		{
			characterController = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void Update()
		{
			Vector3 vector = base.transform.TransformDirection(Vector3.forward);
			Vector3 vector2 = base.transform.TransformDirection(Vector3.right);
			bool key = Input.GetKey(KeyCode.LeftShift);
			float num = (canMove ? ((key ? runSpeed : walkSpeed) * Input.GetAxis("Vertical")) : 0f);
			float num2 = (canMove ? ((key ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal")) : 0f);
			_ = moveDirection;
			moveDirection = vector * num + vector2 * num2;
			if (!characterController.isGrounded)
			{
				moveDirection.y -= gravity * Time.deltaTime;
			}
			characterController.Move(moveDirection * Time.deltaTime);
			if (canMove)
			{
				rotationX += (0f - Input.GetAxis("Mouse Y")) * lookSpeed;
				rotationX = Mathf.Clamp(rotationX, 0f - lookXLimit, lookXLimit);
				playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
				base.transform.rotation *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * lookSpeed, 0f);
			}
		}
	}
}
