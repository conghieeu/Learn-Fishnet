using UnityEngine;

namespace Mimic.Audio
{
	public class AudioFilterReference
	{
		public AudioLowPassFilter LowPass;

		public AudioHighPassFilter HighPass;

		public AudioDistortionFilter Distortion;

		public AudioChorusFilter Chorus;

		public AudioReverbFilter Reverb;

		public AudioEchoFilter Echo;

		public AudioAmplifierFilter Amplifier;

		public void SetEnabled(bool enabled)
		{
			if (LowPass != null)
			{
				LowPass.enabled = enabled;
			}
			if (HighPass != null)
			{
				HighPass.enabled = enabled;
			}
			if (Distortion != null)
			{
				Distortion.enabled = enabled;
			}
			if (Chorus != null)
			{
				Chorus.enabled = enabled;
			}
			if (Reverb != null)
			{
				Reverb.enabled = enabled;
			}
			if (Echo != null)
			{
				Echo.enabled = enabled;
			}
			if (Amplifier != null)
			{
				Amplifier.IsEnabled = enabled;
			}
		}
	}
}
