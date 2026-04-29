using System.Collections.Generic;
using Mimic.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPrefab_GamepadEmote : UIPrefabScript
{
	public const string UEID_Emote1 = "Emote1";

	public const string UEID_Emote2 = "Emote2";

	public const string UEID_Emote3 = "Emote3";

	public const string UEID_Emote4 = "Emote4";

	public const string UEID_Emote5 = "Emote5";

	public const string UEID_Emote6 = "Emote6";

	public const string UEID_Emote7 = "Emote7";

	public const string UEID_Emote8 = "Emote8";

	public const string UEID_Emote9 = "Emote9";

	public const string UEID_Emote10 = "Emote10";

	private Transform _UE_Emote1;

	private Transform _UE_Emote2;

	private Transform _UE_Emote3;

	private Transform _UE_Emote4;

	private Transform _UE_Emote5;

	private Transform _UE_Emote6;

	private Transform _UE_Emote7;

	private Transform _UE_Emote8;

	private Transform _UE_Emote9;

	private Transform _UE_Emote10;

	private int currentSelected = -1;

	private const int EmoteCount = 10;

	private bool isLBPressed;

	public bool isV2;

	public Transform UE_Emote1 => _UE_Emote1 ?? (_UE_Emote1 = PickTransform("Emote1"));

	public Transform UE_Emote2 => _UE_Emote2 ?? (_UE_Emote2 = PickTransform("Emote2"));

	public Transform UE_Emote3 => _UE_Emote3 ?? (_UE_Emote3 = PickTransform("Emote3"));

	public Transform UE_Emote4 => _UE_Emote4 ?? (_UE_Emote4 = PickTransform("Emote4"));

	public Transform UE_Emote5 => _UE_Emote5 ?? (_UE_Emote5 = PickTransform("Emote5"));

	public Transform UE_Emote6 => _UE_Emote6 ?? (_UE_Emote6 = PickTransform("Emote6"));

	public Transform UE_Emote7 => _UE_Emote7 ?? (_UE_Emote7 = PickTransform("Emote7"));

	public Transform UE_Emote8 => _UE_Emote8 ?? (_UE_Emote8 = PickTransform("Emote8"));

	public Transform UE_Emote9 => _UE_Emote9 ?? (_UE_Emote9 = PickTransform("Emote9"));

	public Transform UE_Emote10 => _UE_Emote10 ?? (_UE_Emote10 = PickTransform("Emote10"));

	private void Update()
	{
		if (Hub.s == null)
		{
			return;
		}
		Gamepad current = Gamepad.current;
		if (current == null)
		{
			return;
		}
		if (Hub.s.inputman.isPressed(Mimic.InputSystem.InputAction.EmotePanel))
		{
			isLBPressed = true;
		}
		if (Hub.s.inputman.wasRelesedThisFrame(Mimic.InputSystem.InputAction.EmotePanel))
		{
			if (GetEmoteAction() != Mimic.InputSystem.InputAction.EmotePanel)
			{
				Hub.s.inputman.PressKey(GetEmoteAction());
			}
			isLBPressed = false;
			currentSelected = -1;
			Highlight(-1);
			Hide();
			Hub.s.inputman.isGamepadEmoteActive = false;
		}
		else
		{
			if (!isLBPressed)
			{
				return;
			}
			Hub.s.inputman.isGamepadEmoteActive = true;
			Vector2 vector = current.rightStick.ReadValue();
			if (vector.magnitude > 0.3f)
			{
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f - 90f;
				float num2 = 36f;
				int num3 = Mathf.FloorToInt(Mathf.Repeat(Mathf.Repeat(0f - num, 360f) + num2 * 0.5f - 0.0001f, 360f) / num2);
				if (num3 != currentSelected)
				{
					currentSelected = num3;
					Highlight(num3);
				}
			}
			else if (!isV2)
			{
				Highlight(-1);
				currentSelected = -1;
			}
		}
	}

	private void Highlight(int index)
	{
		for (int i = 0; i < 10; i++)
		{
			Transform emojiTransform = GetEmojiTransform(i, "normal");
			Transform emojiTransform2 = GetEmojiTransform(i, "selected");
			if ((bool)emojiTransform)
			{
				emojiTransform.gameObject.SetActive(index != i);
			}
			if ((bool)emojiTransform2)
			{
				emojiTransform2.gameObject.SetActive(index == i);
			}
		}
	}

	private Transform GetEmojiTransform(int idx, string type)
	{
		return idx switch
		{
			0 => UE_Emote1.Find(type), 
			1 => UE_Emote2.Find(type), 
			2 => UE_Emote3.Find(type), 
			3 => UE_Emote4.Find(type), 
			4 => UE_Emote5.Find(type), 
			5 => UE_Emote6.Find(type), 
			6 => UE_Emote7.Find(type), 
			7 => UE_Emote8.Find(type), 
			8 => UE_Emote9.Find(type), 
			9 => UE_Emote10.Find(type), 
			_ => null, 
		};
	}

	private Mimic.InputSystem.InputAction GetEmoteAction()
	{
		List<Mimic.InputSystem.InputAction> list = Hub.s.tableman.emote.CollectInputActionList();
		if (currentSelected == -1)
		{
			return Mimic.InputSystem.InputAction.EmotePanel;
		}
		if (currentSelected >= list.Count)
		{
			return Mimic.InputSystem.InputAction.EmotePanel;
		}
		return list[currentSelected];
	}

	public int GetSelectedIndex()
	{
		return currentSelected;
	}
}
