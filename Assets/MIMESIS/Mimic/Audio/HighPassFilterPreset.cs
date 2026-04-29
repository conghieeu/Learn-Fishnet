using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "HighPassFilterPreset", menuName = "Mimic/Audio/Filter/HighPassFilterPreset")]
	public class HighPassFilterPreset : ScriptableObject
	{
		public float cutoffFrequency = 200f;

		public float highpassResonanceQ = 1f;
	}
}
