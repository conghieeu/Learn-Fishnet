using System;
using UnityEngine;

namespace Mimic.Audio
{
	[Serializable]
	public class AudioEnvironmentSettings
	{
		[Header("Gun Tail")]
		public string pcActionGunTailIndoorParamName = "PC_Action_Gun_Tail_Indoor.Attenuation";

		public float pcActionGunTailIndoorAttenuation;

		public string pcActionGunTailOutdoorParamName = "PC_Action_Gun_Tail_Outdoor.Attenuation";

		public float pcActionGunTailOutdoorAttenuation;

		public string itemExplosiveTailIndoorParamName = "Item_Explosive_Tail_Indoor.Attenuation";

		public float itemExplosiveTailIndoorAttenuation;

		public string itemExplosiveTailOutdoorParamName = "Item_Explosive_Tail_Outdoor.Attenuation";

		public float itemExplosiveTailOutdoorAttenuation;

		[Header("Ambience")]
		public string ambienceIndoorParamName = "Ambience_Indoor.Attenuation";

		public float ambienceIndoorAttenuation;

		public string ambienceOutdoorParamName = "Ambience_Outdoor.Attenuation";

		public float ambienceOutdoorAttenuation;

		[Header("Music")]
		public string musicIndoorParamName = "Music_Indoor.Attenuation";

		public float musicIndoorAttenuation;

		public string musicOutdoorParamName = "Music_Outdoor.Attenuation";

		public float musicOutdoorAttenuation;

		[Header("Reverb")]
		public string reverbIndoorParamName = "Reverb_Indoor.Attenuation";

		public float reverbIndoorAttenuation;
	}
}
