using System;
using System.Collections;
using System.Runtime.InteropServices;
using Mimic.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class UIPrefab_GamepadCursor : UIPrefabScript
{
	private const uint MOUSEEVENTF_LEFTDOWN = 2u;

	private const uint MOUSEEVENTF_LEFTUP = 4u;

	[SerializeField]
	private float cursorSpeed = 2000f;

	[SerializeField]
	private float deltaThreshold = 0.1f;

	[SerializeField]
	[Range(0f, 2f)]
	private float cursorSpeedMultiplier = 0.5f;

	[DllImport("user32.dll")]
	private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

	private void Start()
	{
		dialogue = false;
	}

	private void Update()
	{
		if (Cursor.lockState != CursorLockMode.Locked && !(Hub.s == null) && Hub.s.inputman.lastInputDevice != Mimic.InputSystem.InputDevice.KeyboardMouse && Gamepad.current != null && Mouse.current != null && !(Mouse.current.delta.ReadValue().sqrMagnitude > deltaThreshold * deltaThreshold))
		{
			Vector2 vector = Gamepad.current.leftStick.ReadValue();
			if (vector != Vector2.zero)
			{
				Vector2 vector2 = Mouse.current.position.ReadValue() + vector * cursorSpeed * cursorSpeedMultiplier * Time.deltaTime;
				vector2.x = Mathf.Clamp(vector2.x, 0f, Screen.currentResolution.width);
				vector2.y = Mathf.Clamp(vector2.y, 0f, Screen.currentResolution.height);
				Mouse.current.WarpCursorPosition(vector2);
				InputState.Change(Mouse.current.position, vector2);
			}
			if (Gamepad.current.aButton.wasPressedThisFrame)
			{
				StartCoroutine(SimulateMouseClick());
			}
		}
	}

	private IEnumerator SimulateMouseClick()
	{
		if (Hub.s != null && Hub.s.inputman != null)
		{
			Hub.s.inputman.isGamepadCursorClick = true;
		}
		mouse_event(2u, 0u, 0u, 0u, UIntPtr.Zero);
		yield return new WaitUntil(() => Gamepad.current.aButton.wasReleasedThisFrame);
		mouse_event(4u, 0u, 0u, 0u, UIntPtr.Zero);
		if (Hub.s != null && Hub.s.inputman != null)
		{
			Hub.s.inputman.isGamepadCursorClick = false;
		}
	}
}
