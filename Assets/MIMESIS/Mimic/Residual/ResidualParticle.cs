using UnityEngine;

namespace Mimic.Residual
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ResidualParticle : ResidualObject<ParticleSystem>
	{
		public override bool ShouldBePreserve()
		{
			if (TryGetComponent<ParticleSystem>(out var component))
			{
				return component.IsAlive();
			}
			return false;
		}

		protected override void OnPreserveStarted(ParticleSystem particle)
		{
			particle.Stop();
		}
	}
}
