using UnityEngine;

public class BoomBox : SocketAudioAttachable
{
	[SerializeField]
	private ParticleSystem particleSystem;

	private bool isAttached;

	public override void OnAttachToSocket()
	{
		base.OnAttachToSocket();
		isAttached = true;
		particleSystem?.Play();
	}

	public override void OnDetachFromSocket()
	{
		base.OnDetachFromSocket();
		isAttached = false;
		particleSystem?.Pause();
	}
}
