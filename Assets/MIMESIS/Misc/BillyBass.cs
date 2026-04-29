using UnityEngine;

public class BillyBass : SocketAudioAttachable, ICycleRandomableItem
{
	private static readonly int ShootTrigger = Animator.StringToHash("ShootTrigger");

	public void OnCycleRandomEvent(int randomSeed)
	{
		if (!(animator == null))
		{
			animator.SetTrigger(ShootTrigger);
			PlaySound();
		}
	}

	protected override void OnAttachSound()
	{
	}

	protected override void OnEnable()
	{
	}

	protected override void Update()
	{
	}
}
