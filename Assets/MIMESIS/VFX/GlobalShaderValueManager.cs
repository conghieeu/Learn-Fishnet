using UnityEngine;

[ExecuteInEditMode]
public class GlobalShaderValueManager : MonoBehaviour
{
	public Color _TestColor;

	public float _NearLightIntensity;

	public Vector2 _NearLightMinMax;

	private void Update()
	{
		Shader.SetGlobalColor("_TestColor", _TestColor);
		Shader.SetGlobalFloat("_NearLightIntensity", _NearLightIntensity);
		Shader.SetGlobalVector("_NearLightMinMax", _NearLightMinMax);
	}
}
