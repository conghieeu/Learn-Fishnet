using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIPrefab_GameStatus : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_contents = "contents";

	public const string UEID_Dropdown_selectMic = "Dropdown_selectMic";

	public const string UEID_quotaAmount = "quotaAmount";

	public const string UEID_OK_button = "OK_button";

	public const string UEID_Cancel_button = "Cancel_button";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_Text _UE_contents;

	private Image _UE_Dropdown_selectMic;

	private TMP_Text _UE_quotaAmount;

	private Button _UE_OK_button;

	private Action<string> _OnOK_button;

	private Button _UE_Cancel_button;

	private Action<string> _OnCancel_button;

	protected List<string> memberNames = new List<string>();

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_contents => _UE_contents ?? (_UE_contents = PickText("contents"));

	public Image UE_Dropdown_selectMic => _UE_Dropdown_selectMic ?? (_UE_Dropdown_selectMic = PickImage("Dropdown_selectMic"));

	public TMP_Text UE_quotaAmount => _UE_quotaAmount ?? (_UE_quotaAmount = PickText("quotaAmount"));

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

	public Button UE_Cancel_button => _UE_Cancel_button ?? (_UE_Cancel_button = PickButton("Cancel_button"));

	public Action<string> OnCancel_button
	{
		get
		{
			return _OnCancel_button;
		}
		set
		{
			_OnCancel_button = value;
			SetOnButtonClick("Cancel_button", value);
		}
	}

	public void AddMember(string memberName)
	{
		memberNames.Add(memberName);
		UE_contents.text = string.Join("\n", memberNames);
	}

	public void RemoveMember(string memberName)
	{
		memberNames.Remove(memberName);
		UE_contents.text = string.Join("\n", memberNames);
	}
}
