using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ReLUPosterizeFeature : ScriptableRendererFeature
{
	private Shader shader;

	private Material material;

	private ReLUPosterizeRenderPass posterizeRenderPass;

	public override void Create()
	{
		shader = Shader.Find("Hidden/ReLU/Posterize");
		if (shader == null)
		{
			Debug.LogError("Shader not found: Hidden/ReLU/Posterize");
			return;
		}
		material = new Material(shader);
		if (material == null)
		{
			Debug.LogError("Material creation failed.");
			return;
		}
		material.SetFloat("_Steps", 5f);
		material.SetFloat("_Gamma", 1f);
		posterizeRenderPass = new ReLUPosterizeRenderPass(material);
		posterizeRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (posterizeRenderPass != null && renderingData.cameraData.cameraType == CameraType.Game)
		{
			renderer.EnqueuePass(posterizeRenderPass);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(material);
		}
		else
		{
			Object.DestroyImmediate(material);
		}
	}
}
