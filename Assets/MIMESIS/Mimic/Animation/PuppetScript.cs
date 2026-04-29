using System;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using Mimic.Actors;
using Mimic.Character.HitSystem;
using MoreMountains.Feedbacks;
using ReluProtocol.Enum;
using UnityEngine;

namespace Mimic.Animation
{
	[HelpURL("https://wiki.krafton.com/x/XoCtNgE")]
	[RequireComponent(typeof(Animator))]
	public class PuppetScript : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("속력 변화에 대한 민감도")]
		private float velocityStickiness = 6f;

		[SerializeField]
		[Tooltip("이동으로 간주되기 위한 최소 속력")]
		private float velocityThreshold = 0.01f;

		[SerializeField]
		[Tooltip("내부적으로 CrossFade 전환되는 애니메이션에 쓰일 normalizedTransitionDuration 값")]
		private float crossFadeNormalizedTransitionDuration = 0.1f;

		[Header("허리 꺾기")]
		[SerializeField]
		[Tooltip("카메라 회전에 따른 회전값을 적용해줄 Spine")]
		private Transform spine;

		[Header("플레이어 전용 연출")]
		[SerializeField]
		[Tooltip("플레이어 제자리 점프시 발동할 트리거")]
		private string sargentJumpTrigger = "SargentJump";

		[SerializeField]
		[Tooltip("플레이어 이동 점프시 발동할 트리거")]
		private string movingJumpTrigger = "MovingJump";

		[SerializeField]
		[Tooltip("플레이어로 최초 스폰시 발동할 트리거")]
		private string firstSpawnAsPlayerTrigger = "FirstSpawnAsPlayer";

		[Header("사망시 추가 처리")]
		[SerializeField]
		[Tooltip("사망 시점에 비활성화할 오브젝트. 봄베시스에 붙여놓은 쿼카를 없애려고 만들어진 로직")]
		private Transform[] deactiveObjectsAfterDeath = Array.Empty<Transform>();

		[Header("미믹 전용 연출")]
		[SerializeField]
		[Tooltip("미믹으로 스폰시 발동할 트리거")]
		private string spawnAsMimicTrigger = "SpawnAsMimic";

		[SerializeField]
		[Tooltip("Hurtbox")]
		private Hurtbox hurtbox;

		[Header("Feel Feedback")]
		[SerializeField]
		[Tooltip("피격시의 Feel Feedback 트리거를 위해 사용. 카메라 흔들기 등")]
		private MMF_Player hitFeedbackPlayer;

		private Renderer[] _renderers;

		private bool renderersVisible = true;

		private ProtoActor protoActor;

		private Transform spineReference;

		private Animator anim;

		private float forward;

		private float strafe;

		private float speed;

		private bool hasEmote;

		public bool IsJumping { get; private set; }

		public bool IsEmotePlaying { get; private set; }

		public bool IsScrapMotionPlaying { get; private set; }

		public ProtoActor Owner => protoActor;

		public bool IsRenderersVisible => renderersVisible;

		private void Awake()
		{
			anim = GetComponent<Animator>();
			_renderers = GetComponentsInChildren<Renderer>();
		}

		private void LateUpdate()
		{
			RotateSpine();
		}

		private void RotateSpine()
		{
			if (!(spine == null) && !(spineReference == null) && !(protoActor == null) && !protoActor.dead && !(Hub.s == null) && !(Hub.s.gameConfig == null) && Hub.s.gameConfig.playerActor != null && Hub.s.gameConfig.playerActor.enableSpineRot)
			{
				float t = Mathf.Clamp01(anim.GetFloat("SpineRotationWeight"));
				float angle = Mathf.LerpAngle(spine.localEulerAngles.x, spineReference.localEulerAngles.x, t);
				float spineRotXMin = Hub.s.gameConfig.playerActor.spineRotXMin;
				float spineRotXMax = Hub.s.gameConfig.playerActor.spineRotXMax;
				float x = Mathf.Clamp(CorrectEulerAngle(angle), spineRotXMin, spineRotXMax);
				spine.localRotation *= Quaternion.Euler(x, 0f, 0f);
			}
		}

		public Hurtbox GetHurtbox()
		{
			if (hurtbox != null)
			{
				return hurtbox;
			}
			foreach (Transform item in base.transform)
			{
				if (item.TryGetComponent<Hurtbox>(out var component))
				{
					return component;
				}
			}
			return null;
		}

		public void SetProtoActor(ProtoActor protoActorComp)
		{
			if (protoActorComp == null)
			{
				Logger.RError("PuppetScript: SetProtoActor called with null protoActor.");
				return;
			}
			protoActor = protoActorComp;
			spineReference = protoActor.FpvCameraRoot;
			Hurtbox hurtBox = GetHurtbox();
			protoActor.SetHurtBox(hurtBox);
			if (protoActor.IsMimic())
			{
				anim.SetTrigger(spawnAsMimicTrigger);
			}
		}

		public void OnRandomTeleported()
		{
			anim.SetTrigger(spawnAsMimicTrigger);
		}

		public bool IsAvatarPuppet()
		{
			if (protoActor != null)
			{
				return protoActor.AmIAvatar();
			}
			return false;
		}

		public void Move(Vector3 targetVelocity, float walkSpeed, float runSpeed, bool isWorldSpace = true)
		{
			if (isWorldSpace)
			{
				targetVelocity = base.transform.InverseTransformDirection(targetVelocity);
			}
			float b = NormalizeVelocity(targetVelocity.z, walkSpeed, runSpeed);
			float b2 = NormalizeVelocity(targetVelocity.x, walkSpeed, runSpeed);
			float newForward = Mathf.Lerp(forward, b, Time.deltaTime * velocityStickiness);
			float newStrafe = Mathf.Lerp(strafe, b2, Time.deltaTime * velocityStickiness);
			float newSpeed = Mathf.Sqrt(strafe * strafe + forward * forward);
			SetMoveParameters(newForward, newStrafe, newSpeed);
		}

		public void Stop()
		{
			SetMoveParameters(0f, 0f, 0f);
		}

		private void SetMoveParameters(float newForward, float newStrafe, float newSpeed)
		{
			forward = newForward;
			strafe = newStrafe;
			speed = newSpeed;
			anim.SetFloat("MoveForward", newForward);
			anim.SetFloat("MoveStrafe", newStrafe);
			anim.SetFloat("MoveSpeed", newSpeed);
			if (protoActor != null)
			{
				protoActor.OnPuppetMove(newForward, newStrafe, newSpeed);
			}
		}

		public void UpdateMoveType(ActorMoveType moveType)
		{
			if (anim != null)
			{
				anim.SetBool("IsSprint", moveType == ActorMoveType.Run);
			}
		}

		public void PlayFirstSpawnMotion()
		{
			if (anim != null)
			{
				anim.SetTrigger(firstSpawnAsPlayerTrigger);
			}
		}

		public void StartJump()
		{
			if (anim != null)
			{
				anim.SetBool("IsJumping", value: true);
				anim.SetTrigger((speed > 0.1f) ? movingJumpTrigger : sargentJumpTrigger);
			}
			IsJumping = true;
		}

		public void StopJump()
		{
			if (anim != null)
			{
				anim.SetBool("IsJumping", value: false);
			}
			IsJumping = false;
		}

		public void StartEmote(string animatorStateName)
		{
			if (anim != null)
			{
				anim.SetBool("IsEmotePlaying", value: true);
				anim.Play(animatorStateName);
			}
			IsEmotePlaying = true;
			hasEmote = true;
		}

		public void StopEmote()
		{
			if (hasEmote)
			{
				if (anim != null)
				{
					anim.SetBool("IsEmotePlaying", value: false);
				}
				IsEmotePlaying = false;
			}
		}

		public void StartScrapMotion(string stateName, out AnimatorStateInfo stateInfo)
		{
			if (anim != null)
			{
				anim.SetBool("IsScrapMotionPlaying", value: true);
				anim.Play(stateName, out stateInfo);
			}
			else
			{
				stateInfo = default(AnimatorStateInfo);
			}
			IsScrapMotionPlaying = true;
		}

		public void StopScrapMotion()
		{
			if (anim != null)
			{
				anim.SetBool("IsScrapMotionPlaying", value: false);
			}
			IsScrapMotionPlaying = false;
		}

		public void HoldByHand(PuppetHandheldState handheldState)
		{
			StopEmote();
			if (anim != null)
			{
				anim.SetInteger("HandheldState", (int)handheldState);
				anim.SetTrigger("ChangeHandheld");
			}
		}

		public void UseSkill(int skillMasterID)
		{
			StopEmote();
			SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
			if (skillInfo == null)
			{
				Logger.RError($"Invalid SkillMasterID: {skillMasterID}");
				return;
			}
			string skillAnimationState = skillInfo.SkillAnimationState;
			if (!string.IsNullOrWhiteSpace(skillAnimationState) && anim != null)
			{
				anim.CrossFade(skillAnimationState, crossFadeNormalizedTransitionDuration);
			}
		}

		public void CancelSkill(string cancelStateName)
		{
			if (anim != null)
			{
				anim.CrossFadeInFixedTime(cancelStateName, 0.1f);
			}
		}

		public void Die(ReasonOfDeath reason)
		{
			Stop();
			StopEmote();
			if (TryGetComponent<easyRagDoll>(out var component) && reason != ReasonOfDeath.Conta)
			{
				component.ActivateRagDoll(protoActor.LastDamagedForceDirection);
			}
			else if (anim != null)
			{
				anim.SetInteger("ReasonOfDeath", (int)reason);
				anim.SetBool("IsDead", value: true);
			}
			if (TryGetComponent<FootstepAudioPlayer>(out var component2))
			{
				component2.enabled = false;
			}
			Transform[] array = deactiveObjectsAfterDeath;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}

		public void PlayHitFeedback()
		{
			if (hitFeedbackPlayer != null)
			{
				hitFeedbackPlayer.PlayFeedbacks();
			}
		}

		public void PlayHitAnim(string stateName, bool loop = false)
		{
			StopEmote();
			if (Owner != null && Owner.ActorType == ActorType.Monster)
			{
				Owner.StopHummingSound();
			}
			if (anim != null)
			{
				anim.SetBool("IsLoopCondition", loop);
				anim.CrossFade(stateName, crossFadeNormalizedTransitionDuration);
			}
		}

		public void CancelHitAnim()
		{
			if (anim != null)
			{
				anim.CrossFadeInFixedTime("Condition.Null", crossFadeNormalizedTransitionDuration);
			}
		}

		public void SetGrabbing(bool isGrabbing)
		{
			if (anim != null)
			{
				anim.SetBool("IsGrabbing", isGrabbing);
			}
		}

		public void OnAnimationEvent(string statement)
		{
			AnimationEventHandler.Execute(base.gameObject, statement, (protoActor != null) ? protoActor.transform : base.transform, protoActor, this);
		}

		public Transform PickSocketFromEffectEventData(ref string eventData)
		{
			Transform result = null;
			if (eventData.Contains("@"))
			{
				string[] array = eventData.Split('@');
				string socketName = array[1];
				eventData = array[0];
				result = SocketNodeMarker.FindFirstInHierarchy(protoActor.transform, socketName);
			}
			return result;
		}

		private static float NormalizeVelocity(float velocity, float walkSpeed, float runSpeed)
		{
			velocity = Mathf.Clamp(velocity, 0f - runSpeed, runSpeed);
			float num = Mathf.Sign(velocity);
			float num2 = Mathf.Abs(velocity);
			if (num2 < walkSpeed)
			{
				return Mathf.Clamp(velocity / walkSpeed, -1f, 1f) * 0.5f;
			}
			float num3 = runSpeed - walkSpeed;
			float num4 = ((!(num3 > 0f)) ? num : Mathf.Clamp(num * (num2 - walkSpeed) / num3, -1f, 1f));
			return (num4 + num) * 0.5f;
		}

		private float CorrectEulerAngle(float angle)
		{
			if (angle > 180f)
			{
				return angle - 360f;
			}
			if (angle < -180f)
			{
				return angle + 360f;
			}
			return angle;
		}

		public void TurnOnRenderes()
		{
			if (_renderers == null || _renderers.Length == 0)
			{
				return;
			}
			renderersVisible = true;
			Renderer[] renderers = _renderers;
			foreach (Renderer renderer in renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = true;
				}
			}
		}

		public void TurnOffRenderes()
		{
			if (_renderers == null || _renderers.Length == 0)
			{
				return;
			}
			renderersVisible = false;
			Renderer[] renderers = _renderers;
			foreach (Renderer renderer in renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
		}
	}
}
