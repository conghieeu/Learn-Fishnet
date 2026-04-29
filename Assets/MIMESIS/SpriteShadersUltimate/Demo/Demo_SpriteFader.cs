using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_SpriteFader : MonoBehaviour
	{
		private SpriteRenderer sr;

		private bool fadeIn;

		private float fadeDelay;

		private void Awake()
		{
			sr = GetComponent<SpriteRenderer>();
			fadeIn = true;
			fadeDelay = 0f;
		}

		private void Update()
		{
			if (fadeDelay > 0f)
			{
				fadeDelay -= Time.unscaledDeltaTime;
				return;
			}
			Color color = sr.color;
			color.a = Mathf.Clamp01(Mathf.Lerp(color.a, fadeIn ? 1.1f : (-0.1f), Time.deltaTime * (fadeIn ? 8f : 4f)));
			sr.color = color;
			if ((fadeIn && color.a >= 1f) || (!fadeIn && color.a <= 0f))
			{
				base.enabled = false;
			}
		}

		public void SetFade(bool fadeState)
		{
			fadeIn = fadeState;
			base.enabled = true;
			if (!fadeIn)
			{
				fadeDelay = 0.15f;
			}
			sr.sortingOrder = (fadeIn ? 4 : 5);
		}
	}
}
