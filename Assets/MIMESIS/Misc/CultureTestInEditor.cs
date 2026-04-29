using System;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class CultureTestInEditor : MonoBehaviour
{
	[Header("에디터 테스트용")]
	[SerializeField]
	private string[] testCultures = new string[4] { "en-US", "pt-PT", "pt-BR", "de-DE" };

	[SerializeField]
	private string[] testFloatStrings = new string[3] { "123.45", "0.5", "999.99" };

	[SerializeField]
	private int selectedCultureIndex;

	[Header("현재 상태")]
	[SerializeField]
	private string currentCulture;

	[SerializeField]
	private string decimalSeparator;

	[SerializeField]
	private string groupSeparator;

	private void Start()
	{
		UpdateCultureInfo();
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			ApplyCulture();
		}
	}

	[ContextMenu("포르투갈어로 변경")]
	private void SetPortuguese()
	{
		selectedCultureIndex = 1;
		ApplyCulture();
	}

	[ContextMenu("영어로 변경")]
	private void SetEnglish()
	{
		selectedCultureIndex = 0;
		ApplyCulture();
	}

	[ContextMenu("테스트 실행")]
	private void RunTest()
	{
		ApplyCulture();
		TestFloatParsing();
	}

	private void ApplyCulture()
	{
		if (selectedCultureIndex >= 0 && selectedCultureIndex < testCultures.Length)
		{
			CultureInfo cultureInfo = new CultureInfo(testCultures[selectedCultureIndex]);
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
			UpdateCultureInfo();
			Debug.Log("문화권 변경됨: " + cultureInfo.Name);
		}
	}

	private void UpdateCultureInfo()
	{
		currentCulture = CultureInfo.CurrentCulture.Name;
		decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
		groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
	}

	private void TestFloatParsing()
	{
		Debug.Log("=== 파싱 테스트 시작 ===");
		Debug.Log("현재 문화권: " + currentCulture);
		Debug.Log("소수점: '" + decimalSeparator + "' | 천의자리: '" + groupSeparator + "'");
		string[] array = testFloatStrings;
		foreach (string text in array)
		{
			Debug.Log("\n--- 테스트: '" + text + "' ---");
			try
			{
				float num = float.Parse(text);
				Debug.Log($"float.Parse(): {num}");
			}
			catch (Exception ex)
			{
				Debug.LogError("float.Parse() 실패: " + ex.Message);
			}
			try
			{
				float num2 = float.Parse(text, CultureInfo.InvariantCulture);
				Debug.Log($"InvariantCulture: {num2}");
			}
			catch (Exception ex2)
			{
				Debug.LogError("InvariantCulture 실패: " + ex2.Message);
			}
		}
	}
}
