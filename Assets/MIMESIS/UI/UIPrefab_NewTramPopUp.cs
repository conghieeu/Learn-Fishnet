using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_NewTramPopUp : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_CheckPopup = "CheckPopup";

	public const string UEID_Popup = "Popup";

	public const string UEID_OK = "OK";

	public const string UEID_Cancel = "Cancel";

	private Image _UE_rootNode;

	private Transform _UE_CheckPopup;

	private Image _UE_Popup;

	private Button _UE_OK;

	private Action<string> _OnOK;

	private Button _UE_Cancel;

	private Action<string> _OnCancel;

	[Header("Progress Animation Settings")]
	[SerializeField]
	private Image progressImage;

	[SerializeField]
	private float holdDuration = 2f;

	private bool isHolding;

	private Coroutine holdCoroutine;

	public Action OnDeleteAndCreateNew;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public Transform UE_CheckPopup => _UE_CheckPopup ?? (_UE_CheckPopup = PickTransform("CheckPopup"));

	public Image UE_Popup => _UE_Popup ?? (_UE_Popup = PickImage("Popup"));

	public Button UE_OK => _UE_OK ?? (_UE_OK = PickButton("OK"));

	public Action<string> OnOK
	{
		get
		{
			return _OnOK;
		}
		set
		{
			_OnOK = value;
			SetOnButtonClick("OK", value);
		}
	}

	public Button UE_Cancel => _UE_Cancel ?? (_UE_Cancel = PickButton("Cancel"));

	public Action<string> OnCancel
	{
		get
		{
			return _OnCancel;
		}
		set
		{
			_OnCancel = value;
			SetOnButtonClick("Cancel", value);
		}
	}

	private void OnEnable()
	{
		UE_OK.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_Cancel.GetComponentInChildren<TMP_Text>().color = Color.white;
	}

	private void Start()
	{
		AddMouseOverEnterEvent(UE_OK.gameObject, UE_OK.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_Cancel.gameObject, UE_Cancel.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_OK.gameObject, UE_OK.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_Cancel.gameObject, UE_Cancel.GetComponentInChildren<TMP_Text>());
	}

	protected override void OnShow()
	{
		base.OnShow();
		SetupHoldButton();
		ResetProgress();
	}

	private void SetupHoldButton()
	{
		if (!(UE_OK == null))
		{
			EventTrigger eventTrigger = UE_OK.gameObject.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = UE_OK.gameObject.AddComponent<EventTrigger>();
			}
			eventTrigger.triggers.Clear();
			AddMouseOverEnterEvent(UE_OK.gameObject, UE_OK.GetComponentInChildren<TMP_Text>());
			AddMouseOverExitEvent(UE_OK.gameObject, UE_OK.GetComponentInChildren<TMP_Text>());
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate(BaseEventData data)
			{
				OnPointerDown((PointerEventData)data);
			});
			eventTrigger.triggers.Add(entry);
			EventTrigger.Entry entry2 = new EventTrigger.Entry();
			entry2.eventID = EventTriggerType.PointerUp;
			entry2.callback.AddListener(delegate(BaseEventData data)
			{
				OnPointerUp((PointerEventData)data);
			});
			eventTrigger.triggers.Add(entry2);
			EventTrigger.Entry entry3 = new EventTrigger.Entry();
			entry3.eventID = EventTriggerType.PointerExit;
			entry3.callback.AddListener(delegate(BaseEventData data)
			{
				OnPointerUp((PointerEventData)data);
			});
			eventTrigger.triggers.Add(entry3);
		}
	}

	private void OnPointerDown(PointerEventData eventData)
	{
		if (!isHolding)
		{
			isHolding = true;
			holdCoroutine = StartCoroutine(HoldProgressCoroutine());
		}
	}

	private void OnPointerUp(PointerEventData eventData)
	{
		if (isHolding)
		{
			isHolding = false;
			if (holdCoroutine != null)
			{
				StopCoroutine(holdCoroutine);
				holdCoroutine = null;
				EventSystem.current?.SetSelectedGameObject(null);
			}
			ResetProgress();
		}
	}

	private IEnumerator HoldProgressCoroutine()
	{
		float elapsed = 0f;
		if (progressImage != null)
		{
			progressImage.gameObject.SetActive(value: true);
			progressImage.fillAmount = 0f;
		}
		while (elapsed < holdDuration)
		{
			elapsed += Time.deltaTime;
			float fillAmount = Mathf.Clamp01(elapsed / holdDuration);
			if (progressImage != null)
			{
				progressImage.fillAmount = fillAmount;
			}
			yield return null;
		}
		OnHoldComplete();
	}

	private void ResetProgress()
	{
		if (progressImage != null)
		{
			progressImage.fillAmount = 0f;
			progressImage.gameObject.SetActive(value: false);
		}
	}

	private void OnHoldComplete()
	{
		isHolding = false;
		ResetProgress();
		OnDeleteAndCreateNew?.Invoke();
	}

	protected override void OnHide()
	{
		base.OnHide();
		if (holdCoroutine != null)
		{
			StopCoroutine(holdCoroutine);
			holdCoroutine = null;
		}
		isHolding = false;
		ResetProgress();
	}
}
