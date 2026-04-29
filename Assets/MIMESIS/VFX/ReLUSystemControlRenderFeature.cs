using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class ReLUSystemControlRenderFeature : ScriptableRendererFeature
{
	private class MyPass : ScriptableRenderPass
	{
		private RenderTargetIdentifier source;

		private ReLUSystemControl volumeComponent;

		private int CTIWindPID;

		private int CTITurbulencedPID;

		public MyPass()
		{
			CTIWindPID = Shader.PropertyToID("_CTI_SRP_Wind");
			CTITurbulencedPID = Shader.PropertyToID("_CTI_SRP_Turbulence");
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			if (VolumeManager.instance == null)
			{
				return;
			}
			VolumeStack stack = VolumeManager.instance.stack;
			if (stack == null)
			{
				return;
			}
			volumeComponent = stack.GetComponent<ReLUSystemControl>();
			if (!(volumeComponent == null) && volumeComponent.IsActive())
			{
				Shader.SetGlobalColor("_TestColor", Color.white);
				Shader.SetGlobalFloat("_NearLightIntensity", volumeComponent.nearLightIntentity.value);
				Shader.SetGlobalVector("_NearLightMinMax", volumeComponent.nearLightMinMax.value);
				Shader.SetGlobalFloat("_NearLightQuantize", volumeComponent.nearLightQuantize.value * 0.5f);
				Vector3 vector = Quaternion.AngleAxis(volumeComponent.CTI_windDirection.value, Vector3.up) * Vector3.right;
				Shader.SetGlobalVector(CTIWindPID, new Vector4(vector.x, vector.y, vector.z, volumeComponent.CTI_windStrength.value));
				Shader.SetGlobalFloat(CTITurbulencedPID, volumeComponent.CTI_windTurbulence.value);
				Shader.SetGlobalFloat(Shader.PropertyToID("_ReLURenderScale"), volumeComponent.internal_ReLURenderScale.value);
				if (volumeComponent.forceChangeSkyAmbientColorEnabled.value)
				{
					RenderSettings.ambientLight = volumeComponent.forceChangeSkyAmbientColorValue.value;
				}
			}
		}
	}

	private MyPass pass;

	public override void Create()
	{
		pass = new MyPass();
		pass.renderPassEvent = RenderPassEvent.BeforeRendering;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(pass);
	}
}
