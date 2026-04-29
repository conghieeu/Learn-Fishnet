using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public sealed class BzThirdPersonRigid : BzThirdPersonBase
	{
		private CapsuleCollider _capsuleCollider;

		private Rigidbody _rigidbody;

		private bool _groundChecker;

		private float _jumpStartedTime;

		protected override Vector3 PlayerVelocity => _rigidbody.linearVelocity;

		protected override void Awake()
		{
			base.Awake();
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_rigidbody = GetComponent<Rigidbody>();
			if (GetComponent<CharacterController>() != null)
			{
				Debug.LogWarning("You do not needed to attach 'CharacterController' to controller with 'Rigidbody'");
			}
		}

		public override void CharacterEnable(bool enable)
		{
			base.CharacterEnable(enable);
			_capsuleCollider.enabled = enable;
			_rigidbody.isKinematic = !enable;
			if (enable)
			{
				_firstAnimatorFrame = true;
			}
		}

		protected override void ApplyCapsuleHeight()
		{
			float num = _animator.GetFloat(_animatorCapsuleY);
			_capsuleCollider.height = num;
			Vector3 center = _capsuleCollider.center;
			center.y = num / 2f;
			_capsuleCollider.center = center;
		}

		private void ProccessOnCollisionOccured(Collision collision)
		{
			float num = base.transform.position.y + _capsuleCollider.center.y - _capsuleCollider.height / 2f + _capsuleCollider.radius * 0.8f;
			ContactPoint[] contacts = collision.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				if (contactPoint.point.y < num && !contactPoint.otherCollider.transform.IsChildOf(base.transform))
				{
					_groundChecker = true;
					Debug.DrawRay(contactPoint.point, contactPoint.normal, Color.blue);
					break;
				}
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			ProccessOnCollisionOccured(collision);
		}

		private void OnCollisionEnter(Collision collision)
		{
			ProccessOnCollisionOccured(collision);
		}

		protected override bool PlayerTouchGound()
		{
			bool groundChecker = _groundChecker;
			_groundChecker = false;
			return groundChecker & (_jumpStartedTime + 0.5f < Time.time);
		}

		protected override void UpdatePlayerPosition(Vector3 deltaPos)
		{
			Vector3 vector = deltaPos / Time.deltaTime;
			if (!_jumpPressed)
			{
				vector.y = _rigidbody.linearVelocity.y;
			}
			else
			{
				_jumpStartedTime = Time.time;
			}
			_airVelocity = vector;
			_rigidbody.linearVelocity = vector;
		}
	}
}
