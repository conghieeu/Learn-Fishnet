using System;
using Dissonance;
using Dissonance.Audio.Codecs.Opus;
using Dissonance.Config;
using Dissonance.Integrations.FishNet;
using Mimic.Actors;
using NAudio.Wave;
using UnityEngine;

namespace Mimic.Voice.SpeechSystem
{
	[RequireComponent(typeof(DissonanceFishNetComms))]
	public class SpeechEventRecorder : BaseMicrophoneSubscriber
	{
		[SerializeField]
		[Tooltip("저장할 음성의 시간 범위(초)입니다. 이 범위보다 긴 음성은 새로운 기록으로 다시 저장되고, 짧은 음성은 기록을 저장하지 않습니다.")]
		private MinMaxFloatRange sampleDurationRange = new MinMaxFloatRange(0.7f, 7f, 0f, 15f);

		private int channels;

		private int sampleRate;

		private float[] buffer;

		private int writeHead;

		private float sampledAmplitudeSum;

		private int sampledAmplitudeCount;

		private bool isSpeaking;

		private bool isRecording;

		private bool _isTransmitterSpeaking;

		private float[] _window = new float[0];

		private DissonanceComms comms => DissonanceFishNetComms.Instance.Comms;

		public event Action<SpeechEvent, bool> OnSpeechEventRecorded;

		private void Start()
		{
			if (comms != null)
			{
				comms.SubscribeToRecordedAudio(this);
			}
		}

		private void OnDestroy()
		{
			if (comms != null)
			{
				comms.UnsubscribeFromRecordedAudio(this);
			}
		}

		public void StartRecording()
		{
			isRecording = true;
		}

		public void StopRecording()
		{
			isRecording = false;
			ResetBuffer();
		}

		protected override void ProcessAudio(ArraySegment<float> data)
		{
			if (isRecording && isSpeaking)
			{
				if (writeHead + data.Count >= buffer.Length)
				{
					CreateSpeechEvent(_isTransmitterSpeaking);
				}
				data.CopyTo(buffer, writeHead);
				writeHead += data.Count;
				for (int i = 0; i < channels && i < data.Count; i++)
				{
					sampledAmplitudeSum += Mathf.Abs(data.Array[i]);
					sampledAmplitudeCount++;
				}
			}
		}

		protected override void ResetAudioStream(WaveFormat waveFormat)
		{
			CreateBuffer(waveFormat.Channels, waveFormat.SampleRate);
			SubscribePlayerSpeaking();
		}

		private void CreateBuffer(int channels, int sampleRate)
		{
			this.channels = channels;
			this.sampleRate = sampleRate;
			buffer = new float[Mathf.CeilToInt((float)(channels * sampleRate) * sampleDurationRange.maxValue)];
			ResetBuffer();
		}

		private void ResetBuffer()
		{
			writeHead = 0;
			sampledAmplitudeSum = 0f;
			sampledAmplitudeCount = 0;
		}

