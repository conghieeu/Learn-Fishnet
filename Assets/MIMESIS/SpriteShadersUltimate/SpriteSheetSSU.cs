using UnityEngine;
using UnityEngine.UI;

namespace SpriteShadersUltimate
{
	[AddComponentMenu("Sprite Shaders Ultimate/Utility/Sprite Sheet SSU")]
	public class SpriteSheetSSU : MonoBehaviour
	{
		public bool updateChanges;

		private SpriteRenderer spriteRenderer;

		private Image image;

		private Sprite lastSprite;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			image = GetComponent<Image>();
			if (image != null && GetComponent<InstancerSSU>() == null)
			{
				image.material = Object.Instantiate(image.material);
			}
		}

		private void Start()
		{
			UpdateSpriteRect();
			if (!updateChanges)
			{
				base.enabled = false;
			}
		}

		private void LateUpdate()
		{
			if ((spriteRenderer != null && lastSprite != spriteRenderer.sprite) || (image != null && lastSprite != image.sprite))
			{
				UpdateSpriteRect();
			}
		}

		public void UpdateSpriteRect()
		{
			if (spriteRenderer != null)
			{
				lastSprite = spriteRenderer.sprite;
			}
			else if (image != null)
			{
				lastSprite = image.sprite;
			}
			if (lastSprite != null)
			{
				if (spriteRenderer != null)
				{
					spriteRenderer.material.SetVector("_SpriteSheetRect", GetSheetVector(lastSprite));
				}
				else if (image != null)
				{
					image.materialForRendering.SetVector("_SpriteSheetRect", GetSheetVector(lastSprite));
				}
			}
		}

		public static Vector4 GetSheetVector(Sprite sprite)
		{
			Rect rect = default(Rect);
			try
			{
				rect = sprite.textureRect;
			}
			catch
			{
				rect = sprite.rect;
			}
			Texture2D texture = sprite.texture;
			float num = texture.width;
			float num2 = texture.height;
			Vector2 min = rect.min;
			Vector2 max = rect.max;
			return new Vector4(min.x / num, min.y / num2, max.x / num, max.y / num2);
		}
	}
}
