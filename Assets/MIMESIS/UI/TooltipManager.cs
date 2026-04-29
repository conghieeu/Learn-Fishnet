using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
	[Serializable]
	public struct TooltipOptions
	{
		public TooltipSide preferredSide;

		public bool followTarget;

		public Vector2 pixelOffset;

		public float showDelay;

		public float hideDelay;

		public float maxWidth;

		public bool anchorToTargetEdge;

		public float edgeGap;

		public static TooltipOptions Default => new TooltipOptions
		{
			preferredSide = TooltipSide.Auto,
			followTarget = false,
			pixelOffset = Vector2.zero,
			showDelay = -1f,
			hideDelay = -1f,
			maxWidth = -1f,
			anchorToTargetEdge = true,
			edgeGap = -1f
		};
	}

	[Header("Setup")]
	[SerializeField]
	private Canvas rootCanvas;

	public TooltipView viewPrefab;

	public MMTooltipTheme defaultTheme;

	[Header("Behavior")]
	public float safeMargin = 8f;

	public Vector2 defaultOffset = new Vector2(0f, 16f);

	public float defaultShowDelay = 0.05f;

	public float defaultHideDelay = 0.05f;

	[Header("Position")]
	public float defaultEdgeGap = 10f;

	private TooltipView _view;

	private Camera _uiCam;

	private RectTransform _canvasRect;

	private Coroutine _fadeCo;

	private Coroutine _delayCo;

	private bool _following;

	private Vector2 _screenPos;

	private Transform _worldTarget;

	private RectTransform _uiTarget;

	private Vector3 _worldOffset;

	private TooltipSide _side;

	private float _currentEdgeGap;

	private bool _useEdgeAnchor;

	private void Start()
	{
		if (rootCanvas == null)
		{
			Logger.RError("TooltipManager: RootCanvas is not set");
		}
		_uiCam = (rootCanvas ? rootCanvas.worldCamera : null);
		_canvasRect = (rootCanvas ? rootCanvas.GetComponent<RectTransform>() : null);
		EnsureView();
		ApplyTheme(defaultTheme);
		HideImmediate();
	}

	private void OnDestroy()
	{
		ResetState();
	}

	private void EnsureView()
	{
		if (!(_view != null))
		{
			_view = UnityEngine.Object.Instantiate(viewPrefab, rootCanvas.transform);
			_view.name = "TooltipView (Runtime)";
		}
	}

	public void ApplyTheme(MMTooltipTheme theme)
	{
		if (_view != null)
		{
			_view.ApplyTheme(theme);
		}
	}

	public void SetFont(TMP_FontAsset font, int size, float charSpacing = 0f, float lineSpacing = 0f)
	{
		if (defaultTheme == null)
		{
			defaultTheme = ScriptableObject.CreateInstance<MMTooltipTheme>();
		}
		ApplyTheme(defaultTheme);
	}

	public void SetMaxWidth(float maxWidth)
	{
		if (_view != null)
		{
			_view.SetMaxWidth(maxWidth);
		}
	}

	private void Show(string text, Vector2 screenPosition, TooltipOptions options)
	{
		EnsureView();
		_following = options.followTarget;
		if (options.maxWidth > 0f)
		{
			_view.SetMaxWidth(options.maxWidth);
		}
		float delay = ((options.showDelay >= 0f) ? options.showDelay : defaultShowDelay);
		_side = options.preferredSide;
		_useEdgeAnchor = options.anchorToTargetEdge;
		_currentEdgeGap = ((options.edgeGap >= 0f) ? options.edgeGap : defaultEdgeGap);
		_screenPos = screenPosition + ((options.pixelOffset == Vector2.zero) ? defaultOffset : options.pixelOffset);
		StartShowFlow(text, delay);
	}

	public void ShowAtWorld(string text, Vector3 worldPos, Camera cam, TooltipOptions options)
	{
		if (cam == null)
		{
			cam = Camera.main;
		}
		Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
		Show(text, screenPosition, options);
		_uiTarget = null;
	}

	public void ShowFor(Transform target, string text, Vector3 worldOffset, Camera cam, TooltipOptions options)
	{
		_worldTarget = target;
		_worldOffset = worldOffset;
		_following = options.followTarget;
		Vector3 worldPos = ((target != null) ? (target.position + worldOffset) : Vector3.zero);
		ShowAtWorld(text, worldPos, cam, options);
	}

	public void ShowFor(RectTransform uiTarget, string text, TooltipOptions options)
	{
		_uiTarget = uiTarget;
		_following = options.followTarget;
		_useEdgeAnchor = options.anchorToTargetEdge;
		_currentEdgeGap = ((options.edgeGap >= 0f) ? options.edgeGap : defaultEdgeGap);
		Vector2 screenPosition = WorldToScreenOfUI(uiTarget);
		Show(text, screenPosition, options);
		_worldTarget = null;
	}

	public void Hide()
	{
		float delay = defaultHideDelay;
		StartHideFlow(delay);
	}

	public void HideImmediate()
	{
		if (_delayCo != null)
		{
			StopCoroutine(_delayCo);
		}
		if (_fadeCo != null)
		{
			StopCoroutine(_fadeCo);
		}
		if (_view != null)
		{
			_view.SetAlpha(0f);
		}
		_following = false;
		_worldTarget = null;
		_uiTarget = null;
	}

	public void ResetState()
	{
		HideImmediate();
		_side = TooltipSide.Auto;
		_screenPos = Vector2.zero;
		_worldOffset = Vector3.zero;
	}

	private void Update()
	{
		if (_following)
		{
			if (_worldTarget != null)
			{
				Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, _worldTarget.position + _worldOffset);
				_screenPos = screenPos;
				Reposition(_screenPos, _side);
			}
			else if (_uiTarget != null)
			{
				Vector2 screenPos2 = WorldToScreenOfUI(_uiTarget);
				_screenPos = screenPos2;
				Reposition(_screenPos, _side);
			}
		}
	}

	private Vector2 WorldToScreenOfUI(RectTransform rt)
	{
		Vector3[] array = new Vector3[4];
		rt.GetWorldCorners(array);
		Vector3 worldPoint = (array[0] + array[2]) * 0.5f;
		return RectTransformUtility.WorldToScreenPoint(_uiCam, worldPoint);
	}

	private void StartShowFlow(string text, float delay)
	{
		if (_delayCo != null)
		{
			StopCoroutine(_delayCo);
		}
		_delayCo = StartCoroutine(CoShow(text, delay));
	}

	private IEnumerator CoShow(string text, float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSecondsRealtime(delay);
		}
		_view.SetText(text);
		Reposition(_screenPos, _side);
		if (_fadeCo != null)
		{
			StopCoroutine(_fadeCo);
		}
		float duration = ((defaultTheme != null) ? defaultTheme.fadeInDuration : 0.1f);
		_fadeCo = _view.FadeTo(this, 1f, duration);
		yield return _fadeCo;
	}

	private void StartHideFlow(float delay)
	{
		if (_delayCo != null)
		{
			StopCoroutine(_delayCo);
		}
		_delayCo = StartCoroutine(CoHide(delay));
	}

	private IEnumerator CoHide(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSecondsRealtime(delay);
		}
		if (_fadeCo != null)
		{
			StopCoroutine(_fadeCo);
		}
		float duration = ((defaultTheme != null) ? defaultTheme.fadeOutDuration : 0.1f);
		_fadeCo = _view.FadeTo(this, 0f, duration);
		yield return _fadeCo;
		_following = false;
		_worldTarget = null;
		_uiTarget = null;
	}

	private void Reposition(Vector2 screenPos, TooltipSide side)
	{
		TooltipSide side2 = ((side == TooltipSide.Auto) ? ChooseSide(screenPos) : side);
		Vector2 vector = screenPos;
		if (_useEdgeAnchor)
		{
			if (_uiTarget != null)
			{
				vector = EdgeAnchorForUI(_uiTarget, side2, _currentEdgeGap);
			}
			else if (_worldTarget != null)
			{
				vector = EdgeAnchorForWorld(_worldTarget, side2, _currentEdgeGap);
			}
		}
		Vector2 pivot = PivotForSide(side2);
		_view.SetPivot(pivot);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, vector, _uiCam, out var localPoint);
		Vector2 vector2 = (_useEdgeAnchor ? Vector2.zero : DefaultOffsetForSide(side2));
		_view.root.anchoredPosition = localPoint + vector2;
		_view.KeepInsideCanvas(safeMargin);
		_view.PlaceTail(side2, vector);
	}

	private Vector2 DefaultOffsetForSide(TooltipSide side)
	{
		return side switch
		{
			TooltipSide.Top => new Vector2(0f, 12f), 
			TooltipSide.Bottom => new Vector2(0f, -12f), 
			TooltipSide.Left => new Vector2(-12f, 0f), 
			TooltipSide.Right => new Vector2(12f, 0f), 
			_ => defaultOffset, 
		};
	}

	private TooltipSide ChooseSide(Vector2 screenPos)
	{
		return TooltipSide.Top;
	}

	private Vector2 PivotForSide(TooltipSide side)
	{
		return side switch
		{
			TooltipSide.Top => new Vector2(0.5f, 0f), 
			TooltipSide.Bottom => new Vector2(0.5f, 1f), 
			TooltipSide.Left => new Vector2(1f, 0.5f), 
			TooltipSide.Right => new Vector2(0f, 0.5f), 
			_ => new Vector2(0.5f, 0f), 
		};
	}

	private Vector2 EdgeAnchorForUI(RectTransform target, TooltipSide side, float gap)
	{
		Vector3[] array = new Vector3[4];
		target.GetWorldCorners(array);
		switch (side)
		{
		case TooltipSide.Top:
		{
			Vector3 vector4 = (array[1] + array[2]) * 0.5f;
			return RectTransformUtility.WorldToScreenPoint(_uiCam, vector4 + Vector3.up * (gap / _canvasRect.lossyScale.y));
		}
		case TooltipSide.Bottom:
		{
			Vector3 vector3 = (array[0] + array[3]) * 0.5f;
			return RectTransformUtility.WorldToScreenPoint(_uiCam, vector3 + Vector3.down * (gap / _canvasRect.lossyScale.y));
		}
		case TooltipSide.Left:
		{
			Vector3 vector2 = (array[0] + array[1]) * 0.5f;
			return RectTransformUtility.WorldToScreenPoint(_uiCam, vector2 + Vector3.left * (gap / _canvasRect.lossyScale.y));
		}
		case TooltipSide.Right:
		{
			Vector3 vector = (array[3] + array[2]) * 0.5f;
			return RectTransformUtility.WorldToScreenPoint(_uiCam, vector + Vector3.right * (gap / _canvasRect.lossyScale.y));
		}
		default:
			return WorldToScreenOfUI(target);
		}
	}

	private Vector2 EdgeAnchorForWorld(Transform t, TooltipSide side, float gap)
	{
		Camera main = Camera.main;
		if (TryGetBounds(t, out var b))
		{
			Vector3 worldPoint = b.center;
			switch (side)
			{
			case TooltipSide.Top:
				worldPoint = new Vector3(worldPoint.x, b.max.y, worldPoint.z) + Vector3.up * gap * 0.01f;
				break;
			case TooltipSide.Bottom:
				worldPoint = new Vector3(worldPoint.x, b.min.y, worldPoint.z) + Vector3.down * gap * 0.01f;
				break;
			case TooltipSide.Left:
				worldPoint = new Vector3(b.min.x, worldPoint.y, worldPoint.z) + Vector3.left * gap * 0.01f;
				break;
			case TooltipSide.Right:
				worldPoint = new Vector3(b.max.x, worldPoint.y, worldPoint.z) + Vector3.right * gap * 0.01f;
				break;
			}
			return RectTransformUtility.WorldToScreenPoint(main, worldPoint);
		}
		return RectTransformUtility.WorldToScreenPoint(main, t.position);
	}

	private bool TryGetBounds(Transform t, out Bounds b)
	{
		Collider component = t.GetComponent<Collider>();
		if ((bool)component)
		{
			b = component.bounds;
			return true;
		}
		Collider2D component2 = t.GetComponent<Collider2D>();
		if ((bool)component2)
		{
			b = new Bounds(component2.bounds.center, component2.bounds.size);
			return true;
		}
		Renderer componentInChildren = t.GetComponentInChildren<Renderer>();
		if ((bool)componentInChildren)
		{
			b = componentInChildren.bounds;
			return true;
		}
		b = default(Bounds);
		return false;
	}
}
