using System;
using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	[Serializable]
	public class Demo_TriggerEvent
	{
		[Header("Delay:")]
		public float delay;

		[Header("Change Fader:")]
		public ShaderFaderSSU fader;

		public bool faderState;

		public bool negateState;

		[Header("Snap Player:")]
		public bool snapPlayer;

		public bool isRelative;

		public Vector3 snapPosition;

		[Header("Hurt Player:")]
		public bool hurtPlayer;

		public Vector2 velocity;

		public void Play(Transform source)
		{
			if (fader != null)
			{
				if (negateState)
				{
					fader.isFaded = !fader.isFaded;
				}
				else
				{
					fader.isFaded = faderState;
				}
			}
			if (snapPlayer)
			{
				Demo_Player.instance.SnapPosition(isRelative ? (source.position + snapPosition) : snapPosition);
			}
			if (hurtPlayer)
			{
				Demo_Player.instance.GetHurt(velocity);
			}
		}
	}
}
