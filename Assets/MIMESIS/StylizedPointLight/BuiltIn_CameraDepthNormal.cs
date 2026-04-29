using UnityEngine;

namespace StylizedPointLight
{
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class BuiltIn_CameraDepthNormal : MonoBehaviour
	{
		private Camera cam;

		private void Start()
		{
			cam = GetComponent<Camera>();
			cam.depthTextureMode |= DepthTextureMode.Depth;
			cam.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
	}
}
