using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MMVoicePitchShiftTable", menuName = "_Mimic/MMVoicePitchShiftTable")]
public class MMVoicePitchShiftTable : ScriptableObject
{
	[Header("Pitch")]
	[SerializeField]
	private MinMaxFloatRange pitchClamp = new MinMaxFloatRange(0.5f, 2f, 0.5f, 2f);

	[SerializeField]
	private AnimationCurve statToPitchCurve = new AnimationCurve(new Keyframe(-1000f, 0.1f), new Keyframe(0f, 1f), new Keyframe(1000f, 3f));

	[Header("Dissonance Remote Mixers")]
	[SerializeField]
	[Tooltip("Pitch Shift를 사용하지 못하는 상황에 사용할 AudioMixerGroup")]
	private AudioMixerGroup defaultMixerGroup;

	[SerializeField]
	private AudioMixerParameter[] remoteMixerParameters;

	public (AudioMixerGroup?, float) GetMixerParameter(long inStatValue)
	{
		float num = 1f;
		AudioMixerGroup audioMixerGroup = null;
		if (-0.01 < (double)inStatValue && (double)inStatValue < 0.01)
		{
			audioMixerGroup = GetDefaultMixerGroup();
		}
		else
		{
			num = GetPitch(inStatValue);
			audioMixerGroup = remoteMixerParameters[0].mixerGroup;
			float num2 = Mathf.Abs(remoteMixerParameters[0].Pitch - num);
			for (int i = 1; i < remoteMixerParameters.Length; i++)
			{
				float num3 = Mathf.Abs(remoteMixerParameters[i].Pitch - num);
				if (num3 < num2)
				{
					num2 = num3;
					audioMixerGroup = remoteMixerParameters[i].mixerGroup;
				}
			}
		}
		return (audioMixerGroup, num);
	}

	public AudioMixerGroup GetDefaultMixerGroup()
	{
		return defaultMixerGroup;
	}

	private float GetPitch(long statValue)
	{
		return Mathf.Clamp(statToPitchCurve.Evaluate(statValue), pitchClamp.minValue, pitchClamp.maxValue);
	}
}
