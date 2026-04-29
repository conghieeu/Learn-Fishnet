using Mimic.Actors;
using ReluNetwork.ConstEnum;
using UnityEngine;
using UnityEngine.AI;

public class TrapNavMeshObstableController : MonoBehaviour
{
	[SerializeField]
	private NavMeshObstacle _targetNavMeshObstacle;

	[SerializeField]
	private Bounds _playerCheckBound;

	[SerializeField]
	private bool _drawBoundGizmo;

	[SerializeField]
	private Color _gizmoColor = Color.red;

	[SerializeField]
	private float _deadCameraChangeSafeRate = 1.25f;

	[InspectorReadOnly]
	private float _deadCameraChangeSafeTime = 10f;

	private void Start()
	{
		if (_targetNavMeshObstacle == null)
		{
			_targetNavMeshObstacle = GetComponentInChildren<NavMeshObstacle>();
		}
		if (_targetNavMeshObstacle == null)
		{
			Logger.RError("[NavMeshObstacle] Cant find _targetNavMeshObstacle");
		}
		else
		{
			_deadCameraChangeSafeTime = _deadCameraChangeSafeRate * Mathf.Max(Hub.s.gameConfig.playerActor.deadCameraTotalDuration, (float)Hub.s.dataman.ExcelDataManager.Consts.C_AutoObservingTargetChangeTime * 0.001f);
		}
	}

	private void Update()
	{
		if (_targetNavMeshObstacle == null || Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.ClientMode != NetworkClientMode.Host)
		{
			return;
		}
		bool flag = false;
		foreach (ProtoActor allPlayer in Hub.s.pdata.main.GetAllPlayers())
		{
			if (allPlayer != null && IsContainPosition(_playerCheckBound, allPlayer.transform.position))
			{
				if (!allPlayer.dead)
				{
					flag = true;
					break;
				}
				if (allPlayer.deadTime + _deadCameraChangeSafeTime > Time.time)
				{
					flag = true;
					break;
				}
			}
		}
		if (_targetNavMeshObstacle.enabled != flag)
		{
			_targetNavMeshObstacle.enabled = flag;
		}
	}

	public bool IsContainPosition(Bounds bound, Vector3 targetPosition)
	{
		Vector3 vector = base.transform.InverseTransformPoint(targetPosition);
		Vector3 vector2 = bound.size * 0.5f;
		Vector3 center = bound.center;
		if (Mathf.Abs(vector.x - center.x) <= vector2.x && Mathf.Abs(vector.y - center.y) <= vector2.y)
		{
			return Mathf.Abs(vector.z - center.z) <= vector2.z;
		}
		return false;
	}
}
