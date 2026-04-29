using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_Player : MonoBehaviour
	{
		public static Demo_Player instance;

		[Header("Sprites:")]
		public List<Sprite> idleSprites;

		public List<Sprite> runningSprites;

		public List<Sprite> hurtSprites;

		[Header("Other:")]
		public bool ignoreMaterials;

		private SpriteRenderer spriteRenderer;

		private Rigidbody2D rig;

		private float nextFrame;

		private int currentIndex;

		private List<Sprite> currentAnimation;

		private Material originalMaterial;

		private Material currentMaterial;

		private Vector3 snapPosition;

		private bool isShadow;

		private void Start()
		{
			instance = this;
			spriteRenderer = base.transform.Find("SpriteRenderer").GetComponent<SpriteRenderer>();
			rig = GetComponent<Rigidbody2D>();
			if (!ignoreMaterials)
			{
				currentMaterial = (originalMaterial = spriteRenderer.material);
			}
			nextFrame = -1f;
			PlayAnimation(idleSprites);
		}

		private void Update()
		{
			if (Time.time > nextFrame)
			{
				spriteRenderer.sprite = currentAnimation[currentIndex];
				if (currentAnimation == runningSprites)
				{
					nextFrame = Time.time + 0.2f / Mathf.Max(Mathf.Abs(rig.linearVelocity.x), 3.5f);
				}
				else
				{
					nextFrame = Time.time + 0.065f;
				}
				currentIndex++;
				if (currentIndex >= currentAnimation.Count)
				{
					currentIndex = 0;
					if (currentAnimation == hurtSprites)
					{
						currentAnimation = idleSprites;
					}
				}
			}
			if (snapPosition != Vector3.zero)
			{
				rig.linearVelocity = Vector2.Lerp(rig.linearVelocity, new Vector2(0f, -1f), Time.deltaTime * 5f);
				if (Mathf.Abs(rig.linearVelocity.x) < 1f && currentAnimation != hurtSprites)
				{
					PlayAnimation(idleSprites);
				}
				base.transform.position = Vector3.Lerp(base.transform.position, snapPosition, Time.deltaTime * 6f);
			}
			else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && currentAnimation != hurtSprites)
			{
				rig.linearVelocity = Vector2.Lerp(rig.linearVelocity, new Vector2(7f, -1f), Time.deltaTime * 5f);
				base.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				PlayAnimation(runningSprites);
			}
			else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && currentAnimation != hurtSprites)
			{
				rig.linearVelocity = Vector2.Lerp(rig.linearVelocity, new Vector2(-7f, -1f), Time.deltaTime * 5f);
				base.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				PlayAnimation(runningSprites);
			}
			else
			{
				rig.linearVelocity = Vector2.Lerp(rig.linearVelocity, new Vector2(0f, -1f), Time.deltaTime * 5f);
				if (Mathf.Abs(rig.linearVelocity.x) < 1f && currentAnimation != hurtSprites)
				{
					PlayAnimation(idleSprites);
				}
			}
			if (isShadow)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				spriteRenderer.GetPropertyBlock(materialPropertyBlock);
				Vector2 vector = currentMaterial.GetVector("_ShadowOffset");
				if (base.transform.eulerAngles.y > 90f)
				{
					vector.x = 0f - vector.x;
				}
				materialPropertyBlock.SetVector("_ShadowOffset", vector);
				spriteRenderer.SetPropertyBlock(materialPropertyBlock);
			}
		}

		public void GetHurt(Vector2 velocity)
		{
			rig.linearVelocity = velocity;
			base.transform.eulerAngles = new Vector3(0f, (velocity.x > 0f) ? 180 : 0, 0f);
			PlayAnimation(hurtSprites);
		}

		private void PlayAnimation(List<Sprite> animation)
		{
			if (currentAnimation != animation)
			{
				currentAnimation = animation;
				currentIndex = 0;
			}
		}

		public void ApplyMaterial(Material material)
		{
			spriteRenderer.material = (currentMaterial = material);
			isShadow = material.name.StartsWith("SSU_Demo_Shadow");
		}

		public void SnapPosition(Vector3 newPosition)
		{
			snapPosition = newPosition;
		}

		public void ResetPosition()
		{
			base.transform.position = new Vector3(6f, -2.645f, 0f);
			base.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		}

		public void ResetMaterial()
		{
			spriteRenderer.material = originalMaterial;
			currentMaterial = originalMaterial;
			isShadow = false;
		}
	}
}
