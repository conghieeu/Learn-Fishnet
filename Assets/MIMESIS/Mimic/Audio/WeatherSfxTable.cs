using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "WeatherSfxTable", menuName = "_Mimic/Audio/WeatherSfxTable")]
	public class WeatherSfxTable : ScriptableObject
	{
		[Serializable]
		private class WeatherSfx
		{
			public SkyAndWeatherSystem.eWeatherPreset weather;

			public string sfxId = string.Empty;
		}

		public string weatherBusName = "Weather";

		public DynamicSoundGroupCreator weatherSoundGroupCreator;

		public string outdoorBgmBusName = "Ambience_Music";

		public string outdoorBgmSfxId = "BGM_Outdoor";

		public DynamicSoundGroupCreator outdoorBgmSoundGroupCreator;

		[Header("SFX Keys")]
		[SerializeField]
		private List<WeatherSfx> weatherSfxs;

		public bool TryGetSfxId(SkyAndWeatherSystem.eWeatherPreset weather, out string? sfxId)
		{
			foreach (WeatherSfx weatherSfx in weatherSfxs)
			{
				if (weatherSfx.weather == weather)
				{
					sfxId = weatherSfx.sfxId;
					return true;
				}
			}
			sfxId = null;
			return false;
		}
	}
}
