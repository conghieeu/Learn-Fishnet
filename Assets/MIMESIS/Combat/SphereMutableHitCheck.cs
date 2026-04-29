using UnityEngine;

public class SphereMutableHitCheck : SphereHitCheck, IMutableHitCheck, IHitCheck
{
	public SphereMutableHitCheck(Vector3 center, float rad)
		: base(center, rad, string.Empty)
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
