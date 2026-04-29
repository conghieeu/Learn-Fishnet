using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DunGen.Analysis
{
	public sealed class NumberSetData
	{
		public float Min { get; private set; }

		public float Max { get; private set; }

		public float Average { get; private set; }

		public float StandardDeviation { get; private set; }

		public NumberSetData(IEnumerable<float> values)
		{
			Min = values.Min();
			Max = values.Max();
			Average = values.Sum() / (float)values.Count();
			float[] array = new float[values.Count()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Mathf.Pow(values.ElementAt(i) - Average, 2f);
			}
			StandardDeviation = Mathf.Sqrt(array.Sum() / (float)array.Length);
		}

		public override string ToString()
		{
			return $"[ Min: {Min:N1}, Max: {Max:N1}, Average: {Average:N1}, Standard Deviation: {StandardDeviation:N2} ]";
		}
	}
}
