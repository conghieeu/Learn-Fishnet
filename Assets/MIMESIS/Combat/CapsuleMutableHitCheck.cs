using UnityEngine;

public class CapsuleMutableHitCheck : CapsuleHitCheck, IMutableHitCheck, IHitCheck
{
	public CapsuleMutableHitCheck(float rad, float length, Vector3 center, Rotator rotation)
		: base(rad, length, center, rotation, string.Empty)
	{
	}

	public CapsuleMutableHitCheck(float rad, float length)
		: base(rad, length, Vector3.zero, Rotator.Zero, string.Empty)
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
