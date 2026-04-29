using System;
using ReluProtocol.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_ReturnToMainMenu : UIPrefabScript
{
	public const string UEID_QuitButton = "QuitButton";

	public const string UEID_CancelButton = "CancelButton";

	private Button _UE_QuitButton;

	private Action<string> _OnQuitButton;

	private Button _UE_CancelButton;

	private Action<string> _OnCancelButton;

	[HideInInspector]
	public GameObject inGameMenuUIObject;

	public Button UE_QuitButton => _UE_QuitButton ?? (_UE_QuitButton = PickButton("QuitButton"));

	public Action<string> OnQuitButton
	{
		get
		{
			return _OnQuitButton;
		}
		set
		{
			_OnQuitButton = value;
			SetOnButtonClick("QuitButton", value);
		}
	}

	public Button UE_CancelButton => _UE_CancelButton ?? (_UE_CancelButton = PickButton("CancelButton"));

	public Action<string> OnCancelButton
	{
		get
		{
			return _OnCancelButton;
		}
		set
		{
			_OnCancelButton = value;
			SetOnButtonClick("CancelButton", value);
		}
	}

	private void Start()
	{
		UE_QuitButton.onClick.AddListener(ReturnToMainMenu);
		UE_CancelButton.onClick.AddListener(CloseQuitPopup);
		AddMouseOverEnterEvent(UE_QuitButton.gameObject, UE_QuitButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_CancelButton.gameObject, UE_CancelButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_QuitButton.gameObject, UE_QuitButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_CancelButton.gameObject, UE_CancelButton.GetComponentInChildren<TMP_Text>());
	}

	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.None;
	}

	public void CloseQuitPopup()
	{
		Hub.s.uiman.CloseRetrunToMainMenu();
	}

	public void ReturnToMainMenu()
	{
		Hub.s.steamInviteDispatcher.joinOnce = false;
		Hub.s.netman2.Disconnect(DisconnectReason.ByClient);
	}
}
