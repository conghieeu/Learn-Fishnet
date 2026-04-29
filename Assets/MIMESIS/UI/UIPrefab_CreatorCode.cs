using System;
using System.Collections;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_CreatorCode : UIPrefabScript
{
	public const string UEID_InputPopup = "InputPopup";

	public const string UEID_RecentCreatorCode = "RecentCreatorCode";

	public const string UEID_Inputconfirm = "Inputconfirm";

	public const string UEID_Inputcancel = "Inputcancel";

	public const string UEID_CodeInputfield = "CodeInputfield";

	public const string UEID_ConfirmPopup = "ConfirmPopup";

	public const string UEID_ConfirmCreatorCode = "ConfirmCreatorCode";

	public const string UEID_CreatorCodeConfirm = "CreatorCodeConfirm";

	public const string UEID_CancleConfirmAndEdit = "CancleConfirmAndEdit";

	public const string UEID_FailPopup = "FailPopup";

	public const string UEID_FailDescription = "FailDescription";

	public const string UEID_FailCreatorCode = "FailCreatorCode";

	public const string UEID_EditCreatorCode = "EditCreatorCode";

	private Image _UE_InputPopup;

	private TMP_Text _UE_RecentCreatorCode;

	private Button _UE_Inputconfirm;

	private Action<string> _OnInputconfirm;

	private Button _UE_Inputcancel;

	private Action<string> _OnInputcancel;

	private TMP_InputField _UE_CodeInputfield;

	private Image _UE_ConfirmPopup;

	private TMP_Text _UE_ConfirmCreatorCode;

	private Button _UE_CreatorCodeConfirm;

	private Action<string> _OnCreatorCodeConfirm;

	private Button _UE_CancleConfirmAndEdit;

	private Action<string> _OnCancleConfirmAndEdit;

	private Image _UE_FailPopup;

	private TMP_Text _UE_FailDescription;

	private TMP_Text _UE_FailCreatorCode;

	private Button _UE_EditCreatorCode;

	private Action<string> _OnEditCreatorCode;

	private int x;

	private int y;

	private int w = Screen.width;

	private int h = Mathf.RoundToInt((float)Screen.height * 0.3f);

	private string inputedCreatorCode = "";

	private string lastCreatorCode = "";

	private bool waitResult;

	public TMP_Text lastCreatorCodeTextUI;

	public Image UE_InputPopup => _UE_InputPopup ?? (_UE_InputPopup = PickImage("InputPopup"));

	public TMP_Text UE_RecentCreatorCode => _UE_RecentCreatorCode ?? (_UE_RecentCreatorCode = PickText("RecentCreatorCode"));

	public Button UE_Inputconfirm => _UE_Inputconfirm ?? (_UE_Inputconfirm = PickButton("Inputconfirm"));

	public Action<string> OnInputconfirm
	{
		get
		{
			return _OnInputconfirm;
		}
		set
		{
			_OnInputconfirm = value;
			SetOnButtonClick("Inputconfirm", value);
		}
	}

	public Button UE_Inputcancel => _UE_Inputcancel ?? (_UE_Inputcancel = PickButton("Inputcancel"));

	public Action<string> OnInputcancel
	{
		get
		{
			return _OnInputcancel;
		}
		set
		{
			_OnInputcancel = value;
			SetOnButtonClick("Inputcancel", value);
		}
	}

	public TMP_InputField UE_CodeInputfield => _UE_CodeInputfield ?? (_UE_CodeInputfield = PickInputField("CodeInputfield"));

	public Image UE_ConfirmPopup => _UE_ConfirmPopup ?? (_UE_ConfirmPopup = PickImage("ConfirmPopup"));

	public TMP_Text UE_ConfirmCreatorCode => _UE_ConfirmCreatorCode ?? (_UE_ConfirmCreatorCode = PickText("ConfirmCreatorCode"));

	public Button UE_CreatorCodeConfirm => _UE_CreatorCodeConfirm ?? (_UE_CreatorCodeConfirm = PickButton("CreatorCodeConfirm"));

	public Action<string> OnCreatorCodeConfirm
	{
		get
		{
			return _OnCreatorCodeConfirm;
		}
		set
		{
			_OnCreatorCodeConfirm = value;
			SetOnButtonClick("CreatorCodeConfirm", value);
		}
	}

	public Button UE_CancleConfirmAndEdit => _UE_CancleConfirmAndEdit ?? (_UE_CancleConfirmAndEdit = PickButton("CancleConfirmAndEdit"));

	public Action<string> OnCancleConfirmAndEdit
	{
		get
		{
			return _OnCancleConfirmAndEdit;
		}
		set
		{
			_OnCancleConfirmAndEdit = value;
			SetOnButtonClick("CancleConfirmAndEdit", value);
		}
	}

	public Image UE_FailPopup => _UE_FailPopup ?? (_UE_FailPopup = PickImage("FailPopup"));

	public TMP_Text UE_FailDescription => _UE_FailDescription ?? (_UE_FailDescription = PickText("FailDescription"));

	public TMP_Text UE_FailCreatorCode => _UE_FailCreatorCode ?? (_UE_FailCreatorCode = PickText("FailCreatorCode"));

	public Button UE_EditCreatorCode => _UE_EditCreatorCode ?? (_UE_EditCreatorCode = PickButton("EditCreatorCode"));

	public Action<string> OnEditCreatorCode
	{
		get
		{
			return _OnEditCreatorCode;
		}
		set
		{
			_OnEditCreatorCode = value;
			SetOnButtonClick("EditCreatorCode", value);
		}
	}

	private void Start()
	{
		AddMouseOverEnterEvent(UE_Inputconfirm.gameObject, UE_Inputconfirm.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_Inputcancel.gameObject, UE_Inputcancel.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_CreatorCodeConfirm.gameObject, UE_CreatorCodeConfirm.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_CancleConfirmAndEdit.gameObject, UE_CancleConfirmAndEdit.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_EditCreatorCode.gameObject, UE_EditCreatorCode.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_Inputconfirm.gameObject, UE_Inputconfirm.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_Inputcancel.gameObject, UE_Inputcancel.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_CreatorCodeConfirm.gameObject, UE_CreatorCodeConfirm.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_CancleConfirmAndEdit.gameObject, UE_CancleConfirmAndEdit.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_EditCreatorCode.gameObject, UE_EditCreatorCode.GetComponentInChildren<TMP_Text>());
	}

	private void SetTextWhite()
	{
		UE_Inputconfirm.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_Inputcancel.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_CreatorCodeConfirm.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_CancleConfirmAndEdit.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_EditCreatorCode.GetComponentInChildren<TMP_Text>().color = Color.white;
	}

	private void OnEnable()
	{
		lastCreatorCode = Hub.s.kosManager.lastCreatorCode;
		UE_RecentCreatorCode.text = lastCreatorCode;
		UE_Inputconfirm.onClick.AddListener(InputConfirm);
		UE_Inputcancel.onClick.AddListener(InputCancel);
		UE_CreatorCodeConfirm.onClick.AddListener(CreatorCodeConfirm);
		UE_CancleConfirmAndEdit.onClick.AddListener(EditCreatorCode);
		UE_EditCreatorCode.onClick.AddListener(EditCreatorCode);
		EditCreatorCode();
		StartCoroutine(FocusInputField());
	}

	private void InputConfirm()
	{
		SetTextWhite();
		UE_InputPopup.gameObject.SetActive(value: false);
		UE_ConfirmPopup.gameObject.SetActive(value: true);
		UE_ConfirmCreatorCode.text = UE_CodeInputfield.text;
		inputedCreatorCode = UE_CodeInputfield.text;
	}

	public void InputCancel()
	{
		SetTextWhite();
		UE_InputPopup.gameObject.SetActive(value: false);
		UE_ConfirmPopup.gameObject.SetActive(value: false);
		UE_FailPopup.gameObject.SetActive(value: false);
		Hide();
	}

	private void EditCreatorCode()
	{
		SetTextWhite();
		UE_InputPopup.gameObject.SetActive(value: true);
		UE_ConfirmPopup.gameObject.SetActive(value: false);
		UE_FailPopup.gameObject.SetActive(value: false);
		UE_CodeInputfield.text = "";
		inputedCreatorCode = "";
	}

	private void CreatorCodeConfirm()
	{
		if (!waitResult)
		{
			waitResult = true;
			Hub.s.kosManager.SetCreatorCode(inputedCreatorCode);
		}
	}

	public void SuccessInputCreatorCode()
	{
		if (this != null)
		{
			lastCreatorCode = inputedCreatorCode;
			UE_RecentCreatorCode.text = lastCreatorCode;
			waitResult = false;
			lastCreatorCodeTextUI.text = lastCreatorCode;
			UE_ConfirmPopup.gameObject.SetActive(value: false);
			UE_FailPopup.gameObject.SetActive(value: false);
			UE_InputPopup.gameObject.SetActive(value: false);
			Hide();
		}
	}

	public void FailInputCreatorCode(string errorCode)
	{
		if (!(this != null))
		{
			return;
		}
		if (!(errorCode == "KCNAlreadyAllAppIdExist"))
		{
			if (!(errorCode == "KCNCodeNotExist"))
			{
			}
			UE_FailDescription.text = Hub.GetL10NText("UI_PREFAB_ENTER_CREATOR_CODE_ERROR_TITLE");
			UE_FailCreatorCode.text = Hub.GetL10NText("UI_PREFAB_ENTER_CREATOR_CODE_ERROR_GUIDE");
		}
		else
		{
			UE_FailDescription.text = Hub.GetL10NText("UI_PREFAB_ENTER_CREATOR_CODE_ERROR_ALREADY");
			UE_FailCreatorCode.text = Hub.GetL10NText("UI_PREFAB_ENTER_CREATOR_CODE_ERROR_ALREADY_GUIDE");
		}
		UE_InputPopup.gameObject.SetActive(value: false);
		UE_ConfirmPopup.gameObject.SetActive(value: false);
		UE_FailPopup.gameObject.SetActive(value: true);
		waitResult = false;
	}

	private IEnumerator FocusInputField()
	{
		yield return null;
		yield return new WaitUntil(() => UE_InputPopup.gameObject.activeSelf);
		if (EventSystem.current != null && UE_CodeInputfield.gameObject.activeSelf)
		{
			EventSystem.current.SetSelectedGameObject(UE_CodeInputfield.gameObject);
			UE_CodeInputfield.Select();
			UE_CodeInputfield.ActivateInputField();
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
}
