using System.Collections.Generic;
using UnityEngine;

namespace Mimic.Residual
{
	public abstract class ResidualObject<T> : MonoBehaviour, IResidualObject where T : Component
	{
		GameObject IResidualObject.gameObject => base.gameObject;

		public abstract bool ShouldBePreserve();

		public bool TryPreserve()
		{
			if (!TryGetComponent<T>(out var component))
			{
				Logger.RError("Residual object has no T, cannot preserve.");
				return false;
			}
			OnPreserveStarted(component);
			ReparentAllChildren(base.transform.parent);
			base.transform.SetParent(Hub.s.residualObject.transform, worldPositionStays: true);
			return true;
		}

		protected virtual void OnPreserveStarted(T component)
		{
		}

		private void ReparentAllChildren(Transform newParent)
		{
			List<Transform> list = new List<Transform>(base.transform.childCount);
			foreach (Transform item in base.transform)
			{
				list.Add(item);
			}
			foreach (Transform item2 in list)
			{
				if (item2 != null)
				{
					item2.SetParent(newParent, worldPositionStays: true);
				}
			}
		}
	}
}
