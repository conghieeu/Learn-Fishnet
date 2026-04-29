using System;
using TMPro;
using UnityEngine.UI;

public class UIPrefab_RepairResult : UIPrefab_ClosableByTimeBase
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_contents = "contents";

	public const string UEID_OK_button = "OK_button";

	private Image _UE_rootNode;

	private TMP_Text _UE_contents;

	private Button _UE_OK_button;

	private Action<string> _OnOK_button;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_contents => _UE_contents ?? (_UE_contents = PickText("contents"));

	public Button UE_OK_button => _UE_OK_button ?? (_UE_OK_button = PickButton("OK_button"));

	public Action<string> OnOK_button
	{
		get
		{
			return _OnOK_button;
		}
		set
		{
			_OnOK_button = value;
			SetOnButtonClick("OK_button", value);
		}
	}
}
