using System;
using Mimic.Audio;
using UnityEngine;

[Serializable]
public class VoiceEffectPresetPair
{
	[SerializeField]
	private string name;

	[SerializeField]
	private VoiceEffectPreset preset;

	public string Name => name;

	public VoiceEffectPreset Preset => preset;
}
