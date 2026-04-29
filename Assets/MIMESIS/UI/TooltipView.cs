using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipView : MonoBehaviour
{
	[Header("Refs")]
	public RectTransform root;

	public RectTransform content;

	public TMP_Text label;

	public Image background;

	public LayoutElement layoutElement;

	public CanvasGroup canvasGroup;

	[Header("Tails (enable one)")]
	public RectTransform tailTop;

	public RectTransform tailBottom;

	public RectTransform tailLeft;

	public RectTransform tailRight;

	[Header("Options")]
	public float edgePadding = 8f;

	public Vector2 extraPadding = new Vector2(12f, 8f);

	private Camera _uiCam;

	private RectTransform _canvasRect;

	private void Start()
	{
		_uiCam = Camera.main;
		_canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
		SetAlpha(0f);
		EnableOnlyTail(null);
	}

	public void ApplyTheme(MMTooltipTheme theme)
	{
		_ = theme == null;
	}

	public void SetText(string text)
	{
		if (label != null)
		{
			label.text = text ?? string.Empty;
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(root);
	}

	public void SetMaxWidth(float maxW)
	{
		if (layoutElement != null)
		{
			layoutElement.preferredWidth = Mathf.Max(0f, maxW);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(root);
	}

	public void SetAlpha(float a)
	{
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = Mathf.Clamp01(a);
		}
	}

	public Coroutine FadeTo(MonoBehaviour runner, float target, float duration)
	{
		return runner.StartCoroutine(CoFade(target, duration));
	}

	private IEnumerator CoFade(float target, float duration)
	{
		if (!canvasGroup || duration <= 0f)
		{
			SetAlpha(target);
			yield break;
		}
		float start = canvasGroup.alpha;
		float t = 0f;
		while (t < duration)
		{
			t += Time.unscaledDeltaTime;
			SetAlpha(Mathf.Lerp(start, target, t / duration));
			yield return null;
		}
		SetAlpha(target);
	}

	public void EnableOnlyTail(RectTransform activeTail)
	{
		if ((bool)tailTop)
		{
			tailTop.gameObject.SetActive(activeTail == tailTop);
		}
		if ((bool)tailBottom)
		{
			tailBottom.gameObject.SetActive(activeTail == tailBottom);
		}
		if ((bool)tailLeft)
		{
			tailLeft.gameObject.SetActive(activeTail == tailLeft);
		}
		if ((bool)tailRight)
		{
			tailRight.gameObject.SetActive(activeTail == tailRight);
		}
	}

	public void SetPivot(Vector2 pivot)
	{
		root.pivot = pivot;
	}

	public void PlaceTail(TooltipSide side, Vector2 anchorScreenPos)
	{
		RectTransform rectTransform = null;
		rectTransform = side switch
		{
			TooltipSide.Top => tailBottom, 
			TooltipSide.Bottom => tailTop, 
			TooltipSide.Left => tailRight, 
			TooltipSide.Right => tailLeft, 
			_ => null, 
		};
		EnableOnlyTail(rectTransform);
		if (!(rectTransform == null))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(root, anchorScreenPos, _uiCam, out var localPoint);
			Vector2 size = root.rect.size;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			if (side == TooltipSide.Top || side == TooltipSide.Bottom)
			{
				float num = size.x * 0.5f - edgePadding;
				anchoredPosition.x = Mathf.Clamp(localPoint.x, 0f - num, num);
			}
			else
			{
				float num2 = size.y * 0.5f - edgePadding;
				anchoredPosition.y = Mathf.Clamp(localPoint.y, 0f - num2, num2);
			}
			rectTransform.anchoredPosition = anchoredPosition;
		}
	}

	public void KeepInsideCanvas(float safeMargin)
	{
		if (!(_canvasRect == null))
		{
			Vector2 size = _canvasRect.rect.size;
			Vector2 size2 = root.rect.size;
			Vector2 anchoredPosition = root.anchoredPosition;
			float num = (0f - size.x) * 0.5f + safeMargin;
			float num2 = size.x * 0.5f - safeMargin;
			float num3 = (0f - size.y) * 0.5f + safeMargin;
			float num4 = size.y * 0.5f - safeMargin;
			float num5 = anchoredPosition.x - size2.x * root.pivot.x;
			float num6 = anchoredPosition.x + size2.x * (1f - root.pivot.x);
			float num7 = anchoredPosition.y - size2.y * root.pivot.y;
			float num8 = anchoredPosition.y + size2.y * (1f - root.pivot.y);
			if (num5 < num)
			{
				anchoredPosition.x += num - num5;
			}
			if (num6 > num2)
			{
				anchoredPosition.x += num2 - num6;
			}
			if (num7 < num3)
			{
				anchoredPosition.y += num3 - num7;
			}
			if (num8 > num4)
			{
				anchoredPosition.y += num4 - num8;
			}
			root.anchoredPosition = anchoredPosition;
		}
	}
}
