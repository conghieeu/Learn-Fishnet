using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class DOTAnimationUtility : MonoBehaviour
{
	public enum eTypes
	{
		None = 0,
		UI_Slightly_enlarging = 1,
		UI_Slightly_enlarging_OtherScale_v1 = 2,
		UI_Slightly_enlarging_OtherScale_v2 = 3,
		UI_HorizontalFold_fast = 4
	}

	public static TweenerCore<Vector3, Vector3, VectorOptions> Do(MonoBehaviour o, eTypes animType)
	{
		return Do(o.gameObject, animType);
	}

	public static TweenerCore<Vector3, Vector3, VectorOptions> Do(Transform t, eTypes animType)
	{
		return Do(t.gameObject, animType);
	}

	public static TweenerCore<Vector3, Vector3, VectorOptions> Do(GameObject o, eTypes animType)
	{
		if (o == null)
		{
			return null;
		}
		return animType switch
		{
			eTypes.UI_Slightly_enlarging => o.GetComponent<RectTransform>()?.DOScale(1f, 0.2f).From(0.8f), 
			eTypes.UI_Slightly_enlarging_OtherScale_v1 => o.GetComponent<RectTransform>()?.DOScale(1.6f, 0.2f).From(0.8f), 
			eTypes.UI_Slightly_enlarging_OtherScale_v2 => o.GetComponent<RectTransform>()?.DOScale(2f, 0.2f).From(0.8f), 
			eTypes.UI_HorizontalFold_fast => o.GetComponent<RectTransform>()?.DOScaleX(0f, 0.1f).From(1f), 
			eTypes.None => null, 
			_ => null, 
		};
	}
}
