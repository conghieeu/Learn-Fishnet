using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_Shaders : MonoBehaviour
	{
		public static Demo_Shaders instance;

		public static float zoomFactor;

		private GameObject environmentGO;

		private List<SpriteRenderer> environmentSprites;

		private Vector3 currentPosition;

		private float lastZoomFactor;

		private void Awake()
		{
			instance = this;
			Transform transform = GameObject.Find("Environment").transform;
			environmentSprites = new List<SpriteRenderer>();
			SpriteRenderer[] componentsInChildren = transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer item in componentsInChildren)
			{
				environmentSprites.Add(item);
			}
			environmentGO = transform.gameObject;
			Demo_Display.selected = null;
			currentPosition = Vector3.zero;
			zoomFactor = 0f;
			lastZoomFactor = -1000f;
		}

		private void Update()
		{
			if (Demo_Display.selected != null)
			{
				zoomFactor += Time.unscaledDeltaTime * 2f;
				if (zoomFactor > 1f)
				{
					zoomFactor = 1f;
				}
			}
			else
			{
				zoomFactor -= Time.unscaledDeltaTime * 2f;
				if (zoomFactor < 0f)
				{
					zoomFactor = 0f;
				}
			}
			float num = 1f + 6.2f * zoomFactor;
			base.transform.localScale = new Vector3(num, num, 1f);
			if (zoomFactor != lastZoomFactor)
			{
				float num2 = Mathf.Clamp01((zoomFactor - 0.75f) / 0.25f);
				foreach (SpriteRenderer environmentSprite in environmentSprites)
				{
					Color color = environmentSprite.color;
					color.a = num2;
					environmentSprite.color = color;
				}
				if (num2 > 0f)
				{
					if (!environmentGO.activeSelf)
					{
						environmentGO.SetActive(value: true);
					}
				}
				else if (environmentGO.activeSelf)
				{
					environmentGO.SetActive(value: false);
				}
				lastZoomFactor = zoomFactor;
			}
			if (Demo_Display.selected != null)
			{
				currentPosition = Vector3.Lerp(currentPosition, -Demo_Display.selected.transform.localPosition, Time.unscaledDeltaTime * 10f);
			}
			else
			{
				float num3 = 0f;
				if (AllowMovement())
				{
					num3 = 2f * ((float)Screen.width * 0.5f - Input.mousePosition.x) / (float)Screen.width;
					if (Mathf.Abs(num3) < 0.6f)
					{
						num3 = 0f;
					}
					else if (Input.mousePosition.x < (float)Screen.width && Input.mousePosition.x > 0f)
					{
						num3 += ((num3 < 0f) ? 0.6f : (-0.6f));
						num3 *= 2f;
						num3 = Mathf.Clamp(num3, -1f, 1f);
					}
					else
					{
						num3 = 0f;
					}
				}
				currentPosition = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x + num3, 0f, 0f), Time.unscaledDeltaTime * 14f / num);
			}
			base.transform.position = currentPosition * num;
			if (Demo_Display.selected != null && Input.GetKeyDown(KeyCode.Escape))
			{
				Demo_Display.selected.Deselect();
			}
		}

		public bool AllowMovement()
		{
			return zoomFactor < 0.1f;
		}

		public bool FadeInGUI()
		{
			return zoomFactor > 0.9f;
		}
	}
}
