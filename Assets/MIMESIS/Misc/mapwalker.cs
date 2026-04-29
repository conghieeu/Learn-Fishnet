using UnityEngine;
using UnityEngine.InputSystem;

public class mapwalker : MonoBehaviour
{
	[SerializeField]
	private float walkSpeed = 5f;

	[SerializeField]
	private float runSpeed = 8f;

	[SerializeField]
	private float turnSpeed = 1f;

	private CharacterController cc;

	private Vector3 initialPos;

	private Transform cam;

	private Light light;

	private void Awake()
	{
		cc = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>().transform;
		light = GetComponentInChildren<Light>();
		initialPos = base.transform.position;
	}

	public void Reset()
	{
		cc.enabled = false;
		base.transform.position = initialPos;
		cc.enabled = true;
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Keyboard keyboard = Hub.GetKeyboard();
		if (keyboard != null)
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				float num = walkSpeed;
				num = ((!keyboard.shiftKey.isPressed) ? walkSpeed : runSpeed);
				if (keyboard.wKey.isPressed)
				{
					cc.Move(base.transform.forward * num * deltaTime);
				}
				if (keyboard.sKey.isPressed)
				{
					cc.Move(-base.transform.forward * num * deltaTime);
				}
				if (keyboard.aKey.isPressed)
				{
					cc.Move(-base.transform.right * num * deltaTime);
				}
				if (keyboard.dKey.isPressed)
				{
					cc.Move(base.transform.right * num * deltaTime);
				}
			}
			cc.Move(Physics.gravity * deltaTime);
			if (keyboard.slashKey.wasPressedThisFrame)
			{
				if (Cursor.lockState == CursorLockMode.Locked)
				{
					Cursor.lockState = CursorLockMode.None;
				}
				else if (Cursor.lockState == CursorLockMode.None)
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
			if (keyboard.eKey.wasPressedThisFrame && light != null)
			{
				light.enabled = !light.enabled;
			}
		}
		Mouse mouse = Hub.GetMouse();
		if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
		{
			Vector3 eulerAngles = cam.transform.rotation.eulerAngles;
			if (eulerAngles.x > 180f)
			{
				eulerAngles.x -= 360f;
			}
			eulerAngles.x -= mouse.delta.y.ReadValue() * turnSpeed;
			if (eulerAngles.x < -80f)
			{
				eulerAngles.x = -80f;
			}
			else if (eulerAngles.x > 80f)
			{
				eulerAngles.x = 80f;
			}
			cam.transform.rotation = Quaternion.Euler(eulerAngles);
			Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
			eulerAngles2.y += mouse.delta.x.ReadValue() * turnSpeed;
			base.transform.rotation = Quaternion.Euler(eulerAngles2);
		}
	}
}
