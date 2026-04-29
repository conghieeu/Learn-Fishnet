using UnityEngine;

namespace BuildingMakerToolset.Demo
{
	public class AudioReverbTrigger : TriggerZone
	{
		public AudioReverbPreset reverbPreset;

		protected override void OnEnter(PlayerMovement player)
		{
			AudioReverbFilter componentInChildren = player.gameObject.GetComponentInChildren<AudioReverbFilter>();
			if (!(componentInChildren == null))
			{
				componentInChildren.reverbPreset = reverbPreset;
			}
		}

		protected override void OnExit(PlayerMovement player)
		{
			AudioReverbFilter componentInChildren = player.gameObject.GetComponentInChildren<AudioReverbFilter>();
			if (!(componentInChildren == null))
			{
				componentInChildren.reverbPreset = AudioReverbPreset.Off;
			}
		}
	}
}
