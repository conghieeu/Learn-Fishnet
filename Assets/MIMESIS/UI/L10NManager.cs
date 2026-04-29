using System;
using System.Collections;
using Bifrost.LocalizationData;
using Steamworks;
using UnityEngine;

public class L10NManager : MonoBehaviour
{
	public enum Language
	{
		en = 0,
		ko = 1,
		es = 2,
		ja = 3,
		fr = 4,
		de = 5,
		zh_cn = 6,
		zh_tw = 7,
		pt_br = 8,
		it = 9,
		ru = 10,
		uk = 11,
		vi = 12,
		th = 13,
		tr = 14,
		pl = 15
	}

	public delegate void OnLanguageChanged();

	public string language { get; private set; } = "ko";

	public event OnLanguageChanged onLanguageChanged;

	public void Start()
	{
		StartCoroutine(GetSteamUILanCoroutine());
	}

	private IEnumerator GetSteamUILanCoroutine()
	{
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		string steamUILan = GetSteamUILan();
		language = PlayerPrefs.GetString("language", steamUILan);
		ChangeLanguage(language);
		this.onLanguageChanged?.Invoke();
	}

	private string GetSteamUILan()
	{
		return SteamUtils.GetSteamUILanguage() switch
		{
			"english" => "en", 
			"koreana" => "ko", 
			"spanish" => "es", 
			"japanese" => "fr", 
			"french" => "es", 
			"german" => "de", 
			"schinese" => "zh_cn", 
			"tchinese" => "zh_tw", 
			"brazilian" => "pt_br", 
			"italian" => "it", 
			"ukrainian" => "uk", 
			"vietnamese" => "vi", 
			"thai" => "th", 
			"turkish" => "tr", 
			"polish" => "pl", 
			_ => "en", 
		};
	}

	public void ChangeLanguage(string newLanguage)
	{
		if (Enum.IsDefined(typeof(Language), newLanguage))
		{
			language = newLanguage;
			this.onLanguageChanged?.Invoke();
			PlayerPrefs.SetString("language", language);
		}
		else
		{
			Logger.RError("Invalid language: " + newLanguage + ". Valid languages: " + string.Join(", ", Enum.GetNames(typeof(Language))));
		}
	}

	public string GetText(string key, params object[] formattingArgs)
	{
		if (Hub.s == null || Hub.s.dataman == null || Hub.s.dataman.ExcelDataManager == null)
		{
			return "|L10N error|";
		}
		if (formattingArgs != null && formattingArgs.Length != 0)
		{
			return string.Format(GetText(key), formattingArgs);
		}
		LocalizationData_MasterData localizationData = Hub.s.dataman.ExcelDataManager.GetLocalizationData(key);
		if (localizationData == null)
		{
			return "|L10N error: " + key + "(" + language + ")|";
		}
		return language switch
		{
			"ko" => localizationData.ko, 
			"en" => localizationData.en, 
			"es" => localizationData.es, 
			"ja" => localizationData.ja, 
			"fr" => localizationData.fr, 
			"de" => localizationData.de, 
			"zh_cn" => localizationData.zh_cn, 
			"zh_tw" => localizationData.zh_tw, 
			"pt_br" => localizationData.pt_br, 
			"it" => localizationData.it, 
			"ru" => localizationData.ru, 
			"uk" => localizationData.uk, 
			"vi" => localizationData.vi, 
			"th" => localizationData.th, 
			"tr" => localizationData.tr, 
			"pl" => localizationData.pl, 
			_ => "|L10N error: " + key + "(" + language + ")|", 
		};
	}
}
