using System;
using UnityEngine.Playables;

[Serializable]
public class MasterAudioMixerBehaviour : PlayableBehaviour
{
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		int inputCount = playable.GetInputCount();
		for (int i = 0; i < inputCount; i++)
		{
			float inputWeight = playable.GetInputWeight(i);
			ScriptPlayable<MasterAudioPlayableBehaviour> playable2 = (ScriptPlayable<MasterAudioPlayableBehaviour>)playable.GetInput(i);
			if (playable2.IsValid())
			{
				MasterAudioPlayableBehaviour behaviour = playable2.GetBehaviour();
				if (behaviour != null && behaviour.hasPlayed && behaviour.soundResult?.ActingVariation != null && behaviour.enableFade && !behaviour.playOnce)
				{
					behaviour.currentWeight = inputWeight;
				}
			}
		}
	}
}
