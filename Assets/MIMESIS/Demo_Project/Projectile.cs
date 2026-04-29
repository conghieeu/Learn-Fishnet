using UnityEngine;

namespace Demo_Project
{
	public class Projectile : MonoBehaviour
	{
		public GameObject impactObject;

		public GameObject muzzleFlashObject;

		public GameObject chargingObject;

		public bool isChargeable;

		public bool rotateSprite = true;

		public bool muzzleFlash = true;

		public Color muzzleFlashColor = Color.white;

		public Color chargeColor = Color.white;

		public bool explodeAtScreenEdge = true;

		public float moveAngle;

		public float spriteAngle;

		public float moveSpeed = 5f;

		public float angleRandomness = 5f;

		private float xOffset;

		public Vector2 bulletOriginPoint = new Vector2(0.36f, 0f);

		public Vector2 muzzleFlashOriginPoint = new Vector2(0f, 0f);

		public Vector2 chargeOriginPoint = new Vector2(0f, 0f);

		private bool rotateClockwise;

		public float rotationSpeed;

		public float rotationRange;

		private float timeSinceLastFrame;

		private void Start()
		{
			SceneManager.listOfBullets.Add(base.gameObject);
			xOffset = base.transform.position.x;
			if (rotateSprite)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, spriteAngle * 57.29578f);
			}
		}

		private void CheckIfOffScreen()
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			if (vector.y < 0f || vector.x > (float)Screen.width || vector.y > (float)Screen.height)
			{
				if (explodeAtScreenEdge)
				{
					Impact();
				}
				else
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		private void Move()
		{
			float num = moveSpeed * timeSinceLastFrame / 100f;
			base.transform.Translate(new Vector3(Mathf.Cos(moveAngle) * num, Mathf.Sin(moveAngle) * num, 0f), Space.World);
		}

		private void Impact()
		{
			GameObject item = Object.Instantiate(impactObject, base.transform.position, base.transform.rotation);
			SceneManager.listOfImpacts.Add(item);
			for (int i = 0; i < SceneManager.listOfBullets.Count; i++)
			{
				if (SceneManager.listOfBullets[i] == base.gameObject)
				{
					SceneManager.listOfBullets.RemoveAt(i);
					break;
				}
			}
			Object.Destroy(base.gameObject);
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			for (int i = 0; i < SceneManager.listOfTargets.Count; i++)
			{
				if (col.gameObject == SceneManager.listOfTargets[i])
				{
					Impact();
				}
			}
			for (int j = 0; j < SceneManager.listOfFloors.Count; j++)
			{
				if (col.gameObject == SceneManager.listOfFloors[j])
				{
					Impact();
				}
			}
		}

		private void RotateProjectile()
		{
			if (!(rotationRange > 0f) || !(rotationSpeed > 0f))
			{
				return;
			}
			if (!rotateClockwise)
			{
				base.transform.Rotate(new Vector3(0f, 0f, rotationSpeed * timeSinceLastFrame));
				if (base.transform.rotation.z * 57.29578f >= rotationRange)
				{
					rotateClockwise = true;
				}
			}
			else
			{
				base.transform.Rotate(new Vector3(0f, 0f, (0f - rotationSpeed) * timeSinceLastFrame));
				if (base.transform.rotation.z * 57.29578f <= 0f - rotationRange)
				{
					rotateClockwise = false;
				}
			}
		}

		private void Update()
		{
			timeSinceLastFrame = Time.deltaTime / 0.001666f;
			Move();
			RotateProjectile();
			CheckIfOffScreen();
		}
	}
}
