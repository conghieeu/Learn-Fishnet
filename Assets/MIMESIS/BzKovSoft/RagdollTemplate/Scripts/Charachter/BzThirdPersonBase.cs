using System;
using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(Animator))]
	public abstract class BzThirdPersonBase : MonoBehaviour, IBzRagdollCharacter, IBzThirdPerson
	{
		private readonly int _animatorForward = Animator.StringToHash("Forward");

		private readonly int _animatorTurn = Animator.StringToHash("Turn");

		private readonly int _animatorCrouch = Animator.StringToHash("Crouch");

		private readonly int _animatorOnGround = Animator.StringToHash("OnGround");

		private readonly int _animatorJump = Animator.StringToHash("Jump");

		private readonly int _animatorJumpLeg = Animator.StringToHash("JumpLeg");

		protected readonly int _animatorCapsuleY = Animator.StringToHash("CapsuleY");

		private readonly int _animatorGrounded = Animator.StringToHash("Base Layer.Grounded.Grounded");

		private const float JumpPower = 5f;

		private const float AirSpeed = 5f;

		private const float AirControl = 2f;

		private const float StationaryTurnSpeed = 180f;

		private const float MovingTurnSpeed = 360f;

		private const float RunCycleLegOffset = 0.2f;

		protected Animator _animator;

		private bool _onGround;

		private Vector3 _moveInput;

		private bool _crouch;

		private bool _jump;

		private float _turnAmount;

		private float _forwardAmount;

		private bool _enabled = true;

		protected Vector3 _airVelocity;

		protected bool _jumpPressed;

		protected bool _firstAnimatorFrame = true;

		public Vector3 CharacterVelocity
		{
			get
			{
				if (!_onGround)
				{
					return _airVelocity;
				}
				return PlayerVelocity;
			}
		}

		protected abstract Vector3 PlayerVelocity { get; }

		protected virtual void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public virtual void CharacterEnable(bool enable)
		{
			_enabled = enable;
		}

		public void Move(Vector3 move, bool crouch, bool jump)
		{
			_moveInput = move;
			_crouch = crouch;
			_jump = jump;
		}

		protected abstract bool PlayerTouchGound();

		protected abstract void ApplyCapsuleHeight();

		protected abstract void UpdatePlayerPosition(Vector3 deltaPos);

		private void HandleGroundedVelocities(int currentAnimation)
		{
			bool flag = currentAnimation == _animatorGrounded;
			if ((_jump & !_crouch) && flag)
			{
				Vector3 characterVelocity = CharacterVelocity;
				characterVelocity.y += 5f;
				_airVelocity = characterVelocity;
				_jump = false;
				_onGround = false;
				_jumpPressed = true;
			}
		}

		private void UpdateAnimator()
		{
			_animator.SetFloat(_animatorForward, _forwardAmount, 0.1f, Time.deltaTime);
			_animator.SetFloat(_animatorTurn, _turnAmount, 0.1f, Time.deltaTime);
			_animator.SetBool(_animatorOnGround, _onGround);
			if (!_onGround)
			{
				_animator.SetFloat(_animatorJump, CharacterVelocity.y);
				return;
			}
			float value = (float)((Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1f) < 0.5f) ? 1 : (-1)) * _forwardAmount;
			_animator.SetFloat(_animatorJumpLeg, value);
		}

		private void ConvertMoveInput()
		{
			Vector3 vector = base.transform.InverseTransformDirection(_moveInput);
			if ((Math.Abs(vector.x) > float.Epsilon) & (Math.Abs(vector.z) > float.Epsilon))
			{
				_turnAmount = Mathf.Atan2(vector.x, vector.z);
			}
			else
			{
				_turnAmount = 0f;
			}
			_forwardAmount = vector.z;
		}

		private void ApplyExtraTurnRotation(int currentAnimation)
		{
			if (currentAnimation == _animatorGrounded)
			{
				float num = Mathf.Lerp(180f, 360f, _forwardAmount);
				base.transform.Rotate(0f, _turnAmount * num * Time.deltaTime, 0f);
			}
		}

		private void HandleAirborneVelocities()
		{
			_airVelocity = Vector3.Lerp(b: new Vector3(_moveInput.x * 5f, _airVelocity.y, _moveInput.z * 5f), a: _airVelocity, t: Time.deltaTime * 2f);
		}

		private void FixedUpdate()
		{
			if (_enabled)
			{
				_onGround = !_jumpPressed && PlayerTouchGound();
				_animator.SetBool(_animatorCrouch, _crouch);
				int fullPathHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
				ApplyCapsuleHeight();
				ApplyExtraTurnRotation(fullPathHash);
				ConvertMoveInput();
				if (_onGround)
				{
					HandleGroundedVelocities(fullPathHash);
				}
				else
				{
					HandleAirborneVelocities();
				}
				UpdateAnimator();
			}
		}

		private void OnAnimatorMove()
		{
			if (!(Time.deltaTime < Mathf.Epsilon))
			{
				Vector3 vector = Physics.gravity * Time.deltaTime;
				_airVelocity += vector;
				Vector3 deltaPos;
				if (_onGround)
				{
					deltaPos = _animator.deltaPosition;
					deltaPos.y -= 5f * Time.deltaTime;
				}
				else
				{
					deltaPos = _airVelocity * Time.deltaTime;
				}
				if (_firstAnimatorFrame)
				{
					deltaPos = new Vector3(0f, deltaPos.y, 0f);
					_firstAnimatorFrame = false;
				}
				UpdatePlayerPosition(deltaPos);
				base.transform.rotation *= _animator.deltaRotation;
				_jumpPressed = false;
			}
		}
	}
}
