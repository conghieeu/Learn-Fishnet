using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IBzDamageable
	{
		float Health { get; set; }

		void Shot(Ray ray, float impact, float maxDistance);

		bool IsDead();

		bool IsFullHealth();
	}
}
