using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class MarqueeText : MonoBehaviour
{
	[Tooltip("스크롤 속도(px/sec)")]
	public float speed = 50f;

	private RectTransform _textRect;

	private RectTransform _maskRect;

	private TextMeshProUGUI _tmp;

	private float _textWidth;

	private float _maskWidth;

	private Vector2 _startPos;

	private void Awake()
	{
		_textRect = GetComponent<RectTransform>();
		_tmp = GetComponent<TextMeshProUGUI>();
		_maskRect = base.transform.parent.GetComponent<RectTransform>();
		TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
	}

	private void OnDestroy()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
	}

	private void Start()
	{
		InitMarquee();
	}

	private void OnTextChanged(Object obj)
	{
		if (obj == _tmp)
		{
			InitMarquee();
		}
	}

	public void InitMarquee()
	{
		_textWidth = _tmp.GetPreferredValues(_tmp.text).x;
		_maskWidth = _maskRect.rect.width;
		if (_textWidth <= _maskWidth)
		{
			_tmp.alignment = TextAlignmentOptions.Right;
			return;
		}
		_startPos = new Vector2(_maskWidth, _textRect.anchoredPosition.y);
		_textRect.anchoredPosition = _startPos;
	}

	private void Update()
	{
		if (!(_textWidth <= _maskWidth))
		{
			float num = _textRect.anchoredPosition.x - speed * Time.deltaTime;
			if (num <= 0f - _textWidth)
			{
				num = _startPos.x;
			}
			_textRect.anchoredPosition = new Vector2(num, _startPos.y);
		}
	}
}
