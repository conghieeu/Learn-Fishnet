using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiPrefab_RoomCard : UIPrefabScript, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public delegate void OnMouseOverTargetChanged();

	public const string UEID_mouseover = "mouseover";

	public const string UEID_flag = "flag";

	public const string UEID_NationCode = "NationCode";

	public const string UEID_RoomName = "RoomName";

	public const string UEID_RoomCycle = "RoomCycle";

	public const string UEID_Status = "Status";

	public const string UEID_Start = "Start";

	public const string UEID_BeforeRepair = "BeforeRepair";

	public const string UEID_AfterRepair = "AfterRepair";

	public const string UEID_PlayerCount = "PlayerCount";

	public const string UEID_RepairStat = "RepairStat";

	private Image _UE_mouseover;

	private Image _UE_flag;

	private TMP_Text _UE_NationCode;

	private TMP_Text _UE_RoomName;

	private TMP_Text _UE_RoomCycle;

	private Image _UE_Status;

	private Image _UE_Start;

	private Image _UE_BeforeRepair;

	private Image _UE_AfterRepair;

	private TMP_Text _UE_PlayerCount;

	private TMP_Text _UE_RepairStat;

	public List<Image> targetImages;

	public List<TMP_Text> textTargets;

	public Action onEnter;

	public Action onExit;

	public string lobbyID;

	public Image UE_mouseover => _UE_mouseover ?? (_UE_mouseover = PickImage("mouseover"));

	public Image UE_flag => _UE_flag ?? (_UE_flag = PickImage("flag"));

	public TMP_Text UE_NationCode => _UE_NationCode ?? (_UE_NationCode = PickText("NationCode"));

	public TMP_Text UE_RoomName => _UE_RoomName ?? (_UE_RoomName = PickText("RoomName"));

	public TMP_Text UE_RoomCycle => _UE_RoomCycle ?? (_UE_RoomCycle = PickText("RoomCycle"));

	public Image UE_Status => _UE_Status ?? (_UE_Status = PickImage("Status"));

	public Image UE_Start => _UE_Start ?? (_UE_Start = PickImage("Start"));

	public Image UE_BeforeRepair => _UE_BeforeRepair ?? (_UE_BeforeRepair = PickImage("BeforeRepair"));

	public Image UE_AfterRepair => _UE_AfterRepair ?? (_UE_AfterRepair = PickImage("AfterRepair"));

	public TMP_Text UE_PlayerCount => _UE_PlayerCount ?? (_UE_PlayerCount = PickText("PlayerCount"));

	public TMP_Text UE_RepairStat => _UE_RepairStat ?? (_UE_RepairStat = PickText("RepairStat"));

	public event OnMouseOverTargetChanged onMouseOverTargetChanged;

	private void OnEnable()
	{
		UE_mouseover.color = new Color(0f, 0f, 0f, 0f);
		targetImages?.ForEach(delegate(Image img)
		{
			img.color = Color.white;
		});
		textTargets?.ForEach(delegate(TMP_Text txt)
		{
			txt.color = Color.white;
		});
	}

	public void SetRoomData(PublicRoomListData data, UIPrefab_PublicRoomList publicroomlist)
	{
		lobbyID = data.lobbyID.ToString();
		Sprite localeFlag = GetLocaleFlag(data.locale);
		UE_NationCode.gameObject.SetActive(value: true);
		UE_NationCode.text = data.locale;
		if (localeFlag == null)
		{
			UE_flag.gameObject.SetActive(value: false);
			UE_NationCode.gameObject.SetActive(value: true);
			UE_NationCode.text = data.locale;
		}
		else
		{
			UE_flag.gameObject.SetActive(value: true);
			UE_flag.sprite = localeFlag;
		}
		switch (data.repairStatus)
		{
		case 0:
			UE_RepairStat.text = Hub.GetL10NText("STRING_LOAD_SLOT_START");
			break;
		case 1:
			UE_RepairStat.text = Hub.GetL10NText("STRING_LOAD_SLOT_REPAIR_BEFORE");
			break;
		case 2:
			UE_RepairStat.text = Hub.GetL10NText("STRING_LOAD_SLOT_REPAIR_AFTER");
			break;
		}
		if (string.IsNullOrEmpty(data.lobbyName))
		{
			UE_RoomName.GetComponent<TMP_Text>().text = "Lobby " + lobbyID;
		}
		else
		{
			UE_RoomName.GetComponent<TMP_Text>().text = data.lobbyName;
		}
		UE_RoomCycle.GetComponent<TMP_Text>().text = Hub.GetL10NText("STRING_LOAD_SLOT_CYCLE", data.cycle.ToString());
		UE_PlayerCount.GetComponent<TMP_Text>().text = data.PlayerCount + "/4";
		GetComponent<Button>().onClick.AddListener(delegate
		{
			publicroomlist.ShowPopup(lobbyID, data.lobbyName);
			EventSystem.current.SetSelectedGameObject(null);
		});
		Show();
	}

	public Sprite GetLocaleFlag(string locale)
	{
		Texture2D texture2D = Hub.s.flagImageLoader.LoadFlagImage(locale);
		if (texture2D == null)
		{
			return null;
		}
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (enableUISound && !string.IsNullOrEmpty(buttonHoverSfxId) && Application.isFocused)
		{
			Hub.s.audioman.PlaySfx(buttonHoverSfxId);
		}
		UE_mouseover.color = new Color(1f, 1f, 1f, 1f);
		targetImages?.ForEach(delegate(Image img)
		{
			img.color = Hub.s.uiman.mouseOverTextColor;
		});
		textTargets?.ForEach(delegate(TMP_Text txt)
		{
			txt.color = Hub.s.uiman.mouseOverTextColor;
		});
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		UE_mouseover.color = new Color(0f, 0f, 0f, 0f);
		targetImages?.ForEach(delegate(Image img)
		{
			img.color = Color.white;
		});
		textTargets?.ForEach(delegate(TMP_Text txt)
		{
			txt.color = Color.white;
		});
	}
}
