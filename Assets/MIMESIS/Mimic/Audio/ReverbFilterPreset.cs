using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "ReverbFilterPreset", menuName = "Component/ReluGames/Audio/ReverbFilterPreset")]
	public class ReverbFilterPreset : ScriptableObject
	{
		public AudioReverbPreset reverbPreset;

		public float dryLevel;

		public float room = -1000f;

		public float roomHF = -100f;

		public float roomLF = -100f;

		public float decayTime = 1.5f;

		public float decayHFRatio = 0.5f;

		public float reflectionsLevel = -10000f;

		public float reflectionsDelay = 0.02f;

		public float reverbLevel;

		public float reverbDelay = 0.04f;

		public float hfReference = 5000f;

		public float lfReference = 250f;

		public float diffusion = 100f;

		public float density = 100f;
	}
}
