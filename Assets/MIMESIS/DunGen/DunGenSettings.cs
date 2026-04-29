using System;
using DunGen.Tags;
using UnityEngine;

namespace DunGen
{
	public sealed class DunGenSettings : ScriptableObject
	{
		private static DunGenSettings instance;

		[SerializeField]
		private DoorwaySocket defaultSocket;

		[SerializeField]
		private TagManager tagManager = new TagManager();

		public static DunGenSettings Instance
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}
				instance = FindOrCreateInstanceAsset();
				return instance;
			}
		}

		public DoorwaySocket DefaultSocket => defaultSocket;

		public TagManager TagManager => tagManager;

		public static DunGenSettings FindOrCreateInstanceAsset()
		{
			instance = Resources.Load<DunGenSettings>("DunGen Settings");
			if (instance == null)
			{
				throw new Exception("No instance of DunGen settings was found.");
			}
			return instance;
		}
	}
}
