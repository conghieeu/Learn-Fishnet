using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_OpeningAgeWarning : UIPrefabScript
{
	public const string UEID_panel = "panel";

	private Image _UE_panel;

	public Image UE_panel => _UE_panel ?? (_UE_panel = PickImage("panel"));

	public IEnumerator Process()
	{
		FadeIn();
		yield return new WaitForSeconds(4f);
		FadeOut();
		yield return new WaitForSeconds(1f);
		Hide();
	}

	private void FadeIn()
	{
		UE_panel.DOColor(new Color(0f, 0f, 0f, 0f), 0.5f);
	}

	private void FadeOut()
	{
		UE_panel.DOColor(new Color(0f, 0f, 0f, 1f), 0.5f);
	}
}
