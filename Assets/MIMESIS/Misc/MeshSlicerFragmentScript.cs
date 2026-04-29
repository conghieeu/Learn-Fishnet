using System.Collections;
using UnityEngine;

public class MeshSlicerFragmentScript : MonoBehaviour
{
	public float lifeTime = 1f;

	public float fadeTime = 1f;

	public GameObject spawningEffect;

	public GameObject vanishingEffect;

	public void Activate(Vector3 externalForce)
	{
		base.transform.SetParent(null);
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody obj in componentsInChildren)
		{
			Vector3 vector = Random.insideUnitSphere * 1f;
			obj.AddForce(externalForce + vector, ForceMode.VelocityChange);
		}
	}

	private IEnumerator Start()
	{
		Material[] materials = null;
		Vector3 initialScale = base.transform.localScale;
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if (meshRenderer != null)
		{
			materials = meshRenderer.materials;
		}
		if (spawningEffect != null)
		{
			Vector3 center = meshRenderer.bounds.center;
			Object.Instantiate(spawningEffect, center, Quaternion.identity, base.transform);
		}
		yield return new WaitForSeconds(lifeTime);
		if (vanishingEffect != null)
		{
			Vector3 center2 = meshRenderer.bounds.center;
			Object.Instantiate(vanishingEffect, center2, Quaternion.identity, base.transform);
		}
		Color[] oldColor = new Color[materials.Length];
		if (materials != null && materials.Length != 0)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				Material material = materials[i];
				if (material != null)
				{
					if (material.HasColor("_BaseColor"))
					{
						oldColor[i] = material.GetColor("_BaseColor");
					}
					else if (material.HasColor("_Color"))
					{
						oldColor[i] = material.GetColor("_Color");
					}
					else
					{
						oldColor[i] = Color.white;
					}
				}
			}
		}
		float t = 0f;
		while (t < fadeTime)
		{
			float t2 = t / fadeTime;
			if (materials != null && materials.Length != 0)
			{
				for (int j = 0; j < materials.Length; j++)
				{
					Material material2 = materials[j];
					if (material2 != null)
					{
						if (material2.HasColor("_BaseColor"))
						{
							material2.SetColor("_BaseColor", Color.Lerp(oldColor[j], new Color(0f, 0f, 0f, 1f), t2));
						}
						else if (material2.HasColor("_Color"))
						{
							material2.SetColor("_Color", Color.Lerp(oldColor[j], new Color(0f, 0f, 0f, 1f), t2));
						}
					}
				}
			}
			base.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t2);
			t += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
