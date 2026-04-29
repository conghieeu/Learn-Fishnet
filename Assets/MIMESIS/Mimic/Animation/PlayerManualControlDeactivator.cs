using Mimic.Actors;
using UnityEngine;

namespace Mimic.Animation
{
	public class PlayerManualControlDeactivator : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (TryGetLocalPlayerActor(animator, out ProtoActor playerActor) && playerActor != null)
			{
				playerActor.DontMove();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (TryGetLocalPlayerActor(animator, out ProtoActor playerActor) && playerActor != null)
			{
				playerActor.CancelDontMove();
			}
		}

		private static bool TryGetLocalPlayerActor(Animator animator, out ProtoActor? playerActor)
		{
			if (animator.TryGetComponent<PuppetScript>(out var component) && component.IsAvatarPuppet() && component.Owner != null)
			{
				playerActor = component.Owner;
				return true;
			}
			playerActor = null;
			return false;
		}
	}
}
