using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MasterAudioPlayableAsset : PlayableAsset, ITimelineClipAsset
{
	[SerializeField]
	public string soundGroupName;

	[SerializeField]
	[Range(0f, 1f)]
	public float volume = 1f;

	[SerializeField]
	public bool enableFade = true;

	[SerializeField]
	public bool playOnce;

	[SerializeField]
	public bool use2DSound;

	public ClipCaps clipCaps => ClipCaps.Extrapolation | ClipCaps.Blending;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<MasterAudioPlayableBehaviour> scriptPlayable = ScriptPlayable<MasterAudioPlayableBehaviour>.Create(graph);
		MasterAudioPlayableBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.soundGroupName = soundGroupName;
		behaviour.volume = volume;
		behaviour.enableFade = enableFade;
		behaviour.playOnce = playOnce;
		behaviour.use2DSound = use2DSound;
		return scriptPlayable;
	}
}
