using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FillDragForwarder : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler, IPointerUpHandler
{
	public Slider targetSlider;

	public void OnPointerDown(PointerEventData e)
	{
		targetSlider.OnPointerDown(e);
	}

	public void OnDrag(PointerEventData e)
	{
		targetSlider.OnDrag(e);
	}

	public void OnPointerUp(PointerEventData e)
	{
		targetSlider.OnPointerUp(e);
	}
}
