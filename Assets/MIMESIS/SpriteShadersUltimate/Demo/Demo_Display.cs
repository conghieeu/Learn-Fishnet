using System.Collections;
using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_Display : MonoBehaviour
	{
		public static Demo_Display selected;

		[Header("Material Settings:")]
		public string firstProperty = "";

		public bool ignorePlayer;

		public float cycleTime = 2f;

		private Transform shader;

		private Material frameMaterial;

		private bool isHovered;

		private float lastScale;

		private float activeUntil;

		private int mainIndex;

		private Renderer mainRenderer;

		private Material mainMaterial;

		private Transform extraSprites;

		private int row;

		private int slot;

		private int maxSlots;

		private float camWidth;

		private void Start()
		{
			frameMaterial = base.transform.Find("Display/Frame").GetComponent<SpriteRenderer>().material;
			shader = base.transform.Find("Shader");
			isHovered = false;
			lastScale = 0f;
			activeUntil = Time.unscaledTime + 2f;
			int siblingIndex = base.transform.GetSiblingIndex();
			row = siblingIndex % 3 - 1;
			slot = siblingIndex / 3;
			int childCount = base.transform.parent.childCount;
			maxSlots = childCount / 3 + ((row + 1 < childCount % 3) ? 1 : 0);
			camWidth = (float)Screen.width / (float)Screen.height * Camera.main.orthographicSize;
			UpdatePosition();
			UpdatePosition();
			mainIndex = 0;
			UpdateIndex();
			extraSprites = shader.Find("Extra Sprites");
			if (extraSprites != null)
			{
				extraSprites.localPosition = new Vector3(4f, 0f, 0f);
			}
			if (firstProperty == null || firstProperty == "")
			{
				firstProperty = "_" + base.gameObject.name.Replace(" ", "") + "Fade";
			}
			Demo_GUI.instance.CreateTitle(base.gameObject.name, base.transform.Find("Display/Title Position"));
			StartCoroutine(CycleShader());
			StartCoroutine(HandlePosition());
		}

		private void Update()
		{
			if (Time.unscaledTime > activeUntil)
			{
				return;
			}
			float x = base.transform.localScale.x;
			if (selected == this)
			{
				activeUntil = Time.unscaledTime + 4f;
				x = Mathf.Clamp(Mathf.Lerp(x, 1.11f, Time.unscaledDeltaTime * 5f), 1f, 1.1f);
				shader.localScale = Vector3.Lerp(shader.localScale, Vector3.one * 1f / 8f, Time.unscaledDeltaTime * 10f);
				shader.localPosition = Vector3.Lerp(shader.localPosition, new Vector3(-0.3f, 0f, 0f), Time.unscaledDeltaTime * 10f);
				if (extraSprites != null)
				{
					extraSprites.localPosition = Vector3.Lerp(extraSprites.localPosition, new Vector3(0f, 0f, 0f), Time.unscaledDeltaTime * 10f);
				}
			}
			else
			{
				x = Mathf.Clamp(Mathf.Lerp(x, (isHovered && selected == null) ? 1.11f : 0.99f, Time.unscaledDeltaTime * 5f), 1f, 1.1f);
				shader.localScale = Vector3.Lerp(shader.localScale, new Vector3(0.45f, 0.45f, 1f), Time.unscaledDeltaTime * 4f);
				shader.localPosition = Vector3.Lerp(shader.localPosition, new Vector3(0f, 0.45f, 0f), Time.unscaledDeltaTime * 4f);
				if (extraSprites != null)
				{
					extraSprites.localPosition = Vector3.Lerp(extraSprites.localPosition, new Vector3(2f, 0f, 0f), Time.unscaledDeltaTime * 10f);
				}
			}
			if (x != lastScale)
			{
				lastScale = x;
				base.transform.localScale = new Vector3(x, x, 1f);
				frameMaterial.SetFloat("_SineGlowFade", (x - 1f) * 10f);
			}
		}

		private IEnumerator CycleShader()
		{
			yield return new WaitForSeconds((float)base.transform.GetSiblingIndex() * 0.01f);
			while (true)
			{
				yield return new WaitForSeconds(cycleTime);
				if (selected != this)
				{
					ChangeIndex();
				}
			}
		}

		private IEnumerator HandlePosition()
		{
			yield return new WaitForSeconds((float)base.transform.GetSiblingIndex() * 0.01f);
			while (true)
			{
				yield return new WaitForSeconds(0.2f);
				UpdatePosition();
			}
		}

		private Transform GetMainSprite(int index)
		{
			if (index <= 0)
			{
				return shader.Find("Main Sprite");
			}
			return shader.Find("Main Sprite " + (index + 1));
		}

		public void ChangeIndex()
		{
			mainIndex++;
			if (GetMainSprite(mainIndex) == null)
			{
				mainIndex = 0;
			}
			UpdateIndex();
		}

		public bool HasAlternatives()
		{
			return shader.Find("Main Sprite 2") != null;
		}

		public void UpdateIndex()
		{
			for (int i = 0; i < 6; i++)
			{
				Transform mainSprite = GetMainSprite(i);
				if (mainSprite != null)
				{
					Demo_SpriteFader component = mainSprite.GetComponent<Demo_SpriteFader>();
					if (component != null)
					{
						component.SetFade(fadeState: false);
					}
					else
					{
						mainSprite.gameObject.SetActive(value: false);
					}
				}
			}
			Transform mainSprite2 = GetMainSprite(mainIndex);
			if (mainSprite2 != null)
			{
				Demo_SpriteFader component2 = mainSprite2.GetComponent<Demo_SpriteFader>();
				if (component2 != null)
				{
					component2.SetFade(fadeState: true);
				}
				else
				{
					mainSprite2.gameObject.SetActive(value: true);
				}
				mainRenderer = mainSprite2.GetComponent<Renderer>();
				mainMaterial = mainRenderer.material;
			}
		}

		private void OnMouseOver()
		{
			isHovered = true;
			activeUntil = Time.unscaledTime + 4f;
		}

		private void OnMouseExit()
		{
			isHovered = false;
			activeUntil = Time.unscaledTime + 4f;
		}

		private void OnMouseDown()
		{
			if (selected == null)
			{
				Select();
			}
		}

		public void Select()
		{
			mainIndex = 0;
			UpdateIndex();
			activeUntil = Time.unscaledTime + 4f;
			selected = this;
			Demo_Player.instance.ResetPosition();
			Demo_GUI.instance.UpdateHud();
		}

		public void Deselect()
		{
			activeUntil = Time.unscaledTime + 4f;
			selected = null;
			ResetMaterial();
		}

		public void ResetMaterial()
		{
			if (mainRenderer != null && mainRenderer.material != null && mainRenderer.material != mainMaterial)
			{
				Object.Destroy(mainRenderer.material);
				mainRenderer.material = mainMaterial;
			}
		}

		public Material InstantiateMaterial()
		{
			if (mainMaterial == null)
			{
				Demo_Player.instance.ResetMaterial();
				return null;
			}
			Material material = Object.Instantiate(mainMaterial);
			mainRenderer.material = material;
			if (material != null && !ignorePlayer)
			{
				Demo_Player.instance.ApplyMaterial(material);
			}
			else
			{
				Demo_Player.instance.ResetMaterial();
			}
			return material;
		}

		private void UpdatePosition()
		{
			float num = base.transform.position.x / Demo_Shaders.instance.transform.localScale.x;
			if (num < (0f - camWidth) * 1.4f)
			{
				slot += maxSlots;
			}
			else if (num > camWidth * 1.4f)
			{
				slot -= maxSlots;
			}
			base.transform.localPosition = new Vector3(2.75f * (float)slot, -3.25f * (float)row, 0f);
		}
	}
}
