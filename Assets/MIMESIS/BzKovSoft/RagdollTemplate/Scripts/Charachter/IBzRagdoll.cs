using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IBzRagdoll
	{
		bool IsRagdolled { get; set; }

		bool Raycast(Ray ray, out RaycastHit hit, float distance);

		void AddExtraMove(Vector3 move);
	}
}
