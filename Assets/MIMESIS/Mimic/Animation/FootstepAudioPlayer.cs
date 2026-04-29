using UnityEngine;

namespace Mimic.Animation
{
	[RequireComponent(typeof(Animator))]
	public class FootstepAudioPlayer : MonoBehaviour
	{
		[Header("Master Audio")]
		[SerializeField]
		[Tooltip("오른 발소리에 해당하는 SoundGroup ID")]
		private string rightFootstepSfxId = string.Empty;

		[SerializeField]
		[Tooltip("발소리에 해당하는 SoundGroup ID")]
		private string leftFootstepSfxId = string.Empty;

		[Header("Animator Parameter")]
		[SerializeField]
		[Tooltip("발소리 재생의 기준이 될 애니메이터 내의 파라미터. 1에 가까울 수록 오른발 소리를, -1에 가까울 수록 왼발 소리를 재생해야 한다.")]
		private string animatorParameterName = "Footstep";

		[SerializeField]
		[Tooltip("애니메이터 파라미터의 임계값. 이보다 작은 크기의 범위에서 일어나는 변화는 무시한다.")]
		private float animatorParameterThreshold = 0.01f;

		private Animator animator;

		private float lastFootstep;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			float num = animator.GetFloat(animatorParameterName);
			if (Mathf.Abs(num) > animatorParameterThreshold)
			{
				if (num > 0f && lastFootstep < 0f)
				{
					PlayFootstepAudio(rightFootstepSfxId);
				}
				else if (num < 0f && lastFootstep > 0f)
				{
					PlayFootstepAudio(leftFootstepSfxId);
				}
				lastFootstep = num;
			}
		}

		private void PlayFootstepAudio(string sfxId)
		{
			if (!string.IsNullOrWhiteSpace(sfxId) && !(Hub.s == null) && !(Hub.s.audioman == null))
			{
				Hub.s.audioman.PlaySfxAtTransform(sfxId, base.transform);
			}
		}
	}
}
