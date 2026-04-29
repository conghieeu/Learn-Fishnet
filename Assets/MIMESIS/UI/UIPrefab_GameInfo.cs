using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_GameInfo : UIPrefabScript
{
	public const string UEID_Info1 = "Info1";

	public const string UEID_TMP_Text_NextMap = "TMP_Text_NextMap";

	public const string UEID_icon_skull_next_1 = "icon_skull_next_1";

	public const string UEID_icon_skull_next_2 = "icon_skull_next_2";

	public const string UEID_icon_skull_next_3 = "icon_skull_next_3";

	public const string UEID_icon_gasmask_next_1 = "icon_gasmask_next_1";

	public const string UEID_icon_gasmask_next_2 = "icon_gasmask_next_2";

	public const string UEID_icon_gasmask_next_3 = "icon_gasmask_next_3";

	public const string UEID_icon_plenty_next_1 = "icon_plenty_next_1";

	public const string UEID_icon_plenty_next_2 = "icon_plenty_next_2";

	public const string UEID_icon_plenty_next_3 = "icon_plenty_next_3";

	public const string UEID_Info2 = "Info2";

	public const string UEID_TMP_Text_TimeNow = "TMP_Text_TimeNow";

	private Transform _UE_Info1;

	private TMP_Text _UE_TMP_Text_NextMap;

	private Image _UE_icon_skull_next_1;

	private Image _UE_icon_skull_next_2;

	private Image _UE_icon_skull_next_3;

	private Image _UE_icon_gasmask_next_1;

	private Image _UE_icon_gasmask_next_2;

	private Image _UE_icon_gasmask_next_3;

	private Image _UE_icon_plenty_next_1;

	private Image _UE_icon_plenty_next_2;

	private Image _UE_icon_plenty_next_3;

	private Transform _UE_Info2;

	private TMP_Text _UE_TMP_Text_TimeNow;

	private string tempMapName = string.Empty;

	public Transform UE_Info1 => _UE_Info1 ?? (_UE_Info1 = PickTransform("Info1"));

	public TMP_Text UE_TMP_Text_NextMap => _UE_TMP_Text_NextMap ?? (_UE_TMP_Text_NextMap = PickText("TMP_Text_NextMap"));

	public Image UE_icon_skull_next_1 => _UE_icon_skull_next_1 ?? (_UE_icon_skull_next_1 = PickImage("icon_skull_next_1"));

	public Image UE_icon_skull_next_2 => _UE_icon_skull_next_2 ?? (_UE_icon_skull_next_2 = PickImage("icon_skull_next_2"));

	public Image UE_icon_skull_next_3 => _UE_icon_skull_next_3 ?? (_UE_icon_skull_next_3 = PickImage("icon_skull_next_3"));

	public Image UE_icon_gasmask_next_1 => _UE_icon_gasmask_next_1 ?? (_UE_icon_gasmask_next_1 = PickImage("icon_gasmask_next_1"));

	public Image UE_icon_gasmask_next_2 => _UE_icon_gasmask_next_2 ?? (_UE_icon_gasmask_next_2 = PickImage("icon_gasmask_next_2"));

	public Image UE_icon_gasmask_next_3 => _UE_icon_gasmask_next_3 ?? (_UE_icon_gasmask_next_3 = PickImage("icon_gasmask_next_3"));

	public Image UE_icon_plenty_next_1 => _UE_icon_plenty_next_1 ?? (_UE_icon_plenty_next_1 = PickImage("icon_plenty_next_1"));

	public Image UE_icon_plenty_next_2 => _UE_icon_plenty_next_2 ?? (_UE_icon_plenty_next_2 = PickImage("icon_plenty_next_2"));

	public Image UE_icon_plenty_next_3 => _UE_icon_plenty_next_3 ?? (_UE_icon_plenty_next_3 = PickImage("icon_plenty_next_3"));

	public Transform UE_Info2 => _UE_Info2 ?? (_UE_Info2 = PickTransform("Info2"));

	public TMP_Text UE_TMP_Text_TimeNow => _UE_TMP_Text_TimeNow ?? (_UE_TMP_Text_TimeNow = PickText("TMP_Text_TimeNow"));

	private void OnEnable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged += SetMapName;
		}
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= SetMapName;
		}
	}

	public void ApplyMap(int dungeonMasterId)
	{
		if (Hub.s.dataman.ExcelDataManager.DungeonInfoDict.TryGetValue(dungeonMasterId, out DungeonMasterInfo value))
		{
			int num = 0;
			Color color = UE_icon_skull_next_1.color;
			color = new Color(color.r, color.g, color.b, 1f);
			Color color2 = new Color(color.r, color.g, color.b, 0f);
			UE_icon_skull_next_1.color = color2;
			UE_icon_skull_next_2.color = color2;
			UE_icon_skull_next_3.color = color2;
			if (num++ < value.HazardLevel)
			{
				UE_icon_skull_next_1.color = new Color(color.r, color.g, color.b, 1f);
			}
			if (num++ < value.HazardLevel)
			{
				UE_icon_skull_next_2.color = new Color(color.r, color.g, color.b, 1f);
			}
			if (num++ < value.HazardLevel)
			{
				UE_icon_skull_next_3.color = new Color(color.r, color.g, color.b, 1f);
			}
			num = 0;
			color = UE_icon_gasmask_next_1.color;
			color = new Color(color.r, color.g, color.b, 1f);
			color2 = new Color(color.r, color.g, color.b, 0f);
			UE_icon_gasmask_next_1.color = color2;
			UE_icon_gasmask_next_2.color = color2;
			UE_icon_gasmask_next_3.color = color2;
			if (num++ < value.PollutionLevel)
			{
				UE_icon_gasmask_next_1.color = new Color(color.r, color.g, color.b, 1f);
			}
			if (num++ < value.PollutionLevel)
			{
				UE_icon_gasmask_next_2.color = new Color(color.r, color.g, color.b, 1f);
			}
			if (num++ < value.PollutionLevel)
			{
				UE_icon_gasmask_next_3.color = new Color(color.r, color.g, color.b, 1f);
			}
			num = 0;
			color = UE_icon_plenty_next_1.color;
			color = new Color(color.r, color.g, color.b, 1f);
			color2 = new Color(color.r, color.g, color.b, 0f);
			UE_icon_plenty_next_1.color = color2;
			UE_icon_plenty_next_2.color = color2;
			UE_icon_plenty_next_3.color = color2;
			if (num++ < value.RewardLevel)
			{
				UE_icon_plenty_next_1.color = new Color(color.r, color.g, color.b, 1f);
			}
			if (num++ < value.RewardLevel)
			{
				UE_icon_plenty_next_2.color = new Color(color.r, color.g, color.b, 1f);
			}
			if (num++ < value.RewardLevel)
			{
				UE_icon_plenty_next_3.color = new Color(color.r, color.g, color.b, 1f);
			}
			tempMapName = value.mapName;
			SetMapName();
		}
	}

	private void SetMapName()
	{
		if (!string.IsNullOrEmpty(tempMapName))
		{
			UE_TMP_Text_NextMap.text = Hub.GetL10NText(tempMapName);
		}
	}

	public void UpdateTime(TimeSpan time)
	{
		if (time.Hours == 0 && time.Minutes == 0)
		{
			UE_TMP_Text_TimeNow.text = "24:00";
		}
		else
		{
			UE_TMP_Text_TimeNow.text = time.ToString("hh\\:mm");
		}
	}
}
