using UnityEngine;

namespace EzySlice
{
	public struct TextureRegion
	{
		private readonly float pos_start_x;

		private readonly float pos_start_y;

		private readonly float pos_end_x;

		private readonly float pos_end_y;

		public float startX => pos_start_x;

		public float startY => pos_start_y;

		public float endX => pos_end_x;

		public float endY => pos_end_y;

		public Vector2 start => new Vector2(startX, startY);

		public Vector2 end => new Vector2(endX, endY);

		public TextureRegion(float startX, float startY, float endX, float endY)
		{
			pos_start_x = startX;
			pos_start_y = startY;
			pos_end_x = endX;
			pos_end_y = endY;
		}

		public Vector2 Map(Vector2 uv)
		{
			return Map(uv.x, uv.y);
		}

		public Vector2 Map(float x, float y)
		{
			float x2 = MAP(x, 0f, 1f, pos_start_x, pos_end_x);
			float y2 = MAP(y, 0f, 1f, pos_start_y, pos_end_y);
			return new Vector2(x2, y2);
		}

		private static float MAP(float x, float in_min, float in_max, float out_min, float out_max)
		{
			return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
		}
	}
}
