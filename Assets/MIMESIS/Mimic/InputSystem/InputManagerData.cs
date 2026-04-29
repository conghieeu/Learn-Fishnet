using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mimic.InputSystem
{
	[Serializable]
	[CreateAssetMenu(fileName = "InputManagerData", menuName = "_Mimic/InputManagerData", order = 0)]
	public class InputManagerData : ScriptableObject
	{
		[Serializable]
		public class Mapping
		{
			public string description;

			public string actionName;

			public InputAction action;

			public string keys;

			public bool staticKey;

			public bool notUsed;

			public string gamepadKeys;

			public bool gamepadStaticKey;

			public Mapping(Mapping original)
			{
				description = original.description;
				actionName = original.actionName;
				action = original.action;
				keys = original.keys;
				gamepadKeys = original.gamepadKeys;
				staticKey = original.staticKey;
				notUsed = original.notUsed;
			}
		}

		public List<Mapping> data = new List<Mapping>();
	}
}
