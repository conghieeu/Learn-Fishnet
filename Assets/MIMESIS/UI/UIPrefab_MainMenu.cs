using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_MainMenu : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_HostAddress = "HostAddress";

	public const string UEID_ConnectButton = "ConnectButton";

	public const string UEID_RefreshButton = "RefreshButton";

	public const string UEID_Content = "Content";

	public const string UEID_Spin = "Spin";

	public const string UEID_versionText = "versionText";

	public const string UEID_HostButton = "HostButton";

	public const string UEID_LoadButton = "LoadButton";

	public const string UEID_JoinButton = "JoinButton";

	public const string UEID_JoinButtonCode = "JoinButtonCode";

	public const string UEID_JoinButtonPublic = "JoinButtonPublic";

	public const string UEID_SteamInventory = "SteamInventory";

	public const string UEID_SettingButton = "SettingButton";

	public const string UEID_QuitButton = "QuitButton";

	public const string UEID_steamButton = "steamButton";

	public const string UEID_youtubeButton = "youtubeButton";

	public const string UEID_discordButton = "discordButton";

	public const string UEID_reluButton = "reluButton";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_InputField _UE_HostAddress;

	private Button _UE_ConnectButton;

	private Action<string> _OnConnectButton;

	private Button _UE_RefreshButton;

	private Action<string> _OnRefreshButton;

	private Transform _UE_Content;

	private Image _UE_Spin;

	private TMP_Text _UE_versionText;

	private Button _UE_HostButton;

	private Action<string> _OnHostButton;

	private Button _UE_LoadButton;

	private Action<string> _OnLoadButton;

	private Button _UE_JoinButton;

	private Action<string> _OnJoinButton;

	private Button _UE_JoinButtonCode;

	private Action<string> _OnJoinButtonCode;

	private Button _UE_JoinButtonPublic;

	private Action<string> _OnJoinButtonPublic;

	private Button _UE_SteamInventory;

	private Action<string> _OnSteamInventory;

	private Button _UE_SettingButton;

	private Action<string> _OnSettingButton;

	private Button _UE_QuitButton;

	private Action<string> _OnQuitButton;

	private Button _UE_steamButton;

	private Action<string> _OnsteamButton;

	private Button _UE_youtubeButton;

	private Action<string> _OnyoutubeButton;

	private Button _UE_discordButton;

	private Action<string> _OndiscordButton;

	private Button _UE_reluButton;

	private Action<string> _OnreluButton;

	private Coroutine spinImageCoroutine;

	[SerializeField]
	private Sprite[] SpinImages;

	[SerializeField]
	private GameObject RoomListItem;

	[SerializeField]
	private GameObject joinWithCodeUIPrefab;

	private UIPrefab_JoinTramWithCode joinWithCodeUI;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_InputField UE_HostAddress => _UE_HostAddress ?? (_UE_HostAddress = PickInputField("HostAddress"));

	public Button UE_ConnectButton => _UE_ConnectButton ?? (_UE_ConnectButton = PickButton("ConnectButton"));

	public Action<string> OnConnectButton
	{
		get
		{
			return _OnConnectButton;
		}
		set
		{
			_OnConnectButton = value;
			SetOnButtonClick("ConnectButton", value);
		}
	}

	public Button UE_RefreshButton => _UE_RefreshButton ?? (_UE_RefreshButton = PickButton("RefreshButton"));

	public Action<string> OnRefreshButton
	{
		get
		{
			return _OnRefreshButton;
		}
		set
		{
			_OnRefreshButton = value;
			SetOnButtonClick("RefreshButton", value);
		}
	}

	public Transform UE_Content => _UE_Content ?? (_UE_Content = PickTransform("Content"));

	public Image UE_Spin => _UE_Spin ?? (_UE_Spin = PickImage("Spin"));

	public TMP_Text UE_versionText => _UE_versionText ?? (_UE_versionText = PickText("versionText"));

	public Button UE_HostButton => _UE_HostButton ?? (_UE_HostButton = PickButton("HostButton"));

	public Action<string> OnHostButton
	{
		get
		{
			return _OnHostButton;
		}
		set
		{
			_OnHostButton = value;
			SetOnButtonClick("HostButton", value);
		}
	}

	public Button UE_LoadButton => _UE_LoadButton ?? (_UE_LoadButton = PickButton("LoadButton"));

	public Action<string> OnLoadButton
	{
		get
		{
			return _OnLoadButton;
		}
		set
		{
			_OnLoadButton = value;
			SetOnButtonClick("LoadButton", value);
		}
	}

	public Button UE_JoinButton => _UE_JoinButton ?? (_UE_JoinButton = PickButton("JoinButton"));

	public Action<string> OnJoinButton
	{
		get
		{
			return _OnJoinButton;
		}
		set
		{
			_OnJoinButton = value;
			SetOnButtonClick("JoinButton", value);
		}
	}

	public Button UE_JoinButtonCode => _UE_JoinButtonCode ?? (_UE_JoinButtonCode = PickButton("JoinButtonCode"));

	public Action<string> OnJoinButtonCode
	{
		get
		{
			return _OnJoinButtonCode;
		}
		set
		{
			_OnJoinButtonCode = value;
			SetOnButtonClick("JoinButtonCode", value);
		}
	}

	public Button UE_JoinButtonPublic => _UE_JoinButtonPublic ?? (_UE_JoinButtonPublic = PickButton("JoinButtonPublic"));

	public Action<string> OnJoinButtonPublic
	{
		get
		{
			return _OnJoinButtonPublic;
		}
		set
		{
			_OnJoinButtonPublic = value;
			SetOnButtonClick("JoinButtonPublic", value);
		}
	}

	public Button UE_SteamInventory => _UE_SteamInventory ?? (_UE_SteamInventory = PickButton("SteamInventory"));

	public Action<string> OnSteamInventory
	{
		get
		{
			return _OnSteamInventory;
		}
		set
		{
			_OnSteamInventory = value;
			SetOnButtonClick("SteamInventory", value);
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

	public Button UE_steamButton => _UE_steamButton ?? (_UE_steamButton = PickButton("steamButton"));

	public Action<string> OnsteamButton
	{
		get
		{
			return _OnsteamButton;
		}
		set
		{
			_OnsteamButton = value;
			SetOnButtonClick("steamButton", value);
		}
	}

	public Button UE_youtubeButton => _UE_youtubeButton ?? (_UE_youtubeButton = PickButton("youtubeButton"));

	public Action<string> OnyoutubeButton
	{
		get
		{
			return _OnyoutubeButton;
		}
		set
		{
			_OnyoutubeButton = value;
			SetOnButtonClick("youtubeButton", value);
		}
	}

	public Button UE_discordButton => _UE_discordButton ?? (_UE_discordButton = PickButton("discordButton"));

	public Action<string> OndiscordButton
	{
		get
		{
			return _OndiscordButton;
		}
		set
		{
			_OndiscordButton = value;
			SetOnButtonClick("discordButton", value);
		}
	}

	public Button UE_reluButton => _UE_reluButton ?? (_UE_reluButton = PickButton("reluButton"));

	public Action<string> OnreluButton
	{
		get
		{
			return _OnreluButton;
		}
		set
		{
			_OnreluButton = value;
			SetOnButtonClick("reluButton", value);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(SelectButtonNull());
	}

	private void Start()
	{
		UE_rootNode.GetComponent<RectTransform>().anchoredPosition += new Vector2(100000f, 0f);
		UE_Spin.gameObject.SetActive(value: false);
		UE_JoinButton.onClick.AddListener(delegate
		{
			Hub.s.steamInviteDispatcher.OpenSteamFriends();
			StartCoroutine(SelectButtonNull());
		});
		UE_JoinButtonCode.onClick.AddListener(delegate
		{
			if (joinWithCodeUI == null)
			{
				joinWithCodeUI = Hub.s.uiman.InstatiateUI<UIPrefab_JoinTramWithCode>(joinWithCodeUIPrefab);
			}
			Hub.s.uiman.ui_escapeStack.Add(joinWithCodeUI);
			joinWithCodeUI.Show();
			StartCoroutine(SelectButtonNull());
		});
		UE_JoinButtonPublic.onClick.AddListener(delegate
		{
			Hub.s.uiman.OpenPublicRoomList();
			StartCoroutine(SelectButtonNull());
		});
		UE_SteamInventory.onClick.AddListener(delegate
		{
			Hub.s.uiman.OpenSteamInventory();
			StartCoroutine(SelectButtonNull());
		});
		UE_SettingButton.onClick.AddListener(delegate
		{
			UE_rootNode.gameObject.SetActive(value: false);
			UE_SettingButton.GetComponentInChildren<TMP_Text>().color = Color.white;
			Hub.s.uiman.OpenGameSettings(this);
		});
		UE_QuitButton.onClick.AddListener(delegate
		{
			Application.Quit();
		});
		AddMouseOverEnterEvent(UE_HostButton.gameObject, UE_HostButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_LoadButton.gameObject, UE_LoadButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_JoinButton.gameObject, UE_JoinButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_JoinButtonCode.gameObject, UE_JoinButtonCode.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_JoinButtonPublic.gameObject, UE_JoinButtonPublic.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_SteamInventory.gameObject, UE_SteamInventory.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_SettingButton.gameObject, UE_SettingButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_QuitButton.gameObject, UE_QuitButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_HostButton.gameObject, UE_HostButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_LoadButton.gameObject, UE_LoadButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_JoinButton.gameObject, UE_JoinButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_JoinButtonCode.gameObject, UE_JoinButtonCode.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_JoinButtonPublic.gameObject, UE_JoinButtonPublic.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_SteamInventory.gameObject, UE_SteamInventory.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_SettingButton.gameObject, UE_SettingButton.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_QuitButton.gameObject, UE_QuitButton.GetComponentInChildren<TMP_Text>());
		SetVersionText();
		UE_steamButton.onClick.AddListener(delegate
		{
			Application.OpenURL("steam://store/2827200");
			StartCoroutine(SelectButtonNull());
		});
		UE_youtubeButton.onClick.AddListener(delegate
		{
			Application.OpenURL("https://www.youtube.com/@ReLUGames");
			StartCoroutine(SelectButtonNull());
		});
		UE_discordButton.onClick.AddListener(delegate
		{
			Application.OpenURL("https://discord.gg/6WJvq36rab");
			StartCoroutine(SelectButtonNull());
		});
		UE_reluButton.onClick.AddListener(delegate
		{
			Application.OpenURL("steam://publisher/relugames");
			StartCoroutine(SelectButtonNull());
		});
	}

	private void SetVersionText()
	{
		string text = "EA";
		string text2 = "0.2.7";
		string text3 = "919187f474";
		UE_versionText.text = text + "/" + text2 + "/" + text3;
	}

	private IEnumerator StartSpinImage()
	{
		UE_Spin.gameObject.SetActive(value: true);
		int count = SpinImages.Count();
		int index = -1;
		while (true)
		{
			index = (index + 1) % count;
			UE_Spin.sprite = SpinImages[index];
			yield return new WaitForSeconds(0.005f);
		}
	}

	public void StartConnecting()
	{
		spinImageCoroutine = StartCoroutine(StartSpinImage());
	}

	public void EndConnecting()
	{
		if (Hub.s.uiman.inviteLoadingUI != null)
		{
			Hub.s.uiman.inviteLoadingUI.Hide();
		}
		if (spinImageCoroutine != null)
		{
			StopCoroutine(spinImageCoroutine);
		}
		UE_Spin.gameObject.SetActive(value: false);
	}

	public void UpdateServerList(ServerListInformer sli, Action<int> connectCallback)
	{
		for (int i = 0; i < UE_Content.childCount; i++)
		{
			UIPrefab_RoomListItem component = UE_Content.GetChild(i).GetComponent<UIPrefab_RoomListItem>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component.gameObject);
			}
		}
		UE_Content.DetachChildren();
	}

	private IEnumerator SelectButtonNull()
	{
		yield return null;
		EventSystem.current.SetSelectedGameObject(null);
	}
}
