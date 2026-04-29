using TMPro;
using UnityEngine;

public class WhiteBoard : SocketAttachable
{
	[SerializeField]
	private string stringKey = "greeting";

	[SerializeField]
	private TextMeshPro textMesh;

	private void Start()
	{
		SetupWhiteBoardText();
	}

	private void OnEnable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged += OnChangedLanguage;
		}
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= OnChangedLanguage;
		}
	}

	private void OnChangedLanguage()
	{
		SetupWhiteBoardText();
	}

	private void SetupWhiteBoardText()
	{
		if (!(textMesh == null))
		{
			string l10NText = Hub.GetL10NText(stringKey);
			textMesh.text = l10NText;
		}
	}
}
