using System.Collections.Generic;
using Mimic.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "MMVoiceEffectTable", menuName = "_Mimic/MMVoiceEffectTable")]
public class MMVoiceEffectTable : ScriptableObject
{
	[SerializeField]
	private List<VoiceEffectPresetPair> voiceEffectPresets = new List<VoiceEffectPresetPair>();

	public VoiceEffectPreset GetPresetByName(string name)
	{
		return voiceEffectPresets.Find((VoiceEffectPresetPair pair) => pair.Name == name)?.Preset;
	}
}
