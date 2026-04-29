using UnityEngine;
using UnityEngine.InputSystem;

namespace DunGen.Demo
{
	public class CameraMovement : MonoBehaviour
	{
		[Header("Movement")]
		public float moveSpeed = 8f;

		[Header("Look")]
		public float lookSensitivity = 0.12f;

		public bool holdRightMouseToLook = true;

		[Header("Zoom")]
		public float scrollZoomSpeed = 5f;

		private Vector2 look;

		private Camera cam;

		private void Awake()
		{
			cam = GetComponent<Camera>();
			if (cam == null)
			{
				cam = Camera.main;
			}
		}

		private void Update()
		{
			HandleInputSystem();
		}

		private void HandleInputSystem()
		{
			Keyboard current = Keyboard.current;
			Mouse current2 = Mouse.current;
			if (current == null || current2 == null)
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			if (current.wKey.isPressed)
			{
				zero += base.transform.forward;
			}
			if (current.sKey.isPressed)
			{
				zero -= base.transform.forward;
			}
			if (current.aKey.isPressed)
			{
				zero -= base.transform.right;
			}
			if (current.dKey.isPressed)
			{
				zero += base.transform.right;
			}
			if (current.spaceKey.isPressed)
			{
				zero += Vector3.up;
			}
			if (current.leftCtrlKey.isPressed || current.cKey.isPressed)
			{
				zero += Vector3.down;
			}
			if (zero.sqrMagnitude > 0f)
			{
				base.transform.position += zero.normalized * moveSpeed * Time.deltaTime;
			}
			if (!holdRightMouseToLook || current2.rightButton.isPressed)
			{
				Vector2 vector = current2.delta.ReadValue() * lookSensitivity;
				look.x += vector.x;
				look.y = Mathf.Clamp(look.y - vector.y, -89f, 89f);
				base.transform.rotation = Quaternion.Euler(look.y, look.x, 0f);
			}
			float y = current2.scroll.ReadValue().y;
			if (cam != null && Mathf.Abs(y) > 0.01f)
			{
				if (cam.orthographic)
				{
					cam.orthographicSize = Mathf.Max(0.01f, cam.orthographicSize - y * (scrollZoomSpeed * 0.02f));
				}
				else
				{
					base.transform.position += base.transform.forward * (y * (scrollZoomSpeed * 0.1f));
				}
			}
		}

		private void HandleLegacyInput()
		{
			float axisRaw = Input.GetAxisRaw("Horizontal");
			float axisRaw2 = Input.GetAxisRaw("Vertical");
			Vector3 vector = base.transform.forward * axisRaw2 + base.transform.right * axisRaw;
			if (Input.GetKey(KeyCode.Space))
			{
				vector += Vector3.up;
			}
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
			{
				vector += Vector3.down;
			}
			if (vector.sqrMagnitude > 0f)
			{
				base.transform.position += vector.normalized * moveSpeed * Time.deltaTime;
			}
			if (!holdRightMouseToLook || Input.GetMouseButton(1))
			{
				float axisRaw3 = Input.GetAxisRaw("Mouse X");
				float axisRaw4 = Input.GetAxisRaw("Mouse Y");
				look.x += axisRaw3 * 10f * lookSensitivity;
				look.y = Mathf.Clamp(look.y - axisRaw4 * 10f * lookSensitivity, -89f, 89f);
				base.transform.rotation = Quaternion.Euler(look.y, look.x, 0f);
			}
			float axisRaw5 = Input.GetAxisRaw("Mouse ScrollWheel");
			if (cam != null && Mathf.Abs(axisRaw5) > 0.0001f)
			{
				if (cam.orthographic)
				{
					cam.orthographicSize = Mathf.Max(0.01f, cam.orthographicSize - axisRaw5 * (scrollZoomSpeed * 10f));
				}
				else
				{
					base.transform.position += base.transform.forward * (axisRaw5 * scrollZoomSpeed);
				}
			}
		}
	}
}
