using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Mimic.InputSystem
{
	public class InputManager : MonoBehaviour
	{
		public delegate void OnLastInputDeviceChanged();

		public class ActionState
		{
			public InputAction action;

			public bool wasPressedThisFrame;

			public bool wasRelesedThisFrame;

			public bool isPressed;

			public List<KeyControl> assignedKeys = new List<KeyControl>();

			public List<ButtonControl> assignedButtons = new List<ButtonControl>();

			public List<DeltaControl> assignedDeltaControlAsButtons_x = new List<DeltaControl>();

			public List<DeltaControl> assignedDeltaControlAsButtons_nx = new List<DeltaControl>();

			public List<DeltaControl> assignedDeltaControlAsButtons_y = new List<DeltaControl>();

			public List<DeltaControl> assignedDeltaControlAsButtons_ny = new List<DeltaControl>();
		}

		public delegate void OnKeyBindChangedCallback();

		private InputDevice _lastInputDevice;

		public bool isGamepadEmoteActive;

		public bool isGamepadCursorClick;

		private bool gamepadInitialized;

		[SerializeField]
		private InputManagerData defaultData;

		[HideInInspector]
		public InputManagerData data;

		private Dictionary<InputAction, ActionState> states = new Dictionary<InputAction, ActionState>();

		private readonly HashSet<InputAction> _virtualPressedActions = new HashSet<InputAction>();

		public Coroutine changeKeyBindCoroutine;

		private float _mouseSensetivity = 1f;

		private bool runToggleActive;

		private int _invertYAxis = 1;

		private string dataKey = "InputManagerData_";

		private string keyDataVersion = "v1.0.2";

		private Dictionary<KeyControl, string> _keyMap;

		private Dictionary<ButtonControl, string> _mouseMap;

		public Keyboard keyboard => Keyboard.current;

		public Mouse mouse => Mouse.current;

		public Gamepad gamepad => Gamepad.current;

		public InputDevice lastInputDevice
		{
			get
			{
				return _lastInputDevice;
			}
			set
			{
				if (_lastInputDevice != value)
				{
					_lastInputDevice = value;
					if (Hub.s != null && !Hub.s.uiman.keyBindGamepadUI.gameObject.activeSelf && !Hub.s.uiman.keyBindUI.gameObject.activeSelf)
					{
						this.onLastInputDeviceChanged?.Invoke();
					}
				}
			}
		}

		public Vector2 mouseMovement { get; private set; }

		public Vector2 gamepadLeftStick { get; private set; }

		public Vector2 gamepadRightStick { get; private set; }

		[HideInInspector]
		public float mouseSensetivity
		{
			get
			{
				return _mouseSensetivity;
			}
			set
			{
				_mouseSensetivity = value;
				PlayerPrefs.SetFloat("mouseSensitivity", value);
			}
		}

		[HideInInspector]
		public int invertYAxis
		{
			get
			{
				return _invertYAxis;
			}
			set
			{
				_invertYAxis = value;
			}
		}

		public event OnLastInputDeviceChanged onLastInputDeviceChanged;

		private void Start()
		{
			Logger.RLog("[AwakeLogs] InputManager.Start ->");
			data = ScriptableObject.CreateInstance<InputManagerData>();
			string json = PlayerPrefs.GetString(dataKey + keyDataVersion, JsonUtility.ToJson(defaultData));
			JsonUtility.FromJsonOverwrite(json, data);
			if (data.data.Count == 0 || data.data.Count != defaultData.data.Count)
			{
				json = JsonUtility.ToJson(defaultData);
				PlayerPrefs.SetString(dataKey + keyDataVersion, json);
				json = PlayerPrefs.GetString(dataKey + keyDataVersion);
				JsonUtility.FromJsonOverwrite(json, data);
			}
			RefreshInputAssign();
			mouseSensetivity = Hub.s.gameSettingManager.mouseSensitivity;
			Logger.RLog("[AwakeLogs] InputManager.Start <-");
		}

		private void Update()
		{
			Keyboard current = Keyboard.current;
			Mouse current2 = Mouse.current;
			Gamepad gamepad = Gamepad.current;
			if (gamepad != null && !gamepadInitialized)
			{
				gamepadInitialized = true;
				RefreshInputAssign();
			}
			bool flag = current != null && (current.anyKey.wasPressedThisFrame || current.anyKey.wasReleasedThisFrame || current.anyKey.isPressed);
			bool flag2 = current2 != null && (current2.delta.x.ReadValue() != 0f || current2.delta.y.ReadValue() != 0f);
			bool flag3 = current2 != null && (current2.leftButton.wasPressedThisFrame || current2.leftButton.wasReleasedThisFrame || current2.leftButton.isPressed || current2.rightButton.wasPressedThisFrame || current2.rightButton.wasReleasedThisFrame || current2.rightButton.isPressed || current2.middleButton.wasPressedThisFrame || current2.middleButton.wasReleasedThisFrame || current2.middleButton.isPressed || current2.forwardButton.wasPressedThisFrame || current2.forwardButton.wasReleasedThisFrame || current2.forwardButton.isPressed || current2.backButton.wasPressedThisFrame || current2.backButton.wasReleasedThisFrame || current2.backButton.isPressed);
			bool flag4 = current2 != null && current2.scroll.ReadValue().y != 0f;
			bool flag5 = false;
			try
			{
				flag5 = gamepad != null && (gamepad.leftStick.x.ReadValue() != 0f || gamepad.leftStick.y.ReadValue() != 0f || gamepad.rightStick.x.ReadValue() != 0f || gamepad.rightStick.y.ReadValue() != 0f);
			}
			catch (Exception)
			{
				gamepad = null;
			}
			bool flag6 = false;
			if (gamepad != null)
			{
				try
				{
					flag6 = gamepad.allControls.Any((InputControl x) => x is ButtonControl buttonControl && x.device.added && (buttonControl.wasPressedThisFrame || buttonControl.wasReleasedThisFrame || buttonControl.isPressed));
				}
				catch (Exception)
				{
					gamepad = null;
				}
			}
			if (current != null && current.anyKey.wasPressedThisFrame)
			{
				lastInputDevice = InputDevice.KeyboardMouse;
			}
			else if (current2 != null && (current2.leftButton.wasPressedThisFrame || current2.rightButton.wasPressedThisFrame || current2.middleButton.wasPressedThisFrame || current2.forwardButton.wasPressedThisFrame || current2.backButton.wasPressedThisFrame))
			{
				if (!isGamepadCursorClick)
				{
					lastInputDevice = InputDevice.KeyboardMouse;
				}
			}
			else if (gamepad != null)
			{
				try
				{
					if (gamepad.allControls.Any((InputControl x) => x is ButtonControl buttonControl && x.device.added && buttonControl.wasPressedThisFrame))
					{
						lastInputDevice = InputDevice.Gamepad;
					}
				}
				catch (Exception)
				{
					gamepad = null;
				}
			}
			if (!flag && !flag2 && !flag3 && !flag4 && !flag5 && !flag6 && _virtualPressedActions.Count == 0)
			{
				mouseMovement = Vector2.zero;
				gamepadLeftStick = Vector2.zero;
				gamepadRightStick = Vector2.zero;
				runToggleActive = false;
				{
					foreach (ActionState value in states.Values)
					{
						value.wasPressedThisFrame = false;
						value.wasRelesedThisFrame = false;
						value.isPressed = false;
					}
					return;
				}
			}
			foreach (ActionState value2 in states.Values)
			{
				bool flag7 = false;
				bool flag8 = false;
				bool flag9 = false;
				foreach (KeyControl assignedKey in value2.assignedKeys)
				{
					flag7 |= assignedKey.wasPressedThisFrame;
					flag8 |= assignedKey.wasReleasedThisFrame;
					flag9 |= assignedKey.isPressed;
					if (flag7 && flag8 && flag9)
					{
						break;
					}
				}
				foreach (ButtonControl assignedButton in value2.assignedButtons)
				{
					try
					{
						if (assignedButton.device.added)
						{
							flag7 |= assignedButton.wasPressedThisFrame;
							flag8 |= assignedButton.wasReleasedThisFrame;
							flag9 |= assignedButton.isPressed;
						}
					}
					catch (InvalidOperationException)
					{
					}
					if (flag7 && flag8 && flag9)
					{
						break;
					}
				}
				if (!(flag7 && flag8 && flag9))
				{
					foreach (DeltaControl item in value2.assignedDeltaControlAsButtons_x)
					{
						if (item.ReadValue().x > 0f)
						{
							flag9 = true;
						}
					}
					foreach (DeltaControl item2 in value2.assignedDeltaControlAsButtons_nx)
					{
						if (item2.ReadValue().x < 0f)
						{
							flag9 = true;
						}
					}
					foreach (DeltaControl item3 in value2.assignedDeltaControlAsButtons_y)
					{
						flag7 |= item3.ReadValue().y > 0f;
					}
					foreach (DeltaControl item4 in value2.assignedDeltaControlAsButtons_ny)
					{
						flag7 |= item4.ReadValue().y < 0f;
					}
				}
				if (_virtualPressedActions.Contains(value2.action))
				{
					flag7 = true;
					flag9 = true;
					flag8 = false;
				}
				value2.wasPressedThisFrame = flag7;
				if (value2.action != InputAction.Run || lastInputDevice != InputDevice.Gamepad)
				{
					value2.wasRelesedThisFrame = flag8;
					value2.isPressed = flag9;
					continue;
				}
				if (value2.wasPressedThisFrame)
				{
					runToggleActive = !runToggleActive;
				}
				float magnitude = gamepadLeftStick.magnitude;
				if (runToggleActive && magnitude < 0.1f)
				{
					runToggleActive = false;
				}
				states[InputAction.Run].isPressed = runToggleActive;
				states[InputAction.Run].wasRelesedThisFrame = !runToggleActive;
			}
			switch (lastInputDevice)
			{
			case InputDevice.KeyboardMouse:
				if (current2 != null)
				{
					mouseMovement = new Vector2(current2.delta.x.ReadValue(), current2.delta.y.ReadValue());
				}
				gamepadLeftStick = Vector2.zero;
				gamepadRightStick = Vector2.zero;
				break;
			case InputDevice.Gamepad:
				if (gamepad != null)
				{
					gamepadLeftStick = new Vector2(gamepad.leftStick.x.ReadValue(), gamepad.leftStick.y.ReadValue());
					gamepadRightStick = new Vector2(gamepad.rightStick.x.ReadValue(), gamepad.rightStick.y.ReadValue());
					mouseMovement = Vector2.zero;
				}
				break;
			}
			_virtualPressedActions.Clear();
		}

		public void PressKey(InputAction action)
		{
			if (states.TryGetValue(action, out var value))
			{
				value.wasPressedThisFrame = true;
				value.isPressed = true;
				value.wasRelesedThisFrame = false;
			}
			_virtualPressedActions.Add(action);
		}

		public bool IsCapturing()
		{
			return Cursor.lockState == CursorLockMode.Locked;
		}

		public void SetCapturing(bool on)
		{
			if (on)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
			}
		}

		public void ToggleCapturing()
		{
			SetCapturing(!IsCapturing());
		}

		public bool wasPressedThisFrame(InputAction action)
		{
			return states[action].wasPressedThisFrame;
		}

		public bool wasRelesedThisFrame(InputAction action)
		{
			return states[action].wasRelesedThisFrame;
		}

		public bool isPressed(InputAction action)
		{
			return states[action].isPressed;
		}

		public bool isPressedGamepadButton(string key)
		{
			return key == GetPressedGamepadKeyString();
		}

		public void RefreshInputAssign()
		{
			foreach (InputManagerData.Mapping datum in data.data)
			{
				if (states.TryGetValue(datum.action, out var state))
				{
					state.assignedKeys.Clear();
					state.assignedButtons.Clear();
				}
				else
				{
					state = new ActionState();
					states.Add(datum.action, state);
					state.action = datum.action;
				}
				datum.keys.Split(',').ToList().ForEach(delegate(string key)
				{
					if (key.StartsWith("k_"))
					{
						if (keyboard != null)
						{
							switch (key)
							{
							case "k_a":
								state.assignedKeys.Add(keyboard.aKey);
								break;
							case "k_b":
								state.assignedKeys.Add(keyboard.bKey);
								break;
							case "k_c":
								state.assignedKeys.Add(keyboard.cKey);
								break;
							case "k_d":
								state.assignedKeys.Add(keyboard.dKey);
								break;
							case "k_e":
								state.assignedKeys.Add(keyboard.eKey);
								break;
							case "k_f":
								state.assignedKeys.Add(keyboard.fKey);
								break;
							case "k_g":
								state.assignedKeys.Add(keyboard.gKey);
								break;
							case "k_h":
								state.assignedKeys.Add(keyboard.hKey);
								break;
							case "k_i":
								state.assignedKeys.Add(keyboard.iKey);
								break;
							case "k_j":
								state.assignedKeys.Add(keyboard.jKey);
								break;
							case "k_k":
								state.assignedKeys.Add(keyboard.kKey);
								break;
							case "k_l":
								state.assignedKeys.Add(keyboard.lKey);
								break;
							case "k_m":
								state.assignedKeys.Add(keyboard.mKey);
								break;
							case "k_n":
								state.assignedKeys.Add(keyboard.nKey);
								break;
							case "k_o":
								state.assignedKeys.Add(keyboard.oKey);
								break;
							case "k_p":
								state.assignedKeys.Add(keyboard.pKey);
								break;
							case "k_q":
								state.assignedKeys.Add(keyboard.qKey);
								break;
							case "k_r":
								state.assignedKeys.Add(keyboard.rKey);
								break;
							case "k_s":
								state.assignedKeys.Add(keyboard.sKey);
								break;
							case "k_t":
								state.assignedKeys.Add(keyboard.tKey);
								break;
							case "k_u":
								state.assignedKeys.Add(keyboard.uKey);
								break;
							case "k_v":
								state.assignedKeys.Add(keyboard.vKey);
								break;
							case "k_w":
								state.assignedKeys.Add(keyboard.wKey);
								break;
							case "k_x":
								state.assignedKeys.Add(keyboard.xKey);
								break;
							case "k_y":
								state.assignedKeys.Add(keyboard.yKey);
								break;
							case "k_z":
								state.assignedKeys.Add(keyboard.zKey);
								break;
							case "k_0":
								state.assignedKeys.Add(keyboard.digit0Key);
								break;
							case "k_1":
								state.assignedKeys.Add(keyboard.digit1Key);
								break;
							case "k_2":
								state.assignedKeys.Add(keyboard.digit2Key);
								break;
							case "k_3":
								state.assignedKeys.Add(keyboard.digit3Key);
								break;
							case "k_4":
								state.assignedKeys.Add(keyboard.digit4Key);
								break;
							case "k_5":
								state.assignedKeys.Add(keyboard.digit5Key);
								break;
							case "k_6":
								state.assignedKeys.Add(keyboard.digit6Key);
								break;
							case "k_7":
								state.assignedKeys.Add(keyboard.digit7Key);
								break;
							case "k_8":
								state.assignedKeys.Add(keyboard.digit8Key);
								break;
							case "k_9":
								state.assignedKeys.Add(keyboard.digit9Key);
								break;
							case "k_f1":
								state.assignedKeys.Add(keyboard.f1Key);
								break;
							case "k_f2":
								state.assignedKeys.Add(keyboard.f2Key);
								break;
							case "k_f3":
								state.assignedKeys.Add(keyboard.f3Key);
								break;
							case "k_f4":
								state.assignedKeys.Add(keyboard.f4Key);
								break;
							case "k_f5":
								state.assignedKeys.Add(keyboard.f5Key);
								break;
							case "k_f6":
								state.assignedKeys.Add(keyboard.f6Key);
								break;
							case "k_f7":
								state.assignedKeys.Add(keyboard.f7Key);
								break;
							case "k_f8":
								state.assignedKeys.Add(keyboard.f8Key);
								break;
							case "k_f9":
								state.assignedKeys.Add(keyboard.f9Key);
								break;
							case "k_f10":
								state.assignedKeys.Add(keyboard.f10Key);
								break;
							case "k_f11":
								state.assignedKeys.Add(keyboard.f11Key);
								break;
							case "k_f12":
								state.assignedKeys.Add(keyboard.f12Key);
								break;
							case "k_num0":
								state.assignedKeys.Add(keyboard.numpad0Key);
								break;
							case "k_num1":
								state.assignedKeys.Add(keyboard.numpad1Key);
								break;
							case "k_num2":
								state.assignedKeys.Add(keyboard.numpad2Key);
								break;
							case "k_num3":
								state.assignedKeys.Add(keyboard.numpad3Key);
								break;
							case "k_num4":
								state.assignedKeys.Add(keyboard.numpad4Key);
								break;
							case "k_num5":
								state.assignedKeys.Add(keyboard.numpad5Key);
								break;
							case "k_num6":
								state.assignedKeys.Add(keyboard.numpad6Key);
								break;
							case "k_num7":
								state.assignedKeys.Add(keyboard.numpad7Key);
								break;
							case "k_num8":
								state.assignedKeys.Add(keyboard.numpad8Key);
								break;
							case "k_num9":
								state.assignedKeys.Add(keyboard.numpad9Key);
								break;
							case "k_numlock":
								state.assignedKeys.Add(keyboard.numLockKey);
								break;
							case "k_divide":
								state.assignedKeys.Add(keyboard.numpadDivideKey);
								break;
							case "k_multiply":
								state.assignedKeys.Add(keyboard.numpadMultiplyKey);
								break;
							case "k_subtract":
								state.assignedKeys.Add(keyboard.numpadMinusKey);
								break;
							case "k_add":
								state.assignedKeys.Add(keyboard.numpadPlusKey);
								break;
							case "k_decimal":
								state.assignedKeys.Add(keyboard.numpadPeriodKey);
								break;
							case "k_numEnter":
								state.assignedKeys.Add(keyboard.numpadEnterKey);
								break;
							case "k_leftShift":
								state.assignedKeys.Add(keyboard.leftShiftKey);
								break;
							case "k_rightShift":
								state.assignedKeys.Add(keyboard.rightShiftKey);
								break;
							case "k_leftCtrl":
								state.assignedKeys.Add(keyboard.leftCtrlKey);
								break;
							case "k_rightCtrl":
								state.assignedKeys.Add(keyboard.rightCtrlKey);
								break;
							case "k_leftAlt":
								state.assignedKeys.Add(keyboard.leftAltKey);
								break;
							case "k_rightAlt":
								state.assignedKeys.Add(keyboard.rightAltKey);
								break;
							case "k_capsLock":
								state.assignedKeys.Add(keyboard.capsLockKey);
								break;
							case "k_tab":
								state.assignedKeys.Add(keyboard.tabKey);
								break;
							case "k_space":
								state.assignedKeys.Add(keyboard.spaceKey);
								break;
							case "k_enter":
								state.assignedKeys.Add(keyboard.enterKey);
								break;
							case "k_escape":
								state.assignedKeys.Add(keyboard.escapeKey);
								break;
							case "k_backspace":
								state.assignedKeys.Add(keyboard.backspaceKey);
								break;
							case "k_insert":
								state.assignedKeys.Add(keyboard.insertKey);
								break;
							case "k_delete":
								state.assignedKeys.Add(keyboard.deleteKey);
								break;
							case "k_home":
								state.assignedKeys.Add(keyboard.homeKey);
								break;
							case "k_end":
								state.assignedKeys.Add(keyboard.endKey);
								break;
							case "k_pageUp":
								state.assignedKeys.Add(keyboard.pageUpKey);
								break;
							case "k_pageDown":
								state.assignedKeys.Add(keyboard.pageDownKey);
								break;
							case "k_pause":
								state.assignedKeys.Add(keyboard.pauseKey);
								break;
							case "k_menu":
								state.assignedKeys.Add(keyboard.contextMenuKey);
								break;
							case "k_printScreen":
								state.assignedKeys.Add(keyboard.printScreenKey);
								break;
							case "k_scrollLock":
								state.assignedKeys.Add(keyboard.scrollLockKey);
								break;
							case "k_upArrow":
								state.assignedKeys.Add(keyboard.upArrowKey);
								break;
							case "k_downArrow":
								state.assignedKeys.Add(keyboard.downArrowKey);
								break;
							case "k_leftArrow":
								state.assignedKeys.Add(keyboard.leftArrowKey);
								break;
							case "k_rightArrow":
								state.assignedKeys.Add(keyboard.rightArrowKey);
								break;
							case "k_comma":
								state.assignedKeys.Add(keyboard.commaKey);
								break;
							case "k_period":
								state.assignedKeys.Add(keyboard.periodKey);
								break;
							case "k_slash":
								state.assignedKeys.Add(keyboard.slashKey);
								break;
							case "k_backslash":
								state.assignedKeys.Add(keyboard.backslashKey);
								break;
							case "k_semicolon":
								state.assignedKeys.Add(keyboard.semicolonKey);
								break;
							case "k_quote":
								state.assignedKeys.Add(keyboard.quoteKey);
								break;
							case "k_leftBracket":
								state.assignedKeys.Add(keyboard.leftBracketKey);
								break;
							case "k_rightBracket":
								state.assignedKeys.Add(keyboard.rightBracketKey);
								break;
							case "k_minus":
								state.assignedKeys.Add(keyboard.minusKey);
								break;
							case "k_equals":
								state.assignedKeys.Add(keyboard.equalsKey);
								break;
							case "k_backtick":
								state.assignedKeys.Add(keyboard.backquoteKey);
								break;
							default:
								Logger.RError("Unknown key: " + key);
								break;
							}
						}
					}
					else if (key.StartsWith("m_") && mouse != null)
					{
						switch (key)
						{
						case "m_left":
							state.assignedButtons.Add(mouse.leftButton);
							break;
						case "m_middle":
							state.assignedButtons.Add(mouse.middleButton);
							break;
						case "m_right":
							state.assignedButtons.Add(mouse.rightButton);
							break;
						case "m_wheel_up_as_button":
							state.assignedDeltaControlAsButtons_y.Add(mouse.scroll);
							break;
						case "m_wheel_down_as_button":
							state.assignedDeltaControlAsButtons_ny.Add(mouse.scroll);
							break;
						case "m_back":
							state.assignedButtons.Add(mouse.backButton);
							break;
						case "m_forward":
							state.assignedButtons.Add(mouse.forwardButton);
							break;
						default:
							Logger.RError("Unknown key: " + key);
							break;
						}
					}
				});
				if (string.IsNullOrEmpty(datum.gamepadKeys))
				{
					continue;
				}
				datum.gamepadKeys.Split(',').ToList().ForEach(delegate(string key)
				{
					if (key.StartsWith("p_") && gamepad != null)
					{
						switch (key)
						{
						case "p_a":
							state.assignedButtons.Add(gamepad.aButton);
							break;
						case "p_b":
							state.assignedButtons.Add(gamepad.bButton);
							break;
						case "p_x":
							state.assignedButtons.Add(gamepad.xButton);
							break;
						case "p_y":
							state.assignedButtons.Add(gamepad.yButton);
							break;
						case "p_east":
							state.assignedButtons.Add(gamepad.buttonEast);
							break;
						case "p_west":
							state.assignedButtons.Add(gamepad.buttonWest);
							break;
						case "p_north":
							state.assignedButtons.Add(gamepad.buttonNorth);
							break;
						case "p_south":
							state.assignedButtons.Add(gamepad.buttonSouth);
							break;
						case "p_select":
							state.assignedButtons.Add(gamepad.selectButton);
							break;
						case "p_start":
							state.assignedButtons.Add(gamepad.startButton);
							break;
						case "p_lb":
							state.assignedButtons.Add(gamepad.leftShoulder);
							break;
						case "p_rb":
							state.assignedButtons.Add(gamepad.rightShoulder);
							break;
						case "p_lt":
							state.assignedButtons.Add(gamepad.leftTrigger);
							break;
						case "p_rt":
							state.assignedButtons.Add(gamepad.rightTrigger);
							break;
						case "p_lsb":
							state.assignedButtons.Add(gamepad.leftStickButton);
							break;
						case "p_rsb":
							state.assignedButtons.Add(gamepad.rightStickButton);
							break;
						case "p_dpad_up":
							state.assignedButtons.Add(gamepad.dpad.up);
							break;
						case "p_dpad_down":
							state.assignedButtons.Add(gamepad.dpad.down);
							break;
						case "p_dpad_left":
							state.assignedButtons.Add(gamepad.dpad.left);
							break;
						case "p_dpad_right":
							state.assignedButtons.Add(gamepad.dpad.right);
							break;
						default:
							Logger.RError("Unknown gamepad key: " + key);
							break;
						}
					}
				});
			}
		}

		public static string GetPressedGamepadKeyString()
		{
			Gamepad current = Gamepad.current;
			if (current == null)
			{
				return null;
			}
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				return "p_a";
			}
			if (current.aButton.wasPressedThisFrame)
			{
				return "p_a";
			}
			if (current.bButton.wasPressedThisFrame)
			{
				return "p_b";
			}
			if (current.xButton.wasPressedThisFrame)
			{
				return "p_x";
			}
			if (current.yButton.wasPressedThisFrame)
			{
				return "p_y";
			}
			if (current.buttonEast.wasPressedThisFrame)
			{
				return "p_east";
			}
			if (current.buttonWest.wasPressedThisFrame)
			{
				return "p_west";
			}
			if (current.buttonNorth.wasPressedThisFrame)
			{
				return "p_north";
			}
			if (current.buttonSouth.wasPressedThisFrame)
			{
				return "p_south";
			}
			if (current.selectButton.wasPressedThisFrame)
			{
				return "p_select";
			}
			if (current.startButton.wasPressedThisFrame)
			{
				return "p_start";
			}
			if (current.leftShoulder.wasPressedThisFrame)
			{
				return "p_lb";
			}
			if (current.rightShoulder.wasPressedThisFrame)
			{
				return "p_rb";
			}
			if (current.leftTrigger.wasPressedThisFrame)
			{
				return "p_lt";
			}
			if (current.rightTrigger.wasPressedThisFrame)
			{
				return "p_rt";
			}
			if (current.leftStickButton.wasPressedThisFrame)
			{
				return "p_lsb";
			}
			if (current.rightStickButton.wasPressedThisFrame)
			{
				return "p_rsb";
			}
			if (current.dpad.up.wasPressedThisFrame)
			{
				return "p_dpad_up";
			}
			if (current.dpad.down.wasPressedThisFrame)
			{
				return "p_dpad_down";
			}
			if (current.dpad.left.wasPressedThisFrame)
			{
				return "p_dpad_left";
			}
			if (current.dpad.right.wasPressedThisFrame)
			{
				return "p_dpad_right";
			}
			return null;
		}

		public static string GetPressedKeyString()
		{
			Keyboard current = Keyboard.current;
			if (current == null)
			{
				return null;
			}
			if (current.aKey.wasPressedThisFrame)
			{
				return "k_a";
			}
			if (current.bKey.wasPressedThisFrame)
			{
				return "k_b";
			}
			if (current.cKey.wasPressedThisFrame)
			{
				return "k_c";
			}
			if (current.dKey.wasPressedThisFrame)
			{
				return "k_d";
			}
			if (current.eKey.wasPressedThisFrame)
			{
				return "k_e";
			}
			if (current.fKey.wasPressedThisFrame)
			{
				return "k_f";
			}
			if (current.gKey.wasPressedThisFrame)
			{
				return "k_g";
			}
			if (current.hKey.wasPressedThisFrame)
			{
				return "k_h";
			}
			if (current.iKey.wasPressedThisFrame)
			{
				return "k_i";
			}
			if (current.jKey.wasPressedThisFrame)
			{
				return "k_j";
			}
			if (current.kKey.wasPressedThisFrame)
			{
				return "k_k";
			}
			if (current.lKey.wasPressedThisFrame)
			{
				return "k_l";
			}
			if (current.mKey.wasPressedThisFrame)
			{
				return "k_m";
			}
			if (current.nKey.wasPressedThisFrame)
			{
				return "k_n";
			}
			if (current.oKey.wasPressedThisFrame)
			{
				return "k_o";
			}
			if (current.pKey.wasPressedThisFrame)
			{
				return "k_p";
			}
			if (current.qKey.wasPressedThisFrame)
			{
				return "k_q";
			}
			if (current.rKey.wasPressedThisFrame)
			{
				return "k_r";
			}
			if (current.sKey.wasPressedThisFrame)
			{
				return "k_s";
			}
			if (current.tKey.wasPressedThisFrame)
			{
				return "k_t";
			}
			if (current.uKey.wasPressedThisFrame)
			{
				return "k_u";
			}
			if (current.vKey.wasPressedThisFrame)
			{
				return "k_v";
			}
			if (current.wKey.wasPressedThisFrame)
			{
				return "k_w";
			}
			if (current.xKey.wasPressedThisFrame)
			{
				return "k_x";
			}
			if (current.yKey.wasPressedThisFrame)
			{
				return "k_y";
			}
			if (current.zKey.wasPressedThisFrame)
			{
				return "k_z";
			}
			if (current.digit0Key.wasPressedThisFrame)
			{
				return "k_0";
			}
			if (current.digit1Key.wasPressedThisFrame)
			{
				return "k_1";
			}
			if (current.digit2Key.wasPressedThisFrame)
			{
				return "k_2";
			}
			if (current.digit3Key.wasPressedThisFrame)
			{
				return "k_3";
			}
			if (current.digit4Key.wasPressedThisFrame)
			{
				return "k_4";
			}
			if (current.digit5Key.wasPressedThisFrame)
			{
				return "k_5";
			}
			if (current.digit6Key.wasPressedThisFrame)
			{
				return "k_6";
			}
			if (current.digit7Key.wasPressedThisFrame)
			{
				return "k_7";
			}
			if (current.digit8Key.wasPressedThisFrame)
			{
				return "k_8";
			}
			if (current.digit9Key.wasPressedThisFrame)
			{
				return "k_9";
			}
			if (current.f1Key.wasPressedThisFrame)
			{
				return "k_f1";
			}
			if (current.f2Key.wasPressedThisFrame)
			{
				return "k_f2";
			}
			if (current.f3Key.wasPressedThisFrame)
			{
				return "k_f3";
			}
			if (current.f4Key.wasPressedThisFrame)
			{
				return "k_f4";
			}
			if (current.f5Key.wasPressedThisFrame)
			{
				return "k_f5";
			}
			if (current.f6Key.wasPressedThisFrame)
			{
				return "k_f6";
			}
			if (current.f7Key.wasPressedThisFrame)
			{
				return "k_f7";
			}
			if (current.f8Key.wasPressedThisFrame)
			{
				return "k_f8";
			}
			if (current.f9Key.wasPressedThisFrame)
			{
				return "k_f9";
			}
			if (current.f10Key.wasPressedThisFrame)
			{
				return "k_f10";
			}
			if (current.f11Key.wasPressedThisFrame)
			{
				return "k_f11";
			}
			if (current.f12Key.wasPressedThisFrame)
			{
				return "k_f12";
			}
			if (current.numpad0Key.wasPressedThisFrame)
			{
				return "k_num0";
			}
			if (current.numpad1Key.wasPressedThisFrame)
			{
				return "k_num1";
			}
			if (current.numpad2Key.wasPressedThisFrame)
			{
				return "k_num2";
			}
			if (current.numpad3Key.wasPressedThisFrame)
			{
				return "k_num3";
			}
			if (current.numpad4Key.wasPressedThisFrame)
			{
				return "k_num4";
			}
			if (current.numpad5Key.wasPressedThisFrame)
			{
				return "k_num5";
			}
			if (current.numpad6Key.wasPressedThisFrame)
			{
				return "k_num6";
			}
			if (current.numpad7Key.wasPressedThisFrame)
			{
				return "k_num7";
			}
			if (current.numpad8Key.wasPressedThisFrame)
			{
				return "k_num8";
			}
			if (current.numpad9Key.wasPressedThisFrame)
			{
				return "k_num9";
			}
			if (current.numLockKey.wasPressedThisFrame)
			{
				return "k_numlock";
			}
			if (current.numpadDivideKey.wasPressedThisFrame)
			{
				return "k_divide";
			}
			if (current.numpadMultiplyKey.wasPressedThisFrame)
			{
				return "k_multiply";
			}
			if (current.numpadMinusKey.wasPressedThisFrame)
			{
				return "k_subtract";
			}
			if (current.numpadPlusKey.wasPressedThisFrame)
			{
				return "k_add";
			}
			if (current.numpadPeriodKey.wasPressedThisFrame)
			{
				return "k_decimal";
			}
			if (current.numpadEnterKey.wasPressedThisFrame)
			{
				return "k_numEnter";
			}
			if (current.leftShiftKey.wasPressedThisFrame)
			{
				return "k_leftShift";
			}
			if (current.rightShiftKey.wasPressedThisFrame)
			{
				return "k_rightShift";
			}
			if (current.leftCtrlKey.wasPressedThisFrame)
			{
				return "k_leftCtrl";
			}
			if (current.rightCtrlKey.wasPressedThisFrame)
			{
				return "k_rightCtrl";
			}
			if (current.leftAltKey.wasPressedThisFrame)
			{
				return "k_leftAlt";
			}
			if (current.rightAltKey.wasPressedThisFrame)
			{
				return "k_rightAlt";
			}
			if (current.capsLockKey.wasPressedThisFrame)
			{
				return "k_capsLock";
			}
			if (current.tabKey.wasPressedThisFrame)
			{
				return "k_tab";
			}
			if (current.spaceKey.wasPressedThisFrame)
			{
				return "k_space";
			}
			if (current.enterKey.wasPressedThisFrame)
			{
				return "k_enter";
			}
			if (current.escapeKey.wasPressedThisFrame)
			{
				return "k_escape";
			}
			if (current.backspaceKey.wasPressedThisFrame)
			{
				return "k_backspace";
			}
			if (current.insertKey.wasPressedThisFrame)
			{
				return "k_insert";
			}
			if (current.deleteKey.wasPressedThisFrame)
			{
				return "k_delete";
			}
			if (current.homeKey.wasPressedThisFrame)
			{
				return "k_home";
			}
			if (current.endKey.wasPressedThisFrame)
			{
				return "k_end";
			}
			if (current.pageUpKey.wasPressedThisFrame)
			{
				return "k_pageUp";
			}
			if (current.pageDownKey.wasPressedThisFrame)
			{
				return "k_pageDown";
			}
			if (current.pauseKey.wasPressedThisFrame)
			{
				return "k_pause";
			}
			if (current.contextMenuKey.wasPressedThisFrame)
			{
				return "k_menu";
			}
			if (current.printScreenKey.wasPressedThisFrame)
			{
				return "k_printScreen";
			}
			if (current.scrollLockKey.wasPressedThisFrame)
			{
				return "k_scrollLock";
			}
			if (current.upArrowKey.wasPressedThisFrame)
			{
				return "k_upArrow";
			}
			if (current.downArrowKey.wasPressedThisFrame)
			{
				return "k_downArrow";
			}
			if (current.leftArrowKey.wasPressedThisFrame)
			{
				return "k_leftArrow";
			}
			if (current.rightArrowKey.wasPressedThisFrame)
			{
				return "k_rightArrow";
			}
			if (current.commaKey.wasPressedThisFrame)
			{
				return "k_comma";
			}
			if (current.periodKey.wasPressedThisFrame)
			{
				return "k_period";
			}
			if (current.slashKey.wasPressedThisFrame)
			{
				return "k_slash";
			}
			if (current.backslashKey.wasPressedThisFrame)
			{
				return "k_backslash";
			}
			if (current.semicolonKey.wasPressedThisFrame)
			{
				return "k_semicolon";
			}
			if (current.quoteKey.wasPressedThisFrame)
			{
				return "k_quote";
			}
			if (current.leftBracketKey.wasPressedThisFrame)
			{
				return "k_leftBracket";
			}
			if (current.rightBracketKey.wasPressedThisFrame)
			{
				return "k_rightBracket";
			}
			if (current.minusKey.wasPressedThisFrame)
			{
				return "k_minus";
			}
			if (current.equalsKey.wasPressedThisFrame)
			{
				return "k_equals";
			}
			if (current.backquoteKey.wasPressedThisFrame)
			{
				return "k_backtick";
			}
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				return "m_left";
			}
			if (Mouse.current.middleButton.wasPressedThisFrame)
			{
				return "m_middle";
			}
			if (Mouse.current.rightButton.wasPressedThisFrame)
			{
				return "m_right";
			}
			if (Mouse.current.scroll.ReadValue().y > 0.1f)
			{
				return "m_wheel_up_as_button";
			}
			if (Mouse.current.scroll.ReadValue().y < -0.1f)
			{
				return "m_wheel_down_as_button";
			}
			if (Mouse.current.backButton.wasPressedThisFrame)
			{
				return "m_back";
			}
			if (Mouse.current.forwardButton.wasPressedThisFrame)
			{
				return "m_forward";
			}
			return null;
		}

		public IEnumerator ChangeKeyBindProcess(InputAction action, bool gamepad = false)
		{
			string text;
			while (true)
			{
				text = ((!gamepad) ? GetPressedKeyString() : GetPressedGamepadKeyString());
				if (text == "k_escape")
				{
					text = null;
				}
				if (text != null)
				{
					break;
				}
				yield return null;
			}
			_ = data.data.Find((InputManagerData.Mapping x) => x.action == action).keys;
			if (!DeleteKeyBind(text, gamepad))
			{
				yield break;
			}
			ChangeKeyBind(action, text, gamepad);
			if (action == InputAction.MoveLeft || (action == InputAction.FireWeapon && !gamepad))
			{
				ChangeKeyBind(InputAction.PreviousSpectatorTarget, data.data.Find((InputManagerData.Mapping x) => x.action == InputAction.MoveLeft).keys + "," + data.data.Find((InputManagerData.Mapping x) => x.action == InputAction.FireWeapon).keys);
			}
			else if (action == InputAction.MoveRight || action == InputAction.Aim)
			{
				ChangeKeyBind(InputAction.NextSpectatorTarget, data.data.Find((InputManagerData.Mapping x) => x.action == InputAction.MoveRight).keys + "," + data.data.Find((InputManagerData.Mapping x) => x.action == InputAction.Aim).keys);
			}
			RefreshInputAssign();
			SaveKeyBind();
		}

		public void ResetKeys()
		{
			string value = JsonUtility.ToJson(defaultData);
			PlayerPrefs.SetString(dataKey + keyDataVersion, value);
			value = PlayerPrefs.GetString(dataKey + keyDataVersion);
			JsonUtility.FromJsonOverwrite(value, data);
			RefreshInputAssign();
			SaveKeyBind();
		}

		private void SaveKeyBind()
		{
			PlayerPrefs.SetString(dataKey + keyDataVersion, JsonUtility.ToJson(data));
		}

		public string GetDescription(InputAction action)
		{
			return data.data.Find((InputManagerData.Mapping x) => x.action == action).description;
		}

		public string GetKeyBind(InputAction action, bool gamepad = false)
		{
			if (gamepad)
			{
				return data.data.Find((InputManagerData.Mapping x) => x.action == action).gamepadKeys;
			}
			return data.data.Find((InputManagerData.Mapping x) => x.action == action).keys;
		}

		public void ChangeKeyBind(InputAction action, string key, bool gamepad = false)
		{
			foreach (InputManagerData.Mapping datum in data.data)
			{
				if (datum.action == action)
				{
					if (gamepad)
					{
						datum.gamepadKeys = key;
					}
					else
					{
						datum.keys = key;
					}
				}
			}
		}

		public void DeleteKeyBind(InputAction action, bool gamepad = false)
		{
			foreach (InputManagerData.Mapping datum in data.data)
			{
				if (datum.action == action)
				{
					if (datum.staticKey)
					{
						Logger.RError("Cannot delete static key");
						break;
					}
					if (gamepad)
					{
						datum.gamepadKeys = "";
					}
					else
					{
						datum.keys = "";
					}
				}
			}
		}

		public bool DeleteKeyBind(string key, bool gamepad = false)
		{
			foreach (InputManagerData.Mapping datum in data.data)
			{
				if (!(datum.keys == key) && !(datum.gamepadKeys == key))
				{
					continue;
				}
				if (gamepad)
				{
					if (datum.gamepadStaticKey)
					{
						if (datum.notUsed)
						{
							return true;
						}
						Logger.RLog("Cannot delete static key" + (gamepad ? "gamepad" : "keyboard"));
						return false;
					}
				}
				else if (datum.staticKey)
				{
					if (datum.notUsed)
					{
						return true;
					}
					Logger.RLog("Cannot delete static key" + (gamepad ? "gamepad" : "keyboard"));
					return false;
				}
				if (gamepad)
				{
					datum.gamepadKeys = " ";
				}
				else
				{
					datum.keys = " ";
				}
			}
			return true;
		}
	}
}
