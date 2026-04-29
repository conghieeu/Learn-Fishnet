using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_TimeWarning : UIPrefabScript
{
	public const string UEID_bg = "bg";

	public const string UEID_timeText = "timeText";

	public const string UEID_age = "age";

	public const string UEID_age2 = "age2";

	public const string UEID_age3 = "age3";

	private Image _UE_bg;

	private TMP_Text _UE_timeText;

	private Image _UE_age;

	private Image _UE_age2;

	private Image _UE_age3;

	public float fadeInTime = 0.5f;

	public float fadeOutTime = 0.5f;

	public float showTime = 3f;

	private int time;

	public Image UE_bg => _UE_bg ?? (_UE_bg = PickImage("bg"));

	public TMP_Text UE_timeText => _UE_timeText ?? (_UE_timeText = PickText("timeText"));

	public Image UE_age => _UE_age ?? (_UE_age = PickImage("age"));

	public Image UE_age2 => _UE_age2 ?? (_UE_age2 = PickImage("age2"));

	public Image UE_age3 => _UE_age3 ?? (_UE_age3 = PickImage("age3"));

	private void OnEnable()
	{
		StartCoroutine(ShowWarning());
	}

	private void Start()
	{
		SetText();
	}

	public void SetText()
	{
		UE_timeText.text = Hub.GetL10NText("STRING_GRAC_DURATION", time);
		UE_bg.color = new Color(0f, 0f, 0f, 0f);
		UE_age.color = new Color(1f, 1f, 1f, 0f);
		UE_age2.color = new Color(1f, 1f, 1f, 0f);
		UE_age3.color = new Color(1f, 1f, 1f, 0f);
		UE_timeText.color = new Color(1f, 1f, 1f, 0f);
	}

	private IEnumerator ShowWarning()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime(3600f);
			time++;
			UE_timeText.text = Hub.GetL10NText("STRING_GRAC_DURATION", time);
			UE_bg.DOColor(new Color(0f, 0f, 0f, 0.8f), fadeInTime);
			UE_timeText.DOColor(new Color(1f, 1f, 1f, 1f), fadeInTime);
			UE_age.DOColor(new Color(1f, 1f, 1f, 1f), fadeInTime);
			UE_age2.DOColor(new Color(1f, 1f, 1f, 1f), fadeInTime);
			UE_age3.DOColor(new Color(1f, 1f, 1f, 1f), fadeInTime);
			yield return new WaitForSeconds(fadeInTime + showTime);
			UE_bg.DOColor(new Color(0f, 0f, 0f, 0f), fadeOutTime);
			UE_timeText.DOColor(new Color(1f, 1f, 1f, 0f), fadeOutTime);
			UE_age.DOColor(new Color(1f, 1f, 1f, 0f), fadeOutTime);
			UE_age2.DOColor(new Color(1f, 1f, 1f, 0f), fadeOutTime);
			UE_age3.DOColor(new Color(1f, 1f, 1f, 0f), fadeOutTime);
			yield return new WaitForSeconds(fadeOutTime);
		}
	}
}
