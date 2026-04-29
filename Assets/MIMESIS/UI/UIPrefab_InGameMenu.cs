using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mimic.Actors;
using Mimic.Voice;
using ReluNetwork.ConstEnum;
using ReluProtocol.Enum;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPrefab_InGameMenu : UIPrefabScript
{
	private class VolumeContollerPlayerInfo
	{
		public string playerId;

		public string steamID;

		public long playerUID;

		public string nickName = "";
	}

	public const string UEID_ContinueGameButton = "ContinueGameButton";

	public const string UEID_InviteFriendsButton = "InviteFriendsButton";

	public const string UEID_SettingButton = "SettingButton";

	public const string UEID_QuitButton = "QuitButton";

	public const string UEID_Player1 = "Player1";

	public const string UEID_ping1 = "ping1";

	public const string UEID_Player2 = "Player2";

	public const string UEID_player2Slider = "player2Slider";

	public const string UEID_ping2 = "ping2";

	public const string UEID_Player3 = "Player3";

	public const string UEID_player3Slider = "player3Slider";

	public const string UEID_ping3 = "ping3";

	public const string UEID_Player4 = "Player4";

	public const string UEID_player4Slider = "player4Slider";

	public const string UEID_ping4 = "ping4";

	public const string UEID_InviteRinkCopy = "InviteRinkCopy";

	public const string UEID_versionText = "versionText";

	public const string UEID_PublicRoom = "PublicRoom";

	public const string UEID_PublicRoomToggle = "PublicRoomToggle";

	public const string UEID_InputFieldRoomName = "InputFieldRoomName";

	public const string UEID_ChangeRoomNameButton = "ChangeRoomNameButton";

	private Button _UE_ContinueGameButton;

	private Action<string> _OnContinueGameButton;

	private Button _UE_InviteFriendsButton;

	private Action<string> _OnInviteFriendsButton;

	private Button _UE_SettingButton;

	private Action<string> _OnSettingButton;

	private Button _UE_QuitButton;

	private Action<string> _OnQuitButton;

	private Transform _UE_Player1;

	private Image _UE_ping1;

	private Transform _UE_Player2;

	private Transform _UE_player2Slider;

	private Image _UE_ping2;

	private Transform _UE_Player3;

	private Transform _UE_player3Slider;

	private Image _UE_ping3;

	private Transform _UE_Player4;

	private Transform _UE_player4Slider;

	private Image _UE_ping4;

	private Button _UE_InviteRinkCopy;

	private Action<string> _OnInviteRinkCopy;

	private TMP_Text _UE_versionText;

	private Transform _UE_PublicRoom;

	private Transform _UE_PublicRoomToggle;

	private TMP_InputField _UE_InputFieldRoomName;

	private Button _UE_ChangeRoomNameButton;

	private Action<string> _OnChangeRoomNameButton;

	public GameObject quitPopupPrefab;

	private UIPrefab_ReturnToMainMenu quitPopup;

	public List<GameObject> playerList = new List<GameObject>();

	public List<TMP_Text> playerNickNameTextList = new List<TMP_Text>();

	public List<Image> playerAvatarImageList = new List<Image>();

	public List<Slider> playerVolumeSliderList = new List<Slider>();

	public List<Button> playerSpeakButtonList = new List<Button>();

	public Sprite volume0;

	public Sprite volume1;

	public Sprite volume2;

	public Sprite volume3;

	private List<float> tempVolumeList = new List<float>();

	[SerializeField]
	private List<Sprite> pingSprites = new List<Sprite>();

	private List<VolumeContollerPlayerInfo> playerInfos = new List<VolumeContollerPlayerInfo>();

	[SerializeField]
	private List<Image> pingImageList = new List<Image>();

	private int currnetPlayerCount;

	private NetworkGradeSig tempNetworkGradeSig = new NetworkGradeSig();

	private Dictionary<CSteamID, Texture2D> avatarCache = new Dictionary<CSteamID, Texture2D>();

	public bool isUpdated;

	private bool copied;

	[HideInInspector]
	public string lobbyName = "";

	public Button UE_ContinueGameButton => _UE_ContinueGameButton ?? (_UE_ContinueGameButton = PickButton("ContinueGameButton"));

	public Action<string> OnContinueGameButton
	{
		get
		{
			return _OnContinueGameButton;
		}
		set
		{
			_OnContinueGameButton = value;
			SetOnButtonClick("ContinueGameButton", value);
		}
	}

	public Button UE_InviteFriendsButton => _UE_InviteFriendsButton ?? (_UE_InviteFriendsButton = PickButton("InviteFriendsButton"));

	public Action<string> OnInviteFriendsButton
	{
		get
		{
			return _OnInviteFriendsButton;
		}
		set
		{
			_OnInviteFriendsButton = value;
			SetOnButtonClick("InviteFriendsButton", value);
		}
	}

	public Button UE_SettingButton => _UE_SettingButton ?? (_UE_SettingButton = PickButton("SettingButton"));

	public Action<string> OnSettingButton
	{
		get
		{
			return _OnSettingButton;
		}
		set
		{
			_OnSettingButton = value;
			SetOnButtonClick("SettingButton", value);
		}
	}

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

	public Transform UE_Player1 => _UE_Player1 ?? (_UE_Player1 = PickTransform("Player1"));

	public Image UE_ping1 => _UE_ping1 ?? (_UE_ping1 = PickImage("ping1"));

	public Transform UE_Player2 => _UE_Player2 ?? (_UE_Player2 = PickTransform("Player2"));

	public Transform UE_player2Slider => _UE_player2Slider ?? (_UE_player2Slider = PickTransform("player2Slider"));

	public Image UE_ping2 => _UE_ping2 ?? (_UE_ping2 = PickImage("ping2"));

	public Transform UE_Player3 => _UE_Player3 ?? (_UE_Player3 = PickTransform("Player3"));

	public Transform UE_player3Slider => _UE_player3Slider ?? (_UE_player3Slider = PickTransform("player3Slider"));

	public Image UE_ping3 => _UE_ping3 ?? (_UE_ping3 = PickImage("ping3"));

	public Transform UE_Player4 => _UE_Player4 ?? (_UE_Player4 = PickTransform("Player4"));

	public Transform UE_player4Slider => _UE_player4Slider ?? (_UE_player4Slider = PickTransform("player4Slider"));

	public Image UE_ping4 => _UE_ping4 ?? (_UE_ping4 = PickImage("ping4"));

	public Button UE_InviteRinkCopy => _UE_InviteRinkCopy ?? (_UE_InviteRinkCopy = PickButton("InviteRinkCopy"));

	public Action<string> OnInviteRinkCopy
	{
		get
		{
			return _OnInviteRinkCopy;
		}
		set
		{
			_OnInviteRinkCopy = value;
			SetOnButtonClick("InviteRinkCopy", value);
		}
	}

	public TMP_Text UE_versionText => _UE_versionText ?? (_UE_versionText = PickText("versionText"));

	public Transform UE_PublicRoom => _UE_PublicRoom ?? (_UE_PublicRoom = PickTransform("PublicRoom"));

	public Transform UE_PublicRoomToggle => _UE_PublicRoomToggle ?? (_UE_PublicRoomToggle = PickTransform("PublicRoomToggle"));

	public TMP_InputField UE_InputFieldRoomName => _UE_InputFieldRoomName ?? (_UE_InputFieldRoomName = PickInputField("InputFieldRoomName"));

	public Button UE_ChangeRoomNameButton => _UE_ChangeRoomNameButton ?? (_UE_ChangeRoomNameButton = PickButton("ChangeRoomNameButton"));

	public Action<string> OnChangeRoomNameButton
	{
		get
		{
			return _OnChangeRoomNameButton;
		}
		set
		{
			_OnChangeRoomNameButton = value;
			SetOnButtonClick("ChangeRoomNameButton", value);
		}
	}

	private void Start()
	{
		UE_ContinueGameButton.onClick.AddListener(ContinueGame);
		UE_InviteFriendsButton.onClick.AddListener(InviteFriends);
		UE_SettingButton.onClick.AddListener(delegate
		{
			UE_SettingButton.GetComponentInChildren<TMP_Text>().color = Color.white;
			Hub.s.uiman.OpenGameSettings(this, inGame: true);
		});
		UE_QuitButton.onClick.AddListener(OpenQuidPopup);
		AddMouseOverEnterEvent(UE_ContinueGameButton.gameObject, UE_ContinueGameButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_InviteFriendsButton.gameObject, UE_InviteFriendsButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_SettingButton.gameObject, UE_SettingButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_QuitButton.gameObject, UE_QuitButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ContinueGameButton.gameObject, UE_ContinueGameButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_InviteFriendsButton.gameObject, UE_InviteFriendsButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_SettingButton.gameObject, UE_SettingButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_QuitButton.gameObject, UE_QuitButton.GetComponentInChildren<TMP_Text>());
		EventTrigger eventTrigger = UE_ChangeRoomNameButton.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = UE_ChangeRoomNameButton.gameObject.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(delegate
		{
			if (UE_ChangeRoomNameButton.interactable)
			{
				UE_ChangeRoomNameButton.transform.GetChild(0).gameObject.SetActive(value: true);
			}
		});
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerExit;
		entry2.callback.AddListener(delegate
		{
			UE_ChangeRoomNameButton.transform.GetChild(0).gameObject.SetActive(value: false);
		});
		eventTrigger.triggers.Add(entry);
		eventTrigger.triggers.Add(entry2);
		for (int num = 0; num < playerSpeakButtonList.Count; num++)
		{
			int index = num;
			playerSpeakButtonList[index].onClick.AddListener(delegate
			{
				OnClickSpeakButton(index);
			});
		}
		SetVersionText();
		UE_InviteRinkCopy.onClick.AddListener(CopyText);
		UE_ChangeRoomNameButton.onClick.AddListener(SetPublicRoomName);
		UE_ChangeRoomNameButton.interactable = false;
		UE_ChangeRoomNameButton.GetComponentInChildren<TMP_Text>().text = Hub.GetL10NText("STRING_PUBLIC_TRAM_BUTTON_APPLIED");
		UE_InputFieldRoomName.placeholder.GetComponent<TMP_Text>().text = Hub.GetL10NText("STRING_PUBLIC_TRAM_TITLE_DEFAULT", Hub.s.pdata.MyNickName);
		UE_InputFieldRoomName.placeholder.color = Color.gray;
		UE_InputFieldRoomName.onValueChanged.AddListener(delegate
		{
			UE_ChangeRoomNameButton.GetComponentInChildren<TMP_Text>().text = Hub.GetL10NText("STRING_PUBLIC_TRAM_BUTTON_APPLY");
			UE_ChangeRoomNameButton.interactable = true;
		});
		UE_PublicRoomToggle.GetComponent<Toggle>().isOn = false;
		UE_PublicRoomToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value)
		{
			if (Hub.s.pdata.ClientMode == NetworkClientMode.Host && SceneManager.GetActiveScene().name == "MaintenanceScene")
			{
				Hub.s.steamInviteDispatcher.SetLobbyPublic(value);
			}
		});
	}

	public void CopyText()
	{
		GUIUtility.systemCopyBuffer = Hub.s.steamInviteDispatcher.HostMatchCode;
		UE_InviteRinkCopy.GetComponentInChildren<TMP_Text>().text = Hub.GetL10NText("STRING_COPY_INVITE_CODE_COPIED");
		StartCoroutine(CopyTextCoroutine());
	}

	private IEnumerator CopyTextCoroutine()
	{
		if (!copied)
		{
			copied = true;
			yield return new WaitForSeconds(1f);
			EventSystem.current.SetSelectedGameObject(null);
			UE_InviteRinkCopy.GetComponentInChildren<UIApplyL10N>().ApplyL10N();
			copied = false;
		}
	}

	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.None;
		EventSystem.current.SetSelectedGameObject(null);
		UE_ContinueGameButton.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_InviteFriendsButton.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_SettingButton.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_QuitButton.GetComponentInChildren<TMP_Text>().color = Color.white;
		string text = Hub.s.steamInviteDispatcher.lobbyName;
		text = SteamMatchmaking.GetLobbyData(Hub.s.steamInviteDispatcher.joinedLobbyID, "LobbyName");
		UE_InputFieldRoomName.text = text;
		if (Hub.s.pdata.ClientMode == NetworkClientMode.Host)
		{
			UE_PublicRoom.GetComponent<RectTransform>().SetAsLastSibling();
		}
		else
		{
			UE_PublicRoom.GetComponent<RectTransform>().SetAsFirstSibling();
			bool isOn = SteamMatchmaking.GetLobbyData(Hub.s.steamInviteDispatcher.joinedLobbyID, "PublicRoom") == "true";
			UE_PublicRoomToggle.GetComponent<Toggle>().isOn = isOn;
		}
		if (SceneManager.GetActiveScene().name != "MaintenanceScene")
		{
			UE_PublicRoom.GetComponent<RectTransform>().SetAsFirstSibling();
		}
		SetRemoteVolumeController_v2();
		if (tempVolumeList.Count < 4)
		{
			while (tempVolumeList.Count < 4)
			{
				tempVolumeList.Add(0f);
			}
		}
		else if (tempVolumeList.Count > 4)
		{
			tempVolumeList.RemoveRange(4, tempVolumeList.Count - 4);
		}
		SetVersionText();
	}

	public void SetPublicRoomName()
	{
		if (UE_InputFieldRoomName.text == "")
		{
			lobbyName = Hub.GetL10NText("STRING_PUBLIC_TRAM_TITLE_DEFAULT", Hub.s.pdata.MyNickName);
		}
		else
		{
			lobbyName = UE_InputFieldRoomName.text;
		}
		Hub.s.steamInviteDispatcher.SetLobbyName(lobbyName);
		UE_ChangeRoomNameButton.interactable = false;
		UE_ChangeRoomNameButton.GetComponentInChildren<TMP_Text>().text = Hub.GetL10NText("STRING_PUBLIC_TRAM_BUTTON_APPLIED");
	}

	private void SetVersionText()
	{
		string text = "EA";
		string text2 = "0.2.7";
		string text3 = "919187f474";
		UE_versionText.text = text + "/" + text2 + "/" + text3;
		if (Hub.s?.pdata?.main is GamePlayScene)
		{
			UE_versionText.text += $"\n{Hub.s.pdata.randDungeonSeed}";
		}
	}

	private void ContinueGame()
	{
		Hide();
		Hub.s.inputman.ToggleCapturing();
		Hub.s.uiman.isGameMenuOpen = false;
	}

	private void InviteFriends()
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.ActivateGameOverlayInviteDialog(Hub.s.steamInviteDispatcher.joinedLobbyID);
		}
	}

	private void OpenQuidPopup()
	{
		base.uiman.OpenRetrunToMainMenu();
	}

	public void SetPingImage(NetworkGradeSig sig)
	{
		if (sig == null)
		{
			return;
		}
		tempNetworkGradeSig = sig;
		int num = playerInfos.Count;
		if (num > 4)
		{
			num = 4;
		}
		for (int i = 0; i < num; i++)
		{
			if (playerInfos[i] != null && !string.IsNullOrEmpty(playerInfos[i].nickName) && sig.grades.ContainsKey(playerInfos[i].nickName))
			{
				switch (sig.grades[playerInfos[i].nickName])
				{
				case NetworkGrade.Broken:
				case NetworkGrade.Terrible:
					pingImageList[i].sprite = pingSprites[3];
					break;
				case NetworkGrade.Slow:
					pingImageList[i].sprite = pingSprites[2];
					break;
				case NetworkGrade.Medium:
					pingImageList[i].sprite = pingSprites[1];
					break;
				case NetworkGrade.Fine:
					pingImageList[i].sprite = pingSprites[0];
					break;
				}
			}
		}
	}

	public void SetRemoteVolumeControllerCoroutineStart()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(SetRemoteVolumeControllerCoroutine());
		}
	}

	private IEnumerator SetRemoteVolumeControllerCoroutine()
	{
		yield return new WaitUntil(() => isUpdated);
		yield return new WaitForSeconds(0.5f);
		SetRemoteVolumeController_v2();
		isUpdated = false;
	}

	public void SetRemoteVolumeController()
	{
		playerList.ForEach(delegate(GameObject player)
		{
			player.SetActive(value: false);
		});
		List<FishNetDissonancePlayer> players = Hub.s.voiceman.Players;
		if (players == null || players.Count == 0)
		{
			Logger.RLog("No players found in voice chat.");
			return;
		}
		playerInfos.Clear();
		if (players.Count > 1)
		{
			for (int num = 0; num < players.Count; num++)
			{
				VolumeContollerPlayerInfo volumeContollerPlayerInfo = new VolumeContollerPlayerInfo();
				volumeContollerPlayerInfo.playerId = players[num].PlayerId;
				volumeContollerPlayerInfo.playerUID = players[num].PlayerUID;
				if (players[num].PlayerUID == Hub.s.pdata.main.GetMyAvatar().UID)
				{
					volumeContollerPlayerInfo.steamID = Hub.s.pdata.GetUserSteamIDString();
				}
				else
				{
					volumeContollerPlayerInfo.steamID = Hub.s.pdata.actorUIDToSteamID[players[num].PlayerUID].ToString();
				}
				foreach (KeyValuePair<int, ProtoActor> item2 in Hub.s.pdata.main.GetProtoActorMap())
				{
					if (item2.Value.UID == volumeContollerPlayerInfo.playerUID)
					{
						volumeContollerPlayerInfo.nickName = item2.Value.name;
						break;
					}
				}
				playerInfos.Add(volumeContollerPlayerInfo);
			}
		}
		else
		{
			VolumeContollerPlayerInfo volumeContollerPlayerInfo2 = new VolumeContollerPlayerInfo();
			volumeContollerPlayerInfo2.playerId = players[0].PlayerId;
			volumeContollerPlayerInfo2.playerUID = players[0].PlayerUID;
			volumeContollerPlayerInfo2.steamID = players[0].steamID.ToString();
			Hub.s.pdata.main.GetProtoActorMap().TryGetValue((int)volumeContollerPlayerInfo2.playerUID, out ProtoActor _);
			foreach (KeyValuePair<int, ProtoActor> item3 in Hub.s.pdata.main.GetProtoActorMap())
			{
				if (item3.Value.UID == volumeContollerPlayerInfo2.playerUID)
				{
					volumeContollerPlayerInfo2.nickName = item3.Value.netSyncActorData.actorName;
					break;
				}
			}
			playerInfos.Add(volumeContollerPlayerInfo2);
		}
		if (Hub.s.pdata.main.GetMyAvatar() == null)
		{
			return;
		}
		ProtoActor myAvatar = Hub.s.pdata.main.GetMyAvatar();
		foreach (VolumeContollerPlayerInfo playerInfo in playerInfos)
		{
			if (playerInfo.playerUID == myAvatar.UID)
			{
				VolumeContollerPlayerInfo item = playerInfo;
				if (playerInfos.Contains(item))
				{
					playerInfos.Remove(item);
					playerInfos.Insert(0, item);
				}
				break;
			}
		}
		Dictionary<int, ProtoActor> protoActorMap = Hub.s.pdata.main.GetProtoActorMap();
		List<int> list = new List<int>(protoActorMap.Keys);
		for (int num2 = 0; num2 < playerInfos.Count; num2++)
		{
			string nameToUseIfNoSteamID = "";
			for (int num3 = 0; num3 < list.Count; num3++)
			{
				int key = list[num3];
				ProtoActor protoActor = protoActorMap[key];
				if (protoActor.UID == playerInfos[num2].playerUID)
				{
					nameToUseIfNoSteamID = protoActor.name;
				}
			}
			playerNickNameTextList[num2].text = Hub.s.pdata.main.ResolveNickName(playerInfos[num2].steamID, nameToUseIfNoSteamID);
			Sprite sprite = LoadSteamAvatar(playerInfos[num2].steamID);
			if (sprite != null)
			{
				playerAvatarImageList[num2].sprite = sprite;
			}
			else
			{
				Logger.RWarn("Steam 아바타 텍스처를 가져오지 못했습니다.");
			}
			playerList[num2].SetActive(value: true);
			if (num2 == 0)
			{
				playerVolumeSliderList[num2].gameObject.SetActive(value: false);
			}
			else
			{
				playerVolumeSliderList[num2].value = GetPlayerVolume(playerInfos[num2].playerId);
			}
		}
	}

	private float GetPlayerVolume(string playerId)
	{
		return Hub.s.voiceman.GetPlayerVolume(playerId);
	}

	private void SetSliderValueNoNotify(Slider slider, float value)
	{
		slider.SetValueWithoutNotify(value);
	}

	private void PruneTempVoiceVolumeCaches(List<VolumeContollerPlayerInfo> infos)
	{
		if (Hub.s == null || Hub.s.uiman == null || infos == null)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < infos.Count; i++)
		{
			if (!string.IsNullOrEmpty(infos[i].steamID))
			{
				hashSet.Add(infos[i].steamID);
			}
		}
		List<string> list = Hub.s.uiman.tempPlayerVolumeDictionary.Keys.ToList();
		for (int j = 0; j < list.Count; j++)
		{
			if (!hashSet.Contains(list[j]))
			{
				Hub.s.uiman.tempPlayerVolumeDictionary.Remove(list[j]);
			}
		}
		List<string> list2 = Hub.s.uiman.tempPlayerVolumeMuteDictionary.Keys.ToList();
		for (int k = 0; k < list2.Count; k++)
		{
			if (!hashSet.Contains(list2[k]))
			{
				Hub.s.uiman.tempPlayerVolumeMuteDictionary.Remove(list2[k]);
			}
		}
	}

	private void ResetVolumeSliderListeners(int maxCount)
	{
		int num = Math.Min(maxCount, playerVolumeSliderList.Count);
		for (int i = 0; i < num; i++)
		{
			if (playerVolumeSliderList[i] != null)
			{
				playerVolumeSliderList[i].onValueChanged.RemoveAllListeners();
			}
		}
	}

	private void OnSliderValueChanged(int index, VolumeContollerPlayerInfo playerInfo, float value)
	{
		if (value < 0.01f)
		{
			playerSpeakButtonList[index].image.sprite = volume0;
		}
		else if (value >= 0.01f && value <= 0.33f)
		{
			playerSpeakButtonList[index].image.sprite = volume1;
		}
		else if (value > 0.33f && value <= 0.66f)
		{
			playerSpeakButtonList[index].image.sprite = volume2;
		}
		else if (value > 0.66f)
		{
			playerSpeakButtonList[index].image.sprite = volume3;
		}
		if (Hub.s.uiman.tempPlayerVolumeMuteDictionary.ContainsKey(playerInfo.steamID))
		{
			Hub.s.uiman.tempPlayerVolumeMuteDictionary[playerInfo.steamID] = value < 0.01f;
		}
		else
		{
			Hub.s.uiman.tempPlayerVolumeMuteDictionary.Add(playerInfo.steamID, value < 0.01f);
		}
		if (value != 0f)
		{
			if (Hub.s.uiman.tempPlayerVolumeDictionary.ContainsKey(playerInfo.steamID))
			{
				Hub.s.uiman.tempPlayerVolumeDictionary[playerInfo.steamID] = value;
			}
			else
			{
				Hub.s.uiman.tempPlayerVolumeDictionary.Add(playerInfo.steamID, value);
			}
		}
		Hub.s.voiceman.SetPlayerVolume(playerInfo.playerId, value);
	}

	private void OnClickSpeakButton(int index)
	{
		if (playerVolumeSliderList == null || playerInfos == null)
		{
			return;
		}
		if (index < 0 || index >= playerVolumeSliderList.Count || index >= playerInfos.Count)
		{
			Logger.RLog("Speak button index out of range: " + index);
			return;
		}
		float value = playerVolumeSliderList[index].value;
		if (value < 0.01f)
		{
			float value2 = Hub.s.voiceman.GetPlayerVolume(playerInfos[index].playerId);
			if (Hub.s.uiman.tempPlayerVolumeDictionary.TryGetValue(playerInfos[index].steamID, out var value3))
			{
				value2 = value3;
			}
			SetSliderValueNoNotify(playerVolumeSliderList[index], value2);
			OnSliderValueChanged(index, playerInfos[index], value2);
		}
		else
		{
			Hub.s.uiman.tempPlayerVolumeDictionary[playerInfos[index].steamID] = value;
			SetSliderValueNoNotify(playerVolumeSliderList[index], 0f);
			OnSliderValueChanged(index, playerInfos[index], 0f);
		}
	}

	public Texture2D GetSteamAvatar(CSteamID steamID)
	{
		if (avatarCache.ContainsKey(steamID))
		{
			return avatarCache[steamID];
		}
		int mediumFriendAvatar = SteamFriends.GetMediumFriendAvatar(steamID);
		if (mediumFriendAvatar == -1)
		{
			Logger.RWarn("아바타를 아직 불러오지 못함");
			return null;
		}
		uint pnWidth = 0u;
		uint pnHeight = 0u;
		if (!SteamUtils.GetImageSize(mediumFriendAvatar, out pnWidth, out pnHeight) || pnWidth == 0 || pnHeight == 0)
		{
			Logger.RWarn("아바타 이미지 크기를 가져오지 못했습니다.");
			return null;
		}
		byte[] array = new byte[pnWidth * pnHeight * 4];
		if (!SteamUtils.GetImageRGBA(mediumFriendAvatar, array, (int)(pnWidth * pnHeight * 4)))
		{
			Logger.RWarn("아바타 데이터를 가져오지 못했습니다.");
			return null;
		}
		Texture2D texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, mipChain: false);
		texture2D.LoadRawTextureData(array);
		texture2D.Apply();
		Texture2D texture2D2 = new Texture2D(texture2D.width, texture2D.height);
		for (int i = 0; i < texture2D.width; i++)
		{
			for (int j = 0; j < texture2D.height; j++)
			{
				texture2D2.SetPixel(i, j, texture2D.GetPixel(i, texture2D.height - 1 - j));
			}
		}
		texture2D2.Apply();
		avatarCache.Add(steamID, texture2D2);
		return texture2D2;
	}

	public void SetRemoteVolumeController_v2()
	{
		playerList.ForEach(delegate(GameObject player)
		{
			player.SetActive(value: false);
		});
		List<FishNetDissonancePlayer> voiceChatPlayers = GetVoiceChatPlayers();
		if (voiceChatPlayers == null || voiceChatPlayers.Count == 0)
		{
			return;
		}
		playerList.ForEach(delegate(GameObject player)
		{
			player.SetActive(value: false);
		});
		playerInfos = BuildPlayerInfos(voiceChatPlayers);
		ReorderLocalPlayerFirst(playerInfos);
		PruneTempVoiceVolumeCaches(playerInfos);
		InitializePlayerUI(playerInfos);
		ResetVolumeSliderListeners(Math.Min(playerInfos.Count, playerVolumeSliderList.Count));
		HookUpVolumeSliders(playerInfos);
		int num = Math.Min(playerInfos.Count, playerVolumeSliderList.Count);
		for (int num2 = 1; num2 < num; num2++)
		{
			float maxValue = playerVolumeSliderList[num2].maxValue;
			if (Hub.s.uiman.tempPlayerVolumeMuteDictionary.ContainsKey(playerInfos[num2].steamID))
			{
				if (Hub.s.uiman.tempPlayerVolumeMuteDictionary[playerInfos[num2].steamID])
				{
					maxValue = 0f;
				}
				else if (Hub.s.uiman.tempPlayerVolumeDictionary.ContainsKey(playerInfos[num2].steamID))
				{
					maxValue = Hub.s.uiman.tempPlayerVolumeDictionary[playerInfos[num2].steamID];
				}
				else
				{
					maxValue = Hub.s.voiceman.GetPlayerVolume(playerInfos[num2].playerId);
					Hub.s.uiman.tempPlayerVolumeDictionary.Add(playerInfos[num2].steamID, maxValue);
				}
			}
			else
			{
				maxValue = Hub.s.voiceman.GetPlayerVolume(playerInfos[num2].playerId);
				Hub.s.uiman.tempPlayerVolumeMuteDictionary.Add(playerInfos[num2].steamID, value: false);
				Hub.s.uiman.tempPlayerVolumeDictionary.Add(playerInfos[num2].steamID, maxValue);
			}
			OnSliderValueChanged(num2, playerInfos[num2], maxValue);
		}
		SetPingImage(tempNetworkGradeSig);
	}

	private List<FishNetDissonancePlayer> GetVoiceChatPlayers()
	{
		return Hub.s.voiceman.Players;
	}

	private List<VolumeContollerPlayerInfo> BuildPlayerInfos(List<FishNetDissonancePlayer> players)
	{
		List<VolumeContollerPlayerInfo> list = new List<VolumeContollerPlayerInfo>();
		if (players == null || Hub.s == null || Hub.s.pdata.main == null)
		{
			return list;
		}
		if (Hub.s.pdata.main.GetMyAvatar() == null)
		{
			return list;
		}
		foreach (FishNetDissonancePlayer player in players)
		{
			if (!(player == null))
			{
				string text = Hub.s.pdata.main.ResolveSteamID(player.PlayerUID);
				VolumeContollerPlayerInfo item = new VolumeContollerPlayerInfo
				{
					playerId = player.PlayerId,
					playerUID = player.PlayerUID,
					steamID = text,
					nickName = ResolveNickName(player.PlayerUID)
				};
				if (text != "0")
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	private string ResolveSteamID(long playerUID)
	{
		if (Hub.s.pdata.actorUIDToSteamID.TryGetValue(playerUID, out var value))
		{
			return value.ToString();
		}
		Logger.RWarn($"SteamID 누락: UID={playerUID}");
		return "0";
	}

	private string ResolveNickName(long playerUID)
	{
		foreach (KeyValuePair<int, ProtoActor> item in Hub.s.pdata.main.GetProtoActorMap())
		{
			if (item.Value.UID == playerUID)
			{
				return item.Value.nickName;
			}
		}
		return "Unknown";
	}

	private void ReorderLocalPlayerFirst(List<VolumeContollerPlayerInfo> infos)
	{
		ProtoActor me = Hub.s.pdata.main.GetMyAvatar();
		if (!(me == null))
		{
			int num = infos.FindIndex((VolumeContollerPlayerInfo x) => x.playerUID == me.UID);
			if (num > 0)
			{
				VolumeContollerPlayerInfo item = infos[num];
				infos.RemoveAt(num);
				infos.Insert(0, item);
			}
		}
	}

	private void InitializePlayerUI(List<VolumeContollerPlayerInfo> infos)
	{
		int num = Math.Min(infos.Count, playerList.Count);
		for (int i = 0; i < num; i++)
		{
			playerList[i].SetActive(value: true);
			playerNickNameTextList[i].text = Hub.s.pdata.main.ResolveNickName(infos[i].steamID, infos[i].nickName);
			Sprite sprite = LoadSteamAvatar(infos[i].steamID);
			if (sprite != null)
			{
				playerAvatarImageList[i].sprite = sprite;
			}
			SetSliderValueNoNotify(playerVolumeSliderList[i], GetPlayerVolume(infos[i].playerId));
		}
	}

	private void HookUpVolumeSliders(List<VolumeContollerPlayerInfo> infos)
	{
		int num = Math.Min(infos.Count, playerVolumeSliderList.Count);
		for (int i = 1; i < num; i++)
		{
			int index = i;
			if (Hub.s.uiman.tempPlayerVolumeMuteDictionary.ContainsKey(infos[index].steamID))
			{
				if (Hub.s.uiman.tempPlayerVolumeMuteDictionary[infos[index].steamID])
				{
					SetSliderValueNoNotify(playerVolumeSliderList[index], 0f);
				}
				else
				{
					if (Hub.s.uiman.tempPlayerVolumeDictionary.ContainsKey(infos[index].steamID))
					{
						SetSliderValueNoNotify(playerVolumeSliderList[index], Hub.s.uiman.tempPlayerVolumeDictionary[infos[index].steamID]);
					}
					else
					{
						Hub.s.uiman.tempPlayerVolumeDictionary.Add(infos[index].steamID, Hub.s.voiceman.GetPlayerVolume(infos[index].playerId));
					}
					SetSliderValueNoNotify(playerVolumeSliderList[index], Hub.s.uiman.tempPlayerVolumeDictionary[infos[index].steamID]);
				}
			}
			playerVolumeSliderList[index].onValueChanged.AddListener(delegate(float val)
			{
				OnSliderValueChanged(index, infos[index], val);
			});
		}
	}

	private Sprite LoadSteamAvatar(string steamID)
	{
		Texture2D steamAvatar = GetSteamAvatar(new CSteamID(ulong.Parse(steamID)));
		if (steamAvatar != null)
		{
			return Sprite.Create(steamAvatar, new Rect(0f, 0f, steamAvatar.width, steamAvatar.height), Vector2.one * 0.5f);
		}
		Logger.RWarn("Steam 아바타 불러오기 실패");
		return null;
	}
}
