using ReluProtocol;
using UnityEngine;

public abstract class MapMarker_TeleportPoint : MapMarker
{
	[SerializeField]
	public TeleportType TeleportType;

	[SerializeField]
	protected string _callSign;

	private PosWithRot? _pos;

	public string CallSign
	{
		get
		{
			return _callSign;
		}
		set
		{
			_callSign = value;
		}
	}

	public PosWithRot Pos
	{
		get
		{
			if (_pos == null)
			{
				_pos = new PosWithRot();
			}
			_pos.x = base.transform.position.x;
			_pos.y = base.transform.position.y;
			_pos.z = base.transform.position.z;
			_pos.yaw = base.transform.rotation.eulerAngles.y;
			return _pos;
		}
	}

	protected void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "icon_mapMarker.png", allowScaling: true, iconColor);
	}
}
