using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_MapSelector : UIPrefabScript
{
	public const string UEID_Display_BG = "Display_BG";

	public const string UEID_DescIcon1 = "DescIcon1";

	public const string UEID_NextHighlight = "NextHighlight";

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

	public const string UEID_Logo = "Logo";

	public const string UEID_Maintenance1 = "Maintenance1";

	public const string UEID_DescIcon2 = "DescIcon2";

	public const string UEID_CandidateHighlight = "CandidateHighlight";

	public const string UEID_TMP_Text_CandidateMap = "TMP_Text_CandidateMap";

	public const string UEID_icon_skull_candidate_1 = "icon_skull_candidate_1";

	public const string UEID_icon_skull_candidate_2 = "icon_skull_candidate_2";

	public const string UEID_icon_skull_candidate_3 = "icon_skull_candidate_3";

	public const string UEID_icon_gasmask_candidate_1 = "icon_gasmask_candidate_1";

	public const string UEID_icon_gasmask_candidate_2 = "icon_gasmask_candidate_2";

	public const string UEID_icon_gasmask_candidate_3 = "icon_gasmask_candidate_3";

	public const string UEID_icon_plenty_candidate_1 = "icon_plenty_candidate_1";

	public const string UEID_icon_plenty_candidate_2 = "icon_plenty_candidate_2";

	public const string UEID_icon_plenty_candidate_3 = "icon_plenty_candidate_3";

	public const string UEID_Maintenance2 = "Maintenance2";

	private Image _UE_Display_BG;

	private Transform _UE_DescIcon1;

	private Image _UE_NextHighlight;

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

	private Image _UE_Logo;

	private Image _UE_Maintenance1;

	private Transform _UE_DescIcon2;

	private Image _UE_CandidateHighlight;

	private TMP_Text _UE_TMP_Text_CandidateMap;

	private Image _UE_icon_skull_candidate_1;

	private Image _UE_icon_skull_candidate_2;

	private Image _UE_icon_skull_candidate_3;

	private Image _UE_icon_gasmask_candidate_1;

	private Image _UE_icon_gasmask_candidate_2;

	private Image _UE_icon_gasmask_candidate_3;

	private Image _UE_icon_plenty_candidate_1;

	private Image _UE_icon_plenty_candidate_2;

	private Image _UE_icon_plenty_candidate_3;

	private Image _UE_Maintenance2;

	private string tempNextMapName;

	private string tempCandidateMapName;

	[SerializeField]
	private Sprite unknownThumbnailImage;

	public Image UE_Display_BG => _UE_Display_BG ?? (_UE_Display_BG = PickImage("Display_BG"));

	public Transform UE_DescIcon1 => _UE_DescIcon1 ?? (_UE_DescIcon1 = PickTransform("DescIcon1"));

	public Image UE_NextHighlight => _UE_NextHighlight ?? (_UE_NextHighlight = PickImage("NextHighlight"));

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

	public Image UE_Logo => _UE_Logo ?? (_UE_Logo = PickImage("Logo"));

	public Image UE_Maintenance1 => _UE_Maintenance1 ?? (_UE_Maintenance1 = PickImage("Maintenance1"));

	public Transform UE_DescIcon2 => _UE_DescIcon2 ?? (_UE_DescIcon2 = PickTransform("DescIcon2"));

	public Image UE_CandidateHighlight => _UE_CandidateHighlight ?? (_UE_CandidateHighlight = PickImage("CandidateHighlight"));

	public TMP_Text UE_TMP_Text_CandidateMap => _UE_TMP_Text_CandidateMap ?? (_UE_TMP_Text_CandidateMap = PickText("TMP_Text_CandidateMap"));

	public Image UE_icon_skull_candidate_1 => _UE_icon_skull_candidate_1 ?? (_UE_icon_skull_candidate_1 = PickImage("icon_skull_candidate_1"));

	public Image UE_icon_skull_candidate_2 => _UE_icon_skull_candidate_2 ?? (_UE_icon_skull_candidate_2 = PickImage("icon_skull_candidate_2"));

	public Image UE_icon_skull_candidate_3 => _UE_icon_skull_candidate_3 ?? (_UE_icon_skull_candidate_3 = PickImage("icon_skull_candidate_3"));

	public Image UE_icon_gasmask_candidate_1 => _UE_icon_gasmask_candidate_1 ?? (_UE_icon_gasmask_candidate_1 = PickImage("icon_gasmask_candidate_1"));

	public Image UE_icon_gasmask_candidate_2 => _UE_icon_gasmask_candidate_2 ?? (_UE_icon_gasmask_candidate_2 = PickImage("icon_gasmask_candidate_2"));

	public Image UE_icon_gasmask_candidate_3 => _UE_icon_gasmask_candidate_3 ?? (_UE_icon_gasmask_candidate_3 = PickImage("icon_gasmask_candidate_3"));

	public Image UE_icon_plenty_candidate_1 => _UE_icon_plenty_candidate_1 ?? (_UE_icon_plenty_candidate_1 = PickImage("icon_plenty_candidate_1"));

	public Image UE_icon_plenty_candidate_2 => _UE_icon_plenty_candidate_2 ?? (_UE_icon_plenty_candidate_2 = PickImage("icon_plenty_candidate_2"));

	public Image UE_icon_plenty_candidate_3 => _UE_icon_plenty_candidate_3 ?? (_UE_icon_plenty_candidate_3 = PickImage("icon_plenty_candidate_3"));

	public Image UE_Maintenance2 => _UE_Maintenance2 ?? (_UE_Maintenance2 = PickImage("Maintenance2"));

	private void OnEnable()
	{
		Hub.s.lcman.onLanguageChanged += OnLanguageChanged_mapName;
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= OnLanguageChanged_mapName;
		}
	}

	public void SelectMap(int index)
	{
		switch (index)
		{
		case 0:
			UE_NextHighlight.gameObject.SetActive(value: true);
			UE_CandidateHighlight.gameObject.SetActive(value: false);
			break;
		case 1:
			UE_NextHighlight.gameObject.SetActive(value: false);
			UE_CandidateHighlight.gameObject.SetActive(value: true);
			break;
		}
	}

	public void ChangeMap(int nextDungeonMasterID, int candidateDungeonMasterID)
	{
		UE_NextHighlight.gameObject.SetActive(value: true);
		UE_CandidateHighlight.gameObject.SetActive(value: false);
		if (Hub.s.dataman.ExcelDataManager.DungeonInfoDict.TryGetValue(nextDungeonMasterID, out DungeonMasterInfo value))
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
			tempNextMapName = value.mapName;
			UE_TMP_Text_NextMap.text = Hub.GetL10NText(value.mapName);
		}
		if (Hub.s.dataman.ExcelDataManager.DungeonInfoDict.TryGetValue(candidateDungeonMasterID, out DungeonMasterInfo value2))
		{
			int num2 = 0;
			Color color3 = UE_icon_skull_candidate_1.color;
			color3 = new Color(color3.r, color3.g, color3.b, 1f);
			Color color4 = new Color(color3.r, color3.g, color3.b, 0f);
			UE_icon_skull_candidate_1.color = color4;
			UE_icon_skull_candidate_2.color = color4;
			UE_icon_skull_candidate_3.color = color4;
			if (num2++ < value2.HazardLevel)
			{
				UE_icon_skull_candidate_1.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			if (num2++ < value2.HazardLevel)
			{
				UE_icon_skull_candidate_2.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			if (num2++ < value2.HazardLevel)
			{
				UE_icon_skull_candidate_3.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			num2 = 0;
			color3 = UE_icon_gasmask_candidate_1.color;
			color3 = new Color(color3.r, color3.g, color3.b, 1f);
			color4 = new Color(color3.r, color3.g, color3.b, 0f);
			UE_icon_gasmask_candidate_1.color = color4;
			UE_icon_gasmask_candidate_2.color = color4;
			UE_icon_gasmask_candidate_3.color = color4;
			if (num2++ < value2.PollutionLevel)
			{
				UE_icon_gasmask_candidate_1.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			if (num2++ < value2.PollutionLevel)
			{
				UE_icon_gasmask_candidate_2.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			if (num2++ < value2.PollutionLevel)
			{
				UE_icon_gasmask_candidate_3.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			num2 = 0;
			color3 = UE_icon_plenty_candidate_1.color;
			color3 = new Color(color3.r, color3.g, color3.b, 1f);
			color4 = new Color(color3.r, color3.g, color3.b, 0f);
			UE_icon_plenty_candidate_1.color = color4;
			UE_icon_plenty_candidate_2.color = color4;
			UE_icon_plenty_candidate_3.color = color4;
			if (num2++ < value2.RewardLevel)
			{
				UE_icon_plenty_candidate_1.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			if (num2++ < value2.RewardLevel)
			{
				UE_icon_plenty_candidate_2.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			if (num2++ < value2.RewardLevel)
			{
				UE_icon_plenty_candidate_3.color = new Color(color3.r, color3.g, color3.b, 1f);
			}
			tempCandidateMapName = value2.mapName;
			UE_TMP_Text_CandidateMap.text = Hub.GetL10NText(value2.mapName);
		}
	}

	private void OnLanguageChanged_mapName()
	{
		UE_TMP_Text_NextMap.text = Hub.GetL10NText(tempNextMapName);
		UE_TMP_Text_CandidateMap.text = Hub.GetL10NText(tempCandidateMapName);
	}

	public void UpdateDate(int dayCount)
	{
		if (dayCount == Hub.s.dataman.ExcelDataManager.Consts.C_AvailableDayPerSession + 1)
		{
			UE_Maintenance1.gameObject.SetActive(value: true);
			UE_Maintenance2.gameObject.SetActive(value: true);
			UE_DescIcon1.gameObject.SetActive(value: false);
			UE_DescIcon2.gameObject.SetActive(value: false);
		}
		else
		{
			UE_Maintenance1.gameObject.SetActive(value: false);
			UE_Maintenance2.gameObject.SetActive(value: false);
			UE_DescIcon1.gameObject.SetActive(value: true);
			UE_DescIcon2.gameObject.SetActive(value: true);
		}
	}
}
