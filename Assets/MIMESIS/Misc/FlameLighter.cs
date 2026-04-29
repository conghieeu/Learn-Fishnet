using UnityEngine;

public class FlameLighter : SocketAttachable, ICycleRandomableItem
{
	[SerializeField]
	private string igniteTrigger = "Ignite";

	public void OnCycleRandomEvent(int randomSeed)
	{
		if (animator != null)
		{
			animator.SetTrigger(igniteTrigger);
		}
	}
}
