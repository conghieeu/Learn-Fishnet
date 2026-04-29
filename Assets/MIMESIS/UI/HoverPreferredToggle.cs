using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverPreferredToggle : Toggle
{
	private bool _pointerInside;

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (IsActive() && IsInteractable())
		{
			_pointerInside = true;
			DoStateTransition(SelectionState.Highlighted, instant: false);
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (IsActive() && IsInteractable())
		{
			_pointerInside = false;
			DoStateTransition(SelectionState.Normal, instant: false);
		}
	}

	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		if (IsActive() && IsInteractable() && !_pointerInside)
		{
			DoStateTransition(SelectionState.Normal, instant: false);
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
		if (IsActive() && IsInteractable() && !_pointerInside)
		{
			DoStateTransition(SelectionState.Normal, instant: false);
		}
	}
}
