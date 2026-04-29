public class OwlStatue : SocketAudioAttachable, ICycleRandomableItem
{
	public void OnCycleRandomEvent(int randomSeed)
	{
		PlaySound();
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
