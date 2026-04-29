using Unity.Cinemachine;
using UnityEngine;

namespace Mimic.Residual
{
	[RequireComponent(typeof(CinemachineCamera))]
	public class ResidualCinemachineCamera : ResidualObject<CinemachineCamera>
	{
		public override bool ShouldBePreserve()
		{
			if (TryGetComponent<CinemachineCamera>(out var component))
			{
				return component.IsLive;
			}
			return false;
		}
	}
}
