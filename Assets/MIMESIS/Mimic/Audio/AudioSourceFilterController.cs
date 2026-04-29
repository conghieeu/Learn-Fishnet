using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Mimic.Audio
{
	public class AudioSourceFilterController : MonoBehaviour
	{
		public AudioLowPassFilter LowPass;

		public AudioHighPassFilter HighPass;

		public AudioDistortionFilter Distortion;

		public AudioChorusFilter Chorus;

		public AudioReverbFilter Reverb;

		public AudioEchoFilter Echo;

		public AudioAmplifierFilter Amplifier;

		private AudioSource? _audioSource;

		private void Awake()
		{
			_audioSource = base.gameObject.GetComponent<AudioSource>();
			if (_audioSource == null)
			{
				Logger.RError("[AudioSourceFilterController] Awake() - AudioSource is null");
			}
		}

		public void Reset(bool inResetPitch)
		{
			try
			{
				if (!(this == null) && !(base.gameObject == null) && !(_audioSource == null))
				{
					if (LowPass != null)
					{
						LowPass.enabled = false;
					}
					if (HighPass != null)
					{
						HighPass.enabled = false;
					}
					if (Distortion != null)
					{
						Distortion.enabled = false;
					}
					if (Chorus != null)
					{
						Chorus.enabled = false;
					}
					if (Reverb != null)
					{
						Reverb.enabled = false;
					}
					if (Echo != null)
					{
						Echo.enabled = false;
					}
					if (Amplifier != null)
					{
						Amplifier.IsEnabled = false;
					}
					if (inResetPitch)
					{
						_audioSource.outputAudioMixerGroup = Hub.s?.tableman?.voicePitchShift?.GetDefaultMixerGroup();
					}
					_audioSource.maxDistance = 20f;
				}
			}
			catch (Exception ex)
			{
				Logger.RError("[AudioSourceFilterController] Reset() - Exception: " + ex.Message);
			}
		}

		public bool IsPlaying()
		{
			if (_audioSource == null)
			{
				return false;
			}
			return _audioSource.isPlaying;
		}

		public void Stop()
		{
			if (!(_audioSource == null))
			{
				_audioSource.Stop();
			}
		}

		public void SetOutputMixer(AudioMixerGroup? inMixerGroup)
		{
			if (_audioSource != null && inMixerGroup != null)
			{
				_audioSource.outputAudioMixerGroup = inMixerGroup;
			}
		}

		public void ApplyFilters(VoiceEffectPreset? inPreset)
		{
			if (inPreset == null)
			{
				Logger.RError("[AudioSourceFilterController] ApplyFilters() - VoiceEffectPreset is null");
				return;
			}
			SetLowPass(inPreset.LowPass);
			SetHighPass(inPreset.HighPass);
			SetDistortion(inPreset.Distortion);
			SetChorus(inPreset.Chorus);
			SetReverb(inPreset.Reverb);
			SetEcho(inPreset.Echo);
			SetAmplifier(inPreset.Amplifier);
			if (_audioSource != null)
			{
				_audioSource.maxDistance = inPreset.distance;
			}
		}

		public void SetDistance(float inDistance)
		{
			if (_audioSource != null)
			{
				_audioSource.maxDistance = inDistance;
			}
		}

		public void Set3DEffect(bool inEnabled)
		{
			if (_audioSource != null)
			{
				_audioSource.spatialBlend = (inEnabled ? 1 : 0);
			}
		}

		public void PlayAudioClip(AudioClip clip, bool isLoop)
		{
			if (_audioSource != null)
			{
				_audioSource.clip = clip;
				_audioSource.loop = isLoop;
				if (clip != null)
				{
					_audioSource.Play();
				}
			}
		}

		public void SetMimicVoiceEcho(bool inEnabled)
		{
			if (Echo != null)
			{
				Echo.enabled = inEnabled;
				if (inEnabled)
				{
					Echo.delay = 100f;
					Echo.decayRatio = 0.4f;
					Echo.wetMix = 1f;
					Echo.dryMix = 0.5f;
				}
			}
		}

		private void SetLowPass(LowPassFilterPreset? inFilter)
		{
			if (inFilter != null)
			{
				LowPass.enabled = true;
				LowPass.cutoffFrequency = inFilter.cutoffFrequency;
				LowPass.lowpassResonanceQ = inFilter.lowpassResonanceQ;
			}
			else
			{
				LowPass.enabled = false;
			}
		}

		private void SetHighPass(HighPassFilterPreset? inFilter)
		{
			if (inFilter != null)
			{
				HighPass.enabled = true;
				HighPass.cutoffFrequency = inFilter.cutoffFrequency;
				HighPass.highpassResonanceQ = inFilter.highpassResonanceQ;
			}
			else
			{
				HighPass.enabled = false;
			}
		}

		private void SetDistortion(DistortionFilterPreset? inFilter)
		{
			if (inFilter != null)
			{
				Distortion.enabled = true;
				Distortion.distortionLevel = inFilter.distortionLevel;
			}
			else
			{
				Distortion.enabled = false;
			}
		}

		private void SetChorus(ChorusFilterPreset? inFilter)
		{
			if (inFilter != null)
			{
				Chorus.enabled = true;
				Chorus.dryMix = inFilter.dryMix;
				Chorus.wetMix1 = inFilter.wetMix1;
				Chorus.wetMix2 = inFilter.wetMix2;
				Chorus.wetMix3 = inFilter.wetMix3;
				Chorus.delay = inFilter.delay;
				Chorus.rate = inFilter.rate;
				Chorus.depth = inFilter.depth;
			}
			else
			{
				Chorus.enabled = false;
			}
		}

		private void SetReverb(ReverbFilterPreset? inFilter)
		{
			if (inFilter != null)
			{
				Reverb.enabled = true;
				if (inFilter.reverbPreset != AudioReverbPreset.Off)
				{
					Reverb.reverbPreset = inFilter.reverbPreset;
					return;
				}
				Reverb.reverbPreset = AudioReverbPreset.Off;
				Reverb.dryLevel = inFilter.dryLevel;
				Reverb.room = inFilter.room;
				Reverb.roomHF = inFilter.roomHF;
				Reverb.roomLF = inFilter.roomLF;
				Reverb.decayTime = inFilter.decayTime;
				Reverb.decayHFRatio = inFilter.decayHFRatio;
				Reverb.reflectionsLevel = inFilter.reflectionsLevel;
				Reverb.reflectionsDelay = inFilter.reflectionsDelay;
				Reverb.reverbLevel = inFilter.reverbLevel;
				Reverb.reverbDelay = inFilter.reverbDelay;
				Reverb.hfReference = inFilter.hfReference;
				Reverb.lfReference = inFilter.lfReference;
				Reverb.diffusion = inFilter.diffusion;
				Reverb.density = inFilter.density;
			}
			else
			{
				Reverb.enabled = false;
			}
		}

		private void SetEcho(EchoFilterPreset? inFilter)
		{
			if (inFilter != null)
			{
				Echo.enabled = true;
				Echo.delay = inFilter.delay;
				Echo.decayRatio = inFilter.decayRatio;
				Echo.wetMix = inFilter.wetMix;
				Echo.dryMix = inFilter.dryMix;
			}
			else
			{
				Echo.enabled = false;
			}
		}

		private void SetAmplifier(AmplifierFilterPreset? inFilter)
		{
			if (inFilter != null)
			{
				Amplifier.IsEnabled = true;
				Amplifier.SetGain(inFilter.gain);
			}
			else
			{
				Amplifier.IsEnabled = false;
				Amplifier.SetGain(1f);
			}
		}
	}
}
