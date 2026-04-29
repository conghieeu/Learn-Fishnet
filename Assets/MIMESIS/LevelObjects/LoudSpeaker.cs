using UnityEngine;

public class LoudSpeaker : SocketAttachable, IToggleableItem, IGaugeableItem
{
	[SerializeField]
	private string turnOnAudioClipName = "loud_speaker_on";

	[SerializeField]
	private string turnOffAudioClipName = "loud_speaker_off";

	public override void OnDetachFromSocket()
	{
		_ = base.owner == null;
	}

	public void OnToggled(long itemID, bool toggleOn)
	{
		if (!(base.owner == null))
		{
			if (toggleOn)
			{
				Hub.s.audioman.PlaySfx(turnOnAudioClipName, base.transform);
			}
			else
			{
				Hub.s.audioman.PlaySfx(turnOffAudioClipName, base.transform);
			}
		}
	}

	public void OnGaugeChanged(long itemID, int remainGauge)
	{
		_ = base.owner == null;
	}
}
