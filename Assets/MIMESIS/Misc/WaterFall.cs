using UnityEngine;

public class WaterFall : MonoBehaviour
{
	public float fallSpeed = 1f;

	public Material mat;

	private void Start()
	{
		mat = GetComponent<MeshRenderer>().sharedMaterial;
	}

	private void OnDisable()
	{
		if (mat != null)
		{
			Vector2 mainTextureOffset = mat.mainTextureOffset;
			mainTextureOffset = new Vector2(0f, 0f);
			mat.mainTextureOffset = mainTextureOffset;
		}
	}

	private void Update()
	{
		if (mat != null)
		{
			Vector2 mainTextureOffset = mat.mainTextureOffset;
			mainTextureOffset.y += Time.deltaTime * fallSpeed;
			mat.mainTextureOffset = mainTextureOffset;
		}
	}
}
