using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_BrightnessCalibration : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_Image = "Image";

	public const string UEID_Slider = "Slider";

	public const string UEID_ButtonOK = "ButtonOK";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private Image _UE_Image;

	private Transform _UE_Slider;

	private Button _UE_ButtonOK;

	private Action<string> _OnButtonOK;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public Image UE_Image => _UE_Image ?? (_UE_Image = PickImage("Image"));

	public Transform UE_Slider => _UE_Slider ?? (_UE_Slider = PickTransform("Slider"));

	public Button UE_ButtonOK => _UE_ButtonOK ?? (_UE_ButtonOK = PickButton("ButtonOK"));

	public Action<string> OnButtonOK
	{
		get
		{
			return _OnButtonOK;
		}
		set
		{
			_OnButtonOK = value;
			SetOnButtonClick("ButtonOK", value);
		}
	}

	private void Start()
	{
		AddMouseOverEnterEvent(UE_ButtonOK.gameObject);
		AddMouseOverExitEvent(UE_ButtonOK.gameObject);
		UE_Slider.GetComponent<Slider>().minValue = Hub.s.gameConfig.gameSetting.minBrightness;
		UE_Slider.GetComponent<Slider>().maxValue = Hub.s.gameConfig.gameSetting.maxBrightness;
		UE_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.brightness;
		UE_Slider.GetComponent<Slider>().onValueChanged.AddListener(delegate(float value)
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				OnValueChangedBrightness(value);
			}
		});
		UpdateUIBrightness(Hub.s.gameSettingManager.brightness);
	}

	public void OnValueChangedBrightness(float value)
	{
		Hub.s.gameSettingManager.brightness = value;
		UpdateUIBrightness(value);
	}

	private void UpdateUIBrightness(float brightnessValue)
	{
		float num = 0.5f + brightnessValue;
		Color color = new Color(num, num, num, 1f);
		UE_Image.color = color;
	}

	private void AddMouseOverEnterEvent(GameObject go)
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
			if (enableUISound && !string.IsNullOrEmpty(buttonHoverSfxId) && Application.isFocused)
			{
				Hub.s.audioman.PlaySfx(buttonHoverSfxId);
			}
		});
		eventTrigger.triggers.Add(entry);
	}

	private void AddMouseOverExitEvent(GameObject go)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = go.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerExit;
		eventTrigger.triggers.Add(entry);
	}
}
