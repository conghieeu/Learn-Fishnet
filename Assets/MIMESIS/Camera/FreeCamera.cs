using UnityEngine;

public class FreeCamera : MonoBehaviour
{
	public bool enableInputCapture = true;

	public bool holdRightMouseCapture;

	public float lookSpeed = 5f;

	public float moveSpeed = 5f;

	public float sprintSpeed = 50f;

	private bool m_inputCaptured;

	private float m_yaw;

	private float m_pitch;

	private void Awake()
	{
		base.enabled = enableInputCapture;
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			base.enabled = enableInputCapture;
		}
	}

	private void CaptureInput()
	{
		m_inputCaptured = true;
		m_yaw = base.transform.eulerAngles.y;
		m_pitch = base.transform.eulerAngles.x;
	}

	private void ReleaseInput()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		m_inputCaptured = false;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (m_inputCaptured && !focus)
		{
			ReleaseInput();
		}
	}

	private void Update()
	{
		if (!m_inputCaptured)
		{
			if (!holdRightMouseCapture && Input.GetMouseButtonDown(0))
			{
				CaptureInput();
			}
			else if (holdRightMouseCapture && Input.GetMouseButtonDown(1))
			{
				CaptureInput();
			}
		}
		if (!m_inputCaptured)
		{
			return;
		}
		if (m_inputCaptured)
		{
			if (!holdRightMouseCapture && Input.GetKeyDown(KeyCode.Escape))
			{
				ReleaseInput();
			}
			else if (holdRightMouseCapture && Input.GetMouseButtonUp(1))
			{
				ReleaseInput();
			}
		}
		float axis = Input.GetAxis("Mouse X");
		float axis2 = Input.GetAxis("Mouse Y");
		m_yaw = (m_yaw + lookSpeed * axis) % 360f;
		m_pitch = (m_pitch - lookSpeed * axis2) % 360f;
		base.transform.rotation = Quaternion.AngleAxis(m_yaw, Vector3.up) * Quaternion.AngleAxis(m_pitch, Vector3.right);
		float num = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
		float num2 = num * Input.GetAxis("Vertical");
		float num3 = num * Input.GetAxis("Horizontal");
		float num4 = num * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));
		base.transform.position += base.transform.forward * num2 + base.transform.right * num3 + Vector3.up * num4;
	}
}
