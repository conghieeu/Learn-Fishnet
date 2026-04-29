using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=4648685001")]
public class UIPrefabScript : MonoBehaviour
{
	protected delegate void MouseOverDelegate();

	[SerializeField]
	private DOTAnimationUtility.eTypes openAnimation;

	[SerializeField]
	private DOTAnimationUtility.eTypes closeAnimation;

	[SerializeField]
	private UIElementMarker[] elements;

	private Dictionary<string, UIElementMarker> dictElements = new Dictionary<string, UIElementMarker>();

	private Dictionary<string, Action<string>> onButtonClick = new Dictionary<string, Action<string>>();

	[HideInInspector]
	public bool dialogue = true;

	[Header("UI Sounds")]
	[SerializeField]
	protected string buttonClickSfxId = "ButtonClick";

	[SerializeField]
	protected string buttonHoverSfxId = "ButtonHover";

	[SerializeField]
	protected bool enableUISound = true;

	protected UIManager uiman => Hub.s.uiman;

	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		UIElementMarker[] array = elements;
		foreach (UIElementMarker element in array)
		{
			if (element == null)
			{
				Logger.RError("UIElement is null @ " + base.name);
			}
			else
			{
				if (element.IgnoreMe)
				{
					continue;
				}
				if (element.asButton != null)
				{
					element.asButton.onClick.AddListener(delegate
					{
						OnButtonClick(element.name);
					});
				}
				string key = element.name.Replace(' ', '_');
				dictElements.Add(key, element);
			}
		}
		base.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(value: true);
			DOTAnimationUtility.Do(this, openAnimation);
		}
		OnShow();
	}

	public void Hide()
	{
		if (QuittingProcess.isQuitting || this == null || base.gameObject == null || !base.gameObject.activeSelf)
		{
			return;
		}
		if (closeAnimation == DOTAnimationUtility.eTypes.None)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			DOTAnimationUtility.Do(base.transform, closeAnimation)?.OnComplete(delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
		EventSystem.current?.SetSelectedGameObject(null);
		OnHide();
	}

	protected virtual void OnShow()
	{
	}

	protected virtual void OnHide()
	{
	}

	public async UniTask UniTask_Hide()
	{
		if (!(base.gameObject == null) && !(this == null) && closeAnimation != DOTAnimationUtility.eTypes.None)
		{
			bool _done = false;
			DOTAnimationUtility.Do(base.transform, closeAnimation)?.OnComplete(delegate
			{
				_done = true;
			});
			await UniTask.WaitWhile(() => !_done);
		}
		base.gameObject.SetActive(value: false);
	}

	public IEnumerator Cor_Hide()
	{
		if (!(base.gameObject == null) && !(this == null) && closeAnimation != DOTAnimationUtility.eTypes.None)
		{
			bool _done = false;
			DOTAnimationUtility.Do(base.transform, closeAnimation)?.OnComplete(delegate
			{
				_done = true;
			});
			yield return new WaitUntil(() => _done);
		}
		base.gameObject.SetActive(value: false);
	}

	private void OnButtonClick(string _id)
	{
		if (enableUISound && !string.IsNullOrEmpty(buttonClickSfxId) && Application.isFocused)
		{
			Hub.s.audioman.PlaySfx(buttonClickSfxId);
		}
		if (dictElements.TryGetValue(_id, out var value))
		{
			if (value.asButton == null)
			{
				LocalLog(_id + " is not a button");
			}
			if (onButtonClick.TryGetValue(_id, out var value2))
			{
				value2?.Invoke(_id);
			}
			else
			{
				LocalLog("unbound button : " + _id);
			}
		}
		else
		{
			LocalLog("cannot find button : " + _id);
		}
	}

	public void SetOnButtonClick(string buttonId, Action<string> onUIButton)
	{
		onButtonClick[buttonId] = onUIButton;
	}

	public void ClearOnButtonClick(string buttonId)
	{
		if (onButtonClick.ContainsKey(buttonId))
		{
			onButtonClick.Remove(buttonId);
		}
	}

	public Button PickButton(string id)
	{
		if (dictElements.TryGetValue(id, out var value))
		{
			return value.asButton;
		}
		return null;
	}

	public TMP_Text PickText(string id)
	{
		if (dictElements.TryGetValue(id, out var value))
		{
			return value.asText;
		}
		return null;
	}

	public Image PickImage(string id)
	{
		if (dictElements.TryGetValue(id, out var value))
		{
			return value.asImage;
		}
		return null;
	}

	public TMP_InputField PickInputField(string id)
	{
		if (dictElements.TryGetValue(id, out var value))
		{
			return value.asInputField;
		}
		return null;
	}

	public Transform PickTransform(string id)
	{
		if (dictElements.TryGetValue(id, out var value))
		{
			return value.transform;
		}
		return null;
	}

	public GameObject FindObjectWithNameRecursive(Transform node, string id)
	{
		if (node.name == id)
		{
			return node.gameObject;
		}
		foreach (Transform item in node)
		{
			GameObject gameObject = FindObjectWithNameRecursive(item, id);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	private void LocalLog(object o)
	{
	}

	private bool IsOtherPrefabsMarker(UIElementMarker emComponent)
	{
		bool result = false;
		Transform transform = emComponent.gameObject.transform;
		while (transform != null)
		{
			Transform parent = transform.parent;
			if (!(parent != null))
			{
				break;
			}
			UIPrefabScript component = parent.GetComponent<UIPrefabScript>();
			if (component != null)
			{
				if (component != this)
				{
					result = true;
					break;
				}
				transform = parent;
			}
			else
			{
				transform = parent;
			}
		}
		return result;
	}

	protected void AddMouseOverEnterEvent(GameObject go, TMP_Text textTarget, MouseOverDelegate @delegate = null)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = go.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(delegate
		{
			if (!go.GetComponent<Button>() || go.GetComponent<Button>().interactable)
			{
				if (enableUISound && !string.IsNullOrEmpty(buttonHoverSfxId) && Application.isFocused)
				{
					Hub.s.audioman.PlaySfx(buttonHoverSfxId);
				}
				textTarget.color = Hub.s.uiman.mouseOverTextColor;
				if (@delegate != null)
				{
					@delegate();
				}
			}
		});
		eventTrigger.triggers.Add(entry);
	}

	protected void AddMouseOverExitEvent(GameObject go, TMP_Text textTarget, MouseOverDelegate @delegate = null)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = go.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerExit;
		entry.callback.AddListener(delegate
		{
			if (!go.GetComponent<Button>() || go.GetComponent<Button>().interactable)
			{
				textTarget.color = Color.white;
				if (@delegate != null)
				{
					@delegate();
				}
			}
		});
		eventTrigger.triggers.Add(entry);
	}
}
