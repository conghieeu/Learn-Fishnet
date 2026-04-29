using UnityEngine;

public class BoxMutableHitCheck : CubeHitCheck, IMutableHitCheck, IHitCheck
{
	public BoxMutableHitCheck(Vector3 center, Vector3 extent, Rotator rotation)
		: base(center, extent, rotation, string.Empty)
	{
	}

	public void ApplyOffset(Vector3 offset)
	{
		base.Center += offset;
	}

	public void SetRotation(Rotator rot)
	{
		base.Rotation = rot;
	}
}
