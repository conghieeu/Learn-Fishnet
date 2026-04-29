using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	public sealed class BzHealth : MonoBehaviour, IBzDamageable
	{
		[SerializeField]
		private IBzRagdoll _bzRagdoll;

		private float _health = 1f;

		private float _impactEndTime;

		private Rigidbody _impactTarget;

		private Vector3 _impactDirection;

		public float Health
		{
			get
			{
				return _health;
			}
			set
			{
				if (_health > Mathf.Epsilon && value <= Mathf.Epsilon)
				{
					if (_bzRagdoll != null)
					{
						_bzRagdoll.IsRagdolled = true;
					}
				}
				else if (_health <= Mathf.Epsilon)
				{
					_ = Mathf.Epsilon;
				}
				_health = Mathf.Clamp01(value);
			}
		}

		private void Awake()
		{
			_bzRagdoll = GetComponent<IBzRagdoll>();
		}

		public void Shot(Ray ray, float force, float distance)
		{
		}

		public bool IsDead()
		{
			return Health <= Mathf.Epsilon;
		}

		public bool IsFullHealth()
		{
			return Health >= 1f - Mathf.Epsilon;
		}

		private void FixedUpdate()
		{
			if (Time.time < _impactEndTime)
			{
				_impactTarget.AddForce(_impactDirection * Time.deltaTime * 80f, ForceMode.VelocityChange);
			}
		}
	}
}
