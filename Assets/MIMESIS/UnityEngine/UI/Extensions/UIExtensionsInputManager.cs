using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.UI.Extensions
{
	public static class UIExtensionsInputManager
	{
		private static bool[] mouseButtons = new bool[3];

		private static Dictionary<KeyCode, bool> keys = new Dictionary<KeyCode, bool>();

		private static Dictionary<string, bool> buttons = new Dictionary<string, bool>();

		public static Vector3 MousePosition => Mouse.current.position.ReadValue();

		public static Vector3 MouseScrollDelta => Mouse.current.position.ReadValue();

		public static bool GetMouseButton(int button)
		{
			if (Mouse.current == null)
			{
				return false;
			}
			return Mouse.current.leftButton.isPressed;
		}

		public static bool GetMouseButtonDown(int button)
		{
			if (Mouse.current == null)
			{
				return false;
			}
			if (Mouse.current.leftButton.isPressed && !mouseButtons[button])
			{
				mouseButtons[button] = true;
				return true;
			}
			return false;
		}

		public static bool GetMouseButtonUp(int button)
		{
			if (Mouse.current == null)
			{
				return false;
			}
			if (mouseButtons[button] && !Mouse.current.leftButton.isPressed)
			{
				mouseButtons[button] = false;
				return true;
			}
			return false;
		}

		public static bool GetButton(string input)
		{
			ButtonControl buttonControlFromString = GetButtonControlFromString(input);
			if (!buttons.ContainsKey(input))
			{
				buttons.Add(input, value: false);
			}
			return buttonControlFromString?.isPressed ?? false;
		}

		private static ButtonControl GetButtonControlFromString(string input)
		{
			if (Gamepad.current == null)
			{
				return null;
			}
			if (!(input == "Submit"))
			{
				if (input == "Cancel")
				{
					return Gamepad.current.bButton;
				}
				return null;
			}
			return Gamepad.current.aButton;
		}

		public static bool GetButtonDown(string input)
		{
			if (GetButtonControlFromString(input).isPressed)
			{
				if (!buttons.ContainsKey(input))
				{
					buttons.Add(input, value: false);
				}
				if (!buttons[input])
				{
					buttons[input] = true;
					return true;
				}
			}
			else
			{
				buttons[input] = false;
			}
			return false;
		}

		public static bool GetButtonUp(string input)
		{
			ButtonControl buttonControlFromString = GetButtonControlFromString(input);
			if (buttons[input] && !buttonControlFromString.isPressed)
			{
				buttons[input] = false;
				return true;
			}
			return false;
		}

		public static bool GetKey(KeyCode key)
		{
			KeyControl keyControlFromKeyCode = GetKeyControlFromKeyCode(key);
			if (!keys.ContainsKey(key))
			{
				keys.Add(key, value: false);
			}
			return keyControlFromKeyCode?.isPressed ?? false;
		}

		private static KeyControl GetKeyControlFromKeyCode(KeyCode key)
		{
			if (Keyboard.current == null)
			{
				return null;
			}
			return key switch
			{
				KeyCode.Escape => Keyboard.current.escapeKey, 
				KeyCode.KeypadEnter => Keyboard.current.numpadEnterKey, 
				KeyCode.UpArrow => Keyboard.current.upArrowKey, 
				KeyCode.DownArrow => Keyboard.current.downArrowKey, 
				KeyCode.RightArrow => Keyboard.current.rightArrowKey, 
				KeyCode.LeftArrow => Keyboard.current.leftArrowKey, 
				KeyCode.LeftShift => Keyboard.current.leftShiftKey, 
				KeyCode.Tab => Keyboard.current.tabKey, 
				_ => null, 
			};
		}

		public static bool GetKeyDown(KeyCode key)
		{
			if (GetKeyControlFromKeyCode(key).isPressed)
			{
				if (!keys.ContainsKey(key))
				{
					keys.Add(key, value: false);
				}
				if (!keys[key])
				{
					keys[key] = true;
					return true;
				}
			}
			else
			{
				keys[key] = false;
			}
			return false;
		}

		public static bool GetKeyUp(KeyCode key)
		{
			KeyControl keyControlFromKeyCode = GetKeyControlFromKeyCode(key);
			if (keys[key] && !keyControlFromKeyCode.isPressed)
			{
				keys[key] = false;
				return true;
			}
			return false;
		}

		public static float GetAxisRaw(string axis)
		{
			if (Gamepad.current == null)
			{
				return 0f;
			}
			if (!(axis == "Horizontal"))
			{
				if (axis == "Vertical")
				{
					return Gamepad.current.leftStick.y.ReadValue();
				}
				return 0f;
			}
			return Gamepad.current.leftStick.x.ReadValue();
		}
	}
}
