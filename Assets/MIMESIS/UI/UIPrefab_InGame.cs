using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_InGame : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_debugText = "debugText";

	public const string UEID_Fx_HPGauge = "Fx_HPGauge";

	public const string UEID_HP_blurBg = "HP_blurBg";

	public const string UEID_HP_bgBlur = "HP_bgBlur";

	public const string UEID_HP_bg = "HP_bg";

	public const string UEID_HP_barBlur = "HP_barBlur";

	public const string UEID_HP_bar = "HP_bar";

	public const string UEID_HP_bg_Glow = "HP_bg_Glow";

	public const string UEID_HP_bg_Glow_orange = "HP_bg_Glow_orange";

	public const string UEID_HP_bg_Glow_red = "HP_bg_Glow_red";

	public const string UEID_HP_icon_Glow = "HP_icon_Glow";

	public const string UEID_HP_iconBlur = "HP_iconBlur";

	public const string UEID_HP_icon = "HP_icon";

	public const string UEID_Currency = "Currency";

	public const string UEID_Saving = "Saving";

	public const string UEID_KillCount = "KillCount";

	public const string UEID_versionText = "versionText";

	public const string UEID_Image_interactable = "Image_interactable";

	public const string UEID_Image_scrap = "Image_scrap";

	private Image _UE_rootNode;

	private TMP_Text _UE_debugText;

	private Transform _UE_Fx_HPGauge;

	private Image _UE_HP_blurBg;

	private Image _UE_HP_bgBlur;

	private Image _UE_HP_bg;

	private Image _UE_HP_barBlur;

	private Image _UE_HP_bar;

	private Image _UE_HP_bg_Glow;

	private Image _UE_HP_bg_Glow_orange;

	private Image _UE_HP_bg_Glow_red;

	private Image _UE_HP_icon_Glow;

	private Image _UE_HP_iconBlur;

	private Image _UE_HP_icon;

	private TMP_Text _UE_Currency;

	private TMP_Text _UE_Saving;

	private TMP_Text _UE_KillCount;

	private TMP_Text _UE_versionText;

	private Image _UE_Image_interactable;

	private Image _UE_Image_scrap;

	private string deathAudioKey = "death_beep";

	[SerializeField]
	private UIPrefab_ProgressBar staminaGauge;

	[SerializeField]
	private UIPrefab_ProgressBar oxyGauge;

	[SerializeField]
	private UIScreenEffectConta contaScreenEffect;

	[SerializeField]
	private List<Sprite> greenLifes;

	[SerializeField]
	private List<Sprite> greenLifesBlur;

	[SerializeField]
	private List<Sprite> orangeLifes;

	[SerializeField]
	private List<Sprite> orangeLifesBlur;

	[SerializeField]
	private List<Sprite> redLifes;

	[SerializeField]
	private List<Sprite> redLifesBlur;

	[SerializeField]
	private Sprite greenIcon;

	[SerializeField]
	private Sprite greenIconBlur;

	[SerializeField]
	private Sprite orangeIcon;

	[SerializeField]
	private Sprite orangeIconBlur;

	[SerializeField]
	private Sprite redIcon;

	[SerializeField]
	private Sprite redIconBlur;

	[SerializeField]
	private Sprite greenBG;

	[SerializeField]
	private Sprite greenBGBlur;

	[SerializeField]
	private Sprite orangeBG;

	[SerializeField]
	private Sprite orangeBGBlur;

	[SerializeField]
	private Sprite redBG;

	[SerializeField]
	private Sprite redBGBlur;

	[SerializeField]
	private Sprite deathHP;

	[SerializeField]
	private Sprite deathHPBlur;

	[Space(10f)]
	[Header("hp바 세팅")]
	private int randomHP;

	[SerializeField]
	private float greenRatio = 0.5f;

	[SerializeField]
	private float orangeRatio = 0.25f;

	[SerializeField]
	private float greenSpeed = 1f;

	[SerializeField]
	private float orangeSpeed = 2f;

	[SerializeField]
	private float redSpeed = 3f;

	private float currPercent = 10f;

	[HideInInspector]
	public bool isDead;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_debugText => _UE_debugText ?? (_UE_debugText = PickText("debugText"));

	public Transform UE_Fx_HPGauge => _UE_Fx_HPGauge ?? (_UE_Fx_HPGauge = PickTransform("Fx_HPGauge"));

	public Image UE_HP_blurBg => _UE_HP_blurBg ?? (_UE_HP_blurBg = PickImage("HP_blurBg"));

	public Image UE_HP_bgBlur => _UE_HP_bgBlur ?? (_UE_HP_bgBlur = PickImage("HP_bgBlur"));

	public Image UE_HP_bg => _UE_HP_bg ?? (_UE_HP_bg = PickImage("HP_bg"));

	public Image UE_HP_barBlur => _UE_HP_barBlur ?? (_UE_HP_barBlur = PickImage("HP_barBlur"));

	public Image UE_HP_bar => _UE_HP_bar ?? (_UE_HP_bar = PickImage("HP_bar"));

	public Image UE_HP_bg_Glow => _UE_HP_bg_Glow ?? (_UE_HP_bg_Glow = PickImage("HP_bg_Glow"));

	public Image UE_HP_bg_Glow_orange => _UE_HP_bg_Glow_orange ?? (_UE_HP_bg_Glow_orange = PickImage("HP_bg_Glow_orange"));

	public Image UE_HP_bg_Glow_red => _UE_HP_bg_Glow_red ?? (_UE_HP_bg_Glow_red = PickImage("HP_bg_Glow_red"));

	public Image UE_HP_icon_Glow => _UE_HP_icon_Glow ?? (_UE_HP_icon_Glow = PickImage("HP_icon_Glow"));

	public Image UE_HP_iconBlur => _UE_HP_iconBlur ?? (_UE_HP_iconBlur = PickImage("HP_iconBlur"));

	public Image UE_HP_icon => _UE_HP_icon ?? (_UE_HP_icon = PickImage("HP_icon"));

	public TMP_Text UE_Currency => _UE_Currency ?? (_UE_Currency = PickText("Currency"));

	public TMP_Text UE_Saving => _UE_Saving ?? (_UE_Saving = PickText("Saving"));

	public TMP_Text UE_KillCount => _UE_KillCount ?? (_UE_KillCount = PickText("KillCount"));

	public TMP_Text UE_versionText => _UE_versionText ?? (_UE_versionText = PickText("versionText"));

	public Image UE_Image_interactable => _UE_Image_interactable ?? (_UE_Image_interactable = PickImage("Image_interactable"));

	public Image UE_Image_scrap => _UE_Image_scrap ?? (_UE_Image_scrap = PickImage("Image_scrap"));

	private void Start()
	{
		randomHP = Random.Range(0, greenLifes.Count);
		SetVersionText();
		contaScreenEffect.Initialize();
	}

	private void Update()
	{
		if (!(Hub.s == null) && Hub.s.pdata != null)
		{
			_ = Hub.s.pdata.main == null;
		}
	}

	protected override void OnShow()
	{
		if (staminaGauge != null)
		{
			staminaGauge.Show();
		}
		if (oxyGauge != null)
		{
			oxyGauge.Show();
		}
	}

	public void OnCurrencyChanged(long currency)
	{
		UE_Currency.SetText(Hub.GetL10NText("UI_PREFAB_IN_GAME_CURRENCY", currency));
	}

	private IEnumerator CorOnSaving(bool auto)
	{
		if (!(UE_Saving == null))
		{
			string text = (auto ? Hub.GetL10NText("STRING_AUTO_SAVING") : Hub.GetL10NText("STRING_MANUAL_SAVING"));
			TMP_Text component = UE_Saving.gameObject.GetComponent<TMP_Text>();
			DOTweenAnimation component2 = UE_Saving.gameObject.GetComponent<DOTweenAnimation>();
			UE_Saving.gameObject.SetActive(value: true);
			if (component2 != null)
			{
				component2.DORestart();
			}
			if (component != null)
			{
				component.text = text;
			}
			yield return new WaitForSeconds(4f);
			UE_Saving.gameObject.SetActive(value: false);
		}
	}

	public void OnSaving(bool auto)
	{
		StartCoroutine(CorOnSaving(auto));
	}

	public void OnHpChanged(long curr, long maxHP)
	{
		if (isDead)
		{
			UE_HP_bg.sprite = redBG;
			UE_HP_icon.sprite = redIcon;
			UE_HP_iconBlur.sprite = redIconBlur;
			UE_HP_icon_Glow.material.SetColor("_AlphaOutlineColor", new Color(1f, 0f, 0f, 0.2f));
			UE_HP_bg_Glow.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_orange.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_red.gameObject.SetActive(value: true);
			if (greenLifes[randomHP] != null)
			{
				UE_HP_bar.sprite = deathHP;
				UE_HP_barBlur.sprite = deathHPBlur;
			}
			else
			{
				UE_HP_bar.sprite = redLifes[0];
				UE_HP_barBlur.sprite = redLifesBlur[0];
			}
			UE_HP_bgBlur.sprite = redBGBlur;
			UE_HP_bar.material.SetFloat("_ScrollSpeed", 0f - redSpeed);
			return;
		}
		float num = (float)curr / (float)maxHP;
		if (currPercent == num)
		{
			return;
		}
		currPercent = num;
		if (num > greenRatio)
		{
			UE_Fx_HPGauge.gameObject.SetActive(value: false);
			UE_HP_bg.sprite = greenBG;
			UE_HP_icon.sprite = greenIcon;
			UE_HP_iconBlur.sprite = greenIconBlur;
			UE_HP_icon_Glow.material.SetColor("_AlphaOutlineColor", new Color(0.498f, 1f, 0.431f, 0.2f));
			UE_HP_bg_Glow.gameObject.SetActive(value: true);
			UE_HP_bg_Glow_orange.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_red.gameObject.SetActive(value: false);
			if (greenLifes[randomHP] != null)
			{
				UE_HP_bar.sprite = greenLifes[randomHP];
				UE_HP_barBlur.sprite = greenLifesBlur[randomHP];
			}
			else
			{
				UE_HP_bar.sprite = greenLifes[0];
				UE_HP_barBlur.sprite = greenLifesBlur[0];
			}
			UE_HP_bgBlur.sprite = greenBGBlur;
			UE_HP_bar.material.SetFloat("_ScrollSpeed", 0f - greenSpeed);
		}
		else if (num > orangeRatio)
		{
			UE_Fx_HPGauge.gameObject.SetActive(value: false);
			UE_HP_bg.sprite = orangeBG;
			UE_HP_icon.sprite = orangeIcon;
			UE_HP_iconBlur.sprite = orangeIconBlur;
			UE_HP_icon_Glow.material.SetColor("_AlphaOutlineColor", new Color(0.831f, 0.694f, 0.42f, 0.2f));
			UE_HP_bg_Glow.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_orange.gameObject.SetActive(value: true);
			UE_HP_bg_Glow_red.gameObject.SetActive(value: false);
			if (greenLifes[randomHP] != null)
			{
				UE_HP_bar.sprite = orangeLifes[randomHP];
				UE_HP_barBlur.sprite = orangeLifesBlur[randomHP];
			}
			else
			{
				UE_HP_bar.sprite = orangeLifes[0];
				UE_HP_barBlur.sprite = orangeLifesBlur[0];
			}
			UE_HP_bgBlur.sprite = orangeBGBlur;
			UE_HP_bar.material.SetFloat("_ScrollSpeed", 0f - orangeSpeed);
		}
		else if (num <= orangeRatio && num > 0f)
		{
			UE_Fx_HPGauge.gameObject.SetActive(value: true);
			UE_HP_bg.sprite = redBG;
			UE_HP_icon.sprite = redIcon;
			UE_HP_iconBlur.sprite = redIconBlur;
			UE_HP_icon_Glow.material.SetColor("_AlphaOutlineColor", new Color(1f, 0f, 0f, 0.2f));
			UE_HP_bg_Glow.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_orange.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_red.gameObject.SetActive(value: true);
			if (greenLifes[randomHP] != null)
			{
				UE_HP_bar.sprite = redLifes[randomHP];
				UE_HP_barBlur.sprite = redLifesBlur[randomHP];
			}
			else
			{
				UE_HP_bar.sprite = redLifes[0];
				UE_HP_barBlur.sprite = redLifesBlur[0];
			}
			UE_HP_bgBlur.sprite = redBGBlur;
			UE_HP_bar.material.SetFloat("_ScrollSpeed", 0f - redSpeed);
		}
		else
		{
			UE_Fx_HPGauge.gameObject.SetActive(value: true);
			UE_HP_bg.sprite = redBG;
			UE_HP_icon.sprite = redIcon;
			UE_HP_iconBlur.sprite = redIconBlur;
			UE_HP_icon_Glow.material.SetColor("_AlphaOutlineColor", new Color(1f, 0f, 0f, 0.2f));
			UE_HP_bg_Glow.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_orange.gameObject.SetActive(value: false);
			UE_HP_bg_Glow_red.gameObject.SetActive(value: true);
			if (greenLifes[randomHP] != null)
			{
				UE_HP_bar.sprite = deathHP;
				UE_HP_barBlur.sprite = deathHPBlur;
			}
			else
			{
				UE_HP_bar.sprite = redLifes[0];
				UE_HP_barBlur.sprite = redLifesBlur[0];
			}
			UE_HP_bgBlur.sprite = redBGBlur;
			UE_HP_bar.material.SetFloat("_ScrollSpeed", 0f - redSpeed);
		}
	}

	public void OnContaChanged(long curr, long maxContaVal)
	{
		if (oxyGauge != null && oxyGauge.gameObject.activeSelf)
		{
			oxyGauge.Refresh(curr, maxContaVal);
		}
		if (contaScreenEffect != null)
		{
			contaScreenEffect.PlayContaScreenEffect((int)curr, (int)maxContaVal);
		}
	}

	public void OnStaminaChanged(long curr, long maxVal)
	{
		if (staminaGauge != null)
		{
			staminaGauge.Refresh(curr, maxVal);
		}
	}

	public void OnKillCountChanged(int killCount)
	{
		UE_KillCount.SetText(Hub.GetL10NText("UI_BATTLE_KILL_COUNT", killCount.ToString()));
	}

	public void SetVersionText()
	{
		UE_versionText.rectTransform.anchoredPosition = new Vector2(0f, -10000f);
		UE_versionText.gameObject.SetActive(value: false);
	}

	public void SetVisibleStaminaGauge(bool visible)
	{
		if (!(staminaGauge == null))
		{
			if (visible)
			{
				staminaGauge.Show();
			}
			else
			{
				staminaGauge.Hide();
			}
		}
	}

	public void SetVisibleKillCount(bool visible)
	{
		if (!(UE_KillCount == null))
		{
			if (visible)
			{
				UE_KillCount.gameObject.SetActive(value: true);
			}
			else
			{
				UE_KillCount.gameObject.SetActive(value: false);
			}
		}
	}
}
