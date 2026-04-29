using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "LowPassFilterPreset", menuName = "Component/ReluGames/Audio/LowPassFilterPreset")]
	public class LowPassFilterPreset : ScriptableObject
	{
		public float cutoffFrequency = 5000f;

		public float lowpassResonanceQ = 1f;
	}
}
