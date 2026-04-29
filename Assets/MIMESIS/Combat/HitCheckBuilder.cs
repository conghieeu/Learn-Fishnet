using System;
using System.Globalization;
using UnityEngine;

public static class HitCheckBuilder
{
	public static IHitCheck Build(string hitCheckString)
	{
		string[] array = hitCheckString.Split('/');
		if (array.Length < 2)
		{
			throw new Exception("Invalid HitCheckString");
		}
		if (!Enum.TryParse<HitCheckShapeType>(array[0], ignoreCase: true, out var result))
		{
			throw new Exception("Invalid HitCheckShapeType");
		}
		return result switch
		{
			HitCheckShapeType.Cube => new CubeHitCheck(new Vector3(Convert.ToSingle(array[1], CultureInfo.InvariantCulture), Convert.ToSingle(array[2], CultureInfo.InvariantCulture), Convert.ToSingle(array[3], CultureInfo.InvariantCulture)), string.Empty), 
			HitCheckShapeType.Capsule => new CapsuleHitCheck(Convert.ToSingle(array[1], CultureInfo.InvariantCulture), Convert.ToSingle(array[2], CultureInfo.InvariantCulture), string.Empty), 
			_ => throw new Exception("Invalid HitCheckShapeType"), 
		};
	}
}
