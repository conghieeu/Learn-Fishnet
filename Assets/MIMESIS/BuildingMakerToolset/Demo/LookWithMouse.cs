using UnityEngine;
using UnityEngine.InputSystem;

namespace BuildingMakerToolset.Demo
{
	public class LookWithMouse : MonoBehaviour
	{
		public float mouseSensitivity = 100f;

		public Transform playerBody;

		private float xRotation;

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			float num = 0f;
			float num2 = 0f;
			if (Mouse.current != null)
			{
				Vector2 vector = Mouse.current.delta.ReadValue() / 15f;
				num += vector.x;
				num2 += vector.y;
			}
			if (Gamepad.current != null)
			{
				Vector2 vector2 = Gamepad.current.rightStick.ReadValue() * 2f;
				num += vector2.x;
				num2 += vector2.y;
			}
			num *= mouseSensitivity * Time.deltaTime;
			num2 *= mouseSensitivity * Time.deltaTime;
			xRotation -= num2;
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);
			base.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
			playerBody.Rotate(Vector3.up * num);
		}
	}
}
