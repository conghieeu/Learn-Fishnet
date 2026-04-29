using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class TMPDropdownAutoScroll : MonoBehaviour
{
	private TMP_Dropdown dropdown;

	private bool hasScrolled;

	private void Awake()
	{
		dropdown = GetComponent<TMP_Dropdown>();
	}

	private void Update()
	{
		Transform transform = base.transform.Find("Dropdown List");
		if (transform != null && transform.gameObject.activeSelf)
		{
			if (!hasScrolled)
			{
				StartCoroutine(ScrollToSelectedItem());
				hasScrolled = true;
			}
		}
		else
		{
			hasScrolled = false;
		}
	}

	private IEnumerator ScrollToSelectedItem()
	{
		yield return new WaitForEndOfFrame();
		Transform transform = base.transform.Find("Dropdown List");
		if (transform == null)
		{
			yield break;
		}
		ScrollRect componentInChildren = transform.GetComponentInChildren<ScrollRect>();
		if (!(componentInChildren == null))
		{
			int value = dropdown.value;
			int count = dropdown.options.Count;
			if (count > 1)
			{
				float value2 = 1f - (float)value / (float)(count - 1);
				componentInChildren.verticalNormalizedPosition = Mathf.Clamp01(value2);
				Canvas.ForceUpdateCanvases();
			}
		}
	}
}
