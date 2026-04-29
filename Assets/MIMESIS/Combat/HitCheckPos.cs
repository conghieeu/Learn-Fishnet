using UnityEngine;

public struct HitCheckPos
{
	public Vector3 Start;

	public Vector3 End;

	public float AngleRad;

	public override string ToString()
	{
		return $"Start:{Start}, End:{End}, AngleRad:{AngleRad}";
	}
}
