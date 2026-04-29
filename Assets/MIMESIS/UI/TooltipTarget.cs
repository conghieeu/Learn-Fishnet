using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTarget : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[TextArea]
	public string tooltipText;

	public TooltipSide preferredSide;

	public bool followWhileHover;

	public Vector2 pixelOffset = Vector2.zero;

	public float showDelay = 0.1f;

	public float hideDelay = 0.05f;

	public float maxWidth = -1f;

	private RectTransform _rect;

	private ITooltipTargetable _tooltipTargetable;

	private void OnEnable()
	{
		_rect = base.transform as RectTransform;
		_tooltipTargetable = GetComponent<ITooltipTargetable>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Logger.RLog("[TooltipTarget] OnTooltipShow - tooltipText: " + tooltipText);
		if (_tooltipTargetable != null)
		{
			tooltipText = _tooltipTargetable.GetTooltipText();
		}
		TooltipManager.TooltipOptions options = TooltipManager.TooltipOptions.Default;
		options.preferredSide = preferredSide;
		options.followTarget = followWhileHover;
		options.pixelOffset = pixelOffset;
		options.showDelay = showDelay;
		options.hideDelay = hideDelay;
		options.maxWidth = maxWidth;
		Hub.s.tooltipManager.ShowFor(_rect, tooltipText, options);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Logger.RLog("[TooltipTarget] OnTooltipHide - tooltipText: " + tooltipText);
		Hub.s.tooltipManager.Hide();
	}
}
