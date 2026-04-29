using ReluProtocol;
using UnityEngine;

public class MapMarker_TargetPoint : MapMarker
{
	[SerializeField]
	private string _callSign;

	[SerializeField]
	private bool _isInDoor;

	private int _id;

	private PosWithRot? _pos;

	public int ID => _id;

	public bool IsInDoor => _isInDoor;

	public PosWithRot Pos
	{
		get
		{
			if (_pos == null)
			{
				PosWithRot posWithRot = new PosWithRot();
				posWithRot.x = base.transform.position.x;
				posWithRot.y = base.transform.position.y;
				posWithRot.z = base.transform.position.z;
				posWithRot.yaw = base.transform.rotation.eulerAngles.y;
				_pos = posWithRot;
			}
			return _pos;
		}
	}

	public void SetID(int id)
	{
		_id = id;
	}

	protected void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "icon_mapMarker.png", allowScaling: true, iconColor);
	}
}
