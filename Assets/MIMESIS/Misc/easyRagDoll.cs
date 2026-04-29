using System.Collections;
using System.Collections.Generic;
using BzKovSoft.RagdollTemplate.Scripts.Charachter;
using UnityEngine;

public sealed class easyRagDoll : MonoBehaviour, IBzRagdoll
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

	private Animator _anim;

	private RagdollState _state;

	private float _ragdollingEndTime;

	private const float RagdollToMecanimBlendTime = 0.5f;

	private readonly List<RigidComponent> _rigids = new List<RigidComponent>();

	private readonly List<TransformComponent> _transforms = new List<TransformComponent>();

	[SerializeField]
	private Transform _hipsTransform;

	[SerializeField]
	private float _freezeAfterSeconds = 3f;

	private Rigidbody _hipsTransformRigid;

	private Vector3 _storedHipsPosition;

	private Vector3 _storedHipsPositionPrivAnim;

	private Vector3 _storedHipsPositionPrivBlend;

	public bool test_ragDollNow;

	public bool test_getupNow;

	public Vector3 test_externalForce;

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
		foreach (RigidComponent rigid in _rigids)
		{
			rigid.RigidBody.AddForce(move, ForceMode.Impulse);
		}
	}

	private Transform FindChildByName(Transform parent, string[] names)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			foreach (string text in names)
			{
				if (transform.name == text)
				{
					return transform;
				}
			}
		}
		return null;
	}

	private void Awake()
	{
		_anim = GetComponent<Animator>();
		if (_hipsTransform == null)
		{
			_hipsTransform = FindChildByName(base.transform, new string[2] { "Hips", "mixamorig:Hips" });
		}
		if (_hipsTransform == null)
		{
			return;
		}
		_hipsTransformRigid = _hipsTransform.GetComponent<Rigidbody>();
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
	}

	private void Update()
	{
		if (!(_hipsTransform == null))
		{
			if (test_ragDollNow)
			{
				test_ragDollNow = false;
				ActivateRagDoll(test_externalForce);
			}
			if (test_getupNow)
			{
				test_getupNow = false;
				_state = RagdollState.Animated;
				_ragdollingEndTime = Time.time;
				_anim.enabled = true;
			}
		}
	}

	public void ActivateRagDoll(Vector3 ExternalForce)
	{
		if (!(_hipsTransform == null))
		{
			RagdollIn();
			AddExtraMove(ExternalForce);
			Invoke("Freeze", _freezeAfterSeconds);
		}
	}

	private void Freeze()
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody obj in componentsInChildren)
		{
			BoxCollider component = obj.gameObject.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = false;
			}
			CapsuleCollider component2 = obj.gameObject.GetComponent<CapsuleCollider>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
			SphereCollider component3 = obj.gameObject.GetComponent<SphereCollider>();
			if (component3 != null)
			{
				component3.enabled = false;
			}
			CharacterJoint component4 = obj.gameObject.GetComponent<CharacterJoint>();
			if (component4 != null)
			{
				Object.DestroyImmediate(component4);
			}
			Object.DestroyImmediate(obj);
		}
	}

	private void LateUpdate()
	{
		if (_hipsTransform == null || _state != RagdollState.BlendToAnim)
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
	}

	private void RagdollOut()
	{
		if (_state == RagdollState.Ragdolled)
		{
			_state = RagdollState.Animated;
			_ragdollingEndTime = Time.time;
			_anim.enabled = true;
		}
	}

	private void ActivateRagdollParts(bool activate)
	{
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
		if (activate)
		{
			CharacterJoint[] componentsInChildren = GetComponentsInChildren<CharacterJoint>();
			foreach (CharacterJoint characterJoint in componentsInChildren)
			{
				Transform obj = characterJoint.transform;
				Transform transform = characterJoint.connectedBody.transform;
				obj.position = transform.position;
				obj.Translate(characterJoint.connectedAnchor, transform);
			}
		}
	}
}
