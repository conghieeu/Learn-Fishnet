using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mimic.Actors;
using Mimic.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_crosshair : UIPrefabScript
{
	[Serializable]
	public class CrosshairTypeImage
	{
		public CrosshairType crosshairType;

		public Image crosshairImage;
	}

	public const string UEID_description = "description";

	public const string UEID_txt_description = "txt_description";

	public const string UEID_keyImage = "keyImage";

	public const string UEID_txt_addtionalDescription = "txt_addtionalDescription";

	private Transform _UE_description;

	private TMP_Text _UE_txt_description;

	private Image _UE_keyImage;

	private TMP_Text _UE_txt_addtionalDescription;

	[SerializeField]
	private List<CrosshairTypeImage> crosshairAll;

	private Dictionary<CrosshairType, Image> crosshairDict = new Dictionary<CrosshairType, Image>();

	private Dictionary<Image, UICircularGauge> crosshairGaugeDict = new Dictionary<Image, UICircularGauge>();

	private Image currentCrosshair;

	private bool IsAdditionalSimpleTextBlinking;

	private Coroutine additionalSimpleTextBlinkingCoroutine;

	public Transform UE_description => _UE_description ?? (_UE_description = PickTransform("description"));

	public TMP_Text UE_txt_description => _UE_txt_description ?? (_UE_txt_description = PickText("txt_description"));

	public Image UE_keyImage => _UE_keyImage ?? (_UE_keyImage = PickImage("keyImage"));

	public TMP_Text UE_txt_addtionalDescription => _UE_txt_addtionalDescription ?? (_UE_txt_addtionalDescription = PickText("txt_addtionalDescription"));

	private void Start()
	{
		foreach (CrosshairTypeImage item in crosshairAll)
		{
			crosshairDict.Add(item.crosshairType, item.crosshairImage);
			if (item.crosshairImage != null)
			{
				UICircularGauge component = item.crosshairImage.GetComponent<UICircularGauge>();
				if (component != null)
				{
					crosshairGaugeDict.Add(item.crosshairImage, component);
				}
			}
		}
	}

	private void OnDestroy()
	{
		crosshairDict.Clear();
		crosshairGaugeDict.Clear();
	}

	private void UpdateCrosshair(Image newCrosshair, float crosshairAnimDuration)
	{
		if (currentCrosshair != null && newCrosshair != null && currentCrosshair == newCrosshair)
		{
			return;
		}
		if (currentCrosshair != null)
		{
			if (crosshairGaugeDict.TryGetValue(currentCrosshair, out var value))
			{
				value.StopAnimation();
			}
			currentCrosshair.enabled = false;
		}
		float duration = 0.5f;
		if (newCrosshair != null)
		{
			newCrosshair.enabled = true;
			newCrosshair.DOColor(new Color(1f, 1f, 1f, 1f), duration).From(new Color(1f, 1f, 1f, 0f));
			if (crosshairGaugeDict.TryGetValue(newCrosshair, out var value2))
			{
				value2.StartAnimation(crosshairAnimDuration);
			}
		}
		currentCrosshair = newCrosshair;
	}

	public void ShowCrosshair(CrosshairType type, float animDuration)
	{
		if (crosshairDict.TryGetValue(type, out var value))
		{
			UpdateCrosshair(value, animDuration);
		}
	}

	public void HideCrosshair()
	{
		if (currentCrosshair != null)
		{
			if (crosshairGaugeDict.TryGetValue(currentCrosshair, out var value))
			{
				value.StopAnimation();
			}
			currentCrosshair.enabled = false;
		}
		currentCrosshair = null;
	}

	public void ShowText(LevelObject levelObject, ProtoActor protoActor)
	{
		string simpleText = levelObject.GetSimpleText(protoActor);
		if (simpleText.Contains("[key:") && simpleText.Contains("]"))
		{
			int num = simpleText.IndexOf("[key:");
			int num2 = simpleText.IndexOf("]", num);
			if (num >= 0 && num2 > num)
			{
				string text = simpleText.Substring(0, num).Trim();
				string text2 = ((num2 < simpleText.Length - 1) ? simpleText.Substring(num2 + 1).Trim() : "");
				string text3 = text;
				if (!string.IsNullOrEmpty(text2))
				{
					text3 = text3 + "\n" + text2;
				}
				UE_txt_description.SetText(text3);
				string value = simpleText.Substring(num + "[key:".Length, num2 - (num + "[key:".Length)).Trim();
				InputAction action = (InputAction)Enum.Parse(typeof(InputAction), value);
				Sprite keyImage = Hub.s.gameSettingManager.keyImageData.GetKeyImage(action);
				if (keyImage != null)
				{
					Sprite sprite = keyImage;
					float width = sprite.rect.width;
					float height = sprite.rect.height;
					Vector2 sizeDelta = new Vector2(width * 0.7f, height * 0.7f);
					UE_keyImage.sprite = sprite;
					UE_keyImage.rectTransform.sizeDelta = sizeDelta;
					UE_keyImage.gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			UE_txt_description.SetText(simpleText);
			UE_keyImage.gameObject.SetActive(value: false);
		}
		UE_txt_description.gameObject.SetActive(value: true);
		UE_description.gameObject.SetActive(value: true);
		string addtionalSimpleText = levelObject.GetAddtionalSimpleText(protoActor);
		if (addtionalSimpleText.Length > 0)
		{
			UE_txt_addtionalDescription.SetText(addtionalSimpleText);
			UE_txt_addtionalDescription.enabled = true;
			if (!IsAdditionalSimpleTextBlinking)
			{
				IsAdditionalSimpleTextBlinking = true;
				additionalSimpleTextBlinkingCoroutine = StartCoroutine(AdditionalSimpleTextBlinking());
			}
		}
		else
		{
			UE_txt_addtionalDescription.enabled = false;
			if (additionalSimpleTextBlinkingCoroutine != null)
			{
				StopCoroutine(additionalSimpleTextBlinkingCoroutine);
				additionalSimpleTextBlinkingCoroutine = null;
				IsAdditionalSimpleTextBlinking = false;
			}
		}
	}

	public void HideText()
	{
		UE_description.gameObject.SetActive(value: false);
		UE_txt_description.gameObject.SetActive(value: false);
		UE_keyImage.gameObject.SetActive(value: false);
		UE_txt_description.SetText("");
		if (additionalSimpleTextBlinkingCoroutine != null)
		{
			StopCoroutine(additionalSimpleTextBlinkingCoroutine);
			additionalSimpleTextBlinkingCoroutine = null;
			IsAdditionalSimpleTextBlinking = false;
		}
	}

	private IEnumerator AdditionalSimpleTextBlinking()
	{
		while (IsAdditionalSimpleTextBlinking)
		{
			UE_txt_addtionalDescription.DOColor(new Color(1f, 1f, 1f, 1f), 0.5f).From(new Color(1f, 1f, 1f, 0f));
			yield return new WaitForSeconds(0.5f);
			UE_txt_addtionalDescription.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f).From(new Color(1f, 1f, 1f, 1f));
			yield return new WaitForSeconds(0.5f);
		}
	}
}
