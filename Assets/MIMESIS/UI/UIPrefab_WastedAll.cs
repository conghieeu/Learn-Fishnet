using TMPro;
using UnityEngine.UI;

public class UIPrefab_WastedAll : UIPrefab_ClosableByTimeBase
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_contentsRed = "contentsRed";

	public const string UEID_contentsBlack = "contentsBlack";

	public Image UE_rootNode => PickImage("rootNode");

	public TMP_Text UE_title => PickText("title");

	public TMP_Text UE_contentsRed => PickText("contentsRed");

	public TMP_Text UE_contentsBlack => PickText("contentsBlack");

	public override void PatchParameter(params object[] parameters)
	{
	}

	private void OnEnable()
	{
		Hub.s.pdata.main.HideCommonUI();
	}
}
