using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

namespace Mimic.Animation
{
	public class MonsterHummingSoundPlayer : StateMachineBehaviour
	{
		[SerializeField]
		[Tooltip("재생할 흥얼거리는 소리의 SFX ID")]
		private string hummingSfxId = "monster_humming";

		[SerializeField]
		[Tooltip("상태 진입 후 소리 재생까지의 지연 시간")]
		private float delayBeforePlay = 0.5f;

		[SerializeField]
		[Tooltip("최소 상태 유지 시간 (이 시간보다 짧으면 소리를 재생하지 않음)")]
		private float minStateDuration = 1f;

		private float stateEnterTime;

		private bool soundPlayed;

		private ProtoActor? cachedMonsterActor;

		private float cachedTime;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			cachedMonsterActor = null;
			cachedTime = Time.time;
			stateEnterTime = cachedTime;
			if (TryGetCachedMonsterActor(animator))
			{
				ProtoActor protoActor = cachedMonsterActor;
				if (protoActor.dead)
				{
					soundPlayed = true;
				}
				else if (protoActor.IsHummingSoundPlaying())
				{
					soundPlayed = true;
				}
				else
				{
					soundPlayed = false;
				}
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (cachedMonsterActor == null && !TryGetCachedMonsterActor(animator))
			{
				return;
			}
			ProtoActor protoActor = cachedMonsterActor;
			if (protoActor.dead)
			{
				soundPlayed = true;
				return;
			}
			if (soundPlayed && !protoActor.IsHummingSoundPlaying())
			{
				soundPlayed = false;
			}
			if (!soundPlayed && !soundPlayed)
			{
				float num = Time.time - stateEnterTime;
				if (num >= delayBeforePlay && num >= minStateDuration)
				{
					protoActor.TryPlayHummingSound(hummingSfxId);
					soundPlayed = true;
				}
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
