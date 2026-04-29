using UnityEngine;

namespace Mimic.Residual
{
	public interface IResidualObject
	{
		GameObject gameObject { get; }

		bool ShouldBePreserve();

		bool TryPreserve();
	}
}
