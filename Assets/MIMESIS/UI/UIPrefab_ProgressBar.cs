using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_ProgressBar : UIPrefabScript
{
	public const string UEID_Img_blurIcon = "Img_blurIcon";

	public const string UEID_Img_Icon = "Img_Icon";

	public const string UEID_Img_blurBg_ = "Img_blurBg_";

	public const string UEID_Img_blurGauge = "Img_blurGauge";

	public const string UEID_Img_Glow = "Img_Glow";

	public const string UEID_Img_Bg = "Img_Bg";

	public const string UEID_Img_GaugeEffect = "Img_GaugeEffect";

	public const string UEID_Img_Gauge = "Img_Gauge";

	public const string UEID_Img_Stroke = "Img_Stroke";

	public const string UEID_Txt_Amount = "Txt_Amount";

	private Image _UE_Img_blurIcon;

	private Image _UE_Img_Icon;

	private Image _UE_Img_blurBg_;

	private Image _UE_Img_blurGauge;

	private Image _UE_Img_Glow;

	private Image _UE_Img_Bg;

	private Image _UE_Img_GaugeEffect;

	private Image _UE_Img_Gauge;

	private Image _UE_Img_Stroke;

	private TMP_Text _UE_Txt_Amount;

	[SerializeField]
	private Color progressBorderColor;

	[SerializeField]
	private Color progressMaxColor;

	[SerializeField]
	private Color progressMedianColor;

	[SerializeField]
	private Color progressMinColor;

	[SerializeField]
	private bool showText;

	[SerializeField]
	private bool isTextRatio;

	[SerializeField]
	private bool isTextCurrentMax;

	private float currentCached;

	private float maxCached;

	private bool currentBlink;

	private Coroutine blinkingCoroutine;

	[SerializeField]
	private List<Sprite> iconSprites = new List<Sprite>();

	[SerializeField]
	private List<Sprite> blueIconSprites = new List<Sprite>();

	[SerializeField]
	private List<Sprite> gaugeSprites = new List<Sprite>();

	[SerializeField]
	private List<Sprite> gaugeBgSprites = new List<Sprite>();

	[SerializeField]
	private List<Sprite> blurGaugeSprites = new List<Sprite>();

	[SerializeField]
	private List<Sprite> blurBgSprites = new List<Sprite>();

	[SerializeField]
	private List<Color> glowColors = new List<Color>();

	[Range(0f, 1f)]
	public float warningRatio = 0.7f;

	public bool useWarning;

	public bool warningGreaterThan;

	public float maxAlphaValue = 1f;

	public float alphaChangeTime = 0.5f;

	private float originAlphaValue = 0.2f;

	private bool warning;

	public List<EffectData> effectDataList = new List<EffectData>();

	public float gaugeEffectBGDuration = 1f;

	public float gaugeEffectDelayTime = 0.5f;

	public float gaugeEffectDuration = 1f;

	private Coroutine gaugeEffectCoroutine;

	private Coroutine gaugeEffectBGCoroutine;

	private string spectatedPlayerName = "";

	public Image UE_Img_blurIcon => _UE_Img_blurIcon ?? (_UE_Img_blurIcon = PickImage("Img_blurIcon"));

	public Image UE_Img_Icon => _UE_Img_Icon ?? (_UE_Img_Icon = PickImage("Img_Icon"));

	public Image UE_Img_blurBg_ => _UE_Img_blurBg_ ?? (_UE_Img_blurBg_ = PickImage("Img_blurBg_"));

	public Image UE_Img_blurGauge => _UE_Img_blurGauge ?? (_UE_Img_blurGauge = PickImage("Img_blurGauge"));

	public Image UE_Img_Glow => _UE_Img_Glow ?? (_UE_Img_Glow = PickImage("Img_Glow"));

	public Image UE_Img_Bg => _UE_Img_Bg ?? (_UE_Img_Bg = PickImage("Img_Bg"));

	public Image UE_Img_GaugeEffect => _UE_Img_GaugeEffect ?? (_UE_Img_GaugeEffect = PickImage("Img_GaugeEffect"));

	public Image UE_Img_Gauge => _UE_Img_Gauge ?? (_UE_Img_Gauge = PickImage("Img_Gauge"));

	public Image UE_Img_Stroke => _UE_Img_Stroke ?? (_UE_Img_Stroke = PickImage("Img_Stroke"));

	public TMP_Text UE_Txt_Amount => _UE_Txt_Amount ?? (_UE_Txt_Amount = PickText("Txt_Amount"));

	protected override void OnShow()
	{
		UE_Img_Bg.color = progressBorderColor;
		UE_Img_Gauge.color = progressMaxColor;
		UE_Txt_Amount.gameObject.SetActive(showText);
		currentCached = 0f;
		maxCached = 0f;
	}

	protected override void OnHide()
	{
		currentCached = 0f;
		maxCached = 0f;
	}

	public void SetBlinkingMode(bool onoff)
	{
		if (onoff)
		{
			blinkingCoroutine = StartCoroutine(Blink());
		}
		else
		{
			if (blinkingCoroutine != null)
			{
				StopCoroutine(blinkingCoroutine);
			}
			if (gaugeSprites.Count >= 10 && gaugeBgSprites.Count >= 10)
			{
				SetGaugeSprite(currentCached, maxCached);
			}
			else
			{
				UE_Img_Gauge.color = GetCurrentColor(currentCached, maxCached);
			}
		}
		currentBlink = onoff;
	}

	private IEnumerator Blink()
	{
		while (true)
		{
			if (gaugeSprites.Count >= 10 && gaugeBgSprites.Count >= 10)
			{
				SetGaugeSprite(currentCached, maxCached);
				continue;
			}
			Color currentColor = GetCurrentColor(currentCached, maxCached);
			Color currentAlertColor = currentColor;
			currentAlertColor.a = 0f;
			DOTween.To(() => currentColor, delegate(Color x)
			{
				UE_Img_Gauge.color = x;
			}, currentAlertColor, 0.5f);
			yield return new WaitForSeconds(0.5f);
			DOTween.To(() => currentAlertColor, delegate(Color x)
			{
				UE_Img_Gauge.color = x;
			}, currentColor, 0.5f);
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void Refresh(float current, float max)
	{
		if (!base.gameObject.activeInHierarchy || UE_Img_Gauge == null || UE_Img_blurGauge == null || UE_Txt_Amount == null || (Mathf.Approximately(currentCached, current) && Mathf.Approximately(maxCached, max)) || max <= 0f)
		{
			return;
		}
		bool changedSpectatedPlayer = false;
		if (Hub.s.cameraman.IsSpectatorMode && (Hub.s.pdata.main.spectatorui != null || Hub.s.pdata.main.spectatorui.gameObject.activeInHierarchy) && spectatedPlayerName != Hub.s.pdata.main.spectatorui.tempPlayerName)
		{
			spectatedPlayerName = Hub.s.pdata.main.spectatorui.tempPlayerName;
			changedSpectatedPlayer = true;
		}
		float num = current / max;
		currentCached = current;
		maxCached = max;
		if (!currentBlink)
		{
			if (gaugeSprites.Count >= 10 && gaugeBgSprites.Count >= 10)
			{
				SetGaugeSprite(currentCached, maxCached);
			}
			else
			{
				UE_Img_Gauge.color = GetCurrentColor(currentCached, maxCached);
			}
		}
		if (effectDataList.Count > 0)
		{
			if (gaugeEffectBGCoroutine != null)
			{
				StopCoroutine(gaugeEffectBGCoroutine);
			}
			gaugeEffectBGCoroutine = StartCoroutine(GaugeEffectBG(num, changedSpectatedPlayer));
			if (gaugeEffectCoroutine != null)
			{
				StopCoroutine(gaugeEffectCoroutine);
				gaugeEffectCoroutine = StartCoroutine(GaugeEffect(num, delay: false, changedSpectatedPlayer));
			}
			else
			{
				gaugeEffectCoroutine = StartCoroutine(GaugeEffect(num, delay: false, changedSpectatedPlayer));
			}
		}
		else
		{
			UE_Img_Gauge.fillAmount = num;
		}
		UE_Img_blurGauge.fillAmount = num;
		if (showText)
		{
			if (isTextRatio)
			{
				UE_Txt_Amount.SetText($"{(int)(num * 100f)} %");
			}
			else if (isTextCurrentMax)
			{
				UE_Txt_Amount.SetText($"{current} / {max}");
			}
			else
			{
				UE_Txt_Amount.SetText($"{(int)(num * 100f)} %");
			}
		}
	}

	private IEnumerator GaugeEffectBG(float ratio, bool changedSpectatedPlayer = false)
	{
		UE_Img_GaugeEffect.gameObject.SetActive(value: true);
		float startFillAmount = UE_Img_GaugeEffect.fillAmount;
		float num = Mathf.Abs(ratio - startFillAmount);
		float actualDuration = num * gaugeEffectBGDuration;
		if (actualDuration < 0.01f || changedSpectatedPlayer)
		{
			UE_Img_GaugeEffect.fillAmount = ratio;
			yield break;
		}
		float elapsedTime = 0f;
		while (elapsedTime < actualDuration)
		{
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / actualDuration);
			UE_Img_GaugeEffect.fillAmount = Mathf.Lerp(startFillAmount, ratio, t);
			yield return null;
		}
		UE_Img_GaugeEffect.fillAmount = ratio;
	}

	private IEnumerator GaugeEffect(float ratio, bool delay = true, bool changedSpectatedPlayer = false)
	{
		if (delay)
		{
			yield return new WaitForSeconds(gaugeEffectDelayTime);
		}
		float startFillAmount = UE_Img_Gauge.fillAmount;
		float num = Mathf.Abs(ratio - startFillAmount);
		float actualDuration = num * gaugeEffectDuration;
		float gaugeWidth = UE_Img_GaugeEffect.rectTransform.rect.width;
		if (actualDuration < 0.01f || changedSpectatedPlayer)
		{
			UE_Img_Gauge.fillAmount = ratio;
			for (int i = 0; i < effectDataList.Count; i++)
			{
				if (effectDataList[i].effectObject == null)
				{
					continue;
				}
				RectTransform component = effectDataList[i].effectObject.GetComponent<RectTransform>();
				float x = ratio * gaugeWidth - gaugeWidth * 0.5f + 25f;
				component.anchoredPosition = new Vector2(x, component.anchoredPosition.y);
				if (ratio > effectDataList[i].minRatio && ratio < effectDataList[i].maxRatio)
				{
					if (!effectDataList[i].effectObject.activeSelf)
					{
						effectDataList[i].effectObject.SetActive(value: true);
					}
				}
				else if (effectDataList[i].effectObject.activeSelf)
				{
					effectDataList[i].effectObject.SetActive(value: false);
				}
			}
			gaugeEffectCoroutine = null;
			yield break;
		}
		float elapsedTime = 0f;
		while (elapsedTime < actualDuration)
		{
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / actualDuration);
			float num2 = Mathf.Lerp(startFillAmount, ratio, t);
			UE_Img_Gauge.fillAmount = num2;
			for (int j = 0; j < effectDataList.Count; j++)
			{
				if (effectDataList[j].effectObject == null)
				{
					continue;
				}
				RectTransform component2 = effectDataList[j].effectObject.GetComponent<RectTransform>();
				float x2 = num2 * gaugeWidth - gaugeWidth * 0.5f + 25f;
				component2.anchoredPosition = new Vector2(x2, component2.anchoredPosition.y);
				if (num2 > effectDataList[j].minRatio && num2 < effectDataList[j].maxRatio)
				{
					if (!effectDataList[j].effectObject.activeSelf)
					{
						effectDataList[j].effectObject.SetActive(value: true);
					}
				}
				else if (effectDataList[j].effectObject.activeSelf)
				{
					effectDataList[j].effectObject.SetActive(value: false);
				}
			}
			yield return null;
		}
		gaugeEffectCoroutine = null;
		UE_Img_Gauge.fillAmount = ratio;
	}

	private IEnumerator WarningGlow()
	{
		yield return new WaitForEndOfFrame();
		while (warning && base.gameObject.activeInHierarchy)
		{
			UE_Img_Glow.DOColor(new Color(UE_Img_Glow.color.r, UE_Img_Glow.color.g, UE_Img_Glow.color.b, maxAlphaValue), alphaChangeTime);
			yield return new WaitForSeconds(alphaChangeTime);
			UE_Img_Glow.DOColor(new Color(UE_Img_Glow.color.r, UE_Img_Glow.color.g, UE_Img_Glow.color.b, originAlphaValue), alphaChangeTime);
			yield return new WaitForSeconds(alphaChangeTime);
		}
	}

	private void SetGaugeSprite(float current, float max)
	{
		if (UE_Img_Bg == null || UE_Img_Gauge == null)
		{
			return;
		}
		float num = current / max;
		int num2 = Mathf.Clamp((int)(num * 10f), 0, 9);
		UE_Img_Bg.color = Color.white;
		UE_Img_Gauge.color = Color.white;
		UE_Img_Bg.sprite = gaugeBgSprites[num2];
		UE_Img_Gauge.sprite = gaugeSprites[num2];
		if (UE_Img_blurGauge != null && blurGaugeSprites.Count >= 10)
		{
			UE_Img_blurGauge.sprite = blurGaugeSprites[num2];
		}
		if (UE_Img_blurBg_ != null && blurBgSprites.Count >= 10)
		{
			UE_Img_blurBg_.sprite = blurBgSprites[num2];
		}
		if (UE_Img_Icon != null && iconSprites.Count >= 10)
		{
			UE_Img_Icon.sprite = iconSprites[num2];
		}
		if (UE_Img_blurIcon != null && blueIconSprites.Count >= 10)
		{
			UE_Img_blurIcon.sprite = blueIconSprites[num2];
		}
		if (UE_Img_Glow != null)
		{
			if (glowColors.Count == 0)
			{
				UE_Img_Glow.color = Color.white;
			}
			else if (glowColors.Count <= num2 && glowColors.Count > 0)
			{
				UE_Img_Glow.color = new Color(glowColors[glowColors.Count - 1].r, glowColors[glowColors.Count - 1].g, glowColors[glowColors.Count - 1].b, UE_Img_Glow.color.a);
			}
			else if (glowColors.Count > num2)
			{
				UE_Img_Glow.color = new Color(glowColors[num2].r, glowColors[num2].g, glowColors[num2].b, UE_Img_Glow.color.a);
			}
		}
		if (!useWarning)
		{
			return;
		}
		if (warningGreaterThan)
		{
			if (num >= warningRatio)
			{
				if (!warning)
				{
					warning = true;
					if (base.gameObject.activeInHierarchy)
					{
						StartCoroutine(WarningGlow());
					}
				}
			}
			else if (warning)
			{
				warning = false;
				if (base.gameObject.activeInHierarchy)
				{
					StopCoroutine(WarningGlow());
				}
				UE_Img_Glow.color = new Color(UE_Img_Glow.color.r, UE_Img_Glow.color.g, UE_Img_Glow.color.b, originAlphaValue);
			}
		}
		else if (num < warningRatio)
		{
			if (warning)
			{
				warning = false;
				if (base.gameObject.activeInHierarchy)
				{
					StopCoroutine(WarningGlow());
				}
				UE_Img_Glow.color = new Color(UE_Img_Glow.color.r, UE_Img_Glow.color.g, UE_Img_Glow.color.b, originAlphaValue);
			}
		}
		else if (!warning)
		{
			warning = true;
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(WarningGlow());
			}
		}
	}

	private Color GetCurrentColor(float current, float max)
	{
		float num2;
		float num = (num2 = current / max);
		Color a = progressMedianColor;
		Color b = progressMaxColor;
		if (num < 0.5f)
		{
			a = progressMinColor;
			b = progressMedianColor;
			num2 *= 2f;
		}
		else
		{
			num2 = num2 * 2f - 1f;
		}
		num2 = Mathf.Clamp(num2, 0f, 1f);
		currentCached = current;
		maxCached = max;
		return Color.Lerp(a, b, num2);
	}
}
