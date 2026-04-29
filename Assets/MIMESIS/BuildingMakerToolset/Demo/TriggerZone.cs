using UnityEngine;

namespace BuildingMakerToolset.Demo
{
	public class TriggerZone : MonoBehaviour
	{
		private BoxCollider trigger;

		private void Start()
		{
			trigger = base.gameObject.GetComponent<BoxCollider>();
			trigger.isTrigger = true;
		}

		private void OnTriggerEnter(Collider other)
		{
			PlayerMovement component = other.gameObject.GetComponent<PlayerMovement>();
			if (!(component == null))
			{
				OnEnter(component);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			PlayerMovement component = other.gameObject.GetComponent<PlayerMovement>();
			if (!(component == null))
			{
				OnExit(component);
			}
		}

		protected virtual void OnEnter(PlayerMovement player)
		{
		}

		protected virtual void OnExit(PlayerMovement player)
		{
		}
	}
}
