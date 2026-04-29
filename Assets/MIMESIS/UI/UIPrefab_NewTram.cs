using System;
using System.Linq;
using ReluProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_NewTram : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_SavedFile1 = "SavedFile1";

	public const string UEID_Text1 = "Text1";

	public const string UEID_SavedFile2 = "SavedFile2";

	public const string UEID_Text2 = "Text2";

	public const string UEID_SavedFile3 = "SavedFile3";

	public const string UEID_Text3 = "Text3";

	public const string UEID_ButtonClose = "ButtonClose";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private Button _UE_SavedFile1;

	private Action<string> _OnSavedFile1;

	private TMP_Text _UE_Text1;

	private Button _UE_SavedFile2;

	private Action<string> _OnSavedFile2;

	private TMP_Text _UE_Text2;

	private Button _UE_SavedFile3;

	private Action<string> _OnSavedFile3;

	private TMP_Text _UE_Text3;

	private Button _UE_ButtonClose;

	private Action<string> _OnButtonClose;

	private bool _IsSlot1Empty = true;

	private bool _IsSlot2Empty = true;

	private bool _IsSlot3Empty = true;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

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
		AddMouseOverEnterEvent(UE_SavedFile1.gameObject, UE_Text1);
		AddMouseOverEnterEvent(UE_SavedFile2.gameObject, UE_Text2);
		AddMouseOverEnterEvent(UE_SavedFile3.gameObject, UE_Text3);
		AddMouseOverEnterEvent(UE_ButtonClose.gameObject, UE_ButtonClose.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_SavedFile1.gameObject, UE_Text1);
		AddMouseOverExitEvent(UE_SavedFile2.gameObject, UE_Text2);
		AddMouseOverExitEvent(UE_SavedFile3.gameObject, UE_Text3);
		AddMouseOverExitEvent(UE_ButtonClose.gameObject, UE_ButtonClose.GetComponentInChildren<TMP_Text>());
	}

	public void InitSaveInfoList()
	{
		string l10NText = Hub.GetL10NText("UI_PREFAB_MAIN_MENU_NEW_TRAM");
		for (int i = 1; i <= 3; i++)
		{
			if (!MonoSingleton<PlatformMgr>.Instance.IsSaveFileExist(MMSaveGameData.GetSaveFileName(i)))
			{
				switch (i)
				{
				case 1:
					_IsSlot1Empty = true;
					UE_Text1.text = l10NText;
					break;
				case 2:
					_IsSlot2Empty = true;
					UE_Text2.text = l10NText;
					break;
				case 3:
					_IsSlot3Empty = true;
					UE_Text3.text = l10NText;
					break;
				}
				continue;
			}
			MMSaveGameData mMSaveGameData = MonoSingleton<PlatformMgr>.Instance.Load<MMSaveGameData>(MMSaveGameData.GetSaveFileName(i));
			if (mMSaveGameData == null)
			{
				switch (i)
				{
				case 1:
					_IsSlot1Empty = true;
					UE_Text1.text = l10NText;
					break;
				case 2:
					_IsSlot2Empty = true;
					UE_Text2.text = l10NText;
					break;
				case 3:
					_IsSlot3Empty = true;
					UE_Text3.text = l10NText;
					break;
				}
				continue;
			}
			_ = mMSaveGameData.RegDateTime;
			string text = mMSaveGameData.RegDateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
			string l10NText2 = Hub.GetL10NText("STRING_LOAD_SLOT_CYCLE", mMSaveGameData.CycleCount);
			string text2 = "";
			text2 = ((mMSaveGameData.DayCount != 1) ? (mMSaveGameData.TramRepaired ? Hub.GetL10NText("STRING_LOAD_SLOT_REPAIR_AFTER") : Hub.GetL10NText("STRING_LOAD_SLOT_REPAIR_BEFORE")) : Hub.GetL10NText("STRING_LOAD_SLOT_START"));
			string text3 = string.Join(", ", mMSaveGameData.PlayerNames.ToArray());
			string text4 = mMSaveGameData.Currency.ToString();
			string text5 = $"[{text}], {l10NText2}, {text2}, ${text4}, {text3}";
			switch (i)
			{
			case 1:
				_IsSlot1Empty = false;
				UE_Text1.text = text5;
				break;
			case 2:
				_IsSlot2Empty = false;
				UE_Text2.text = text5;
				break;
			case 3:
				_IsSlot3Empty = false;
				UE_Text3.text = text5;
				break;
			}
		}
	}

	public bool IsSlotEmpty(int slotId)
	{
		return slotId switch
		{
			1 => _IsSlot1Empty, 
			2 => _IsSlot2Empty, 
			3 => _IsSlot3Empty, 
			_ => true, 
		};
	}
}
