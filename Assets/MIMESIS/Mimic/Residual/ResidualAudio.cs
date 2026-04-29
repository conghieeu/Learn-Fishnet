using UnityEngine;

namespace Mimic.Residual
{
	[RequireComponent(typeof(AudioSource))]
	public class ResidualAudio : ResidualObject<AudioSource>
	{
		public override bool ShouldBePreserve()
		{
			if (TryGetComponent<AudioSource>(out var component) && !component.loop)
			{
				return component.isPlaying;
			}
			return false;
		}
	}
}
