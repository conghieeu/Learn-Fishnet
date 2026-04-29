using UnityEngine;

public class MapMarker_LootingObjectSpawnPoint : MapMarker_SpawnPoint
{
	[SerializeField]
	private int _durability;

	[SerializeField]
	private int _defaultGauge;

	[SerializeField]
	private int _stackCount;

	[SerializeField]
	private bool _ignoreNav;

	[SerializeField]
	private string _itemPropertyKey;

	public int durability => _durability;

	public int defaultGauge => _defaultGauge;

	public int stackCount => _stackCount;

	public bool ignoreNav => _ignoreNav;

	public string itemPropertyKey => _itemPropertyKey;
}
