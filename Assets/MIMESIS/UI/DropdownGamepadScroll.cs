using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DropdownGamepadScroll : MonoBehaviour
{
	public TMP_Dropdown dropdown;

	public GameObject dropdownContent;

	public GameObject dropdownList;

	private int childCount;

	private float currentSpeed = 100f;

	private float minSpeed = 100f;

	private float maxSpeed = 330f;

	private float accelerationRate = 50f;

	private float holdTimer;

	private void Start()
	{
		dropdown = GetComponent<TMP_Dropdown>();
		childCount = dropdown.transform.childCount;
	}

	private void Update()
	{
		if (dropdown.transform.childCount <= childCount)
		{
			return;
		}
		dropdownList = GameObject.Find("Dropdown List");
		if (dropdownList == null)
		{
			return;
		}
		dropdownContent = dropdownList.GetComponent<ScrollRect>().content.gameObject;
		if (dropdownContent == null)
		{
			return;
		}
		Gamepad current = Gamepad.current;
		if (current == null)
		{
			return;
		}
		float num = current.rightStick.y.ReadValue();
		if (Mathf.Abs(num) > 0.2f)
		{
			holdTimer += Time.deltaTime;
			currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, holdTimer / 2f);
			currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
			if (num > 0.2f)
			{
				dropdownContent.transform.position -= new Vector3(0f, currentSpeed, 0f) * Time.deltaTime;
			}
			else if (num < -0.2f)
			{
				dropdownContent.transform.position += new Vector3(0f, currentSpeed, 0f) * Time.deltaTime;
			}
		}
		else
		{
			holdTimer = 0f;
			currentSpeed = minSpeed;
		}
	}
}
