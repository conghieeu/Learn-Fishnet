using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MMUIPrefabTable", menuName = "_Mimic/MMUIPrefabTable", order = 0)]
public class MMUIPrefabTable : ScriptableObject
{
	[Serializable]
	public class DialogBoxRow
	{
		public string id;

		public GameObject prefab;

		public eUIHeight height;

		public string title;

		[SerializeField]
		[TextArea(20, 40)]
		public string contents;

		public string ok;

		public string cancel;

		public bool hideOkBtn;

		public bool hideCancelBtn;

		public IEnumerator CorShowDialog(Action<eUIDialogueBoxResult> callback, Action<GameObject> informObject = null, string inTitle = "", string inContents = "", params string[] contentsArgs)
		{
			if (Hub.s == null)
			{
				yield break;
			}
			UIPrefab_dialogueBox dialogueBox = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_dialogueBox>(prefab, height);
			if (dialogueBox == null)
			{
				yield break;
			}
			informObject?.Invoke(dialogueBox.gameObject);
			dialogueBox.SetTitle(title, inTitle);
			dialogueBox.SetContents(contents, inContents, contentsArgs);
			dialogueBox.SetOKButton(ok, hideOkBtn);
			dialogueBox.SetCancelButton(cancel, hideCancelBtn);
			dialogueBox.SetCallbacks();
			dialogueBox.Show();
			yield return new WaitWhile(() => !dialogueBox.DialogResult.HasValue);
			if (!(dialogueBox == null))
			{
				yield return dialogueBox.Cor_Hide();
				if (!(dialogueBox == null))
				{
					Hub.s.uiman.ClearUIPrefab(dialogueBox.gameObject);
					eUIDialogueBoxResult obj = dialogueBox.DialogResult ?? eUIDialogueBoxResult.Cancel;
					callback?.Invoke(obj);
				}
			}
		}
	}

	[Serializable]
	public class TimerDialogBoxRow
	{
		public string id;

		public bool onlyOneAtMoment;

		public GameObject prefab;
	}

	public string[] prefabFolders = new string[1] { "Assets/_mimic/prefabs/VFX" };

	public DialogBoxRow[] commonDialogBoxes;

	public TimerDialogBoxRow[] timerDialogBoxes;

	public void ShowDialog(string id, Action<eUIDialogueBoxResult> callback, Action<GameObject> informObject = null, string title = "", string contents = "", params string[] contentsArgs)
	{
		DialogBoxRow dialogBoxRow = FindDialogRow(id);
		if (dialogBoxRow != null && Hub.s.uiman != null && Hub.s.uiman.isActiveAndEnabled)
		{
			Hub.s.uiman.StartCoroutine(dialogBoxRow.CorShowDialog(callback, informObject, title, contents, contentsArgs));
		}
	}

	private DialogBoxRow? FindDialogRow(string rowId)
	{
		DialogBoxRow[] array = commonDialogBoxes;
		foreach (DialogBoxRow dialogBoxRow in array)
		{
			if (dialogBoxRow != null && dialogBoxRow.id == rowId)
			{
				return dialogBoxRow;
			}
		}
		return null;
	}

	public float ShowTimerDialog(string id, float overrideDurationSec = 0f, params object[] contentsArgs)
	{
		TimerDialogBoxRow timerDialogBoxRow = FindTimerDialogRow(id);
		if (timerDialogBoxRow != null)
		{
			Hub.s.uiman.ShowTimerDialog(id, timerDialogBoxRow.prefab, timerDialogBoxRow.onlyOneAtMoment, overrideDurationSec, contentsArgs);
			UIPrefab_ClosableByTimeBase component = timerDialogBoxRow.prefab.GetComponent<UIPrefab_ClosableByTimeBase>();
			if (component != null)
			{
				if (!(overrideDurationSec > 0f))
				{
					return component.openingDurationSec;
				}
				return overrideDurationSec;
			}
		}
		return 0f;
	}

	private TimerDialogBoxRow? FindTimerDialogRow(string rowId)
	{
		TimerDialogBoxRow[] array = timerDialogBoxes;
		foreach (TimerDialogBoxRow timerDialogBoxRow in array)
		{
			if (timerDialogBoxRow != null && timerDialogBoxRow.id == rowId)
			{
				return timerDialogBoxRow;
			}
		}
		return null;
	}
}
