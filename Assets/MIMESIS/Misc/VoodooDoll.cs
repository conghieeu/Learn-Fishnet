using UnityEngine;

public class VoodooDoll : SocketAttachable
{
	[SerializeField]
	private string blackOutTrigger = "BlackOut";

	public void OnBlackOut()
	{
		if (animator != null)
		{
			animator.SetTrigger(blackOutTrigger);
		}
	}
}
