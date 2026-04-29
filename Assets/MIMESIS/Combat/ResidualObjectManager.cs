using System;
using Cysharp.Threading.Tasks;
using Mimic.Residual;
using Unity.Cinemachine;
using UnityEngine;

public class ResidualObjectManager : MonoBehaviour
{
	[SerializeField]
	private float destroyInterval = 1f;

	private async UniTaskVoid Start()
	{
		while (!base.destroyCancellationToken.IsCancellationRequested)
		{
			IResidualObject[] array = FindAll(this);
			foreach (IResidualObject residualObject in array)
			{
				if (residualObject != null && !residualObject.ShouldBePreserve())
				{
					UnityEngine.Object.Destroy(residualObject.gameObject);
				}
			}
			await UniTask.WaitForSeconds(destroyInterval, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
		}
	}

	public void PreserveAllInChildren(Component root)
	{
		if (!(root == null))
		{
			AddComponentsOnChildren<CinemachineCamera, ResidualCinemachineCamera>(root);
			IResidualObject[] array = FindAll(root);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TryPreserve();
			}
		}
	}

	private IResidualObject[] FindAll(Component root)
	{
		if (root == null)
		{
			Logger.RError("Root is null, cannot find residual objects.");
			return Array.Empty<IResidualObject>();
		}
		return root.GetComponentsInChildren<IResidualObject>();
	}

	private void AddComponentsOnChildren<Target, New>(Component root) where Target : Component where New : Component
	{
		Target[] componentsInChildren = root.GetComponentsInChildren<Target>();
		foreach (Target val in componentsInChildren)
		{
			if (!val.TryGetComponent<New>(out var _))
			{
				val.gameObject.AddComponent<New>();
			}
		}
	}
}
