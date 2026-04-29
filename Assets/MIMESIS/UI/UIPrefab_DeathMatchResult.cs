using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class UIPrefab_DeathMatchResult : UIPrefab_ClosableByTimeBase
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_NoSurvivors = "NoSurvivors";

	public const string UEID_Player1 = "Player1";

	public const string UEID_P1Name = "P1Name";

	public const string UEID_P1Survival = "P1Survival";

	public const string UEID_P1Award = "P1Award";

	public const string UEID_P1WellDone = "P1WellDone";

	public const string UEID_Player2 = "Player2";

	public const string UEID_P2Name = "P2Name";

	public const string UEID_P2Survival = "P2Survival";

	public const string UEID_P2Award = "P2Award";

	public const string UEID_P2WellDone = "P2WellDone";

	public const string UEID_Player3 = "Player3";

	public const string UEID_P3Name = "P3Name";

	public const string UEID_P3Survival = "P3Survival";

	public const string UEID_P3Award = "P3Award";

	public const string UEID_P3WellDone = "P3WellDone";

	public const string UEID_Player4 = "Player4";

	public const string UEID_P4Name = "P4Name";

	public const string UEID_P4Survival = "P4Survival";

	public const string UEID_P4Award = "P4Award";

	public const string UEID_P4WellDone = "P4WellDone";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_Text _UE_NoSurvivors;

	private Transform _UE_Player1;

	private TMP_Text _UE_P1Name;

	private TMP_Text _UE_P1Survival;

	private TMP_Text _UE_P1Award;

	private Image _UE_P1WellDone;

	private Transform _UE_Player2;

	private TMP_Text _UE_P2Name;

	private TMP_Text _UE_P2Survival;

	private TMP_Text _UE_P2Award;

	private Image _UE_P2WellDone;

	private Transform _UE_Player3;

	private TMP_Text _UE_P3Name;

	private TMP_Text _UE_P3Survival;

	private TMP_Text _UE_P3Award;

	private Image _UE_P3WellDone;

	private Transform _UE_Player4;

	private TMP_Text _UE_P4Name;

	private TMP_Text _UE_P4Survival;

	private TMP_Text _UE_P4Award;

	private Image _UE_P4WellDone;

	[SerializeField]
	private float stampDelay = 0.5f;

	private List<GameObject> stampObjectsToShow = new List<GameObject>();

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_NoSurvivors => _UE_NoSurvivors ?? (_UE_NoSurvivors = PickText("NoSurvivors"));

	public Transform UE_Player1 => _UE_Player1 ?? (_UE_Player1 = PickTransform("Player1"));

	public TMP_Text UE_P1Name => _UE_P1Name ?? (_UE_P1Name = PickText("P1Name"));

	public TMP_Text UE_P1Survival => _UE_P1Survival ?? (_UE_P1Survival = PickText("P1Survival"));

	public TMP_Text UE_P1Award => _UE_P1Award ?? (_UE_P1Award = PickText("P1Award"));

	public Image UE_P1WellDone => _UE_P1WellDone ?? (_UE_P1WellDone = PickImage("P1WellDone"));

	public Transform UE_Player2 => _UE_Player2 ?? (_UE_Player2 = PickTransform("Player2"));

	public TMP_Text UE_P2Name => _UE_P2Name ?? (_UE_P2Name = PickText("P2Name"));

	public TMP_Text UE_P2Survival => _UE_P2Survival ?? (_UE_P2Survival = PickText("P2Survival"));

	public TMP_Text UE_P2Award => _UE_P2Award ?? (_UE_P2Award = PickText("P2Award"));

	public Image UE_P2WellDone => _UE_P2WellDone ?? (_UE_P2WellDone = PickImage("P2WellDone"));

	public Transform UE_Player3 => _UE_Player3 ?? (_UE_Player3 = PickTransform("Player3"));

	public TMP_Text UE_P3Name => _UE_P3Name ?? (_UE_P3Name = PickText("P3Name"));

	public TMP_Text UE_P3Survival => _UE_P3Survival ?? (_UE_P3Survival = PickText("P3Survival"));

	public TMP_Text UE_P3Award => _UE_P3Award ?? (_UE_P3Award = PickText("P3Award"));

	public Image UE_P3WellDone => _UE_P3WellDone ?? (_UE_P3WellDone = PickImage("P3WellDone"));

	public Transform UE_Player4 => _UE_Player4 ?? (_UE_Player4 = PickTransform("Player4"));

	public TMP_Text UE_P4Name => _UE_P4Name ?? (_UE_P4Name = PickText("P4Name"));

	public TMP_Text UE_P4Survival => _UE_P4Survival ?? (_UE_P4Survival = PickText("P4Survival"));

	public TMP_Text UE_P4Award => _UE_P4Award ?? (_UE_P4Award = PickText("P4Award"));

	public Image UE_P4WellDone => _UE_P4WellDone ?? (_UE_P4WellDone = PickImage("P4WellDone"));

	private void OnEnable()
	{
		Hub.s.pdata.main.HideCommonUI();
	}

	protected override void OnShow()
	{
		base.OnShow();
		Hub.s.audioman.PlaySfx("GameOver");
		foreach (GameObject item in stampObjectsToShow)
		{
			StartCoroutine(CorShowStamp(item, stampDelay));
		}
	}

	public override void PatchParameter(params object[] parameters)
	{
		List<Transform> list = new List<Transform>();
		list.Add(UE_Player1);
		list.Add(UE_Player2);
		list.Add(UE_Player3);
		list.Add(UE_Player4);
		List<TMP_Text> list2 = new List<TMP_Text>();
		list2.Add(UE_P1Name);
		list2.Add(UE_P2Name);
		list2.Add(UE_P3Name);
		list2.Add(UE_P4Name);
		List<Image> list3 = new List<Image>();
		list3.Add(UE_P1WellDone);
		list3.Add(UE_P2WellDone);
		list3.Add(UE_P3WellDone);
		list3.Add(UE_P4WellDone);
		List<TMP_Text> list4 = new List<TMP_Text>();
		list4.Add(UE_P1Survival);
		list4.Add(UE_P2Survival);
		list4.Add(UE_P3Survival);
		list4.Add(UE_P4Survival);
		List<TMP_Text> list5 = new List<TMP_Text>();
		list5.Add(UE_P1Award);
		list5.Add(UE_P2Award);
		list5.Add(UE_P3Award);
		list5.Add(UE_P4Award);
		stampObjectsToShow.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].gameObject.SetActive(value: false);
			list3[i].gameObject.SetActive(value: false);
		}
		int num = parameters.Length / 3;
		bool flag = true;
		for (int j = 0; j < num; j++)
		{
			list[j].gameObject.SetActive(value: true);
			list2[j].SetText(parameters[3 * j].ToString());
			if ((bool)parameters[3 * j + 1])
			{
				stampObjectsToShow.Add(list3[j].gameObject);
				list4[j].gameObject.SetActive(value: true);
				list5[j].gameObject.SetActive(value: false);
				flag = false;
			}
			else
			{
				list5[j].SetText(parameters[3 * j + 2].ToString());
				list3[j].gameObject.SetActive(value: false);
				list4[j].gameObject.SetActive(value: false);
				list5[j].gameObject.SetActive(value: true);
			}
		}
		if (flag)
		{
			UE_NoSurvivors.gameObject.SetActive(value: true);
		}
		else
		{
			UE_NoSurvivors.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator CorShowStamp(GameObject stamp, float dealy)
	{
		yield return new WaitForSeconds(dealy);
		stamp.SetActive(value: true);
	}
}
