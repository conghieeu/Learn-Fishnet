using System;
using BzKovSoft.RagdollTemplate.Scripts.Charachter;
using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Tools
{
	public sealed class BzFreeLookCam : MonoBehaviour
	{
		[SerializeField]
		private Transform _pivot;

		[SerializeField]
		private float _turnSmoothing = 0.1f;

		[SerializeField]
		private float _tiltMax = 75f;

		[SerializeField]
		private float _tiltMin = 45f;

		[SerializeField]
		private float _maxDistanse = 3f;

		[SerializeField]
		private float _minDistanse = 0.7f;

		[Range(0f, 10f)]
		[SerializeField]
		private float _mouseSensitive = 1.5f;

		private Transform _cameraPivot;

		private Transform _camera;

		private IBzRagdoll _ragdoll;

		private float _yawAngle;

		private float _pitchAngle;

		private float _smoothX;

		private float _smoothY;

		private float _smoothXvelocity;

		private float _smoothYvelocity;

		private int _callCountChecker;

		private void Start()
		{
			_camera = Camera.main.transform;
			if (_pivot == null)
			{
				Debug.LogError("CameraFree Missing pivot");
				return;
			}
			_cameraPivot = _camera.parent;
			_ragdoll = _pivot.GetComponentInParent<IBzRagdoll>();
		}

		private void Update()
		{
			if (UpdateCameraPos(lateUpdate: false))
			{
				_callCountChecker++;
			}
		}

		private void LateUpdate()
		{
			if (UpdateCameraPos(lateUpdate: true))
			{
				_callCountChecker++;
			}
			if (_callCountChecker != 1)
			{
				throw new InvalidOperationException("There are invalid count of 'setting camera' calls. Count = " + _callCountChecker);
			}
			_callCountChecker = 0;
		}

		private bool UpdateCameraPos(bool lateUpdate)
		{
			if (_ragdoll != null && _ragdoll.IsRagdolled)
			{
				if (lateUpdate)
				{
					return false;
				}
			}
			else if (!lateUpdate)
			{
				return false;
			}
			if (_pivot == null || Time.deltaTime < Mathf.Epsilon)
			{
				return true;
			}
			base.transform.position = _pivot.transform.position;
			HandleRotationMovement();
			HandleWalls();
			return true;
		}

		private void HandleRotationMovement()
		{
		}

		private void HandleWalls()
		{
			Vector3 vector = _camera.position - _cameraPivot.position;
			vector.Normalize();
			RaycastHit[] array = Physics.SphereCastAll(_cameraPivot.position, 0.3f, vector, _maxDistanse);
			float num = _maxDistanse;
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (raycastHit.transform.root != _pivot.transform.root && num > raycastHit.distance)
				{
					num = raycastHit.distance;
				}
			}
			if (num < _minDistanse)
			{
				num = _minDistanse;
			}
			Debug.DrawRay(_cameraPivot.position, vector * num);
			_camera.position = _cameraPivot.position + vector * num;
		}
	}
}
