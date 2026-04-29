using Mimic.Actors;
using Mimic.Character;
using UnityEngine;

namespace Mimic.Animation
{
	public class StateBasedPersonViewModeSwitcher : StateMachineBehaviour
	{
		[SerializeField]
		[Tooltip("상태 진입 시 전환 여부")]
		private bool switchOnStateEnter;

		[SerializeField]
		[Tooltip("상태 진입 시 전환할 모드")]
		private PersonViewMode modeOnStateEnter;

		[SerializeField]
		[Tooltip("상태 탈출 시 전환 여부")]
		private bool switchOnStateExit;

		[SerializeField]
		[Tooltip("상태 진입 시 전환할 모드")]
		private PersonViewMode modeOnStateExit;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (switchOnStateEnter)
			{
				SwitchMode(animator, modeOnStateEnter);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (switchOnStateExit)
			{
				SwitchMode(animator, modeOnStateExit);
			}
		}

		private void SwitchMode(Animator animator, PersonViewMode mode)
		{
			if (animator.TryGetComponent<PuppetPersonViewModeSwitcher>(out var component) && component != null)
			{
				component.SwitchMode(mode);
			}
			if (!TryGetProtoActor(animator, out ProtoActor protoActor) || !(protoActor != null))
			{
				return;
			}
			foreach (InventoryItem inventoryItem in protoActor.GetInventoryItems())
			{
				inventoryItem?.SetPersonViewMode(mode);
			}
		}

		private static bool TryGetProtoActor(Animator animator, out ProtoActor? protoActor)
		{
			if (animator.TryGetComponent<PuppetScript>(out var component) && component.Owner != null)
			{
				protoActor = component.Owner;
				return true;
			}
			protoActor = null;
			return false;
		}
	}
}
