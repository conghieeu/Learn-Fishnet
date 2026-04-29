using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPrefab_TermsOfService : UIPrefabScript
{
	public const string UEID_title = "title";

	public const string UEID_Scroll_View = "Scroll_View";

	public const string UEID_Content = "Content";

	public const string UEID_text = "text";

	public const string UEID_Yes_mouseOver = "Yes_mouseOver";

	public const string UEID_No_mouseOver = "No_mouseOver";

	public const string UEID_yes = "yes";

	public const string UEID_no = "no";

	private TMP_Text _UE_title;

	private Image _UE_Scroll_View;

	private Transform _UE_Content;

	private TMP_Text _UE_text;

	private Image _UE_Yes_mouseOver;

	private Image _UE_No_mouseOver;

	private Button _UE_yes;

	private Action<string> _Onyes;

	private Button _UE_no;

	private Action<string> _Onno;

	public string L10NKey;

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public Image UE_Scroll_View => _UE_Scroll_View ?? (_UE_Scroll_View = PickImage("Scroll_View"));

	public Transform UE_Content => _UE_Content ?? (_UE_Content = PickTransform("Content"));

	public TMP_Text UE_text => _UE_text ?? (_UE_text = PickText("text"));

	public Image UE_Yes_mouseOver => _UE_Yes_mouseOver ?? (_UE_Yes_mouseOver = PickImage("Yes_mouseOver"));

	public Image UE_No_mouseOver => _UE_No_mouseOver ?? (_UE_No_mouseOver = PickImage("No_mouseOver"));

	public Button UE_yes => _UE_yes ?? (_UE_yes = PickButton("yes"));

	public Action<string> Onyes
	{
		get
		{
			return _Onyes;
		}
		set
		{
			_Onyes = value;
			SetOnButtonClick("yes", value);
		}
	}

	public Button UE_no => _UE_no ?? (_UE_no = PickButton("no"));

	public Action<string> Onno
	{
		get
		{
			return _Onno;
		}
		set
		{
			_Onno = value;
			SetOnButtonClick("no", value);
		}
	}

	private void Start()
	{
		Onyes = YesBtn;
		Onno = NoBtn;
		AddMouseOverEnterEvent(UE_yes.gameObject, new List<Image> { UE_Yes_mouseOver }, new List<TMP_Text> { UE_yes.GetComponentInChildren<TMP_Text>() });
		AddMouseOverEnterEvent(UE_no.gameObject, new List<Image> { UE_No_mouseOver }, new List<TMP_Text> { UE_no.GetComponentInChildren<TMP_Text>() });
		AddMouseOverExitEvent(UE_yes.gameObject, new List<Image> { UE_Yes_mouseOver }, new List<TMP_Text> { UE_yes.GetComponentInChildren<TMP_Text>() });
		AddMouseOverExitEvent(UE_no.gameObject, new List<Image> { UE_No_mouseOver }, new List<TMP_Text> { UE_no.GetComponentInChildren<TMP_Text>() });
	}

	private void AddMouseOverEnterEvent(GameObject go, List<Image> targetImage, List<TMP_Text> textTarget, MouseOverDelegate @delegate = null)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = go.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(delegate
		{
			targetImage?.ForEach(delegate(Image List)
			{
				List.color = new Color(1f, 1f, 1f, 1f);
			});
			textTarget?.ForEach(delegate(TMP_Text List)
			{
				List.color = Hub.s.uiman.mouseOverTextColor;
			});
			if (@delegate != null)
			{
				@delegate();
			}
		});
		eventTrigger.triggers.Add(entry);
	}

	private void AddMouseOverExitEvent(GameObject go, List<Image> targetImage, List<TMP_Text> textTarget, MouseOverDelegate @delegate = null)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = go.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerExit;
		entry.callback.AddListener(delegate
		{
			targetImage?.ForEach(delegate(Image List)
			{
				List.color = new Color(1f, 1f, 1f, 0f);
			});
			textTarget?.ForEach(delegate(TMP_Text List)
			{
				List.color = Color.white;
			});
			if (@delegate != null)
			{
				@delegate();
			}
		});
		eventTrigger.triggers.Add(entry);
	}

	private void NoBtn(string obj)
	{
		PlayerPrefs.SetInt("tos", 0);
		if (SceneManager.GetActiveScene().name.Equals("LobbyScene"))
		{
			Hub.s.uiman.CloseAgreeUI(this);
		}
		else
		{
			Application.Quit();
		}
	}

	private void YesBtn(string obj)
	{
		Hub.s.uiman.tos = true;
		PlayerPrefs.SetInt("tos", 1);
		if (SceneManager.GetActiveScene().name.Equals("LobbyScene"))
		{
			Hub.s.uiman.CloseAgreeUI(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(SetText());
	}

	private IEnumerator SetText()
	{
		yield return new WaitUntil(() => Hub.s.dataman.ExcelDataManager != null);
		if (L10NKey != "")
		{
			UE_text.text = Hub.GetL10NText(L10NKey);
		}
		RectTransform rectTransform = UE_text.rectTransform;
		RectTransform component = UE_Content.GetComponent<RectTransform>();
		float num = rectTransform.anchoredPosition.y + (1f - rectTransform.pivot.y) * rectTransform.sizeDelta.y;
		UE_text.ForceMeshUpdate();
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, UE_text.preferredHeight);
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, num - (1f - rectTransform.pivot.y) * rectTransform.sizeDelta.y);
		float num2 = component.anchoredPosition.y + (1f - component.pivot.y) * component.sizeDelta.y;
		component.sizeDelta = new Vector2(component.sizeDelta.x, rectTransform.sizeDelta.y);
		component.anchoredPosition = new Vector2(component.anchoredPosition.x, num2 - (1f - component.pivot.y) * component.sizeDelta.y);
	}
}
