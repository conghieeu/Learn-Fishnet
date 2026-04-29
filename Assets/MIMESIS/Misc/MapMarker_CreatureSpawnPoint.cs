using UnityEngine;

public class MapMarker_CreatureSpawnPoint : MapMarker_SpawnPoint
{
	[SerializeField]
	public string _aiName = string.Empty;

	[SerializeField]
	private string _btName = string.Empty;

	[SerializeField]
	[Tooltip("몬스터 스폰 포인트 활성화 확률. MasterID가 0이 아닐때만 적용. (100%=10000)")]
	private int _fixedSpawnPointActiveRate = 10000;

	[SerializeField]
	public bool _isFirstSpawnPoint;

	public string aiName => _aiName;

	public string btName => _btName;

	public int FixedSpawnPointActiveRate => _fixedSpawnPointActiveRate;

	public bool isFirstSpawnPoint => _isFirstSpawnPoint;

	private new void OnDrawGizmos()
	{
		base.OnDrawGizmos();
	}
}
