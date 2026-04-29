using System;
using UnityEngine;

namespace Demo_Project
{
	public class Player : MonoBehaviour
	{
		private Transform transformComponent;

		private int rightClickWaitTime;

		public int rightClickWaitTimeLimit = 10;

		public float angleMax = 90f;

		public float angleMin = -70f;

		private int chargeTime;

		private int midChargeTime = 60;

		private int fullChargeLimit = 400;

		private Color muzzleFlashColor;

		private GameObject projectileObject;

		private GameObject burstObject;

		private GameObject loopObject;

		private GameObject chargeObject;

		private void Start()
		{
			SceneManager.listOfArms.Add(base.gameObject);
			transformComponent = GetComponent<Transform>();
		}

		private void SetArmRotation()
		{
			Quaternion rotation = default(Quaternion);
			float z = GetAngle() * 57.29578f;
			rotation.eulerAngles = new Vector3(0f, 0f, z);
			base.transform.rotation = rotation;
		}

		public float GetAngle()
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Mathf.Atan2(vector.y - base.transform.position.y, vector.x - base.transform.position.x) * 57.29578f > angleMax)
			{
				return MathF.PI / 180f * angleMax;
			}
			if (Mathf.Atan2(vector.y - base.transform.position.y, vector.x - base.transform.position.x) * 57.29578f < angleMin)
			{
				return MathF.PI / 180f * angleMin;
			}
			return Mathf.Atan2(vector.y - base.transform.position.y, vector.x - base.transform.position.x);
		}

		public Vector2 CircleAroundCenter(float centerX, float centerY, float dstX, float dstY)
		{
			return new Vector2
			{
				x = Mathf.Cos(GetAngle()) * (dstX - centerX) - Mathf.Sin(GetAngle()) * (dstY - centerY) + centerX,
				y = Mathf.Sin(GetAngle()) * (dstX - centerX) + Mathf.Cos(GetAngle()) * (dstY - centerY) + centerY
			};
		}

		public void CheckShot()
		{
			if (rightClickWaitTime < rightClickWaitTimeLimit)
			{
				rightClickWaitTime++;
			}
			for (int i = 0; i < SceneManager.listOfMenuObjects.Count; i++)
			{
				if (!(SceneManager.listOfMenuObjects[i].GetComponentInParent<SideMenu>().selectedItem == "projectile"))
				{
					continue;
				}
				projectileObject = SceneManager.listOfMenuObjects[i].GetComponent<SideMenu>().selectedItemObject;
				if ((Input.GetMouseButtonUp(0) && projectileObject.GetComponent<Projectile>().isChargeable) || (Input.GetMouseButtonDown(0) && !projectileObject.GetComponent<Projectile>().isChargeable))
				{
					ShootBullet();
					chargeTime = 0;
				}
				if (projectileObject.GetComponent<Projectile>().isChargeable && Input.GetMouseButton(0))
				{
					chargeTime++;
					if (chargeTime > 40 && chargeObject == null)
					{
						chargeObject = UnityEngine.Object.Instantiate(projectileObject.GetComponent<Projectile>().chargingObject, CircleAroundCenter(base.transform.position.x, base.transform.position.y, base.transform.position.x + projectileObject.GetComponent<Projectile>().chargeOriginPoint.x, base.transform.position.y + projectileObject.GetComponent<Projectile>().chargeOriginPoint.y), base.transform.rotation);
						if (chargeObject.transform.childCount > 0)
						{
							ParticleSystem.MainModule main = chargeObject.transform.GetChild(0).GetComponent<ParticleSystem>().main;
							main.startColor = projectileObject.GetComponent<Projectile>().chargeColor;
						}
					}
					ChargeParticleCheck();
				}
				if (Input.GetMouseButton(1) && rightClickWaitTime >= rightClickWaitTimeLimit)
				{
					ShootBullet();
					rightClickWaitTime = 0;
				}
			}
		}

		public void ChargeParticleCheck()
		{
			if (chargeTime == 40)
			{
				chargeObject = UnityEngine.Object.Instantiate(projectileObject.GetComponent<Projectile>().chargingObject, CircleAroundCenter(base.transform.position.x, base.transform.position.y, base.transform.position.x + projectileObject.GetComponent<Projectile>().chargeOriginPoint.x, base.transform.position.y + projectileObject.GetComponent<Projectile>().chargeOriginPoint.y), base.transform.rotation);
				if (chargeObject.transform.childCount > 0)
				{
					ParticleSystem.MainModule main = chargeObject.transform.GetChild(0).GetComponent<ParticleSystem>().main;
					main.startColor = projectileObject.GetComponent<Projectile>().chargeColor;
				}
			}
			if (chargeTime > 40)
			{
				chargeObject.transform.position = CircleAroundCenter(base.transform.position.x, base.transform.position.y, base.transform.position.x + projectileObject.GetComponent<Projectile>().chargeOriginPoint.x, base.transform.position.y + projectileObject.GetComponent<Projectile>().chargeOriginPoint.y);
			}
		}

		public void CheckBurst()
		{
			for (int i = 0; i < SceneManager.listOfMenuObjects.Count; i++)
			{
				if (SceneManager.listOfMenuObjects[i].GetComponentInParent<SideMenu>().selectedItem == "burst")
				{
					Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					if ((!SceneManager.listOfMenuObjects[i].GetComponentInParent<SideMenu>().isMenuOpen || (SceneManager.listOfMenuObjects[i].GetComponentInParent<SideMenu>().isMenuOpen && (double)vector.y < 4.3 && (double)vector.x < -7.2) || (SceneManager.listOfMenuObjects[i].GetComponentInParent<SideMenu>().isMenuOpen && (double)vector.x > -7.2)) && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
					{
						PlayBurst();
					}
				}
			}
		}

		public void PlayBurst()
		{
			for (int i = 0; i < SceneManager.listOfMenuObjects.Count; i++)
			{
				if (burstObject != null)
				{
					UnityEngine.Object.Destroy(burstObject);
				}
				burstObject = UnityEngine.Object.Instantiate(SceneManager.listOfMenuObjects[i].GetComponentInParent<SideMenu>().selectedItemObject, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
				burstObject.gameObject.SetActive(value: true);
			}
		}

		public void BurstManualRemoval()
		{
			if (burstObject != null)
			{
				UnityEngine.Object.Destroy(burstObject);
			}
		}

		public void CheckLoop()
		{
			for (int i = 0; i < SceneManager.listOfMenuObjects.Count; i++)
			{
				if (loopObject != null && loopObject.transform.childCount > 0)
				{
					if (SceneManager.listOfMenuObjects[i].GetComponentInParent<SideMenu>().selectedItem == "loop")
					{
						loopObject.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().renderMode = ParticleSystemRenderMode.Billboard;
						loopObject.transform.position = loopObject.GetComponent<Loop>().loopPosition;
					}
					else
					{
						UnityEngine.Object.Destroy(loopObject);
					}
				}
			}
		}

		public void ShootBullet()
		{
			Vector2 vector = CircleAroundCenter(base.transform.position.x, base.transform.position.y, base.transform.position.x + projectileObject.GetComponent<Projectile>().bulletOriginPoint.x, base.transform.position.y + projectileObject.GetComponent<Projectile>().bulletOriginPoint.y);
			if (projectileObject.GetComponent<Projectile>().rotateSprite)
			{
				GetAngle();
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(projectileObject, new Vector3(vector.x, vector.y, 1f), Quaternion.Euler(0f, 0f, 0f));
			gameObject.GetComponent<Projectile>().moveAngle = GetAngle() + MathF.PI / 180f * UnityEngine.Random.Range(0f - gameObject.GetComponent<Projectile>().angleRandomness, gameObject.GetComponent<Projectile>().angleRandomness);
			gameObject.GetComponent<Projectile>().spriteAngle = gameObject.GetComponent<Projectile>().moveAngle;
			if (projectileObject.GetComponent<Projectile>().isChargeable)
			{
				if (chargeTime >= fullChargeLimit)
				{
					float num = 1.5f;
					gameObject.transform.localScale = new Vector3(num, num, num);
					if (gameObject.transform.childCount > 0)
					{
						gameObject.transform.GetChild(0).transform.localScale = new Vector3(num, num, num);
					}
				}
				else if (chargeTime >= midChargeTime)
				{
					float num2 = 1.25f;
					gameObject.transform.localScale = new Vector3(num2, num2, num2);
					if (gameObject.transform.childCount > 0)
					{
						gameObject.transform.GetChild(0).transform.localScale = new Vector3(num2, num2, num2);
					}
				}
				RemoveChargeObject();
			}
			gameObject.SetActive(value: true);
			if (projectileObject.GetComponent<Projectile>().muzzleFlash)
			{
				Vector2 vector2 = CircleAroundCenter(base.transform.position.x, base.transform.position.y, base.transform.position.x + projectileObject.GetComponent<Projectile>().muzzleFlashOriginPoint.x, base.transform.position.y + projectileObject.GetComponent<Projectile>().muzzleFlashOriginPoint.y);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject.GetComponent<Projectile>().muzzleFlashObject, new Vector3(vector2.x, vector2.y, base.transform.position.z), Quaternion.Euler(0f, 0f, 57.29578f * GetAngle()));
				ParticleSystem.MainModule main = gameObject2.transform.GetChild(0).GetComponent<ParticleSystem>().main;
				main.startColor = projectileObject.GetComponent<Projectile>().muzzleFlashColor;
				SceneManager.listOfMuzzleFlashes.Add(gameObject2);
			}
		}

		public void RemoveChargeObject()
		{
			if (chargeObject != null)
			{
				UnityEngine.Object.Destroy(chargeObject);
			}
		}

		public void SendLimitsToHead()
		{
			for (int i = 0; i < SceneManager.listOfHeads.Count; i++)
			{
				SceneManager.listOfHeads[i].GetComponent<Head>().SetAngles(angleMin, angleMax);
			}
		}

		public void ReceiveLoopInformation(GameObject tempLoopObject)
		{
			for (int num = SceneManager.listOfLoops.Count - 1; num >= 0; num--)
			{
				if (SceneManager.listOfLoops[num] == loopObject)
				{
					SceneManager.listOfLoops.RemoveAt(num);
					UnityEngine.Object.DestroyImmediate(loopObject);
					break;
				}
			}
			loopObject = UnityEngine.Object.Instantiate(tempLoopObject);
			loopObject.SetActive(value: true);
		}

		private void CheckMuzzleFlashAndImpact()
		{
			for (int num = SceneManager.listOfImpacts.Count - 1; num >= 0; num--)
			{
				if (SceneManager.listOfImpacts[num].GetComponent<ParticleSystem>().time >= SceneManager.listOfImpacts[num].GetComponent<ParticleSystem>().main.duration)
				{
					UnityEngine.Object.Destroy(SceneManager.listOfImpacts[num]);
					SceneManager.listOfImpacts.RemoveAt(num);
				}
			}
			for (int num2 = SceneManager.listOfMuzzleFlashes.Count - 1; num2 >= 0; num2--)
			{
				if (SceneManager.listOfMuzzleFlashes[num2].GetComponent<ParticleSystem>().time >= SceneManager.listOfMuzzleFlashes[num2].GetComponent<ParticleSystem>().main.duration)
				{
					UnityEngine.Object.Destroy(SceneManager.listOfMuzzleFlashes[num2]);
					SceneManager.listOfMuzzleFlashes.RemoveAt(num2);
				}
			}
		}

		private void Update()
		{
			SendLimitsToHead();
			SetArmRotation();
			CheckShot();
			CheckBurst();
			CheckLoop();
			CheckMuzzleFlashAndImpact();
		}
	}
}
