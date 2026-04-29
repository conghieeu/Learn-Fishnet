using System;
using UnityEngine.Rendering;

[Serializable]
[VolumeComponentMenu("ReLU/Posterize")]
public class ReLUPosterizeVolume : VolumeComponent, IPostProcessComponent
{
	public BoolParameter enable = new BoolParameter(value: false);

	public ClampedIntParameter steps = new ClampedIntParameter(5, 1, 1024);

	public ClampedFloatParameter gamma = new ClampedFloatParameter(1f, 0.01f, 10f);

	public bool IsActive()
	{
		return enable.value;
	}

	public bool IsTileCompatible()
	{
		return false;
	}
}
