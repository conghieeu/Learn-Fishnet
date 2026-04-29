using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IBzRagdollCharacter
	{
		Vector3 CharacterVelocity { get; }

		void CharacterEnable(bool enable);
	}
}
