using System;
using UnityEngine;

namespace SpriteShadersUltimate
{
	[Serializable]
	public class VectorFaderSSU : BaseFaderSSU
	{
		[Header("Range:")]
		public Vector4 fromValue;

		public Vector4 toValue;

		public VectorFaderSSU(string newName, Vector4 newFrom, Vector4 newTo)
		{
			propertyName = newName;
			fromValue = newFrom;
			toValue = newTo;
			fromTime = 0f;
			toTime = 1f;
		}
	}
}
