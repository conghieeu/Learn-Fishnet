using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(IBzThirdPerson))]
	public sealed class BzThirdPersonControl : MonoBehaviour
	{
		private IBzThirdPerson _character;

		private IBzRagdoll _ragdoll;

		private IBzDamageable _health;

		private Transform _camTransform;

		private bool _jumpPressed;

		private bool _fire;

		private bool _crouch;

		private void Start()
		{
			if (Camera.main == null)
			{
				Debug.LogError("Error: no main camera found.");
			}
			else
			{
				_camTransform = Camera.main.transform;
			}
			_character = GetComponent<IBzThirdPerson>();
			_health = GetComponent<IBzDamageable>();
			_ragdoll = GetComponent<IBzRagdoll>();
		}

		private void Update()
		{
		}

		private void FixedUpdate()
		{
		}

		private void ProcessDamage()
		{
		}
	}
}
