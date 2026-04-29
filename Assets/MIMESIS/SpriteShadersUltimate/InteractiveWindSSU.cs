using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadersUltimate
{
	[AddComponentMenu("Sprite Shaders Ultimate/Wind/Interactive Wind")]
	public class InteractiveWindSSU : InstancerSSU
	{
		[Tooltip("How much physical interaction bends the sprite.")]
		public float rotationFactor = 1.5f;

		[Tooltip("How fast physical interaction bending fades in.")]
		public float bendInSpeed = 8f;

		[Tooltip("How fast physical interaction bending fades out.")]
		public float bendOutSpeed = 8f;

		[Tooltip("If disabled the sprite will only bend during active movement.")]
		public bool stayBent = true;

		[Tooltip("The minimum speed of the interacting object to trigger bending.")]
		public float minBendSpeed = 1f;

		[Tooltip("Swaps the material with the default sprite material while inactive.")]
		public bool hyperPerformanceMode;

		[Tooltip("Adds a tiny little offset to the Z position on start.\nTo prevent random resorting of render order.")]
		public bool randomOffsetZ = true;

		[Tooltip("Define a material to switch to while inactive.")]
		public bool customMaterial;

		[Tooltip("The shader used for the default sprite material.")]
		public string inactiveShader = "Sprites/Default";

		[Tooltip("The material used when inactive.")]
		public Material inactiveMaterial;

		[Tooltip("Slightly changes 'Wiggle Frequency', to desync the wiggle shaders of multiple sprites.")]
		public bool randomizeWiggle;

		[Tooltip("The editor-side script set's the layer to 'Ignore Raycast' to fix potential issues. Enable this to disable that and set the layer to a different one.")]
		public bool allowCustomLayer;

		private HashSet<Collider2D> collidersInside;

		private BoxCollider2D boxCollider;

		private float currentBending;

		private float currentRotationDirection;

		private bool isActive;

		private bool newDirection;

		private float lastPosition;

		private float lastBend;

		private float currentBendTarget;

		private bool bentInLastFrame;

		private SpriteRenderer sr;

		private static Material defaultMaterial;

		private int rotationId;

		private void Start()
		{
			collidersInside = new HashSet<Collider2D>();
			boxCollider = GetComponent<BoxCollider2D>();
			sr = GetComponent<SpriteRenderer>();
			runtimeMaterial = sr.material;
			if (defaultMaterial == null)
			{
				if (customMaterial)
				{
					defaultMaterial = inactiveMaterial;
				}
				else
				{
					defaultMaterial = new Material(Shader.Find(inactiveShader));
				}
			}
			if (hyperPerformanceMode)
			{
				sr.material = defaultMaterial;
				if (randomOffsetZ)
				{
					Vector3 position = base.transform.position;
					position.z += Random.value * 0.1f;
					base.transform.position = position;
				}
			}
			if (randomizeWiggle && runtimeMaterial != null)
			{
				float value = runtimeMaterial.GetFloat("_WiggleFrequency") * (0.9f + 0.2f * Random.value);
				runtimeMaterial.SetFloat("_WiggleFrequency", value);
			}
			rotationId = Shader.PropertyToID("_WindRotation");
		}

		private void FixedUpdate()
		{
			if (!isActive)
			{
				return;
			}
			Vector2 vector = new Vector2(0f, -1000000f);
			foreach (Collider2D item in collidersInside)
			{
				if (!(item != null))
				{
					continue;
				}
				if (vector.y < -99999f)
				{
					vector = item.bounds.center - base.transform.position;
				}
				else if (!newDirection)
				{
					Vector2 vector2 = item.bounds.center - base.transform.position;
					if ((currentRotationDirection < 0f && vector2.x > vector.x) || (currentRotationDirection > 0f && vector2.x < vector.x))
					{
						vector = vector2;
					}
				}
				else
				{
					vector = ((Vector2)(item.bounds.center - base.transform.position) + vector) * 0.5f;
				}
			}
			if (vector.y > -99999f)
			{
				if (newDirection)
				{
					if (vector.x < 0f)
					{
						currentRotationDirection = -1f;
					}
					else
					{
						currentRotationDirection = 1f;
					}
					newDirection = false;
				}
				float num = 0f;
				num = ((!(currentRotationDirection < 0f)) ? Mathf.Clamp01((boxCollider.size.x * 0.5f - vector.x) / boxCollider.size.x) : Mathf.Clamp01((vector.x + boxCollider.size.x * 0.5f) / boxCollider.size.x));
				if (stayBent)
				{
					currentBendTarget = num;
				}
				else
				{
					bool flag = Mathf.Abs(lastPosition - vector.x) > Time.fixedDeltaTime * minBendSpeed;
					if (flag && lastBend != 0f && currentRotationDirection > 0f == vector.x - lastPosition > 0f)
					{
						flag = false;
					}
					if (flag || bentInLastFrame)
					{
						currentBendTarget = num;
						lastBend = num;
						bentInLastFrame = true;
						if (!flag)
						{
							bentInLastFrame = false;
						}
					}
					else
					{
						currentBendTarget = Mathf.Lerp(currentBendTarget, 0f, Time.fixedDeltaTime * bendInSpeed);
						if (Mathf.Abs(currentBending) < 0.01f)
						{
							newDirection = true;
						}
					}
					lastPosition = vector.x;
				}
				currentBending += (currentBendTarget * currentRotationDirection - currentBending) * Mathf.Min(bendInSpeed * Time.fixedDeltaTime, 1f);
				UpdateShader();
				return;
			}
			currentBending -= currentBending * Mathf.Min(bendOutSpeed * Time.fixedDeltaTime, 1f);
			UpdateShader();
			if (Mathf.Abs(currentBending) < 0.005f)
			{
				isActive = false;
				lastBend = 0f;
				if (hyperPerformanceMode)
				{
					sr.material = defaultMaterial;
				}
			}
		}

		public void UpdateShader()
		{
			runtimeMaterial.SetFloat(rotationId, -1f * currentBending * rotationFactor);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collidersInside.Count == 0 || Mathf.Abs(currentBending) < 0.2f)
			{
				newDirection = true;
			}
			collidersInside.Add(collision);
			if (hyperPerformanceMode && !isActive)
			{
				sr.material = runtimeMaterial;
			}
			isActive = true;
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collidersInside.Contains(collision))
			{
				collidersInside.Remove(collision);
			}
		}

		public static void DefaultCollider(BoxCollider2D box)
		{
			box.isTrigger = true;
			box.size = new Vector2(2f, 1f);
			box.offset = new Vector2(0f, -0.5f);
		}
	}
}
