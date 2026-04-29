using System;
using UnityEngine;

namespace StylizedPointLight
{
	[ExecuteInEditMode]
	public class LightInstancer : MonoBehaviour
	{
		[SerializeField]
		private GameObject lightPrefab;

		[SerializeField]
		private int instances = 10;

		[SerializeField]
		private float offset = 1f;

		[SerializeField]
		private float lightSize = 50f;

		[SerializeField]
		private bool refresh;

		private Vector2[] phyllotaxisOffset;

		private static float _degree = 137.5f;

		private void GenerateGrid()
		{
			ClearGrid();
			phyllotaxisOffset = new Vector2[instances];
			for (int i = 0; i < instances; i++)
			{
				phyllotaxisOffset[i] = CalculatePhyllotaxis(i, offset);
			}
			for (int j = 0; j < instances; j++)
			{
				Vector3 position = new Vector3(phyllotaxisOffset[j].x, 0f, phyllotaxisOffset[j].y);
				position += base.transform.position;
				GameObject obj = UnityEngine.Object.Instantiate(lightPrefab, base.transform);
				obj.transform.position = position;
				obj.transform.localScale = Vector3.one * lightSize;
			}
		}

		public static Vector2 CalculatePhyllotaxis(int pointCount, float pointRadius)
		{
			double num = (float)pointCount * (_degree * (MathF.PI / 180f));
			float num2 = pointRadius * Mathf.Sqrt(pointCount);
			float x = num2 * (float)Math.Cos(num);
			float y = num2 * (float)Math.Sin(num);
			return new Vector2(x, y);
		}

		private void ClearGrid()
		{
			GameObject[] array = new GameObject[base.transform.childCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = base.transform.GetChild(i).gameObject;
			}
			GameObject[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				_ = array2[j];
			}
		}

		private void OnValidate()
		{
			if (!Application.isPlaying && refresh)
			{
				refresh = false;
				GenerateGrid();
			}
		}
	}
}
