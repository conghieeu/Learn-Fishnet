using BzKovSoft.RagdollTemplate.Scripts.Charachter;
using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Tools
{
	public sealed class BzHealthItem : MonoBehaviour
	{
		[SerializeField]
		private float _addHealth = 0.25f;

		private void OnTriggerEnter(Collider collider)
		{
			IBzDamageable component = collider.GetComponent<IBzDamageable>();
			if (component != null && !component.IsFullHealth())
			{
				component.Health += _addHealth;
				Object.Destroy(base.gameObject);
			}
		}
	}
}
