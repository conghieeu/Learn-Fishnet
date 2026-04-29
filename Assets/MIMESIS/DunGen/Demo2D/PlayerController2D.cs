using DunGen.Demo;
using UnityEngine;

namespace DunGen.Demo2D
{
	internal sealed class PlayerController2D : MonoBehaviour
	{
		public float MovementSpeed = 6f;

		private CircleCollider2D collider;

		private RaycastHit2D[] hitBuffer;

		private DungeonGenerator dungeonGenerator;

		private void Start()
		{
			collider = GetComponent<CircleCollider2D>();
			hitBuffer = new RaycastHit2D[10];
			Generator generator = UnityUtil.FindObjectByType<Generator>();
			dungeonGenerator = generator.DungeonGenerator.Generator;
			dungeonGenerator.OnGenerationStatusChanged += OnGeneratorStatusChanged;
		}

		private void OnDestroy()
		{
			dungeonGenerator.OnGenerationStatusChanged -= OnGeneratorStatusChanged;
		}

		private void OnGeneratorStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			base.transform.position = Vector3.zero;
		}

		private void Update()
		{
			Vector2 vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			if (vector.sqrMagnitude > 1f)
			{
				vector.Normalize();
			}
			Vector3 vector2 = new Vector3(vector.x, vector.y, 0f);
			float num = MovementSpeed * Time.deltaTime;
			Vector3 vector3 = vector2 * num;
			if (collider.Cast(vector2, hitBuffer, num) > 0)
			{
				RaycastHit2D raycastHit2D = hitBuffer[0];
				vector3 = vector2 * raycastHit2D.distance;
			}
			base.transform.position += vector3;
		}
	}
}
