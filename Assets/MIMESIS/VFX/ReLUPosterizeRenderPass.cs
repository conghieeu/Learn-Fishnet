using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class ReLUPosterizeRenderPass : ScriptableRenderPass
{
	private static readonly int stepsId = Shader.PropertyToID("_Steps");

	private static readonly int gammaId = Shader.PropertyToID("_Gamma");

	private const string k_PosterizeTextureName = "_PosterizeTexture";

	private const string k_PosterizePassName = "PosterizeRenderPass";

	private Material material;

	private RenderTextureDescriptor blurTextureDescriptor;

	public ReLUPosterizeRenderPass(Material material)
	{
		this.material = material;
		blurTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
	}

	private void UpdateShaderValues()
	{
		if (!(material == null))
		{
			ReLUPosterizeVolume component = VolumeManager.instance.stack.GetComponent<ReLUPosterizeVolume>();
			int num = (component.steps.overrideState ? component.steps.value : 5);
			float value = (component.gamma.overrideState ? component.gamma.value : 1f);
			material.SetFloat(stepsId, num);
			material.SetFloat(gammaId, value);
		}
	}

	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
	{
		if (material == null)
		{
			return;
		}
		ReLUPosterizeVolume component = VolumeManager.instance.stack.GetComponent<ReLUPosterizeVolume>();
		if (component == null || !component.IsActive())
		{
			return;
		}
		UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
		if (universalResourceData == null)
		{
			return;
		}
		UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
		if (universalCameraData != null && !universalResourceData.isActiveTargetBackBuffer)
		{
			blurTextureDescriptor.width = universalCameraData.cameraTargetDescriptor.width;
			blurTextureDescriptor.height = universalCameraData.cameraTargetDescriptor.height;
			blurTextureDescriptor.depthBufferBits = 0;
			TextureHandle activeColorTexture = universalResourceData.activeColorTexture;
			TextureHandle textureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, blurTextureDescriptor, "_PosterizeTexture", clear: false);
			if (activeColorTexture.IsValid() && textureHandle.IsValid())
			{
				UpdateShaderValues();
				renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(activeColorTexture, textureHandle, material, 0), "PosterizeRenderPass", "D:\\Repository\\ProjectMIMIC\\Source\\mimicUnity\\Assets\\_mimic\\scripts\\system\\ReLUPosterizeRenderPass.cs", 84);
				renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(textureHandle, activeColorTexture, material, 1), "PosterizeRenderPass", "D:\\Repository\\ProjectMIMIC\\Source\\mimicUnity\\Assets\\_mimic\\scripts\\system\\ReLUPosterizeRenderPass.cs", 87);
			}
		}
	}
}
