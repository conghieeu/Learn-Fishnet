using UnityEngine;

public class TorusMutableHitCheck : TorusHitCheck, IMutableHitCheck, IHitCheck
{
	public TorusMutableHitCheck(Vector3 center, float height, float outerRad, float innerRad)
		: base(center, height, outerRad, innerRad, string.Empty)
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
