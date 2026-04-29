using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Animator))]
[ExecuteAlways]
public class UIShaderDriver : MonoBehaviour
{
	private static readonly int ChromAberrAmountID = Shader.PropertyToID("_ChromAberrAmount");

	private static readonly int GlitchAmountID = Shader.PropertyToID("_GlitchAmount");

	private static readonly int GlitchSizeID = Shader.PropertyToID("_GlitchSize");

	private Animator anim;

	private Image img;

	private Material runtimeMat;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		img = GetComponent<Image>();
		if (img != null && img.material != null)
		{
			runtimeMat = (Application.isPlaying ? new Material(img.material) : Object.Instantiate(img.material));
			runtimeMat.EnableKeyword("GLITCH_ON");
			runtimeMat.EnableKeyword("CHROMABERR_ON");
			img.material = runtimeMat;
		}
	}

	private void LateUpdate()
	{
		ApplyValues();
	}

	private void OnDidApplyAnimationProperties()
	{
		ApplyValues();
	}

	private void ApplyValues()
	{
		if (!(runtimeMat == null) && !(anim == null))
		{
			runtimeMat.SetFloat(ChromAberrAmountID, anim.GetFloat("ChromAberrAmount"));
			runtimeMat.SetFloat(GlitchAmountID, anim.GetFloat("GlitchAmount"));
			runtimeMat.SetFloat(GlitchSizeID, anim.GetFloat("GlitchSize"));
		}
	}

	private void OnDestroy()
	{
		if (runtimeMat != null)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(runtimeMat);
			}
			else
			{
				Object.DestroyImmediate(runtimeMat);
			}
		}
	}
}
