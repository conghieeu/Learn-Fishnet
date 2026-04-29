using UnityEngine;

public class MapMarker_FieldSkillSpawnPoint : MapMarker_SpawnPoint
{
	[SerializeField]
	private Vector3 _surfaceNormalVector = Vector3.down;

	[SerializeField]
	private Vector3 _decalDirectionVector = Vector3.down;

	public Vector3 SurfaceNormalVector => _surfaceNormalVector;

	public Vector3 DecalDirectionVector => _decalDirectionVector;

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Hub.DrawGizmo_Arrow(base.transform.position - _decalDirectionVector.normalized * 3f, base.transform.position - _decalDirectionVector.normalized, iconColor);
	}
}
