using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Mimic.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_Spectator : UIPrefabScript
{
	public const string UEID_SpectatedPlayerName = "SpectatedPlayerName";

	public const string UEID_ptevText = "ptevText";

	public const string UEID_prevImage = "prevImage";

	public const string UEID_prevImage2 = "prevImage2";

	public const string UEID_nextChangeTimeText = "nextChangeTimeText";

	public const string UEID_nextText = "nextText";

	public const string UEID_nextImage = "nextImage";

	public const string UEID_nextImage2 = "nextImage2";

	private TMP_Text _UE_SpectatedPlayerName;

	private TMP_Text _UE_ptevText;

	private Image _UE_prevImage;

	private Image _UE_prevImage2;

	private TMP_Text _UE_nextChangeTimeText;

	private TMP_Text _UE_nextText;

	private Image _UE_nextImage;

	private Image _UE_nextImage2;

	[SerializeField]
	private UIPrefab_Spectator_PlayerListView playerListView;

	private bool IsShow;

	[HideInInspector]
	public string tempPlayerName = "";

	public TMP_Text UE_SpectatedPlayerName => _UE_SpectatedPlayerName ?? (_UE_SpectatedPlayerName = PickText("SpectatedPlayerName"));

	public TMP_Text UE_ptevText => _UE_ptevText ?? (_UE_ptevText = PickText("ptevText"));

	public Image UE_prevImage => _UE_prevImage ?? (_UE_prevImage = PickImage("prevImage"));

	public Image UE_prevImage2 => _UE_prevImage2 ?? (_UE_prevImage2 = PickImage("prevImage2"));

	public TMP_Text UE_nextChangeTimeText => _UE_nextChangeTimeText ?? (_UE_nextChangeTimeText = PickText("nextChangeTimeText"));

	public TMP_Text UE_nextText => _UE_nextText ?? (_UE_nextText = PickText("nextText"));

	public Image UE_nextImage => _UE_nextImage ?? (_UE_nextImage = PickImage("nextImage"));

	public Image UE_nextImage2 => _UE_nextImage2 ?? (_UE_nextImage2 = PickImage("nextImage2"));

	private void OnEnable()
	{
		if (Hub.s != null)
		{
			Hub.s.inputman.onLastInputDeviceChanged += OnChangeLanguageSetKeyTip;
			Hub.s.lcman.onLanguageChanged += OnChangeLanguageSetKeyTip;
			Hub.s.lcman.onLanguageChanged += OnChangeLanguageSetSpectatedPlayerName;
		}
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.inputman.onLastInputDeviceChanged -= OnChangeLanguageSetKeyTip;
			Hub.s.lcman.onLanguageChanged -= OnChangeLanguageSetKeyTip;
			Hub.s.lcman.onLanguageChanged -= OnChangeLanguageSetSpectatedPlayerName;
		}
	}

	private void OnChangeLanguageSetSpectatedPlayerName()
	{
		SetSpectatedPlayerName(tempPlayerName);
	}

	public void SetSpectatedPlayerName(string playerName)
	{
		tempPlayerName = playerName;
		UE_SpectatedPlayerName.SetText(Hub.GetL10NText("SPECTATOR_CAMERA_VIEW", tempPlayerName));
	}

	public bool CheckActiveSpectatedPlayerName()
	{
		return UE_SpectatedPlayerName.IsActive();
	}

	public void SetActiveSpectatedPlayerName(bool isActive)
	{
		UE_SpectatedPlayerName.gameObject.SetActive(isActive);
	}

	public void SetKeyDescText(string[] prevArr, string[] nextArr)
	{
		SetKeysTip("SPECTATOR_CAMERA_PREV", "SPECTATOR_CAMERA_NEXT");
	}

	private void OnChangeLanguageSetKeyTip()
	{
		SetKeysTip("SPECTATOR_CAMERA_PREV", "SPECTATOR_CAMERA_NEXT");
	}

	private void SetKeysTip(string prevTextKey, string nextTextKey)
	{
		string l10NText = Hub.GetL10NText(prevTextKey);
		if (l10NText.Contains("[key:") && l10NText.Contains("]"))
		{
			int num = l10NText.IndexOf("[key:") + 4;
			string text = ((num - 4 > 0) ? l10NText.Substring(0, num - 4).Trim() : l10NText);
			UE_ptevText.SetText(text);
			int num2 = l10NText.IndexOf("]");
			if (num >= 0 && num2 > num)
			{
				string value = l10NText.Substring(num + 1, num2 - num - 1).Trim();
				InputAction action = (InputAction)Enum.Parse(typeof(InputAction), value);
				bool gamepad = Hub.s.inputman.lastInputDevice == InputDevice.Gamepad;
				string[] array = Hub.s.inputman.GetKeyBind(action, gamepad).Split(',');
				if (array.Length > 1)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = array[i].Trim();
					}
					Sprite keyImage = Hub.s.gameSettingManager.keyImageData.GetKeyImage(array[0]);
					if (keyImage != null)
					{
						float width = keyImage.rect.width;
						float height = keyImage.rect.height;
						UE_prevImage.sprite = keyImage;
						UE_prevImage.rectTransform.sizeDelta = new Vector2(width * 0.7f, height * 0.7f);
						UE_prevImage.gameObject.SetActive(value: true);
					}
					Sprite keyImage2 = Hub.s.gameSettingManager.keyImageData.GetKeyImage(array[1]);
					if (keyImage2 != null)
					{
						float width2 = keyImage2.rect.width;
						float height2 = keyImage2.rect.height;
						UE_prevImage2.sprite = keyImage2;
						UE_prevImage2.rectTransform.sizeDelta = new Vector2(width2 * 0.7f, height2 * 0.7f);
						UE_prevImage2.gameObject.SetActive(value: true);
					}
				}
				else
				{
					Sprite keyImage3 = Hub.s.gameSettingManager.keyImageData.GetKeyImage(action);
					if (keyImage3 != null)
					{
						UE_prevImage.sprite = keyImage3;
						UE_prevImage.gameObject.SetActive(value: true);
						UE_prevImage2.gameObject.SetActive(value: false);
					}
				}
			}
		}
		string l10NText2 = Hub.GetL10NText(nextTextKey);
		if (!l10NText2.Contains("[key:") || !l10NText2.Contains("]"))
		{
			return;
		}
		int num3 = l10NText2.IndexOf("[key:") + 4;
		string text2 = ((num3 - 4 > 0) ? l10NText2.Substring(0, num3 - 4).Trim() : l10NText2);
		UE_nextText.SetText(text2);
		int num4 = l10NText2.IndexOf("]");
		if (num3 < 0 || num4 <= num3)
		{
			return;
		}
		string value2 = l10NText2.Substring(num3 + 1, num4 - num3 - 1).Trim();
		InputAction action2 = (InputAction)Enum.Parse(typeof(InputAction), value2);
		bool gamepad2 = Hub.s.inputman.lastInputDevice == InputDevice.Gamepad;
		string[] array2 = Hub.s.inputman.GetKeyBind(action2, gamepad2).Split(',');
		if (array2.Length > 1)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = array2[j].Trim();
			}
			Sprite keyImage4 = Hub.s.gameSettingManager.keyImageData.GetKeyImage(array2[0]);
			if (keyImage4 != null)
			{
				float width3 = keyImage4.rect.width;
				float height3 = keyImage4.rect.height;
				UE_nextImage.sprite = keyImage4;
				UE_nextImage.rectTransform.sizeDelta = new Vector2(width3 * 0.7f, height3 * 0.7f);
				UE_nextImage.gameObject.SetActive(value: true);
			}
			Sprite keyImage5 = Hub.s.gameSettingManager.keyImageData.GetKeyImage(array2[1]);
			if (keyImage5 != null)
			{
				float width4 = keyImage5.rect.width;
				float height4 = keyImage5.rect.height;
				UE_nextImage2.sprite = keyImage5;
				UE_nextImage2.rectTransform.sizeDelta = new Vector2(width4 * 0.7f, height4 * 0.7f);
				UE_nextImage2.gameObject.SetActive(value: true);
			}
		}
		else
		{
			Sprite keyImage6 = Hub.s.gameSettingManager.keyImageData.GetKeyImage(action2);
			if (keyImage6 != null)
			{
				UE_nextImage.sprite = keyImage6;
				UE_nextImage.gameObject.SetActive(value: true);
				UE_nextImage2.gameObject.SetActive(value: false);
			}
		}
	}

	public new void Show()
	{
		base.Show();
		SetActiveSpectatedPlayerName(isActive: false);
		playerListView.Show();
		playerListView.gameObject.SetActive(value: true);
		UE_nextChangeTimeText.gameObject.SetActive(value: false);
		IsShow = true;
	}

	public new void Hide()
	{
		base.Hide();
		SetActiveSpectatedPlayerName(isActive: false);
		playerListView.Hide();
		UE_nextChangeTimeText.gameObject.SetActive(value: false);
		IsShow = false;
	}

	public void UpdatePlayerListView(List<Tuple<int, bool, bool>> actorsInfo, CancellationToken cancellationToken)
	{
		if (IsShow)
		{
			playerListView.UpdatePlayerListView(actorsInfo, cancellationToken);
		}
	}

	public void ShowNextChangeTime()
	{
		if (IsShow)
		{
			UE_nextChangeTimeText.gameObject.SetActive(value: true);
		}
	}

	public void UpdateNextChangeTime(float elapsedTime, float nextChangeTime)
	{
		if (IsShow)
		{
			if (elapsedTime < 0f || nextChangeTime < 0f)
			{
				HideNextChangeTime();
				return;
			}
			int value = (int)nextChangeTime - (int)elapsedTime;
			StringBuilder stringBuilder = new StringBuilder(10);
			stringBuilder.Append("( ");
			stringBuilder.Append(value);
			stringBuilder.Append(" )");
			UE_nextChangeTimeText.text = stringBuilder.ToString();
		}
	}

	public void HideNextChangeTime()
	{
		if (IsShow)
		{
			UE_nextChangeTimeText.gameObject.SetActive(value: false);
		}
	}
}
