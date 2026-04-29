using UnityEngine;
using UnityEngine.Playables;

public class LoopTimeline : MonoBehaviour
{
	public PlayableDirector director;

	private void Start()
	{
		director.stopped += OnTimelineStopped;
		director.Play();
	}

	private void OnTimelineStopped(PlayableDirector pd)
	{
		if (pd == director)
		{
			director.Play();
		}
	}

	private void OnDestroy()
	{
		director.stopped -= OnTimelineStopped;
	}
}
