using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(-1000)]
public class ThinPerfHud : MonoBehaviour
{
	private Stopwatch[] stopWatches;

	private List<List<long>> history = new List<List<long>>();

	[SerializeField]
	private LineRenderer[] lineRenderers;

	[SerializeField]
	private long divider = 10000L;

	[SerializeField]
	private LineRenderer guideLine;

	[SerializeField]
	private LineRenderer customGuideLine;

	[SerializeField]
	private MeshRenderer legend;

	private bool visible;

	private bool customVisible;

	private float customLineScale = 1f;

	private Camera _orthoCamera;

	private void Awake()
	{
		for (int i = 0; i < 10; i++)
		{
			Object.Destroy(lineRenderers[i].gameObject);
		}
		Object.Destroy(guideLine.gameObject);
		Object.Destroy(customGuideLine.gameObject);
		Object.Destroy(legend.gameObject);
	}

	[Conditional("USE_THINPERFHUD")]
	public void SetCustomHUDScale(int miliseconds)
	{
	}

	[Conditional("USE_THINPERFHUD")]
	public void AtStart(PerfCat catergory)
	{
		if (catergory >= PerfCat.Main && (int)catergory < stopWatches.Length)
		{
			stopWatches[(int)catergory].Start();
		}
	}

	[Conditional("USE_THINPERFHUD")]
	public void AtEnd(PerfCat category)
	{
		if (category >= PerfCat.Main && (int)category < stopWatches.Length)
		{
			stopWatches[(int)category].Stop();
		}
	}

	[Conditional("USE_THINPERFHUD")]
	public void AttachToMainCamera()
	{
		Camera component = GetComponent<Camera>();
		Camera main = Camera.main;
		if (!(main == null) && !(component == null))
		{
			UniversalAdditionalCameraData component2 = main.GetComponent<UniversalAdditionalCameraData>();
			if (!component2.cameraStack.Contains(component))
			{
				component2.cameraStack.Add(component);
			}
		}
	}
}
