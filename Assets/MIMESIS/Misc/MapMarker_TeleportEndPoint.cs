using ReluProtocol;
using UnityEngine;

public class MapMarker_TeleportEndPoint : MapMarker_TeleportPoint
{
	[SerializeField]
	[Tooltip("현재 Player만 영향 받습니다")]
	private bool _isIndoor;

	public float ForwardOffset = 1f;

	public bool IsIndoor => _isIndoor;

	private void Start()
	{
		GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
		if (gamePlayScene != null)
		{
			bool flag = gamePlayScene.CheckPosIsIndoor(base.transform.position);
			if (_isIndoor != flag)
			{
				Logger.RError($"TeleportEndPoint({base.CallSign}) need check indoor value. input:{_isIndoor} != scene:{flag}");
			}
		}
	}

	public PosWithRot GetTargetRandomPos(float distance = 0.1f)
	{
		Vector2 vector = Random.insideUnitCircle.normalized * distance;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
		Vector3 vector3 = base.transform.position + vector2;
		if (TeleportType == TeleportType.EventActionRandom)
		{
			vector3 = base.transform.position + vector2 + base.transform.forward * ForwardOffset;
		}
		PosWithRot posWithRot = new PosWithRot();
		posWithRot.x = vector3.x;
		posWithRot.y = vector3.y;
		posWithRot.z = vector3.z;
		posWithRot.yaw = base.transform.rotation.eulerAngles.y;
		return posWithRot;
	}
}
