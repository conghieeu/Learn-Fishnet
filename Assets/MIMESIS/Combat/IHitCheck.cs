using UnityEngine;

public interface IHitCheck
{
	HitCheckShapeType ShapeType { get; }

	Vector3 Center { get; }

	Rotator Rotation { get; }

	float CheckRadius { get; }

	string Key { get; }

	IMutableHitCheck Clone();
}
