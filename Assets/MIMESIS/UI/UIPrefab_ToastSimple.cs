using TMPro;

public class UIPrefab_ToastSimple : UIPrefab_ClosableByTimeBase
{
	public const string UEID_contents = "contents";

	private TMP_Text _UE_contents;

	public TMP_Text UE_contents => _UE_contents ?? (_UE_contents = PickText("contents"));

	public override void PatchParameter(params object[] parameters)
	{
		if (UE_contents != null)
		{
			UE_contents.SetText(parameters[0].ToString());
		}
	}
}
