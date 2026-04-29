using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_QuestBriefing : UIPrefab_ClosableByTimeBase
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_NextQuota = "NextQuota";

	public const string UEID_NextQuotaValue = "NextQuotaValue";

	public const string UEID_NextJudgement = "NextJudgement";

	public const string UEID_RemainDays = "RemainDays";

	public const string UEID_DaysAfter = "DaysAfter";

	public const string UEID_TMP_StashListTitle = "TMP_StashListTitle";

	public const string UEID_TMP_StashList = "TMP_StashList";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_Text _UE_NextQuota;

	private TMP_Text _UE_NextQuotaValue;

	private TMP_Text _UE_NextJudgement;

	private TMP_Text _UE_RemainDays;

	private TMP_Text _UE_DaysAfter;

	private TMP_Text _UE_TMP_StashListTitle;

	private TMP_Text _UE_TMP_StashList;

	[SerializeField]
	private int lostStashItemDisplayCount;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_NextQuota => _UE_NextQuota ?? (_UE_NextQuota = PickText("NextQuota"));

	public TMP_Text UE_NextQuotaValue => _UE_NextQuotaValue ?? (_UE_NextQuotaValue = PickText("NextQuotaValue"));

	public TMP_Text UE_NextJudgement => _UE_NextJudgement ?? (_UE_NextJudgement = PickText("NextJudgement"));

	public TMP_Text UE_RemainDays => _UE_RemainDays ?? (_UE_RemainDays = PickText("RemainDays"));

	public TMP_Text UE_DaysAfter => _UE_DaysAfter ?? (_UE_DaysAfter = PickText("DaysAfter"));

	public TMP_Text UE_TMP_StashListTitle => _UE_TMP_StashListTitle ?? (_UE_TMP_StashListTitle = PickText("TMP_StashListTitle"));

	public TMP_Text UE_TMP_StashList => _UE_TMP_StashList ?? (_UE_TMP_StashList = PickText("TMP_StashList"));

	public override void PatchParameter(params object[] parameters)
	{
		UE_NextQuotaValue.SetText(parameters[0].ToString());
		UE_RemainDays.SetText(parameters[1].ToString());
		string text = "";
		int val = (int)parameters[2];
		val = Math.Min(val, lostStashItemDisplayCount);
		for (int i = 0; i < val; i++)
		{
			string key = parameters[i * 2 + 3].ToString();
			int num = (int)parameters[i * 2 + 4];
			text += $"{Hub.GetL10NText(key),-15} : ${num,-5}\n";
		}
		UE_TMP_StashList.SetText(text);
		if (val == 0)
		{
			UE_TMP_StashListTitle.transform.parent.gameObject.SetActive(value: false);
		}
	}
}
