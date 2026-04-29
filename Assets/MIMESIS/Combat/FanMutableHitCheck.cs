using UnityEngine;

public class FanMutableHitCheck : FanHitCheck, IMutableHitCheck, IHitCheck
{
	public FanMutableHitCheck(Vector3 center, Rotator rotation, float height, float innerRad, float outerRad, float angle)
		: base(center, rotation, height, innerRad, outerRad, angle, string.Empty)
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
