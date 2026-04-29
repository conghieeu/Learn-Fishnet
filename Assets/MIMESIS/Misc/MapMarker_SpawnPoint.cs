using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class MapMarker_SpawnPoint : MapMarker
{
	[SerializeField]
	private bool _isIndoor;

	[SerializeField]
	private int _respawnCount;

	[SerializeField]
	private long _spawnWaitTime = 3000000L;

	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private ActorType _type;

	[SerializeField]
	private bool _enableReset;

	[SerializeField]
	private int _masterID;

	[SerializeField]
	private SpawnType _spawnType;

	private int _id;

	private PosWithRot? _pos;

	public bool IsIndoor => _isIndoor;

	public int MaxRespawnCount => _respawnCount;

	public long spawnWaitTime => _spawnWaitTime;

	public string Name => _name;

	public ActorType type => _type;

	public bool enableReset => _enableReset;

	public int masterID => _masterID;

	public SpawnType spawnType => _spawnType;

	public int ID => _id;

	public PosWithRot pos
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

	public void SetID(int id)
	{
		_id = id;
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		string text = type switch
		{
			ActorType.Player => "icon_spawnPoint_player.png", 
			ActorType.Monster => "icon_spawnPoint_monster.png", 
			ActorType.LootingObject => "icon_spawnPoint_scrap.png", 
			ActorType.FieldSkill => "icon_trap.png", 
			_ => "icon_mapMarker.png", 
		};
		Gizmos.DrawIcon(base.transform.position, text, allowScaling: true, iconColor);
		Hub.DrawGizmo_Arrow(base.transform.position, base.transform.position + base.transform.forward * 0.3f, iconColor);
	}
}
