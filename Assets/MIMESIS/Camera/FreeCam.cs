using UnityEngine;

public class FreeCam : MonoBehaviour
{
	public float movementSpeed = 10f;

	public float fastMovementSpeed = 100f;

	public float freeLookSensitivity = 3f;

	public float zoomSensitivity = 10f;

	public float fastZoomSensitivity = 50f;

	private bool looking;

	private void Update()
	{
		bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		float num = (flag ? fastMovementSpeed : movementSpeed);
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			base.transform.position = base.transform.position + -base.transform.right * num * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			base.transform.position = base.transform.position + base.transform.right * num * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			base.transform.position = base.transform.position + base.transform.forward * num * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			base.transform.position = base.transform.position + -base.transform.forward * num * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			base.transform.position = base.transform.position + base.transform.up * num * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.E))
		{
			base.transform.position = base.transform.position + -base.transform.up * num * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
		{
			base.transform.position = base.transform.position + Vector3.up * num * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
		{
			base.transform.position = base.transform.position + -Vector3.up * num * Time.deltaTime;
		}
		if (looking)
		{
			float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
			float x = base.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
			base.transform.localEulerAngles = new Vector3(x, y, 0f);
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			float num2 = (flag ? fastZoomSensitivity : zoomSensitivity);
			base.transform.position = base.transform.position + base.transform.forward * axis * num2;
		}
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			StartLooking();
		}
		else if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			StopLooking();
		}
	}

	private void OnDisable()
	{
		StopLooking();
	}

	public void StartLooking()
	{
		looking = true;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void StopLooking()
	{
		looking = false;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
}
