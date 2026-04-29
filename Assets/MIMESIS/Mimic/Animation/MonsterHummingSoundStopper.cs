using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

namespace Mimic.Animation
{
	public class MonsterHummingSoundStopper : StateMachineBehaviour
	{
		[SerializeField]
		[Tooltip("상태 진입 시 즉시 소리를 정지할지 여부")]
		private bool stopOnEnter = true;

		[SerializeField]
		[Tooltip("상태 진입 후 소리 정지까지의 지연 시간 (초)")]
		private float delayBeforeStop;

		private bool soundStopped;

		private float stateEnterTime;

		private ProtoActor? cachedMonsterActor;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			cachedMonsterActor = null;
			stateEnterTime = Time.time;
			soundStopped = false;
			if (TryGetCachedMonsterActor(animator))
			{
				ProtoActor protoActor = cachedMonsterActor;
				if (stopOnEnter && delayBeforeStop <= 0f)
				{
					protoActor.StopHummingSound();
					soundStopped = true;
				}
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!soundStopped && stopOnEnter && Time.time - stateEnterTime >= delayBeforeStop && (!(cachedMonsterActor == null) || TryGetCachedMonsterActor(animator)))
			{
				cachedMonsterActor.StopHummingSound();
				soundStopped = true;
			}
		}

		private bool TryGetCachedMonsterActor(Animator animator)
		{
			if (cachedMonsterActor != null)
			{
				return true;
			}
			if (TryGetMonsterActor(animator, out ProtoActor monsterActor) && monsterActor != null)
			{
				cachedMonsterActor = monsterActor;
				return true;
			}
			return false;
		}

		private static bool TryGetMonsterActor(Animator animator, out ProtoActor? monsterActor)
		{
			if (animator.TryGetComponent<PuppetScript>(out var component))
			{
				ProtoActor owner = component.Owner;
				if ((object)owner != null && owner.ActorType == ActorType.Monster)
				{
					monsterActor = component.Owner;
					return true;
				}
			}
			monsterActor = null;
			return false;
		}
	}
}
