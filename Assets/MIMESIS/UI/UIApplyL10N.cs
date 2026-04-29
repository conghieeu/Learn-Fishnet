using TMPro;
using UnityEngine;

public class UIApplyL10N : MonoBehaviour
{
	public enum L10NType
	{
		Text = 0,
		Image = 1
	}

	public string L10nkey;

	public L10NType l10NType;

	private bool initailized;

	private void Awake()
	{
		initailized = false;
		InitText();
	}

	private void InitText()
	{
		if (Hub.s != null && Hub.s.lcman != null)
		{
			ApplyL10N();
			Hub.s.lcman.onLanguageChanged += ApplyL10N;
			initailized = true;
		}
	}

	private void Update()
	{
		if (!initailized)
		{
			InitText();
		}
	}

	private void OnDestroy()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= ApplyL10N;
		}
	}

	public void ApplyL10N()
	{
		TMP_Text component = GetComponent<TMP_Text>();
		if (component == null)
		{
			Logger.RError("UIApplyL10N must be attached to a GameObject with a TMP_Text component. : " + L10nkey);
		}
		else
		{
			component.text = Hub.GetL10NText(L10nkey);
		}
	}
}
