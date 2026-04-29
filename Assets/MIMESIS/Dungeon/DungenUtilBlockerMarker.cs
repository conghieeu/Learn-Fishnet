using UnityEngine;

public class DungenUtilBlockerMarker : MonoBehaviour
{
	[Tooltip("이것을 켜면, random teleport 시작점으로 바뀔 수 있다")]
	public bool canReplacedWithRandomTeleportStart;

	[Tooltip("random teleport 시작점으로 사용될 때 교체될 prefab. None 으로 설정하면, GamePlayScene 에 설정한 공통 prefab 을 사용한다")]
	public GameObject RandomTeleportCustomPrefab;
}
