using TMPro;
using UnityEngine.UI;

public class UIPrefab_CollectionResult : UIPrefab_ClosableByTimeBase
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_TotalQuota = "TotalQuota";

	public const string UEID_QuotaInThisStage = "QuotaInThisStage";

	public const string UEID_CollectionResultSuccess = "CollectionResultSuccess";

	public const string UEID_CollectionResultFailed = "CollectionResultFailed";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_Text _UE_TotalQuota;

	private TMP_Text _UE_QuotaInThisStage;

	private TMP_Text _UE_CollectionResultSuccess;

	private TMP_Text _UE_CollectionResultFailed;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_TotalQuota => _UE_TotalQuota ?? (_UE_TotalQuota = PickText("TotalQuota"));

	public TMP_Text UE_QuotaInThisStage => _UE_QuotaInThisStage ?? (_UE_QuotaInThisStage = PickText("QuotaInThisStage"));

	public TMP_Text UE_CollectionResultSuccess => _UE_CollectionResultSuccess ?? (_UE_CollectionResultSuccess = PickText("CollectionResultSuccess"));

	public TMP_Text UE_CollectionResultFailed => _UE_CollectionResultFailed ?? (_UE_CollectionResultFailed = PickText("CollectionResultFailed"));

	private void OnEnable()
	{
		Hub.s.pdata.main.HideCommonUI();
	}

	public override void PatchParameter(params object[] parameters)
	{
		bool flag = (bool)parameters[0];
		int num = (int)parameters[1];
		int num2 = (int)parameters[2];
		UE_TotalQuota.SetText(num.ToString());
		UE_QuotaInThisStage.SetText(num2.ToString());
		UE_CollectionResultSuccess.gameObject.SetActive(flag);
		UE_CollectionResultFailed.gameObject.SetActive(!flag);
	}
}
