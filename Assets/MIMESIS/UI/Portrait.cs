using UnityEngine;

public class Portrait : SocketAttachable, ICycleRandomableItem
{
	[SerializeField]
	private string stareTriggerName = "Stare";

	public void OnCycleRandomEvent(int randomSeed)
	{
		if (animator != null)
		{
			animator.SetTrigger(stareTriggerName);
		}
	}
}
