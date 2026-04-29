using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(MasterAudioPlayableAsset))]
[TrackBindingType(typeof(GameObject))]
public class MasterAudioTrack : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		return ScriptPlayable<MasterAudioMixerBehaviour>.Create(graph, inputCount);
	}
}
