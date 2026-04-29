using UnityEngine;

namespace Mimic.Animation
{
	public static class AnimatorExtensions
	{
		public static void Play(this Animator animator, string stateName, out AnimatorStateInfo stateInfo)
		{
			animator.Play(stateName);
			int layerIndex = animator.GetLayerIndex("Action");
			stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
		}
	}
}
