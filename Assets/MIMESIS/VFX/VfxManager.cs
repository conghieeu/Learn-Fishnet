using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
	private List<ParticleSystem> activated = new List<ParticleSystem>();

	private Coroutine lifeTimeCheckCoroutine;

	private IEnumerator CheckLifetimeAndDestory()
	{
		while (activated != null)
		{
			for (int num = activated.Count - 1; num >= 0; num--)
			{
				if (activated[num] != null && activated[num].time >= activated[num].main.duration)
				{
					Object.Destroy(activated[num].gameObject);
					activated.RemoveAt(num);
				}
			}
			yield return new WaitForSeconds(5f);
		}
	}

	private void Start()
	{
		Logger.RLog("[AwakeLogs] VfxManager.Start ->");
		lifeTimeCheckCoroutine = StartCoroutine(CheckLifetimeAndDestory());
		Logger.RLog("[AwakeLogs] VfxManager.Start <-");
	}

	private void OnDestroy()
	{
		if (lifeTimeCheckCoroutine != null)
		{
			StopCoroutine(lifeTimeCheckCoroutine);
			lifeTimeCheckCoroutine = null;
		}
	}

	public GameObject InstantiateVfx(string key, Transform parent)
	{
		MMVfxTable.Row row = Hub.s.tableman.vfx.rows.FirstOrDefault((MMVfxTable.Row x) => x.id == key);
		if (row == null)
		{
			Logger.RError("[VfxManager] Vfx not found: " + key);
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject gameObject = row.Instantiate(parent);
		activated.Add(gameObject.GetComponent<ParticleSystem>());
		return gameObject;
	}

	public GameObject InstantiateVfx(string key, Vector3 position)
	{
		MMVfxTable.Row row = Hub.s.tableman.vfx.rows.FirstOrDefault((MMVfxTable.Row x) => x.id == key);
		if (row == null)
		{
			Logger.RError("[VfxManager] Vfx not found: " + key);
			return null;
		}
		GameObject gameObject = row.Instantiate(position);
		activated.Add(gameObject.GetComponent<ParticleSystem>());
		return gameObject;
	}
}
