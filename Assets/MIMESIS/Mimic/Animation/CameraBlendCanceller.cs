using UnityEngine;

namespace Mimic.Animation
{
	public class CameraBlendCanceller : StateMachineBehaviour
	{
		[SerializeField]
		[Tooltip("취소 기능을 액터가 죽어있을때는 사용하지 않습니다.")]
		private bool disableWhenDead;

		[SerializeField]
		[Tooltip("상태 진입 시 수행 중인 카메라 블렌딩이 있다면 취소합니다.")]
		private bool cancelOnStateEnter;

		[SerializeField]
		[Tooltip("상태 탈출 시 수행 중인 카메라 블렌딩이 있다면 취소합니다.")]
		private bool cancelOnStateExit;

		[SerializeField]
		[Tooltip("상태 머신 진입 시 수행 중인 카메라 블렌딩이 있다면 취소합니다.")]
		private bool cancelOnStateMachineEnter;

		[SerializeField]
		[Tooltip("상태 머신 탈출 시 수행 중인 카메라 블렌딩이 있다면 취소합니다.")]
		private bool cancelOnStateMachineExit;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (cancelOnStateEnter)
			{
				CancelCameraBlend(animator);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (cancelOnStateExit)
			{
				CancelCameraBlend(animator);
			}
		}

		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			if (cancelOnStateMachineEnter)
			{
				CancelCameraBlend(animator);
			}
		}

		public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
		{
			if (cancelOnStateMachineExit)
			{
				CancelCameraBlend(animator);
			}
		}

		private void CancelCameraBlend(Animator animator)
		{
			if (!(Hub.s == null) && !(Hub.s.cameraman == null) && animator.TryGetComponent<PuppetScript>(out var component) && component != null && component.IsAvatarPuppet() && component.Owner != null && (!disableWhenDead || !component.Owner.dead))
			{
				Hub.s.cameraman.CancelBlendTo();
			}
		}
	}
}
