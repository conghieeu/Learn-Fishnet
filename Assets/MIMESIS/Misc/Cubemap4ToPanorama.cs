using System.IO;
using UnityEngine;

public class Cubemap4ToPanorama : MonoBehaviour
{
	public Texture2D texFront;

	public Texture2D texBack;

	public Texture2D texLeft;

	public Texture2D texRight;

	public Cubemap texCube;

	public Shader panoramaShader;

	[ContextMenu("Generate Panoramic Texture")]
	public void GeneratePanorama()
	{
		if (panoramaShader == null || texFront == null || texBack == null || texLeft == null || texRight == null)
		{
			Debug.LogError("Missing textures or shader");
			return;
		}
		int num = 2048;
		int num2 = 1024;
		Material material = new Material(panoramaShader);
		material.SetTexture("_TexFront", texFront);
		material.SetTexture("_TexBack", texBack);
		material.SetTexture("_TexLeft", texLeft);
		material.SetTexture("_TexRight", texRight);
		material.SetTexture("_MainTex", texCube);
		RenderTexture renderTexture = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGB32);
		renderTexture.Create();
		RenderTexture.active = renderTexture;
		Graphics.Blit(null, renderTexture, material);
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		string text = Path.Combine(Application.dataPath, "PanoramicOutput.png");
		File.WriteAllBytes(text, bytes);
		Debug.Log("Saved to: " + text);
		RenderTexture.active = null;
		renderTexture.Release();
		Object.DestroyImmediate(material);
	}
}
