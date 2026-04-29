using System;
using UnityEngine;

namespace SpriteShadersUltimate
{
	[Serializable]
	public class BaseFaderSSU
	{
		[Header("Property Name:")]
		public string propertyName;

		[Header("Time:")]
		[Range(0f, 1f)]
		public float fromTime;

		[Range(0f, 1f)]
		public float toTime = 1f;

		public BaseFaderSSU()
		{
			toTime = 1f;
		}
	}
}
