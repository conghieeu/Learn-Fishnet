using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "AmplifierFilterPreset", menuName = "Mimic/Audio/Filter/AmplifierFilterPreset")]
	public class AmplifierFilterPreset : ScriptableObject
	{
		public float gain = 10f;
	}
}
