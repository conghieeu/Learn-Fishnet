using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "VoiceEffectPreset", menuName = "Mimic/Audio/VoiceEffectPreset")]
	public class VoiceEffectPreset : ScriptableObject
	{
		[Header("프리셋 설명")]
		[SerializeField]
		protected string presetName = "";

		[TextArea(3, 5)]
		public string description = "음성 효과 프리셋 설명을 입력하세요.";

		[Header("프리셋 설정")]
		[SerializeField]
		protected LowPassFilterPreset lowPass;

		[SerializeField]
		protected HighPassFilterPreset highPass;

		[SerializeField]
		protected DistortionFilterPreset distortion;

		[SerializeField]
		protected ChorusFilterPreset chorus;

		[SerializeField]
		protected ReverbFilterPreset reverb;

		[SerializeField]
		protected EchoFilterPreset echo;

		[SerializeField]
		protected AmplifierFilterPreset amplifier;

		[Header("거리 설정")]
		public float distance = 20f;

		[Header("Vibration 설정")]
		public bool isVibrationEnabled;

		public float modulationSpeed = 12f;

		public float modulationIntensity = 300f;

		public float wobbleSpeed = 25f;

		public float superWobbleSpeed = 40f;

		public LowPassFilterPreset LowPass => lowPass;

		public HighPassFilterPreset HighPass => highPass;

		public DistortionFilterPreset Distortion => distortion;

		public ChorusFilterPreset Chorus => chorus;

		public ReverbFilterPreset Reverb => reverb;

		public EchoFilterPreset Echo => echo;

		public AmplifierFilterPreset Amplifier => amplifier;
	}
}
