using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollForwarder : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public ScrollRect targetScrollRect;

	public void OnScroll(PointerEventData eventData)
	{
		if (targetScrollRect != null)
		{
			targetScrollRect.OnScroll(eventData);
		}
	}
}
