using UnityEngine;

public class UIDoTweenAnimationInvoker : MonoBehaviour
{
	[SerializeField]
	private DOTAnimationUtility.eTypes openAnimation = DOTAnimationUtility.eTypes.UI_Slightly_enlarging;

	private void OnEnable()
	{
		DOTAnimationUtility.Do(this, openAnimation);
	}
}
