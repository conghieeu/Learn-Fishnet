using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(CharacterController))]
	public sealed class BzThirdPersonChCtrler : BzThirdPersonBase
	{
		private CharacterController _characterController;

		protected override Vector3 PlayerVelocity => _characterController.velocity;

		protected override void Awake()
		{
			base.Awake();
			_characterController = GetComponent<CharacterController>();
			if (GetComponent<CapsuleCollider>() != null)
			{
				Debug.LogWarning("You do not needed to attach 'CapsuleCollider' to controller with 'CharacterController'");
			}
			if (GetComponent<Rigidbody>() != null)
			{
				Debug.LogWarning("You do not needed to attach 'rigidbody' to controller with 'CharacterController'");
			}
		}

		public override void CharacterEnable(bool enable)
		{
		}

		protected override void ApplyCapsuleHeight()
		{
		}

		protected override bool PlayerTouchGound()
		{
			return _characterController.isGrounded;
		}

		protected override void UpdatePlayerPosition(Vector3 deltaPos)
		{
			if (_characterController.enabled)
			{
				_characterController.Move(deltaPos);
				if (!_characterController.isGrounded)
				{
					return;
				}
			}
			_airVelocity = Vector3.zero;
		}

		private void asd()
		{
		}
	}
}
