using System;
using System.Linq;
using ReluProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_LoadTram : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_Title0 = "Title0";

	public const string UEID_SavedFile0 = "SavedFile0";

	public const string UEID_Text0 = "Text0";

	public const string UEID_Title1 = "Title1";

	public const string UEID_SavedFile1 = "SavedFile1";

	public const string UEID_Text1 = "Text1";

	public const string UEID_Title2 = "Title2";

	public const string UEID_SavedFile2 = "SavedFile2";

	public const string UEID_Text2 = "Text2";

	public const string UEID_Title3 = "Title3";

	public const string UEID_SavedFile3 = "SavedFile3";

	public const string UEID_Text3 = "Text3";

	public const string UEID_ButtonClose = "ButtonClose";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_Text _UE_Title0;

	private Button _UE_SavedFile0;

	private Action<string> _OnSavedFile0;

	private TMP_Text _UE_Text0;

	private TMP_Text _UE_Title1;

	private Button _UE_SavedFile1;

	private Action<string> _OnSavedFile1;

	private TMP_Text _UE_Text1;

	private TMP_Text _UE_Title2;

	private Button _UE_SavedFile2;

	private Action<string> _OnSavedFile2;

	private TMP_Text _UE_Text2;

	private TMP_Text _UE_Title3;

	private Button _UE_SavedFile3;

	private Action<string> _OnSavedFile3;

	private TMP_Text _UE_Text3;

	private Button _UE_ButtonClose;

	private Action<string> _OnButtonClose;

	private MMSaveGameData[] _loadedSaveData = new MMSaveGameData[4];

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_Title0 => _UE_Title0 ?? (_UE_Title0 = PickText("Title0"));

	public Button UE_SavedFile0 => _UE_SavedFile0 ?? (_UE_SavedFile0 = PickButton("SavedFile0"));

	public Action<string> OnSavedFile0
	{
		get
		{
			return _OnSavedFile0;
		}
		set
		{
			_OnSavedFile0 = value;
			SetOnButtonClick("SavedFile0", value);
		}
	}

	public TMP_Text UE_Text0 => _UE_Text0 ?? (_UE_Text0 = PickText("Text0"));

	public TMP_Text UE_Title1 => _UE_Title1 ?? (_UE_Title1 = PickText("Title1"));

	public Button UE_SavedFile1 => _UE_SavedFile1 ?? (_UE_SavedFile1 = PickButton("SavedFile1"));

	public Action<string> OnSavedFile1
	{
		get
		{
			return _OnSavedFile1;
		}
		set
		{
			_OnSavedFile1 = value;
			SetOnButtonClick("SavedFile1", value);
		}
	}

	public TMP_Text UE_Text1 => _UE_Text1 ?? (_UE_Text1 = PickText("Text1"));

	public TMP_Text UE_Title2 => _UE_Title2 ?? (_UE_Title2 = PickText("Title2"));

	public Button UE_SavedFile2 => _UE_SavedFile2 ?? (_UE_SavedFile2 = PickButton("SavedFile2"));

	public Action<string> OnSavedFile2
	{
		get
		{
			return _OnSavedFile2;
		}
		set
		{
			_OnSavedFile2 = value;
			SetOnButtonClick("SavedFile2", value);
		}
	}

	public TMP_Text UE_Text2 => _UE_Text2 ?? (_UE_Text2 = PickText("Text2"));

	public TMP_Text UE_Title3 => _UE_Title3 ?? (_UE_Title3 = PickText("Title3"));

	public Button UE_SavedFile3 => _UE_SavedFile3 ?? (_UE_SavedFile3 = PickButton("SavedFile3"));

	public Action<string> OnSavedFile3
	{
		get
		{
			return _OnSavedFile3;
		}
		set
		{
			_OnSavedFile3 = value;
			SetOnButtonClick("SavedFile3", value);
		}
	}

	public TMP_Text UE_Text3 => _UE_Text3 ?? (_UE_Text3 = PickText("Text3"));

	public Button UE_ButtonClose => _UE_ButtonClose ?? (_UE_ButtonClose = PickButton("ButtonClose"));

	public Action<string> OnButtonClose
	{
		get
		{
			return _OnButtonClose;
		}
		set
		{
			_OnButtonClose = value;
			SetOnButtonClick("ButtonClose", value);
		}
	}

	private void OnEnable()
	{
		GetComponentsInChildren<TMP_Text>().ToList().ForEach(delegate(TMP_Text x)
		{
			x.color = Color.white;
		});
	}

	private void Start()
	{
		AddMouseOverEnterEvent(UE_SavedFile0.gameObject, UE_Text0);
		AddMouseOverEnterEvent(UE_SavedFile1.gameObject, UE_Text1);
		AddMouseOverEnterEvent(UE_SavedFile2.gameObject, UE_Text2);
		AddMouseOverEnterEvent(UE_SavedFile3.gameObject, UE_Text3);
		AddMouseOverEnterEvent(UE_ButtonClose.gameObject, UE_ButtonClose.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_SavedFile0.gameObject, UE_Text0);
		AddMouseOverExitEvent(UE_SavedFile1.gameObject, UE_Text1);
		AddMouseOverExitEvent(UE_SavedFile2.gameObject, UE_Text2);
		AddMouseOverExitEvent(UE_SavedFile3.gameObject, UE_Text3);
		AddMouseOverExitEvent(UE_ButtonClose.gameObject, UE_ButtonClose.GetComponentInChildren<TMP_Text>());
	}

	public void InitSaveInfoList()
	{
		string l10NText = Hub.GetL10NText("UI_PREFAB_MAIN_MENU_NEW_TRAM");
		TMP_Text[] array = new TMP_Text[4] { UE_Text0, UE_Text1, UE_Text2, UE_Text3 };
		Button[] buttonElements = new Button[4] { UE_SavedFile0, UE_SavedFile1, UE_SavedFile2, UE_SavedFile3 };
		for (int i = 0; i <= 3; i++)
		{
			if (!MonoSingleton<PlatformMgr>.Instance.IsSaveFileExist(MMSaveGameData.GetSaveFileName(i)))
			{
				if (i == 0)
				{
					UE_SavedFile0.transform.parent.parent.gameObject.SetActive(value: false);
				}
				_loadedSaveData[i] = null;
				SetEmptySlot(i, array, buttonElements, l10NText);
				continue;
			}
			MMSaveGameData mMSaveGameData = MonoSingleton<PlatformMgr>.Instance.Load<MMSaveGameData>(MMSaveGameData.GetSaveFileName(i));
			if (mMSaveGameData == null)
			{
				_loadedSaveData[i] = null;
				SetEmptySlot(i, array, buttonElements, l10NText);
				continue;
			}
			_loadedSaveData[i] = mMSaveGameData;
			_ = mMSaveGameData.RegDateTime;
			string text = mMSaveGameData.RegDateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
			string l10NText2 = Hub.GetL10NText("STRING_LOAD_SLOT_CYCLE", mMSaveGameData.CycleCount);
			string text2 = "";
			text2 = ((mMSaveGameData.DayCount != 1) ? (mMSaveGameData.TramRepaired ? Hub.GetL10NText("STRING_LOAD_SLOT_REPAIR_AFTER") : Hub.GetL10NText("STRING_LOAD_SLOT_REPAIR_BEFORE")) : Hub.GetL10NText("STRING_LOAD_SLOT_START"));
			string text3 = string.Join(", ", mMSaveGameData.PlayerNames.ToArray());
			string text4 = mMSaveGameData.Currency.ToString();
			string text5 = $"[{text}], {l10NText2}, {text2}, ${text4}, {text3}";
			if (i == 0)
			{
				UE_Title0.text = Hub.GetL10NText("STRING_LOAD_SLOT_AUTO_SAVED").Replace("{0}", "#" + mMSaveGameData.SlotID);
				UE_Text0.text = text5;
				UE_Text0.color = Color.white;
			}
			else
			{
				array[i].text = text5;
				array[i].color = Color.white;
			}
		}
	}

	private void SetEmptySlot(int slotIndex, TMP_Text[] textElements, Button[] buttonElements, string emptyText)
	{
		textElements[slotIndex].text = emptyText;
		buttonElements[slotIndex].interactable = true;
	}

	public MMSaveGameData GetLoadedSaveData(int slotID)
	{
		if (slotID < 0 || slotID >= _loadedSaveData.Length)
		{
			Logger.RError($"[UIPrefab_LoadTram] Invalid slotID: {slotID}");
			return null;
		}
		return _loadedSaveData[slotID];
	}
}
