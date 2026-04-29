using System;
using UnityEngine;

namespace Mimic.Audio
{
	[Serializable]
	[CreateAssetMenu(fileName = "ChorusFilterPreset", menuName = "Mimic/Audio/Filter/ChorusFilterPreset")]
	public class ChorusFilterPreset : ScriptableObject
	{
		public float dryMix = 0.5f;

		public float wetMix1 = 0.5f;

		public float wetMix2 = 0.5f;

		public float wetMix3 = 0.5f;

		public float delay = 40f;

		public float rate = 0.8f;

		public float depth = 0.3f;
	}
}
