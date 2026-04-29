using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

internal class UIPrefab_login : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_OK_button = "OK_button";

	public const string UEID_InputField_name = "InputField_name";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private Button _UE_OK_button;

	private Action<string> _OnOK_button;

	private TMP_InputField _UE_InputField_name;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public Button UE_OK_button => _UE_OK_button ?? (_UE_OK_button = PickButton("OK_button"));

	public Action<string> OnOK_button
	{
		get
		{
			return _OnOK_button;
		}
		set
		{
			_OnOK_button = value;
			SetOnButtonClick("OK_button", value);
		}
	}

	public TMP_InputField UE_InputField_name => _UE_InputField_name ?? (_UE_InputField_name = PickInputField("InputField_name"));

	private void Start()
	{
		AddMouseOverEnterEvent(UE_OK_button.gameObject);
		AddMouseOverExitEvent(UE_OK_button.gameObject);
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
