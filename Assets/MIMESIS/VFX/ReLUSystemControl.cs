using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[VolumeComponentMenu("ReLU/SystemControl")]
public class ReLUSystemControl : VolumeComponent, IPostProcessComponent
{
	public BoolParameter enable = new BoolParameter(value: false);

	public FloatParameter nearLightIntentity = new FloatParameter(2f);

	public Vector2Parameter nearLightMinMax = new Vector2Parameter(new Vector2(3f, 4f));

	public FloatParameter nearLightQuantize = new FloatParameter(255f);

	public FloatParameter CTI_windDirection = new FloatParameter(0f);

	public FloatParameter CTI_windStrength = new FloatParameter(1f);

	public FloatParameter CTI_windTurbulence = new FloatParameter(1f);

	public BoolParameter forceChangeSkyAmbientColorEnabled = new BoolParameter(value: false);

	public ColorParameter forceChangeSkyAmbientColorValue = new ColorParameter(Color.white);

	public FloatParameter internal_ReLURenderScale = new FloatParameter(1f);

	public bool IsActive()
	{
		return enable.value;
	}

	public bool IsTileCompatible()
	{
		return false;
	}
}
