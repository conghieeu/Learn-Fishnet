using System;
using System.Collections.Generic;
using Mimic.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_GameTips : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_GameTips = "GameTips";

	public const string UEID_GameTip1 = "GameTip1";

	private Image _UE_rootNode;

	private TMP_Text _UE_GameTips;

	private Transform _UE_GameTip1;

	private string temp_l10nKey_itemMasterInfoName;

	[SerializeField]
	private List<Transform> gameTipList = new List<Transform>();

	private string currentTipTextL10nKeys;

	private InputManager.OnLastInputDeviceChanged onInputDeviceChangedHandler;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_GameTips => _UE_GameTips ?? (_UE_GameTips = PickText("GameTips"));

	public Transform UE_GameTip1 => _UE_GameTip1 ?? (_UE_GameTip1 = PickTransform("GameTip1"));

	public void SetTips(string tipTxt)
	{
		OffTips();
		string[] array = tipTxt.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			SetTip(gameTipList[i], array[i]);
			gameTipList[i].gameObject.SetActive(value: true);
		}
	}

	public void SetTips_v2(string l10nKey_itemMasterInfoName)
	{
		temp_l10nKey_itemMasterInfoName = l10nKey_itemMasterInfoName;
		if (l10nKey_itemMasterInfoName == "" || l10nKey_itemMasterInfoName == null || l10nKey_itemMasterInfoName == string.Empty)
		{
			OffTips();
			return;
		}
		gameTipList.ForEach(delegate(Transform tip)
		{
			tip.gameObject.SetActive(value: false);
		});
		currentTipTextL10nKeys = l10nKey_itemMasterInfoName;
		string[] array = l10nKey_itemMasterInfoName.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int num = 0; num < array.Length; num++)
		{
			string[] array2 = array[num].Split(',');
			string key = ((array2.Length != 0) ? array2[0] : "");
			string txt = string.Format(arg0: Hub.GetL10NText((array2.Length > 1) ? array2[1] : "") + "\n", format: Hub.GetL10NText(key));
			SetTip(gameTipList[num], txt);
			gameTipList[num].gameObject.SetActive(value: true);
		}
	}

	private void OnChangedLanguage()
	{
		if (currentTipTextL10nKeys != "")
		{
			SetTips_v2(currentTipTextL10nKeys);
		}
	}

	public void OffTips()
	{
		currentTipTextL10nKeys = "";
		for (int i = 0; i < gameTipList.Count; i++)
		{
			Transform transform = gameTipList[i];
			if (!(transform == null))
			{
				transform.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetTip(Transform tip, string txt)
	{
		if (txt.Contains("[key:") && txt.Contains("]"))
		{
			int num = txt.IndexOf("[key:") + 4;
			string text = ((num - 4 > 0) ? txt.Substring(0, num - 4).Trim() : txt);
			tip.GetChild(0).GetComponent<TMP_Text>().text = text;
			int num2 = txt.IndexOf(']');
			if (num >= 0 && num2 > num)
			{
				string value = txt.Substring(num + 1, num2 - num - 1).Trim();
				InputAction action = (InputAction)Enum.Parse(typeof(InputAction), value);
				Sprite keyImage = Hub.s.gameSettingManager.keyImageData.GetKeyImage(action);
				if (keyImage != null)
				{
					Sprite sprite = keyImage;
					float width = sprite.rect.width;
					float height = sprite.rect.height;
					Vector2 sizeDelta = new Vector2(width * 0.7f, height * 0.7f);
					tip.GetChild(1).GetComponent<Image>().sprite = sprite;
					tip.GetChild(1).GetComponent<RectTransform>().sizeDelta = sizeDelta;
					tip.GetChild(1).GetComponent<Image>().gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			tip.GetChild(0).GetComponent<TMP_Text>().text = txt;
			tip.GetChild(1).GetComponent<Image>().gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged += OnChangedLanguage;
			onInputDeviceChangedHandler = delegate
			{
				SetTips_v2(temp_l10nKey_itemMasterInfoName);
			};
			Hub.s.inputman.onLastInputDeviceChanged += onInputDeviceChangedHandler;
		}
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= OnChangedLanguage;
			if (onInputDeviceChangedHandler != null)
			{
				Hub.s.inputman.onLastInputDeviceChanged -= onInputDeviceChangedHandler;
				onInputDeviceChangedHandler = null;
			}
		}
		OffTips();
	}
}
