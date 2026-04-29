using UnityEngine;

namespace TobyFredson
{
	[RequireComponent(typeof(CharacterController))]
	public class FPSController : MonoBehaviour
	{
		public Camera playerCamera;

		public float walkSpeed = 6f;

		public float runSpeed = 12f;

		public float jumpPower = 7f;

		public float gravity = 10f;

		public float lookSpeed = 2f;

		public float lookXLimit = 45f;

		private Vector3 moveDirection = Vector3.zero;

		private float rotationX;

		public bool canMove = true;

		private CharacterController characterController;

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
			float y = moveDirection.y;
			moveDirection = vector * num + vector2 * num2;
			if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
			{
				moveDirection.y = jumpPower;
			}
			else
			{
				moveDirection.y = y;
			}
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
