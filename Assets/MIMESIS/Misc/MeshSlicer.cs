using UnityEngine;

public class MeshSlicer : MonoBehaviour
{
	[SerializeField]
	private GameObject source;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private Material crossMat;

	[SerializeField]
	private GameObject crashEffect;

	[SerializeField]
	private GameObject fragmentSpawningEffect;

	[SerializeField]
	private GameObject fragmentVanishingEffect;

	[SerializeField]
	public float fragmentLifeTime = 10f;

	[SerializeField]
	public float fragmentFadeTime = 1f;

	[SerializeField]
	private bool test_switchNow;

	private GameObject _crashEffectInstance;

	public void Switch(Vector3 externalForce)
	{
		if (crashEffect != null)
		{
			Vector3 center = source.GetComponent<MeshRenderer>().bounds.center;
			_crashEffectInstance = Object.Instantiate(crashEffect, center, Quaternion.identity);
		}
		source.SetActive(value: false);
		target.gameObject.SetActive(value: true);
		MeshSlicerFragmentScript[] componentsInChildren = target.GetComponentsInChildren<MeshSlicerFragmentScript>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Activate(externalForce);
		}
	}

	public void OnDestroy()
	{
		if (_crashEffectInstance != null)
		{
			Object.Destroy(_crashEffectInstance, fragmentLifeTime);
		}
	}

	private void Update()
	{
		if (test_switchNow)
		{
			test_switchNow = false;
			Switch(Vector3.zero);
		}
	}
}
