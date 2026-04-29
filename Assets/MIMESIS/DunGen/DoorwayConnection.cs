using System;
using UnityEngine;

namespace DunGen
{
	[Serializable]
	public sealed class DoorwayConnection
	{
		[SerializeField]
		private Doorway a;

		[SerializeField]
		private Doorway b;

		public Doorway A => a;

		public Doorway B => b;

		public DoorwayConnection(Doorway a, Doorway b)
		{
			this.a = a;
			this.b = b;
		}
	}
}
