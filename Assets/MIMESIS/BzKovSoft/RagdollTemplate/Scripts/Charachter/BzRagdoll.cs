using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(IBzRagdollCharacter))]
	public sealed class BzRagdoll : MonoBehaviour, IBzRagdoll
	{
		private sealed class TransformComponent
		{
			public readonly Transform Transform;

			public Quaternion PrivRotation;

			public Quaternion StoredRotation;

			public Vector3 PrivPosition;

			public Vector3 StoredPosition;

			public TransformComponent(Transform t)
			{
				Transform = t;
			}
		}

		private struct RigidComponent
		{
			public readonly Rigidbody RigidBody;

			public readonly CharacterJoint Joint;

			public readonly Vector3 ConnectedAnchorDefault;

			public RigidComponent(Rigidbody rigid)
			{
				RigidBody = rigid;
				Joint = rigid.GetComponent<CharacterJoint>();
				if (Joint != null)
				{
					ConnectedAnchorDefault = Joint.connectedAnchor;
				}
				else
				{
					ConnectedAnchorDefault = Vector3.zero;
				}
			}
		}

		private enum RagdollState
		{
			Animated = 0,
			WaitStablePosition = 1,
			Ragdolled = 2,
			BlendToAnim = 3
		}

		[SerializeField]
		private string _animationGetUpFromBelly = "GetUp.GetUpFromBelly";

		[SerializeField]
		private string _animationGetUpFromBack = "GetUp.GetUpFromBack";

		private Animator _anim;

		private IBzRagdollCharacter _bzRagdollCharacter;

		private const float AirSpeed = 5f;

		[SerializeField]
		private RagdollState _state;

		private float _ragdollingEndTime;

		private const float RagdollToMecanimBlendTime = 0.5f;

		private readonly List<RigidComponent> _rigids = new List<RigidComponent>();

		private readonly List<TransformComponent> _transforms = new List<TransformComponent>();

		private Transform _hipsTransform;

		private Rigidbody _hipsTransformRigid;

		private Vector3 _storedHipsPosition;

		private Vector3 _storedHipsPositionPrivAnim;

		private Vector3 _storedHipsPositionPrivBlend;

		public bool IsRagdolled
		{
			get
			{
				if (_state != RagdollState.Ragdolled)
				{
					return _state == RagdollState.WaitStablePosition;
				}
				return true;
			}
			set
			{
				if (value)
				{
					RagdollIn();
				}
				else
				{
					RagdollOut();
				}
			}
		}

		public bool Raycast(Ray ray, out RaycastHit hit, float distance)
		{
			RaycastHit[] array = Physics.RaycastAll(ray, distance);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (raycastHit.transform != base.transform && raycastHit.transform.root == base.transform.root)
				{
					hit = raycastHit;
					return true;
				}
			}
			hit = default(RaycastHit);
			return false;
		}

		public void AddExtraMove(Vector3 move)
		{
			if (!IsRagdolled)
			{
				return;
			}
			Vector3 vector = new Vector3(move.x * 5f, 0f, move.z * 5f);
			foreach (RigidComponent rigid in _rigids)
			{
				rigid.RigidBody.AddForce(vector / 100f, ForceMode.VelocityChange);
			}
		}

		private void Start()
		{
			_anim = GetComponent<Animator>();
			_hipsTransform = _anim.GetBoneTransform(HumanBodyBones.Hips);
			_hipsTransformRigid = _hipsTransform.GetComponent<Rigidbody>();
			_bzRagdollCharacter = GetComponent<IBzRagdollCharacter>();
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rigidbody in componentsInChildren)
			{
				if (!(rigidbody.transform == base.transform))
				{
					RigidComponent item = new RigidComponent(rigidbody);
					_rigids.Add(item);
				}
			}
			ActivateRagdollParts(activate: false);
			Transform[] componentsInChildren2 = GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				TransformComponent item2 = new TransformComponent(componentsInChildren2[i]);
				_transforms.Add(item2);
			}
			RagdollIn();
		}

		private void Update()
		{
			if (_state == RagdollState.WaitStablePosition && _hipsTransformRigid.linearVelocity.magnitude < 0.1f)
			{
				GetUp();
			}
			if (_state == RagdollState.Animated && _bzRagdollCharacter.CharacterVelocity.y < -10f)
			{
				RagdollIn();
				RagdollOut();
			}
		}

		private void LateUpdate()
		{
			if (_state != RagdollState.BlendToAnim)
			{
				return;
			}
			float num = 1f - Mathf.InverseLerp(_ragdollingEndTime, _ragdollingEndTime + 0.5f, Time.time);
			if (_storedHipsPositionPrivBlend != _hipsTransform.position)
			{
				_storedHipsPositionPrivAnim = _hipsTransform.position;
			}
			_storedHipsPositionPrivBlend = Vector3.Lerp(_storedHipsPositionPrivAnim, _storedHipsPosition, num);
			_hipsTransform.position = _storedHipsPositionPrivBlend;
			foreach (TransformComponent transform in _transforms)
			{
				if (transform.PrivRotation != transform.Transform.localRotation)
				{
					transform.PrivRotation = Quaternion.Slerp(transform.Transform.localRotation, transform.StoredRotation, num);
					transform.Transform.localRotation = transform.PrivRotation;
				}
				if (transform.PrivPosition != transform.Transform.localPosition)
				{
					transform.PrivPosition = Vector3.Slerp(transform.Transform.localPosition, transform.StoredPosition, num);
					transform.Transform.localPosition = transform.PrivPosition;
				}
			}
			if (Mathf.Abs(num) < Mathf.Epsilon)
			{
				_state = RagdollState.Animated;
			}
		}

		private static IEnumerator FixTransformAndEnableJoint(RigidComponent joint)
		{
			if (!(joint.Joint == null) && joint.Joint.autoConfigureConnectedAnchor)
			{
				SoftJointLimit highTwistLimit2;
				SoftJointLimit highTwistLimit = (highTwistLimit2 = joint.Joint.highTwistLimit);
				SoftJointLimit curHighTwistLimit = highTwistLimit2;
				SoftJointLimit lowTwistLimit = (highTwistLimit2 = joint.Joint.lowTwistLimit);
				SoftJointLimit curLowTwistLimit = highTwistLimit2;
				SoftJointLimit swing1Limit = (highTwistLimit2 = joint.Joint.swing1Limit);
				SoftJointLimit curSwing1Limit = highTwistLimit2;
				SoftJointLimit swing2Limit = (highTwistLimit2 = joint.Joint.swing2Limit);
				SoftJointLimit curSwing2Limit = highTwistLimit2;
				float aTime = 0.3f;
				Vector3 startConPosition = joint.Joint.connectedBody.transform.InverseTransformVector(joint.Joint.transform.position - joint.Joint.connectedBody.transform.position);
				joint.Joint.autoConfigureConnectedAnchor = false;
				for (float t = 0f; t < 1f; t += Time.deltaTime / aTime)
				{
					Vector3 connectedAnchor = Vector3.Lerp(startConPosition, joint.ConnectedAnchorDefault, t);
					joint.Joint.connectedAnchor = connectedAnchor;
					curHighTwistLimit.limit = Mathf.Lerp(177f, highTwistLimit.limit, t);
					curLowTwistLimit.limit = Mathf.Lerp(-177f, lowTwistLimit.limit, t);
					curSwing1Limit.limit = Mathf.Lerp(177f, swing1Limit.limit, t);
					curSwing2Limit.limit = Mathf.Lerp(177f, swing2Limit.limit, t);
					joint.Joint.highTwistLimit = curHighTwistLimit;
					joint.Joint.lowTwistLimit = curLowTwistLimit;
					joint.Joint.swing1Limit = curSwing1Limit;
					joint.Joint.swing2Limit = curSwing2Limit;
					yield return null;
				}
				joint.Joint.connectedAnchor = joint.ConnectedAnchorDefault;
				yield return new WaitForFixedUpdate();
				joint.Joint.autoConfigureConnectedAnchor = true;
				joint.Joint.highTwistLimit = highTwistLimit;
				joint.Joint.lowTwistLimit = lowTwistLimit;
				joint.Joint.swing1Limit = swing1Limit;
				joint.Joint.swing2Limit = swing2Limit;
			}
		}

		private void RagdollIn()
		{
			ActivateRagdollParts(activate: true);
			_anim.enabled = false;
			_state = RagdollState.Ragdolled;
			ApplyVelocity(_bzRagdollCharacter.CharacterVelocity);
		}

		private void RagdollOut()
		{
			if (_state == RagdollState.Ragdolled)
			{
				_state = RagdollState.WaitStablePosition;
			}
		}

		private void GetUp()
		{
			_ragdollingEndTime = Time.time;
			_anim.enabled = true;
			_state = RagdollState.BlendToAnim;
			_storedHipsPositionPrivAnim = Vector3.zero;
			_storedHipsPositionPrivBlend = Vector3.zero;
			_storedHipsPosition = _hipsTransform.position;
			Vector3 shiftPos = _hipsTransform.position - base.transform.position;
			shiftPos.y = GetDistanceToFloor(shiftPos.y);
			MoveNodeWithoutChildren(shiftPos);
			foreach (TransformComponent transform in _transforms)
			{
				transform.StoredRotation = transform.Transform.localRotation;
				transform.PrivRotation = transform.Transform.localRotation;
				transform.StoredPosition = transform.Transform.localPosition;
				transform.PrivPosition = transform.Transform.localPosition;
			}
			string stateName = (CheckIfLieOnBack() ? _animationGetUpFromBack : _animationGetUpFromBelly);
			_anim.Play(stateName, 0, 0f);
			ActivateRagdollParts(activate: false);
		}

		private float GetDistanceToFloor(float currentY)
		{
			RaycastHit[] array = Physics.RaycastAll(new Ray(_hipsTransform.position, Vector3.down));
			float num = float.MinValue;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!raycastHit.transform.IsChildOf(base.transform))
				{
					num = Mathf.Max(num, raycastHit.point.y);
				}
			}
			if (Mathf.Abs(num - float.MinValue) > Mathf.Epsilon)
			{
				currentY = num - base.transform.position.y;
			}
			return currentY;
		}

		private void MoveNodeWithoutChildren(Vector3 shiftPos)
		{
			Vector3 ragdollDirection = GetRagdollDirection();
			_hipsTransform.position -= shiftPos;
			base.transform.position += shiftPos;
			Vector3 forward = base.transform.forward;
			base.transform.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * base.transform.rotation;
			_hipsTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * _hipsTransform.rotation;
		}

		private bool CheckIfLieOnBack()
		{
			Vector3 position = _anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
			Vector3 position2 = _anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
			Vector3 position3 = _hipsTransform.position;
			position -= position3;
			position.y = 0f;
			position2 -= position3;
			position2.y = 0f;
			return (Quaternion.FromToRotation(position, Vector3.right) * position2).z < 0f;
		}

		private Vector3 GetRagdollDirection()
		{
			Vector3 position = _anim.GetBoneTransform(HumanBodyBones.Hips).position;
			Vector3 position2 = _anim.GetBoneTransform(HumanBodyBones.Head).position;
			Vector3 vector = position - position2;
			vector.y = 0f;
			vector = vector.normalized;
			if (CheckIfLieOnBack())
			{
				return vector;
			}
			return -vector;
		}

		private void ApplyVelocity(Vector3 predieVelocity)
		{
			foreach (RigidComponent rigid in _rigids)
			{
				rigid.RigidBody.linearVelocity = predieVelocity;
			}
		}

		private void ActivateRagdollParts(bool activate)
		{
			_bzRagdollCharacter.CharacterEnable(!activate);
			foreach (RigidComponent rigid in _rigids)
			{
				Collider component = rigid.RigidBody.GetComponent<Collider>();
				if (component == null)
				{
					string n = rigid.RigidBody.name + "_ColliderRotator";
					component = rigid.RigidBody.transform.Find(n).GetComponent<Collider>();
				}
				component.isTrigger = !activate;
				if (activate)
				{
					rigid.RigidBody.isKinematic = false;
					StartCoroutine(FixTransformAndEnableJoint(rigid));
				}
				else
				{
					rigid.RigidBody.isKinematic = true;
				}
			}
		}
	}
}
