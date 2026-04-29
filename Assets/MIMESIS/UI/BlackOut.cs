using UnityEngine;

public class BlackOut : MonoBehaviour
{
	private bool isEnableAtStart;

	private void Start()
	{
		isEnableAtStart = base.enabled;
	}

	public void OnBlackOut()
	{
		if (isEnableAtStart && base.gameObject != null)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void OnRecover()
	{
		if (isEnableAtStart && base.gameObject != null)
		{
			base.gameObject.SetActive(value: true);
		}
	}
}