		private void CreateSpeechEvent(bool isTransmitterSpeaking = false)
		{
			if (writeHead == 0)
			{
				return;
			}
			ProtoActor myActor = SpeechEventAdditionalGameData.GetMyActor();
			if (myActor == null || Hub.s?.dataman?.ExcelDataManager == null)
			{
				ResetBuffer();
				return;
			}
			if ((float)writeHead >= sampleDurationRange.minValue * (float)sampleRate * (float)channels)
			{
				float[] array = new float[writeHead];
				Array.Copy(buffer, array, writeHead);
				float averageAmplitude = ((sampledAmplitudeCount > 0) ? (sampledAmplitudeSum / (float)sampledAmplitudeCount) : 0f);
				_ = (float)writeHead / (float)(sampleRate * channels);
				SpeechType_Area area = (isTransmitterSpeaking ? SpeechType_Area.Transmitter : myActor.GetAreaType(myActor.transform.position));
				SpeechEventAdditionalGameData gameData = new SpeechEventAdditionalGameData(myActor.GetAdjacentPlayerCount(), area, SpeechEventAdditionalGameData.GetGameTime(), myActor.GetFacingPlayerCount(), myActor.GetScrapObjects(), myActor.GetMonsters(), myActor.GetTeleporter(), myActor.GetIndoorEntered(), myActor.GetCharger(), myActor.GetCrowShop(), myActor.GetIncomingEvents());
				SpeechEvent speechEvent = null;
				int num = sampleRate;
				float[] array2 = array;
				if (sampleRate != 48000)
				{
					Logger.RError($"[SpeechEventRecorder] Sample rate mismatch: {sampleRate} != {48000}", sendToLogServer: false, "mimicvoice");
					return;
				}
				byte[] array3 = null;
				try
				{
					_ = array.Length;
					array3 = OpusAudioUtility.Encode(array, VoiceSettings.Instance.Quality, VoiceSettings.Instance.FrameSize);
					_ = array3?.Length;
				}
				catch (Exception arg)
				{
					ResetBuffer();
					Logger.RError($"[SpeechEventRecorder] Failed to compress audio data.\nException: {arg}");
					return;
				}
				speechEvent = new SpeechEvent(comms.LocalPlayerName, Hub.s.timeutil.GetCurrentTickSec(), channels, num, array3, array2.Length, averageAmplitude, DateTime.Now, gameData, Hub.s.timeutil.GetCurrentTickSec());
				if ((bool)myActor)
				{
					myActor.SetVoiceTypeDebugText(speechEvent);
				}
				if (speechEvent.GameData.IncomingEvent.Count > 0)
				{
					foreach (IncomingEvent item in speechEvent.GameData.IncomingEvent)
					{
						_ = item;
					}
				}
				if (speechEvent != null)
				{
					this.OnSpeechEventRecorded?.Invoke(speechEvent, arg2: false);
				}
			}
			ResetBuffer();
		}

		private void SubscribePlayerSpeaking()
		{
			UnsubscribePlayerSpeaking();
			string localPlayerName = comms.LocalPlayerName;
			if (localPlayerName != null)
			{
				VoicePlayerState voicePlayerState = comms.FindPlayer(localPlayerName);
				if (voicePlayerState != null)
				{
					voicePlayerState.OnStartedSpeaking += OnStartedSpeaking;
					voicePlayerState.OnStoppedSpeaking += OnStoppedSpeaking;
				}
			}
		}

		private void UnsubscribePlayerSpeaking()
		{
			string localPlayerName = comms.LocalPlayerName;
			if (localPlayerName != null)
			{
				VoicePlayerState voicePlayerState = comms.FindPlayer(localPlayerName);
				if (voicePlayerState != null)
				{
					voicePlayerState.OnStartedSpeaking -= OnStartedSpeaking;
					voicePlayerState.OnStoppedSpeaking -= OnStoppedSpeaking;
				}
			}
			isSpeaking = false;
		}

		private void OnStartedSpeaking(VoicePlayerState playerState)
		{
			isSpeaking = true;
		}

		private void OnStoppedSpeaking(VoicePlayerState playerState)
		{
			isSpeaking = false;
			CreateSpeechEvent(_isTransmitterSpeaking);
		}

		public void OnChangeTransmitterSpeaking(bool newTransmitterSpeaking)
		{
			if (_isTransmitterSpeaking && !newTransmitterSpeaking)
			{
				CreateSpeechEvent(isTransmitterSpeaking: true);
			}
			else if (!_isTransmitterSpeaking && newTransmitterSpeaking)
			{
				CreateSpeechEvent();
			}
			_isTransmitterSpeaking = newTransmitterSpeaking;
		}

		public static float[] ResampleAudio(float[] sourceData, int sourceRate, int targetRate)
		{
			if (sourceRate == targetRate)
			{
				return sourceData;
			}
			double num = (double)sourceRate / (double)targetRate;
			int num2 = (int)Math.Ceiling((double)sourceData.Length / num);
			float[] array = new float[num2];
			for (int i = 0; i < num2; i++)
			{
				double num3 = (double)i * num;
				int num4 = (int)Math.Floor(num3);
				int num5 = Math.Min(num4 + 1, sourceData.Length - 1);
				float t = (float)(num3 - (double)num4);
				array[i] = Mathf.Lerp(sourceData[num4], sourceData[num5], t);
			}
			return array;
		}

