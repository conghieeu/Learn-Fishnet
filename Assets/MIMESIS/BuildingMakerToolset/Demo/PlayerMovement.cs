using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BuildingMakerToolset.Demo
{
	public class PlayerMovement : MonoBehaviour
	{
		public enum CurrentTask
		{
			none = 0,
			fly = 1,
			ladder = 2
		}

		[Serializable]
		public class FootStepAudio
		{
			public string name;

			public AudioClip[] audioClips;

			public bool IsValid()
			{
				if (audioClips == null || audioClips.Length == 0)
				{
					return false;
				}
				return true;
			}

			public AudioClip GetRandomClip()
			{
				if (!IsValid())
				{
					return null;
				}
				return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
			}
		}

		public CharacterController controller;

		public Transform cameraTransform;

		public AudioSource footstepSource;

		public float speed = 12f;

		public float gravity = -10f;

		public float jumpHeight = 2f;

		public Transform groundCheck;

		public float groundDistance = 0.4f;

		public LayerMask groundMask;

		private Vector3 lastPosition;

		private Vector3 targetVelocity;

		private bool isGrounded;

		private Collider[] hitColliders;

		public int numOfGroundColliders;

		private float distAfterLastFootstep;

		public CurrentTask currentTask;

		private FootStepAudio curFootstepAudio;

		public FootStepAudio[] footStepAudios;

		public FootStepAudio DefaultFootstepAudio;

		private InputAction movement;

		private InputAction jump;

		private void OnEnable()
		{
			hitColliders = new Collider[8];
			lastPosition = base.transform.position;
		}

		private void Start()
		{
			movement = new InputAction("PlayerMovement", InputActionType.Value, "<Gamepad>/leftStick");
			movement.AddCompositeBinding("Dpad").With("Up", "<Keyboard>/w").With("Up", "<Keyboard>/upArrow")
				.With("Down", "<Keyboard>/s")
				.With("Down", "<Keyboard>/downArrow")
				.With("Left", "<Keyboard>/a")
				.With("Left", "<Keyboard>/leftArrow")
				.With("Right", "<Keyboard>/d")
				.With("Right", "<Keyboard>/rightArrow");
			jump = new InputAction("PlayerJump", InputActionType.Value, "<Gamepad>/a");
			jump.AddBinding("<Keyboard>/space");
			movement.Enable();
			jump.Enable();
		}

		private void Update()
		{
			float num = 0f;
			float num2 = 0f;
			bool flag = false;
			movement.ReadValue<Vector2>();
			flag = Mathf.Approximately(jump.ReadValue<float>(), 1f);
			if (cameraTransform == null)
			{
				currentTask = CurrentTask.none;
			}
			Vector3 vector = Vector3.zero;
			switch (currentTask)
			{
			case CurrentTask.none:
				isGrounded = CheckGround();
				vector = Vector3.ClampMagnitude(base.transform.right * num + base.transform.forward * num2, 1f) * speed;
				targetVelocity.y += gravity * Time.deltaTime;
				break;
			case CurrentTask.fly:
				isGrounded = false;
				vector = Vector3.ClampMagnitude(cameraTransform.right * num + cameraTransform.forward * num2, 1f) * speed;
				targetVelocity.y = vector.y;
				break;
			case CurrentTask.ladder:
				isGrounded = false;
				vector = (CheckGround() ? (Vector3.ClampMagnitude(cameraTransform.right * num + cameraTransform.forward * num2, 1f) * speed) : (Vector3.ClampMagnitude(base.transform.right * num + base.transform.up * num2, 1f) * speed));
				targetVelocity.y = vector.y;
				break;
			}
			targetVelocity.x = vector.x;
			targetVelocity.z = vector.z;
			if (isGrounded && targetVelocity.y < 0f)
			{
				targetVelocity.y = -2f;
			}
			if (flag && isGrounded)
			{
				targetVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			}
			lastPosition = base.transform.position;
			controller.Move(targetVelocity * Time.deltaTime);
			distAfterLastFootstep += (base.transform.position - lastPosition).magnitude;
			if (distAfterLastFootstep > 1f)
			{
				distAfterLastFootstep = 0f;
				MakeFootstep();
			}
		}

		private void MakeFootstep()
		{
			if (footstepSource == null)
			{
				return;
			}
			if (currentTask == CurrentTask.none)
			{
				if (numOfGroundColliders == 0)
				{
					return;
				}
				SetFootstepAudio(hitColliders[0]);
			}
			footstepSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
			footstepSource.PlayOneShot(curFootstepAudio.GetRandomClip());
		}

		private bool CheckGround()
		{
			numOfGroundColliders = Physics.OverlapSphereNonAlloc(groundCheck.position, groundDistance, hitColliders, groundMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < numOfGroundColliders; i++)
			{
				if (hitColliders[i].gameObject == base.gameObject)
				{
					numOfGroundColliders--;
				}
			}
			if (numOfGroundColliders == 0)
			{
				return false;
			}
			return true;
		}

		public void SetFootstepAudio(ref string something)
		{
			if (something == null)
			{
				curFootstepAudio = DefaultFootstepAudio;
				return;
			}
			bool flag = false;
			for (int i = 0; i < footStepAudios.Length; i++)
			{
				if (footStepAudios[i].IsValid() && something.Contains(footStepAudios[i].name))
				{
					flag = true;
					curFootstepAudio = footStepAudios[i];
					break;
				}
			}
			if (!flag)
			{
				curFootstepAudio = DefaultFootstepAudio;
			}
		}

		public void SetFootstepAudio(Collider something)
		{
			if (something == null)
			{
				curFootstepAudio = DefaultFootstepAudio;
				return;
			}
			bool flag = false;
			for (int i = 0; i < footStepAudios.Length; i++)
			{
				if (something.material != null && footStepAudios[i].IsValid() && something.material.name.Contains(footStepAudios[i].name))
				{
					flag = true;
					curFootstepAudio = footStepAudios[i];
					break;
				}
			}
			if (!flag)
			{
				curFootstepAudio = DefaultFootstepAudio;
			}
		}
	}
}
