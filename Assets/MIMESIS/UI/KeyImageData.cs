using System;
using System.Collections.Generic;
using Mimic.InputSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyImageData", menuName = "Scriptable Objects/KeyImageData")]
public class KeyImageData : ScriptableObject
{
	[Serializable]
	public class KeyImage
	{
		public string keyName;

		public Sprite keyImage;
	}

	public List<KeyImage> keyImages = new List<KeyImage>();

	public Sprite emptyKeyImage;

	public Sprite GetKeyImage(string keyName)
	{
		foreach (KeyImage keyImage in keyImages)
		{
			if (keyImage.keyName.Equals(keyName, StringComparison.OrdinalIgnoreCase))
			{
				return keyImage.keyImage;
			}
		}
		Logger.RWarn("Key image not found for key name: " + keyName);
		return emptyKeyImage;
	}

	public Sprite GetKeyImage(InputAction action)
	{
		Sprite sprite = null;
		InputManagerData.Mapping mapping = Hub.s.inputman.data.data.Find((InputManagerData.Mapping x) => x.action == action);
		if (Hub.s.inputman.lastInputDevice == InputDevice.KeyboardMouse)
		{
			return GetKeyImage(mapping.keys);
		}
		return GetKeyImage(mapping.gamepadKeys);
	}
}
