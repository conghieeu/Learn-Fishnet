using System;
using TMPro;
using UnityEngine.UI;

public class UIPrefab_ToastSimpleNormal : UIPrefab_dialogueBox
{
	public const string UEID_message = "message";

	public const string UEID_btnOK = "btnOK";

	private TMP_Text _UE_message;

	private Button _UE_btnOK;

	private Action<string> _OnbtnOK;

	public TMP_Text UE_message => _UE_message ?? (_UE_message = PickText("message"));

	public Button UE_btnOK => _UE_btnOK ?? (_UE_btnOK = PickButton("btnOK"));

	public Action<string> OnbtnOK
	{
		get
		{
			return _OnbtnOK;
		}
		set
		{
			_OnbtnOK = value;
			SetOnButtonClick("btnOK", value);
		}
	}

	public override void SetContents(string tableContents, string inContents = "", params string[] contentsArgs)
	{
		if (UE_message != null)
		{
			TMP_Text uE_message = UE_message;
			object[] formattingArgs = contentsArgs;
			uE_message.text = Hub.GetL10NText(inContents, formattingArgs);
		}
		else if (tableContents.Length > 0)
		{
			TMP_Text uE_message2 = UE_message;
			object[] formattingArgs = contentsArgs;
			uE_message2.text = string.Format(tableContents, formattingArgs);
		}
		else
		{
			UE_message.text = "";
		}
	}

	public override void SetCallbacks()
	{
		if (UE_btnOK != null)
		{
			OnbtnOK = delegate
			{
				_dialogResult = eUIDialogueBoxResult.OK;
			};
		}
	}
}
