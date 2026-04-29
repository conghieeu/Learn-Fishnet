using UnityEngine;

namespace EzySlice
{
	public static class TextureRegionExtension
	{
		public static TextureRegion GetTextureRegion(this Material mat, int pixX, int pixY, int pixWidth, int pixHeight)
		{
			return mat.mainTexture.GetTextureRegion(pixX, pixY, pixWidth, pixHeight);
		}

		public static TextureRegion GetTextureRegion(this Texture tex, int pixX, int pixY, int pixWidth, int pixHeight)
		{
			int width = tex.width;
			int height = tex.height;
			int num = Mathf.Min(width, pixWidth);
			int num2 = Mathf.Min(height, pixHeight);
			int num3 = Mathf.Min(Mathf.Abs(pixX), width);
			int num4 = Mathf.Min(Mathf.Abs(pixY), height);
			float startX = (float)num3 / (float)width;
			float startY = (float)num4 / (float)height;
			float endX = (float)(num3 + num) / (float)width;
			float endY = (float)(num4 + num2) / (float)height;
			return new TextureRegion(startX, startY, endX, endY);
		}
	}
}
