using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class LCDShaderPropertyControl : MonoBehaviour
{
	public bool execInEditor = true;

	[ColorUsage(true, true)]
	public Color emmisiveTint = Color.white;

	private Material runtimeMat;

	private Image img;

	private int EmissiveTintID = Shader.PropertyToID("_EmissiveTint");

	private void Start()
	{
		RetrieveMaterial();
	}

	private void Update()
	{
		if (runtimeMat != null)
		{
			runtimeMat.SetColor(EmissiveTintID, emmisiveTint);
		}
	}

	private void RetrieveMaterial()
	{
		img = GetComponent<Image>();
		if (img == null)
		{
			runtimeMat = null;
			return;
		}
		runtimeMat = new Material(img.material);
		EmissiveTintID = Shader.PropertyToID("_EmissiveTint");
	}
}
