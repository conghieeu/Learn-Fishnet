using System;
using System.Collections;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPrefab_JoinTramWithCode : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_yourname = "yourname";

	public const string UEID_InputField_code = "InputField_code";

	public const string UEID_ButtonOK = "ButtonOK";

	public const string UEID_ButtonBack = "ButtonBack";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_Text _UE_yourname;

	private TMP_InputField _UE_InputField_code;

	private Button _UE_ButtonOK;

	private Action<string> _OnButtonOK;

	private Button _UE_ButtonBack;

	private Action<string> _OnButtonBack;

	private int x;

	private int y;

	private int w = Screen.width;

	private int h = Mathf.RoundToInt((float)Screen.height * 0.3f);

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_yourname => _UE_yourname ?? (_UE_yourname = PickText("yourname"));

	public TMP_InputField UE_InputField_code => _UE_InputField_code ?? (_UE_InputField_code = PickInputField("InputField_code"));

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

	public Button UE_ButtonBack => _UE_ButtonBack ?? (_UE_ButtonBack = PickButton("ButtonBack"));

	public Action<string> OnButtonBack
	{
		get
		{
			return _OnButtonBack;
		}
		set
		{
			_OnButtonBack = value;
			SetOnButtonClick("ButtonBack", value);
		}
	}

	private void Start()
	{
		dialogue = false;
		UE_ButtonOK.onClick.AddListener(OKBtn);
		UE_ButtonBack.onClick.AddListener(BackBtn);
		AddMouseOverEnterEvent(UE_ButtonOK.gameObject, UE_ButtonOK.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_ButtonBack.gameObject, UE_ButtonBack.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ButtonOK.gameObject, UE_ButtonOK.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ButtonBack.gameObject, UE_ButtonBack.GetComponentInChildren<TMP_Text>());
	}

	private void OnEnable()
	{
		UE_ButtonOK.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_ButtonBack.GetComponentInChildren<TMP_Text>().color = Color.white;
		StartCoroutine(FocusInputField());
	}

	private IEnumerator FocusInputField()
	{
		yield return null;
		if (EventSystem.current != null && UE_InputField_code != null)
		{
			EventSystem.current.SetSelectedGameObject(UE_InputField_code.gameObject);
			UE_InputField_code.Select();
			UE_InputField_code.ActivateInputField();
			OpenKeyboardSteamdeck();
		}
	}

	private void OpenKeyboardSteamdeck()
	{
		if (SteamManager.Initialized && SteamUtils.IsSteamRunningOnSteamDeck())
		{
			SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, x, y, w, h);
		}
	}

	public void OKBtn()
	{
		string text = UE_InputField_code.text;
		if (Hub.s != null)
		{
			text = text.Trim();
			Hub.s.steamInviteDispatcher.JoinFriendWithMatchKeyProcess(text);
			Hide();
		}
	}

	public void BackBtn()
	{
		Hide();
	}

	private void Update()
	{
		if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
		{
			OKBtn();
		}
	}
}
