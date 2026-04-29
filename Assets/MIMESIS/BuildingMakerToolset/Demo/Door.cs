using UnityEngine;

namespace BuildingMakerToolset.Demo
{
	public class Door : MonoBehaviour
	{
		private float triggerDist = 2.2f;

		private float openingSpeed = 1f;

		private bool triggered;

		private bool reseting;

		private Transform triggerer;

		private Quaternion initialRotation;

		private Vector3 initialRotationEuler;

		private float openValue;

		private bool approachFromFront;

		private void Start()
		{
			PlayerMovement playerMovement = Object.FindObjectOfType(typeof(PlayerMovement)) as PlayerMovement;
			if (playerMovement == null)
			{
				base.enabled = false;
				return;
			}
			triggerer = playerMovement.transform;
			initialRotation = base.transform.localRotation;
			initialRotationEuler = base.transform.localRotation.eulerAngles;
		}

		private void Update()
		{
			if (reseting)
			{
				Reset_Loop();
			}
			else if (triggered)
			{
				PostTrigger_Loop();
			}
			else
			{
				PreTrigger_Loop();
			}
		}

		private void PreTrigger_Loop()
		{
			if (Vector3.SqrMagnitude(triggerer.position - base.transform.position) < triggerDist * triggerDist)
			{
				if (openValue == 0f)
				{
					TriggerSomething();
				}
			}
			else if (openValue == 1f)
			{
				reseting = true;
			}
		}

		private void PostTrigger_Loop()
		{
			openValue += Time.deltaTime * openingSpeed;
			if (openValue >= 1f)
			{
				openValue = 1f;
				triggered = false;
			}
			base.transform.localEulerAngles = Vector3.Lerp(initialRotationEuler, new Vector3(initialRotationEuler.x, initialRotationEuler.y + (float)(approachFromFront ? 100 : (-100)), initialRotationEuler.z), openValue);
		}

		private void Reset_Loop()
		{
			openValue -= Time.deltaTime * openingSpeed;
			if (openValue <= 0f)
			{
				openValue = 0f;
				reseting = false;
			}
			base.transform.localEulerAngles = Vector3.Lerp(initialRotationEuler, new Vector3(initialRotationEuler.x, initialRotationEuler.y + (float)(approachFromFront ? 100 : (-100)), initialRotationEuler.z), openValue);
		}

		private void TriggerSomething()
		{
			triggered = true;
			approachFromFront = base.transform.InverseTransformDirection(triggerer.position).z > 0f;
		}
	}
}
