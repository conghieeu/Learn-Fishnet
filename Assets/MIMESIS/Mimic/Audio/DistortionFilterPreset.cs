using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "DistortionFilterPreset", menuName = "Mimic/Audio/Filter/DistortionFilterPreset")]
	public class DistortionFilterPreset : ScriptableObject
	{
		public float distortionLevel = 0.5f;
	}
}
