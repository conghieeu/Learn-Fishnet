using UnityEngine;

namespace Mimic.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public sealed class AudioAmplifierFilter : MonoBehaviour
	{
		[SerializeField]
		[Range(0.5f, 3f)]
		private float gain = 1f;

		public bool IsEnabled;

		public void SetGain(float value)
		{
			gain = Mathf.Clamp(value, 0.5f, 3f);
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (IsEnabled)
			{
				for (int i = 0; i < data.Length; i++)
				{
					data[i] *= gain;
				}
			}
		}
	}
}
