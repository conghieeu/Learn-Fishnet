using UnityEngine;

namespace StylizedPointLight
{
	public class RandomFlicker : MonoBehaviour
	{
		private void OnEnable()
		{
			if (TryGetComponent<MeshRenderer>(out var component))
			{
				component.material.SetFloat("_randomOffset", Random.value);
			}
		}
	}
}
