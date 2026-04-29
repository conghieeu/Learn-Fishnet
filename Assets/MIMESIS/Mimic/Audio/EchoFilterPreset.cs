using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "EchoFilterPreset", menuName = "Mimic/Audio/Filter/EchoFilterPreset")]
	public class EchoFilterPreset : ScriptableObject
	{
		public float delay = 500f;

		public float decayRatio = 0.5f;

		public float wetMix = 1f;

		public float dryMix = 1f;
	}
}
