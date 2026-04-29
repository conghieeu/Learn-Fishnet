using System;
using System.Collections;
using System.Collections.Generic;
using Timeline.Samples;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine.Video;

public class CutScenePlayer : MonoBehaviour
{
	[Serializable]
	public class CutScene
	{
		public string name;

		public PlayableDirector direction;

		public VideoClip videoClip;

		public bool lockInput;

		public bool turnOffPostProcessing;

		public bool turnOffDecal;
	}

	public class CutScenePlayData
	{
		private static WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);

		public CutScene cutScene;

		public void Play(VideoCutScenePlayer videoCutScenePlayer)
		{
			if (cutScene.turnOffDecal)
			{
				Hub.s.decalManamger.TurnOffDecal();
			}
			if (cutScene.direction != null)
			{
				cutScene.direction.stopped += delegate
				{
					if (cutScene.turnOffPostProcessing)
					{
						UniversalAdditionalCameraData universalAdditionalCameraData2 = Camera.main.GetUniversalAdditionalCameraData();
						if (universalAdditionalCameraData2 != null)
						{
							universalAdditionalCameraData2.renderPostProcessing = true;
						}
					}
				};
				cutScene.direction.Play();
				if (cutScene.turnOffPostProcessing)
				{
					UniversalAdditionalCameraData universalAdditionalCameraData = Camera.main.GetUniversalAdditionalCameraData();
					if (universalAdditionalCameraData != null)
					{
						universalAdditionalCameraData.renderPostProcessing = false;
					}
				}
				if (Hub.s.pdata.main != null && !HasVideoTrack(cutScene.direction))
				{
					Hub.s.pdata.main.StartCoroutine(CorEndSceneLoading());
				}
			}
			else if (cutScene.videoClip != null)
			{
				videoCutScenePlayer.videoClip = cutScene.videoClip;
				videoCutScenePlayer.PlayVideo();
			}
		}

		public void Stop(VideoCutScenePlayer videoCutScenePlayer)
		{
			if (cutScene.direction != null)
			{
				cutScene.direction.Stop();
			}
			else if (cutScene.videoClip != null)
			{
				videoCutScenePlayer.StopVideo();
			}
			if (cutScene.turnOffDecal)
			{
				Hub.s.decalManamger.TurnOnDecal();
			}
		}

		public IEnumerator CorEndSceneLoading()
		{
			yield return _waitForSeconds0_1;
			Hub.s.pdata.main.EndSceneLoading();
		}
	}

	[SerializeField]
	public List<CutScene> cutscenes;

	[SerializeField]
	public VideoCutScenePlayer videoCutScenePlayer;

	private CutScenePlayData currentPlayingCutScene;

	public UnityAction<CutScene> OnPrePlayCutScene;

	public UnityAction<CutScene> OnPostPlayCutScene;

	public void PlayCutScene(string name)
	{
		if (currentPlayingCutScene != null)
		{
			currentPlayingCutScene.Stop(videoCutScenePlayer);
		}
		CutScene cutScene = cutscenes.Find((CutScene c) => c.name == name);
		if (cutScene != null)
		{
			OnPrePlayCutScene?.Invoke(cutScene);
			currentPlayingCutScene = new CutScenePlayData
			{
				cutScene = cutScene
			};
			currentPlayingCutScene.Play(videoCutScenePlayer);
			OnPostPlayCutScene?.Invoke(cutScene);
		}
	}

	public void StopCutScene(string name)
	{
		if (currentPlayingCutScene != null)
		{
			currentPlayingCutScene.Stop(videoCutScenePlayer);
		}
		currentPlayingCutScene = null;
	}

	public void OnTimelineVideoReady()
	{
		if (Hub.s?.pdata?.main != null)
		{
			Hub.s.pdata.main.StartCoroutine(currentPlayingCutScene.CorEndSceneLoading());
		}
	}

	public void OnTimelineVideoCompleted()
	{
		Debug.Log("Timeline Video Completed");
	}

	private static bool HasVideoTrack(PlayableDirector director)
	{
		if (director == null || director.playableAsset == null)
		{
			return false;
		}
		TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
		if (timelineAsset == null)
		{
			return false;
		}
		foreach (TrackAsset outputTrack in timelineAsset.GetOutputTracks())
		{
			if (outputTrack is VideoTrack)
			{
				return true;
			}
		}
		return false;
	}
}
