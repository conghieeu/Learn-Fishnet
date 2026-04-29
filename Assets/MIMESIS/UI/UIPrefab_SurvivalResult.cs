using System.Collections.Generic;
using ReluProtocol.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class UIPrefab_SurvivalResult : UIPrefab_ClosableByTimeBase
{
	public enum eActorSurvivalState
	{
		Survive = 0,
		Wasted = 1,
		Killed = 2,
		Unknown = 3
	}

	public const string UEID_rootNode = "rootNode";

	public const string UEID_RandomSeed = "RandomSeed";

	public const string UEID_title = "title";

	public const string UEID_P1Name = "P1Name";

	public const string UEID_P1Survival = "P1Survival";

	public const string UEID_P1Killed = "P1Killed";

	public const string UEID_P1Wasted = "P1Wasted";

	public const string UEID_P1Award = "P1Award";

	public const string UEID_P2Name = "P2Name";

	public const string UEID_P2Survival = "P2Survival";

	public const string UEID_P2Killed = "P2Killed";

	public const string UEID_P2Wasted = "P2Wasted";

	public const string UEID_P2Award = "P2Award";

	public const string UEID_P3Name = "P3Name";

	public const string UEID_P3Survival = "P3Survival";

	public const string UEID_P3Killed = "P3Killed";

	public const string UEID_P3Wasted = "P3Wasted";

	public const string UEID_P3Award = "P3Award";

	public const string UEID_P4Name = "P4Name";

	public const string UEID_P4Survival = "P4Survival";

	public const string UEID_P4Killed = "P4Killed";

	public const string UEID_P4Wasted = "P4Wasted";

	public const string UEID_P4Award = "P4Award";

	public const string UEID_LostAllScrab = "LostAllScrab";

	public const string UEID_LostScrabs = "LostScrabs";

	public const string UEID_Scrab3 = "Scrab3";

	public const string UEID_Scrab2 = "Scrab2";

	public const string UEID_Scrab1 = "Scrab1";

	private Image _UE_rootNode;

	private TMP_Text _UE_RandomSeed;

	private TMP_Text _UE_title;

	private TMP_Text _UE_P1Name;

	private TMP_Text _UE_P1Survival;

	private TMP_Text _UE_P1Killed;

	private TMP_Text _UE_P1Wasted;

	private TMP_Text _UE_P1Award;

	private TMP_Text _UE_P2Name;

	private TMP_Text _UE_P2Survival;

	private TMP_Text _UE_P2Killed;

	private TMP_Text _UE_P2Wasted;

	private TMP_Text _UE_P2Award;

	private TMP_Text _UE_P3Name;

	private TMP_Text _UE_P3Survival;

	private TMP_Text _UE_P3Killed;

	private TMP_Text _UE_P3Wasted;

	private TMP_Text _UE_P3Award;

	private TMP_Text _UE_P4Name;

	private TMP_Text _UE_P4Survival;

	private TMP_Text _UE_P4Killed;

	private TMP_Text _UE_P4Wasted;

	private TMP_Text _UE_P4Award;

	private TMP_Text _UE_LostAllScrab;

	private Transform _UE_LostScrabs;

	private TMP_Text _UE_Scrab3;

	private TMP_Text _UE_Scrab2;

	private TMP_Text _UE_Scrab1;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_RandomSeed => _UE_RandomSeed ?? (_UE_RandomSeed = PickText("RandomSeed"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_P1Name => _UE_P1Name ?? (_UE_P1Name = PickText("P1Name"));

	public TMP_Text UE_P1Survival => _UE_P1Survival ?? (_UE_P1Survival = PickText("P1Survival"));

	public TMP_Text UE_P1Killed => _UE_P1Killed ?? (_UE_P1Killed = PickText("P1Killed"));

	public TMP_Text UE_P1Wasted => _UE_P1Wasted ?? (_UE_P1Wasted = PickText("P1Wasted"));

	public TMP_Text UE_P1Award => _UE_P1Award ?? (_UE_P1Award = PickText("P1Award"));

	public TMP_Text UE_P2Name => _UE_P2Name ?? (_UE_P2Name = PickText("P2Name"));

	public TMP_Text UE_P2Survival => _UE_P2Survival ?? (_UE_P2Survival = PickText("P2Survival"));

	public TMP_Text UE_P2Killed => _UE_P2Killed ?? (_UE_P2Killed = PickText("P2Killed"));

	public TMP_Text UE_P2Wasted => _UE_P2Wasted ?? (_UE_P2Wasted = PickText("P2Wasted"));

	public TMP_Text UE_P2Award => _UE_P2Award ?? (_UE_P2Award = PickText("P2Award"));

	public TMP_Text UE_P3Name => _UE_P3Name ?? (_UE_P3Name = PickText("P3Name"));

	public TMP_Text UE_P3Survival => _UE_P3Survival ?? (_UE_P3Survival = PickText("P3Survival"));

	public TMP_Text UE_P3Killed => _UE_P3Killed ?? (_UE_P3Killed = PickText("P3Killed"));

	public TMP_Text UE_P3Wasted => _UE_P3Wasted ?? (_UE_P3Wasted = PickText("P3Wasted"));

	public TMP_Text UE_P3Award => _UE_P3Award ?? (_UE_P3Award = PickText("P3Award"));

	public TMP_Text UE_P4Name => _UE_P4Name ?? (_UE_P4Name = PickText("P4Name"));

	public TMP_Text UE_P4Survival => _UE_P4Survival ?? (_UE_P4Survival = PickText("P4Survival"));

	public TMP_Text UE_P4Killed => _UE_P4Killed ?? (_UE_P4Killed = PickText("P4Killed"));

	public TMP_Text UE_P4Wasted => _UE_P4Wasted ?? (_UE_P4Wasted = PickText("P4Wasted"));

	public TMP_Text UE_P4Award => _UE_P4Award ?? (_UE_P4Award = PickText("P4Award"));

	public TMP_Text UE_LostAllScrab => _UE_LostAllScrab ?? (_UE_LostAllScrab = PickText("LostAllScrab"));

	public Transform UE_LostScrabs => _UE_LostScrabs ?? (_UE_LostScrabs = PickTransform("LostScrabs"));

	public TMP_Text UE_Scrab3 => _UE_Scrab3 ?? (_UE_Scrab3 = PickText("Scrab3"));

	public TMP_Text UE_Scrab2 => _UE_Scrab2 ?? (_UE_Scrab2 = PickText("Scrab2"));

	public TMP_Text UE_Scrab1 => _UE_Scrab1 ?? (_UE_Scrab1 = PickText("Scrab1"));

	private void OnEnable()
	{
		Hub.s.pdata.main.HideCommonUI();
	}

	public override void PatchParameter(params object[] parameters)
	{
		List<TMP_Text> list = new List<TMP_Text>();
		list.Add(UE_P1Name);
		list.Add(UE_P2Name);
		list.Add(UE_P3Name);
		list.Add(UE_P4Name);
		List<TMP_Text> list2 = new List<TMP_Text>();
		list2.Add(UE_P1Survival);
		list2.Add(UE_P2Survival);
		list2.Add(UE_P3Survival);
		list2.Add(UE_P4Survival);
		List<TMP_Text> list3 = new List<TMP_Text>();
		list3.Add(UE_P1Killed);
		list3.Add(UE_P2Killed);
		list3.Add(UE_P3Killed);
		list3.Add(UE_P4Killed);
		List<TMP_Text> list4 = new List<TMP_Text>();
		list4.Add(UE_P1Wasted);
		list4.Add(UE_P2Wasted);
		list4.Add(UE_P3Wasted);
		list4.Add(UE_P4Wasted);
		List<TMP_Text> list5 = new List<TMP_Text>();
		list5.Add(UE_P1Award);
		list5.Add(UE_P2Award);
		list5.Add(UE_P3Award);
		list5.Add(UE_P4Award);
		List<TMP_Text> list6 = new List<TMP_Text>();
		list6.Add(UE_Scrab1);
		list6.Add(UE_Scrab2);
		list6.Add(UE_Scrab3);
		int c_MaxPlayerCount = Hub.s.dataman.ExcelDataManager.Consts.C_MaxPlayerCount;
		int num = (int)parameters[0];
		string l10NText = Hub.GetL10NText("UI_PREFAB_SURVIVAL_RESULT_TITLE", num);
		UE_title.SetText(l10NText);
		bool flag = (bool)parameters[1];
		int num2 = (int)parameters[2];
		int num3 = num2;
		if (num2 > c_MaxPlayerCount)
		{
			Logger.RWarn($"playerCount({num2}) > available UI slots({c_MaxPlayerCount}). Clamping.");
			num2 = c_MaxPlayerCount;
		}
		for (int i = 0; i < num2; i++)
		{
			list[i].transform.parent.gameObject.SetActive(value: true);
			list[i].SetText(parameters[3 * i + 3].ToString());
			eActorSurvivalState eActorSurvivalState2 = (eActorSurvivalState)parameters[3 * i + 4];
			switch (eActorSurvivalState2)
			{
			case eActorSurvivalState.Survive:
				list2[i].gameObject.SetActive(value: true);
				break;
			case eActorSurvivalState.Killed:
				list3[i].gameObject.SetActive(value: true);
				break;
			case eActorSurvivalState.Wasted:
				list4[i].gameObject.SetActive(value: true);
				break;
			case eActorSurvivalState.Unknown:
				Logger.RError($"Unknown ActorSurvivalState: {eActorSurvivalState2} for {list[i].text}");
				break;
			}
			switch ((AwardType)parameters[3 * i + 5])
			{
			case AwardType.None:
				list5[i].SetText("");
				list5[i].gameObject.SetActive(value: true);
				break;
			case AwardType.BestCarryItem:
				list5[i].SetText(Hub.GetL10NText("STRING_SETTLEMENT_REPORT_CASE_1"));
				list5[i].gameObject.SetActive(value: true);
				break;
			case AwardType.BestDamageToAlly:
				list5[i].SetText(Hub.GetL10NText("STRING_SETTLEMENT_REPORT_CASE_2"));
				list5[i].gameObject.SetActive(value: true);
				break;
			case AwardType.BestMimicEncounter:
				list5[i].SetText(Hub.GetL10NText("STRING_SETTLEMENT_REPORT_CASE_3"));
				list5[i].gameObject.SetActive(value: true);
				break;
			case AwardType.BestCamper:
				list5[i].SetText(Hub.GetL10NText("STRING_SETTLEMENT_REPORT_CASE_4"));
				list5[i].gameObject.SetActive(value: true);
				break;
			}
		}
		if (flag)
		{
			UE_LostAllScrab.gameObject.SetActive(value: false);
			int num4 = (int)parameters[num3 * 3 + 3];
			if (num4 == 0)
			{
				UE_LostScrabs.gameObject.SetActive(value: false);
			}
			else
			{
				UE_LostScrabs.gameObject.SetActive(value: true);
				for (int j = 0; j < num4; j++)
				{
					string key = parameters[num3 * 3 + 4 + j * 2].ToString();
					int num5 = (int)parameters[num3 * 3 + 4 + j * 2 + 1];
					if (list6.Count > j)
					{
						list6[j].gameObject.SetActive(value: true);
						list6[j].SetText($"\\ {Hub.GetL10NText(key),-15} : ${num5,-5}\n");
					}
				}
			}
		}
		else
		{
			UE_LostScrabs.gameObject.SetActive(value: false);
			UE_LostAllScrab.gameObject.SetActive(value: true);
		}
		if (UE_RandomSeed != null)
		{
			UE_RandomSeed.text += $"{Hub.s.pdata.randDungeonSeed}";
		}
	}
}
