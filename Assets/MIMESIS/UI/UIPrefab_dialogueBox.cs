using System;
using TMPro;
using UnityEngine.UI;

public class UIPrefab_dialogueBox : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_contents = "contents";

	public const string UEID_OK_button = "OK_button";

	public const string UEID_Cancel_button = "Cancel_button";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_Text _UE_contents;

	private Button _UE_OK_button;

	private Action<string> _OnOK_button;

	private Button _UE_Cancel_button;

	private Action<string> _OnCancel_button;

	protected eUIDialogueBoxResult? _dialogResult;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

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

	public eUIDialogueBoxResult? DialogResult => _dialogResult;

	private void OnEnable()
	{
		if (!(Hub.s == null) && !(Hub.s.uiman == null))
		{
			Hub.s.uiman.ui_escapeStack.Add(this);
		}
	}

	private void OnDisable()
	{
		if (!(Hub.s == null) && !(Hub.s.uiman == null))
		{
			Hub.s.uiman.ui_escapeStack.Remove(this);
		}
	}

	public virtual void Setup(string tableTitle, string inTitle, string tableContents, string inContents, string[] contentsArgs, string tableOk, bool hideOkBtn, string tableCancel, bool hideCancelBtn)
	{
		_dialogResult = null;
		SetupTitle(tableTitle, inTitle);
		SetupContents(tableContents, inContents, contentsArgs);
		SetupOKButton(tableOk, hideOkBtn);
		SetupCancelButton(tableCancel, hideCancelBtn);
		SetupCallbacks();
	}

	protected virtual void SetupTitle(string tableTitle, string inTitle)
	{
		if (!(UE_title == null))
		{
			if (inTitle.Length > 0)
			{
				UE_title.text = Hub.GetL10NText(inTitle);
			}
			else if (tableTitle.Length > 0)
			{
				UE_title.text = tableTitle;
			}
		}
	}

	protected virtual void SetupContents(string tableContents, string inContents, string[] contentsArgs)
	{
		if (!(UE_contents == null))
		{
			if (inContents.Length > 0)
			{
				TMP_Text uE_contents = UE_contents;
				object[] formattingArgs = contentsArgs;
				uE_contents.text = Hub.GetL10NText(inContents, formattingArgs);
			}
			else if (tableContents.Length > 0)
			{
				TMP_Text uE_contents2 = UE_contents;
				object[] formattingArgs = contentsArgs;
				uE_contents2.text = string.Format(tableContents, formattingArgs);
			}
		}
	}

	protected virtual void SetupOKButton(string tableOk, bool hideOkBtn)
	{
		if (UE_OK_button == null)
		{
			return;
		}
		if (hideOkBtn)
		{
			UE_OK_button.gameObject.SetActive(value: false);
		}
		else if (tableOk.Length > 0)
		{
			TMP_Text componentInChildren = UE_OK_button.GetComponentInChildren<TMP_Text>();
			if (componentInChildren != null)
			{
				componentInChildren.text = tableOk;
			}
		}
	}

	protected virtual void SetupCancelButton(string tableCancel, bool hideCancelBtn)
	{
		if (UE_Cancel_button == null)
		{
			return;
		}
		if (hideCancelBtn)
		{
			UE_Cancel_button.gameObject.SetActive(value: false);
		}
		else if (tableCancel.Length > 0)
		{
			TMP_Text componentInChildren = UE_Cancel_button.GetComponentInChildren<TMP_Text>();
			if (componentInChildren != null)
			{
				componentInChildren.text = tableCancel;
			}
		}
	}

	protected virtual void SetupCallbacks()
	{
		if (UE_OK_button != null)
		{
			OnOK_button = delegate
			{
				_dialogResult = eUIDialogueBoxResult.OK;
			};
		}
		if (UE_Cancel_button != null)
		{
			OnCancel_button = delegate
			{
				_dialogResult = eUIDialogueBoxResult.Cancel;
			};
		}
	}

	public virtual void ForceClose(eUIDialogueBoxResult forceResult = eUIDialogueBoxResult.Cancel)
	{
		if (forceResult == eUIDialogueBoxResult.OK)
		{
			OnOK_button?.Invoke(null);
		}
		else
		{
			OnCancel_button?.Invoke(null);
		}
	}

	public virtual void SetTitle(string tableTitle, string inTitle = "")
	{
		SetupTitle(tableTitle, inTitle);
	}

	public virtual void SetContents(string tableContents, string inContents = "", params string[] contentsArgs)
	{
		SetupContents(tableContents, inContents, contentsArgs);
	}

	public virtual void SetOKButton(string tableOk, bool hideOkBtn = false)
	{
		SetupOKButton(tableOk, hideOkBtn);
	}

	public virtual void SetCancelButton(string tableCancel, bool hideCancelBtn = false)
	{
		SetupCancelButton(tableCancel, hideCancelBtn);
	}

	public virtual void SetCallbacks()
	{
		_dialogResult = null;
		SetupCallbacks();
	}

	public virtual void SetCustomCallbacks(Action onOK = null, Action onCancel = null)
	{
		_dialogResult = null;
		if (UE_OK_button != null && onOK != null)
		{
			OnOK_button = delegate
			{
				_dialogResult = eUIDialogueBoxResult.OK;
				onOK?.Invoke();
			};
		}
		if (UE_Cancel_button != null && onCancel != null)
		{
			OnCancel_button = delegate
			{
				_dialogResult = eUIDialogueBoxResult.Cancel;
				onCancel?.Invoke();
			};
		}
	}
}
