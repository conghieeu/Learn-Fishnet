using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_CheckAgeForVoiceChat : UIPrefabScript
{
	public const string UEID_title = "title";

	public const string UEID_contents = "contents";

	public const string UEID_Over = "Over";

	public const string UEID_Under = "Under";

	private TMP_Text _UE_title;

	private TMP_Text _UE_contents;

	private Button _UE_Over;

	private Action<string> _OnOver;

	private Button _UE_Under;

	private Action<string> _OnUnder;

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_Text UE_contents => _UE_contents ?? (_UE_contents = PickText("contents"));

	public Button UE_Over => _UE_Over ?? (_UE_Over = PickButton("Over"));

	public Action<string> OnOver
	{
		get
		{
			return _OnOver;
		}
		set
		{
			_OnOver = value;
			SetOnButtonClick("Over", value);
		}
	}

	public Button UE_Under => _UE_Under ?? (_UE_Under = PickButton("Under"));

	public Action<string> OnUnder
	{
		get
		{
			return _OnUnder;
		}
		set
		{
			_OnUnder = value;
			SetOnButtonClick("Under", value);
		}
	}

	private void Start()
	{
		UE_Over.onClick.AddListener(delegate
		{
			Up();
		});
		UE_Under.onClick.AddListener(delegate
		{
			Down();
		});
		AddMouseOverEnterEvent(UE_Over.gameObject, UE_Over.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_Under.gameObject, UE_Under.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_Over.gameObject, UE_Over.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_Under.gameObject, UE_Under.GetComponentInChildren<TMP_Text>());
	}

	private void Up()
	{
		PlayerPrefs.SetInt("ageCheck", 1);
		Hub.s.uiman.ageCheck = 1;
		Hide();
	}

	private void Down()
	{
		Application.Quit();
	}
}
