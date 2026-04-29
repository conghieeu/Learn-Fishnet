using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoCutScenePlayer : MonoBehaviour
{
	[SerializeField]
	public VideoPlayer videoPlayer;

	[SerializeField]
	public VideoClip videoClip;

	private RenderTexture renderTexture;

	public void PlayVideo(Action onPlayFininhsed = null)
	{
		SetupVideoPlayer();
		videoPlayer.loopPointReached += delegate
		{
			onPlayFininhsed?.Invoke();
			Hub.s.uiman.ReleaseVideoPlay();
			renderTexture.Release();
			videoPlayer.targetTexture = null;
			Hub.s.uiman.FadeIn(0f);
		};
		Hub.s.uiman.FadeOut(Color.black, 0f);
		videoPlayer.Prepare();
	}

	public void StopVideo()
	{
		videoPlayer.Stop();
		videoPlayer.clip = null;
		Hub.s.uiman.ReleaseVideoPlay();
		renderTexture.Release();
		videoPlayer.targetTexture = null;
	}

	private void SetupVideoPlayer()
	{
		videoPlayer.source = VideoSource.VideoClip;
		videoPlayer.clip = videoClip;
		videoPlayer.isLooping = false;
		videoPlayer.renderMode = VideoRenderMode.RenderTexture;
		int width = Screen.width;
		int height = Screen.height;
		renderTexture = new RenderTexture(width, height, 0);
		renderTexture.Create();
		videoPlayer.targetTexture = renderTexture;
		Hub.s.uiman.PrepareVideoPlay(renderTexture);
		videoPlayer.prepareCompleted += OnVideoPrepared;
	}

	private void OnVideoPrepared(VideoPlayer vp)
	{
		vp.Play();
		Hub.s.pdata.main.EndSceneLoading();
	}
}
