using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IBzThirdPerson
	{
		void Move(Vector3 move, bool crouch, bool jump);
	}
}