		[Obsolete("Use ResampleAudio instead")]
		public static float[] DownsampleAudioWithInterpolation(float[] sourceData, int sourceRate, int targetRate)
		{
			return ResampleAudio(sourceData, sourceRate, targetRate);
		}

		private bool IsVoice(float[] inAudioData)
		{
			if (inAudioData.Length < 2048)
			{
				return false;
			}
			if (_window == null || _window.Length < 2048)
			{
				_window = new float[2048];
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num3 <= inAudioData.Length - 2048 && num2 < 8)
			{
				Array.Copy(inAudioData, num3, _window, 0, 2048);
				if (IsSingleVoice(_window, sampleRate))
				{
					num++;
				}
				num3 += 1024;
				num2++;
			}
			return ((num2 > 0) ? ((float)num / (float)num2) : 0f) >= 0.5f;
		}

		private bool IsSingleVoice(float[] audioData, int sampleRate)
		{
			int num = audioData.Length;
			float[] array = new float[num];
			if (audioData.Length < num)
			{
				Array.Copy(audioData, array, audioData.Length);
			}
			else
			{
				Array.Copy(audioData, array, num);
			}
			for (int i = 0; i < num; i++)
			{
				float num2 = 0.5f * (1f - Mathf.Cos(MathF.PI * 2f * (float)i / (float)(num - 1)));
				array[i] *= num2;
			}
			float[] magnitudeSpectrum;
			try
			{
				magnitudeSpectrum = AudioFFT.GetMagnitudeSpectrum(AudioFFT.FFT(array));
			}
			catch (Exception ex)
			{
				Logger.RError("[IsSingleVoice] FFT failed: " + ex.Message, sendToLogServer: false, "deathmatch");
				return false;
			}
			float num3 = 0f;
			for (int j = 0; j < magnitudeSpectrum.Length; j++)
			{
				num3 += magnitudeSpectrum[j];
			}
			if (num3 <= 0f)
			{
				return false;
			}
			float num4 = (float)sampleRate / 2f;
			float num5 = num4 / (float)magnitudeSpectrum.Length;
			int num6 = Mathf.Min(3000, (int)num4);
			int num7 = 80;
			int num8 = 300;
			int num9 = Mathf.Clamp(Mathf.RoundToInt(80f / num5), 0, magnitudeSpectrum.Length - 1);
			int num10 = Mathf.Clamp(Mathf.RoundToInt((float)num6 / num5), num9, magnitudeSpectrum.Length - 1);
			int num11 = Mathf.Clamp(Mathf.RoundToInt((float)num7 / num5), num9, num10);
			int num12 = Mathf.Clamp(Mathf.RoundToInt((float)num8 / num5), num11, num10);
			float num13 = 0f;
			for (int k = num9; k <= num10; k++)
			{
				num13 += magnitudeSpectrum[k];
			}
			if (num13 <= 0f)
			{
				return false;
			}
			int num14 = Mathf.Max(1, num10 - num9 + 1);
			float num15 = num13 / (float)num14;
			float num16 = num15 * 2f;
			int num17 = 0;
			for (int l = num11 + 1; l < num12; l++)
			{
				float num18 = magnitudeSpectrum[l];
				if (num18 > num16 && num18 >= magnitudeSpectrum[l - 1] && num18 >= magnitudeSpectrum[l + 1])
				{
					num17++;
				}
			}
			float num19 = 0f;
			for (int m = num9; m <= num10; m++)
			{
				num19 += Mathf.Log(magnitudeSpectrum[m] + 1E-12f);
			}
			float num20 = Mathf.Exp(num19 / (float)num14) / (num15 + 1E-12f);
			float num21 = 0f;
			for (int n = num9; n <= num10; n++)
			{
				float num22 = (float)n * num5;
				num21 += num22 * magnitudeSpectrum[n];
			}
			float num23 = num21 / num13;
			bool num24 = num13 / num3 > 0.08f;
			bool flag = num17 >= 1 && num17 <= 10;
			bool flag2 = num20 < 0.7f;
			bool flag3 = num23 >= 150f && num23 <= 1200f;
			return num24 && flag && flag2 && flag3;
		}
	}
}
