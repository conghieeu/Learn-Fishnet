using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class RenderPerformanceManager : MonoBehaviour
{
	private float currentRenderScale = 1f;

	private UniversalRenderPipelineAsset? urpAsset;

	private UniversalRenderPipelineAsset? GetURPAsset()
	{
		if ((object)urpAsset == null)
		{
			urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		}
		return urpAsset;
	}

	public bool IsURPAssetAvailable()
	{
		return GetURPAsset() != null;
	}

	public void SetRenderScale(float value)
	{
		if (!(GetURPAsset() != null))
		{
			return;
		}
		currentRenderScale = value;
		urpAsset.renderScale = value;
		VolumeStack stack = VolumeManager.instance.stack;
		if (stack != null)
		{
			ReLUSystemControl component = stack.GetComponent<ReLUSystemControl>();
			if (component != null)
			{
				component.internal_ReLURenderScale.value = value;
			}
		}
		else
		{
			Shader.SetGlobalFloat("_ReLURenderScale", value);
		}
	}

	public void ResetRenderScaleBelowFHD()
	{
		if (IsCurrentResolutionBelowFHD())
		{
			ResetRenderScale();
		}
	}

	public void ResetRenderScale()
	{
		SetRenderScale(1f);
	}

	public bool IsCurrentResolutionBelowFHD()
	{
		if (Screen.height <= 1200)
		{
			return true;
		}
		return false;
	}
}
