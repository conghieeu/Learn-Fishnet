using UnityEngine;

public class EffectBase : MonoBehaviour
{
	[SerializeField]
	public float lifespan = 1f;

	public bool loop;

	private void Start()
	{
		if (!loop)
		{
			Invoke("DestroySelf", lifespan);
		}
	}

	private void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
