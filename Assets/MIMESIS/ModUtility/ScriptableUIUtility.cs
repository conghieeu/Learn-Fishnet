using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModUtility
{
	public class ScriptableUIUtility : MonoBehaviour
	{
		private Dictionary<string, RectTransform> panels = new Dictionary<string, RectTransform>();

		[SerializeField]
		private GameObject prefab_panelTemplate;

		[SerializeField]
		private GameObject prefab_lanaTextTemplate;

		[SerializeField]
		private GameObject prefab_notoTextTemplate;

		public RectTransform CreatePanel(string key, int x, int y, int width, int height)
		{
			GameObject obj = Object.Instantiate(prefab_panelTemplate, base.transform);
			DestroyPanel(key);
			RectTransform component = obj.GetComponent<RectTransform>();
			panels.Add(key, component);
			component.offsetMin = new Vector2(x, y);
			component.offsetMax = new Vector2(x + width, y + height);
			return component;
		}

		public RectTransform GetPanel(string key)
		{
			if (panels.ContainsKey(key))
			{
				return panels[key];
			}
			return null;
		}

		public void DestroyPanel(string key)
		{
			if (panels.ContainsKey(key))
			{
				Object.Destroy(panels[key].gameObject);
				panels.Remove(key);
			}
		}

		public Image AttachPanel(RectTransform _parent, int x, int y, int width, int height)
		{
			if (_parent == null)
			{
				Debug.LogError("AttachPanel : parent is null!");
				return null;
			}
			GameObject obj = Object.Instantiate(prefab_panelTemplate, _parent);
			RectTransform component = obj.GetComponent<RectTransform>();
			component.offsetMin = new Vector2(x, y);
			component.offsetMax = new Vector2(x + width, y + height);
			return obj.GetComponent<Image>();
		}

		public TMP_Text AttachLanaText(RectTransform _parent, int x, int y, int width, int height, string text, int fontSize)
		{
			return AttachText(prefab_lanaTextTemplate, _parent, x, y, width, height, text, fontSize);
		}

		public TMP_Text AttachNotoText(RectTransform _parent, int x, int y, int width, int height, string text, int fontSize)
		{
			return AttachText(prefab_notoTextTemplate, _parent, x, y, width, height, text, fontSize);
		}

		private TMP_Text AttachText(GameObject template, RectTransform _parent, int x, int y, int width, int height, string text, int fontSize)
		{
			GameObject obj = Object.Instantiate(template, _parent);
			TMP_Text component = obj.GetComponent<TMP_Text>();
			component.fontSize = fontSize;
			component.text = text;
			RectTransform component2 = obj.GetComponent<RectTransform>();
			component2.offsetMin = new Vector2(x, y);
			component2.offsetMax = new Vector2(x + width, y + height);
			return component;
		}
	}
}
