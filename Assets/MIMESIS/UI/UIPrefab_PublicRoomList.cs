using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_PublicRoomList : UIPrefabScript
{
	public delegate void OnMouseOverTargetChanged();

	public const string UEID_Title = "Title";

	public const string UEID_ButtonBack = "ButtonBack";

	public const string UEID_ButtonRefresh = "ButtonRefresh";

	public const string UEID_Content = "Content";

	public const string UEID_popup = "popup";

	public const string UEID_SubTxt = "SubTxt";

	public const string UEID_RecentCreatorCode = "RecentCreatorCode";

	public const string UEID_confirm = "confirm";

	public const string UEID_cancel = "cancel";

	public const string UEID_EmptyListText = "EmptyListText";

	private TMP_Text _UE_Title;

	private Button _UE_ButtonBack;

	private Action<string> _OnButtonBack;

	private Button _UE_ButtonRefresh;

	private Action<string> _OnButtonRefresh;

	private Transform _UE_Content;

	private Image _UE_popup;

	private TMP_Text _UE_SubTxt;

	private TMP_Text _UE_RecentCreatorCode;

	private Button _UE_confirm;

	private Action<string> _Onconfirm;

	private Button _UE_cancel;

	private Action<string> _Oncancel;

	private TMP_Text _UE_EmptyListText;

	private string tempLobbyID = "";

	[SerializeField]
	private UiPrefab_RoomCard roomCardPrefab;

	private List<UiPrefab_RoomCard> roomCards = new List<UiPrefab_RoomCard>();

	private List<PublicRoomListData> roomListData = new List<PublicRoomListData>();

	public TMP_Text UE_Title => _UE_Title ?? (_UE_Title = PickText("Title"));

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

	public Button UE_ButtonRefresh => _UE_ButtonRefresh ?? (_UE_ButtonRefresh = PickButton("ButtonRefresh"));

	public Action<string> OnButtonRefresh
	{
		get
		{
			return _OnButtonRefresh;
		}
		set
		{
			_OnButtonRefresh = value;
			SetOnButtonClick("ButtonRefresh", value);
		}
	}

	public Transform UE_Content => _UE_Content ?? (_UE_Content = PickTransform("Content"));

	public Image UE_popup => _UE_popup ?? (_UE_popup = PickImage("popup"));

	public TMP_Text UE_SubTxt => _UE_SubTxt ?? (_UE_SubTxt = PickText("SubTxt"));

	public TMP_Text UE_RecentCreatorCode => _UE_RecentCreatorCode ?? (_UE_RecentCreatorCode = PickText("RecentCreatorCode"));

	public Button UE_confirm => _UE_confirm ?? (_UE_confirm = PickButton("confirm"));

	public Action<string> Onconfirm
	{
		get
		{
			return _Onconfirm;
		}
		set
		{
			_Onconfirm = value;
			SetOnButtonClick("confirm", value);
		}
	}

	public Button UE_cancel => _UE_cancel ?? (_UE_cancel = PickButton("cancel"));

	public Action<string> Oncancel
	{
		get
		{
			return _Oncancel;
		}
		set
		{
			_Oncancel = value;
			SetOnButtonClick("cancel", value);
		}
	}

	public TMP_Text UE_EmptyListText => _UE_EmptyListText ?? (_UE_EmptyListText = PickText("EmptyListText"));

	public event OnMouseOverTargetChanged onMouseOverTargetChanged;

	private void OnEnable()
	{
		UE_popup.gameObject.SetActive(value: false);
		GetComponentsInChildren<TMP_Text>().ToList().ForEach(delegate(TMP_Text x)
		{
			x.color = Color.white;
		});
		if (UE_ButtonRefresh.interactable)
		{
			RefreshBtn();
		}
		StartCoroutine(ButtonRefreshCoroutine());
	}

	private void Start()
	{
		ShowPopup("", "");
		UE_popup.gameObject.SetActive(value: false);
		AddMouseOverEnterEvent(UE_ButtonBack.gameObject, UE_ButtonBack.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_ButtonRefresh.gameObject, UE_ButtonRefresh.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ButtonBack.gameObject, UE_ButtonBack.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ButtonRefresh.gameObject, UE_ButtonRefresh.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_confirm.gameObject, UE_confirm.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_cancel.gameObject, UE_cancel.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_confirm.gameObject, UE_confirm.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_cancel.gameObject, UE_cancel.GetComponentInChildren<TMP_Text>());
		UE_ButtonBack.onClick.AddListener(delegate
		{
			Hide();
			EventSystem.current.SetSelectedGameObject(null);
		});
		UE_ButtonRefresh.onClick.AddListener(delegate
		{
			RefreshBtn();
		});
		UE_confirm.onClick.AddListener(delegate
		{
			EventSystem.current.SetSelectedGameObject(null);
			Logger.RLog("Join Lobby: " + tempLobbyID);
			if (Hub.s.uiman.inviteLoadingUI == null)
			{
				Hub.s.uiman.inviteLoadingUI = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_dialogueBox>(Hub.s.uiman.prefab_Invite_Loading, eUIHeight.OverTheTop);
			}
			Hub.s.steamInviteDispatcher.RequestPublicJoinLobby(tempLobbyID, delegate
			{
				if (SteamMatchmaking.GetLobbyData(new CSteamID(ulong.Parse(tempLobbyID)), "PublicRoom") == "true")
				{
					Hub.s.steamInviteDispatcher.JoinFriendWithMatchKeyProcess(tempLobbyID);
				}
				else
				{
					Hub.s.uiman.ShowEnterPublicRoomFailed("STRING_CANT_ENTER_PUBLICROOM_DESCRIPTION", new string[1] { "0" });
				}
				UE_popup.gameObject.SetActive(value: false);
				Hide();
			});
		});
		UE_cancel.onClick.AddListener(delegate
		{
			EventSystem.current.SetSelectedGameObject(null);
			UE_popup.gameObject.SetActive(value: false);
		});
	}

	public void RefreshBtn()
	{
		roomCards.ForEach(delegate(UiPrefab_RoomCard x)
		{
			x.Hide();
		});
		Hub.s.steamInviteDispatcher.RequestLobbyList(50, this);
		EventSystem.current.SetSelectedGameObject(null);
		StartCoroutine(ButtonRefreshCoroutine());
	}

	private IEnumerator ButtonRefreshCoroutine()
	{
		UE_ButtonRefresh.interactable = false;
		UE_ButtonRefresh.GetComponentInChildren<TMP_Text>().color = Color.gray;
		while (Hub.s.steamInviteDispatcher.refreshTime > 0)
		{
			yield return new WaitForSeconds(0.1f);
			UE_ButtonRefresh.GetComponentInChildren<TMP_Text>().text = Hub.GetL10NText("STRING_PUBLICROOM_REFRESH_BUTTON") + " (" + Hub.s.steamInviteDispatcher.refreshTime + ")";
		}
		UE_ButtonRefresh.GetComponentInChildren<TMP_Text>().text = Hub.GetL10NText("STRING_PUBLICROOM_REFRESH_BUTTON");
		UE_ButtonRefresh.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_ButtonRefresh.interactable = true;
	}

	public void SetRoomList(List<CSteamID> lobbyIDs)
	{
		roomListData.Clear();
		string locale = Hub.s.steamInviteDispatcher.GetCurrentLocale();
		foreach (CSteamID lobbyID in lobbyIDs)
		{
			PublicRoomListData publicRoomListData = new PublicRoomListData();
			publicRoomListData.PlayerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
			if (publicRoomListData.PlayerCount < 4)
			{
				publicRoomListData.lobbyID = lobbyID;
				publicRoomListData.locale = SteamMatchmaking.GetLobbyData(lobbyID, "Locale");
				publicRoomListData.lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");
				int.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "Cycle"), out publicRoomListData.cycle);
				int result = 0;
				int.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "RepairStatus"), out result);
				if (publicRoomListData.cycle == 1)
				{
					publicRoomListData.repairStatus = 0;
				}
				else
				{
					publicRoomListData.repairStatus = result + 1;
				}
				roomListData.Add(publicRoomListData);
			}
		}
		Dictionary<string, int> localeCounts = (from x in roomListData
			group x by x.locale).ToDictionary((IGrouping<string, PublicRoomListData> g) => g.Key, (IGrouping<string, PublicRoomListData> g) => g.Count());
		roomListData = (from x in roomListData
			orderby x.locale == locale descending, localeCounts[x.locale], x.cycle, x.repairStatus, x.PlayerCount descending
			select x).ToList();
		if (roomListData.Count == 0)
		{
			UE_EmptyListText.gameObject.SetActive(value: true);
			return;
		}
		UE_EmptyListText.gameObject.SetActive(value: false);
		SetRoomListUI();
	}

	public void SetEmptyListText()
	{
		UE_EmptyListText.gameObject.SetActive(value: true);
	}

	private void AddMouseOverEnterEvent(GameObject go, List<Image> targetImage, List<TMP_Text> textTarget, MouseOverDelegate @delegate = null)
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
			this.onMouseOverTargetChanged?.Invoke();
			targetImage?.ForEach(delegate(Image List)
			{
				List.color = new Color(1f, 1f, 1f, 1f);
			});
			textTarget?.ForEach(delegate(TMP_Text List)
			{
				List.color = Hub.s.uiman.mouseOverTextColor;
			});
			if (@delegate != null)
			{
				@delegate();
			}
		});
		eventTrigger.triggers.Add(entry);
		AddDragPassthrough(go);
	}

	private void AddMouseOverExitEvent(GameObject go, List<Image> targetImage, List<TMP_Text> textTarget, MouseOverDelegate @delegate = null)
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
			targetImage?.ForEach(delegate(Image List)
			{
				List.color = new Color(1f, 1f, 1f, 0f);
			});
			textTarget?.ForEach(delegate(TMP_Text List)
			{
				List.color = Color.white;
			});
			if (@delegate != null)
			{
				@delegate();
			}
		});
		onMouseOverTargetChanged += delegate
		{
			if (go.GetComponent<Image>().raycastTarget)
			{
				targetImage?.ForEach(delegate(Image List)
				{
					List.color = new Color(1f, 1f, 1f, 0f);
				});
				textTarget?.ForEach(delegate(TMP_Text List)
				{
					List.color = Color.white;
				});
				if (@delegate != null)
				{
					@delegate();
				}
			}
		};
		eventTrigger.triggers.Add(entry);
		AddDragPassthrough(go);
	}

	private void AddDragPassthrough(GameObject go)
	{
		EventTrigger component = go.GetComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.BeginDrag;
		entry.callback.AddListener(delegate(BaseEventData data)
		{
			PassEventToScrollView(data as PointerEventData, ExecuteEvents.beginDragHandler);
		});
		component.triggers.Add(entry);
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.Drag;
		entry2.callback.AddListener(delegate(BaseEventData data)
		{
			PassEventToScrollView(data as PointerEventData, ExecuteEvents.dragHandler);
		});
		component.triggers.Add(entry2);
		EventTrigger.Entry entry3 = new EventTrigger.Entry();
		entry3.eventID = EventTriggerType.EndDrag;
		entry3.callback.AddListener(delegate(BaseEventData data)
		{
			PassEventToScrollView(data as PointerEventData, ExecuteEvents.endDragHandler);
		});
		component.triggers.Add(entry3);
		EventTrigger.Entry entry4 = new EventTrigger.Entry();
		entry4.eventID = EventTriggerType.Scroll;
		entry4.callback.AddListener(delegate(BaseEventData data)
		{
			PassEventToScrollView(data as PointerEventData, ExecuteEvents.scrollHandler);
		});
		component.triggers.Add(entry4);
	}

	private void PassEventToScrollView<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
	{
		ScrollRect componentInParent = GetComponentInParent<ScrollRect>();
		if (componentInParent != null)
		{
			ExecuteEvents.Execute(componentInParent.gameObject, data, function);
		}
	}

	public void SetRoomListUI()
	{
		roomCards.RemoveAll((UiPrefab_RoomCard x) => x == null);
		roomCards.ForEach(delegate(UiPrefab_RoomCard x)
		{
			x.Hide();
		});
		for (int num = 0; num < roomListData.Count; num++)
		{
			if (roomCards.Count > num)
			{
				roomCards[num].Show();
				roomCards[num].SetRoomData(roomListData[num], this);
				continue;
			}
			UiPrefab_RoomCard uiPrefab_RoomCard = UnityEngine.Object.Instantiate(roomCardPrefab, UE_Content);
			uiPrefab_RoomCard.Show();
			uiPrefab_RoomCard.SetRoomData(roomListData[num], this);
			roomCards.Add(uiPrefab_RoomCard);
		}
		UE_Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, (float)roomCards.Count * roomCards[0].GetComponent<RectTransform>().sizeDelta.y * 1.1f);
	}

	public void ShowPopup(string _lobbyID, string _lobbyName)
	{
		UE_popup.gameObject.SetActive(value: true);
		Hub.s.uiman.ui_escapeStack.Add(UE_popup.GetComponent<UIPrefabScript>());
		UE_cancel.GetComponentInChildren<TMP_Text>().color = Color.white;
		UE_confirm.GetComponentInChildren<TMP_Text>().color = Color.white;
		tempLobbyID = _lobbyID;
		UE_SubTxt.text = Hub.GetL10NText("STRING_ENTER_PUBLICROOM_DESCRIPTION", _lobbyName);
	}
}
