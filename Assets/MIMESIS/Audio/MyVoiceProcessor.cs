using System;
using System.Collections;
using System.Collections.Concurrent;
using Dissonance;
using Mimic.InputSystem;
using NAudio.Wave;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MyVoiceProcessor : BaseMicrophoneSubscriber
{
	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	[Tooltip("재생 딜레이 시간 (초)")]
	private float _delayInSeconds = 0.1f;

	private bool _isActivated;

	private int _sampleRate = 48000;

	private int _delaySamples;

	private bool _hasStartedPlaying;

	private Coroutine _deactivateCoroutine;

	private WaitForSeconds _waitForSeconds;

	public ConcurrentQueue<float> _pcmQueue = new ConcurrentQueue<float>();

	protected void Awake()
	{
		if (_audioSource == null)
		{
			_audioSource = base.gameObject.GetComponentInChildren<AudioSource>();
			if (_audioSource == null)
			{
				Logger.RError("AudioSource를 찾을 수 없습니다.");
				return;
			}
		}
		_sampleRate = AudioSettings.outputSampleRate;
		_delaySamples = Mathf.RoundToInt(_delayInSeconds * (float)_sampleRate);
		_audioSource.clip = AudioClip.Create("DummyClip", _sampleRate * 3, 1, _sampleRate, stream: false);
		_audioSource.loop = true;
		_waitForSeconds = new WaitForSeconds(_delayInSeconds + 0.1f);
		_isActivated = false;
	}

	public void Activate(bool isActivated)
	{
		if (_isActivated != isActivated)
		{
			StopDeactivateCoroutine();
			if (isActivated)
			{
				base.gameObject.SetActive(value: true);
				Activate();
			}
			else if (base.gameObject.activeInHierarchy)
			{
				_deactivateCoroutine = StartCoroutine(Deactivate());
			}
		}
	}

	private void Activate()
	{
		DissonanceComms dissonanceComms = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
		if (dissonanceComms != null)
		{
			dissonanceComms.SubscribeToRecordedAudio(this);
		}
		else
		{
			Logger.RError("DissonanceComms를 찾을 수 없습니다.");
		}
		ReInit();
	}

	private IEnumerator Deactivate()
	{
		DissonanceComms dissonanceComms = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
		if (dissonanceComms != null)
		{
			dissonanceComms.UnsubscribeFromRecordedAudio(this);
		}
		_isActivated = false;
		yield return new WaitForSeconds(_delayInSeconds);
		if (_audioSource.isPlaying)
		{
			_audioSource.Stop();
		}
		_pcmQueue.Clear();
		base.gameObject.SetActive(value: false);
	}

	protected override void ResetAudioStream(WaveFormat waveFormat)
	{
		_sampleRate = waveFormat.SampleRate;
		_delaySamples = Mathf.RoundToInt(_delayInSeconds * (float)_sampleRate);
		_pcmQueue.Clear();
		_hasStartedPlaying = false;
		Debug.Log($"Audio stream reset - SampleRate: {_sampleRate} Hz, Channels: {waveFormat.Channels}, Delay: {_delaySamples} samples");
	}

	private void ReInit()
	{
		_hasStartedPlaying = false;
		_pcmQueue.Clear();
		if (_audioSource.isPlaying)
		{
			_audioSource.Stop();
		}
		_audioSource.timeSamples = 0;
		_audioSource.Play();
		_isActivated = true;
	}

	protected override void ProcessAudio(ArraySegment<float> buffer)
	{
		if (_isActivated && (!(Hub.s.gameSettingManager != null) || Hub.s.gameSettingManager.voiceMode != CommActivationMode.PushToTalk || !(Hub.s.inputman != null) || Hub.s.inputman.isPressed(InputAction.PushToTalk)))
		{
			float[] array = new float[buffer.Count];
			buffer.CopyTo(array);
			float[] array2 = array;
			foreach (float item in array2)
			{
				_pcmQueue.Enqueue(item);
			}
		}
	}

	public AudioSource GetAudioSource()
	{
		return _audioSource;
	}

	private void OnAudioFilterRead(float[] data, int channels)
	{
		if (!_hasStartedPlaying)
		{
			if (_pcmQueue.Count < _delaySamples)
			{
				for (int i = 0; i < data.Length; i++)
				{
					data[i] = 0f;
				}
				return;
			}
			_hasStartedPlaying = true;
		}
		switch (channels)
		{
		case 1:
			OnAudioFilterRead_Mono(data);
			break;
		case 2:
			OnAudioFilterRead_Stereo(data);
			break;
		default:
			OnAudioFilterRead_MultiChannel(data, channels);
			break;
		}
		if (_pcmQueue.Count > 48000)
		{
			while (_pcmQueue.Count > 24000)
			{
				_pcmQueue.TryDequeue(out var _);
			}
			Debug.LogWarning("Audio queue overflow! Reducing latency.");
		}
	}

	private void OnAudioFilterRead_Mono(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (_pcmQueue.TryDequeue(out var result))
			{
				data[i] = result;
			}
			else
			{
				data[i] = 0f;
			}
		}
	}

	private void OnAudioFilterRead_Stereo(float[] data)
	{
		for (int i = 0; i < data.Length; i += 2)
		{
			if (_pcmQueue.TryDequeue(out var result))
			{
				data[i] = result;
				data[i + 1] = result;
			}
			else
			{
				data[i] = 0f;
				data[i + 1] = 0f;
			}
		}
	}

	private void OnAudioFilterRead_MultiChannel(float[] data, int channels)
	{
		for (int i = 0; i < data.Length; i += channels)
		{
			if (_pcmQueue.TryDequeue(out var result))
			{
				for (int j = 0; j < channels; j++)
				{
					data[i + j] = result;
				}
			}
			else
			{
				for (int k = 0; k < channels; k++)
				{
					data[i + k] = 0f;
				}
			}
		}
	}

	private void StopDeactivateCoroutine()
	{
		if (_deactivateCoroutine != null)
		{
			StopCoroutine(_deactivateCoroutine);
			_deactivateCoroutine = null;
		}
	}
}
