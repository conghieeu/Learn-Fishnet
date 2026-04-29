using UnityEngine;

namespace SpriteShadersUltimate
{
	[AddComponentMenu("Sprite Shaders Ultimate/Wind/Wind Parallax")]
	public class WindParallaxSSU : MonoBehaviour
	{
		private float originalXPosition;

		private void Awake()
		{
			originalXPosition = base.transform.position.x;
		}

		private void Start()
		{
			GetComponent<Renderer>().material.SetFloat("_WindXPosition", originalXPosition);
		}
	}
}
