using UnityEngine;

public interface IMutableHitCheck : IHitCheck
{
	void ApplyOffset(Vector3 offset);

	void SetRotation(Rotator rot);
}
