using System;

namespace Mimic.Voice.SpeechSystem
{
	public static class AudioFFT
	{
		public static Complex[] FFT(float[] timeData)
		{
			int num = timeData.Length;
			if ((num & (num - 1)) != 0)
			{
				int num2;
				for (num2 = 1; num2 < num; num2 <<= 1)
				{
				}
				Array.Resize(ref timeData, num2);
				num = num2;
			}
			Complex[] array = new Complex[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new Complex(timeData[i], 0.0);
			}
			return FFTRecursive(array);
		}

		private static Complex[] FFTRecursive(Complex[] x)
		{
			int num = x.Length;
			if (num <= 1)
			{
				return x;
			}
			Complex[] array = new Complex[num / 2];
			Complex[] array2 = new Complex[num / 2];
			for (int i = 0; i < num / 2; i++)
			{
				array[i] = x[2 * i];
				array2[i] = x[2 * i + 1];
			}
			Complex[] array3 = FFTRecursive(array);
			Complex[] array4 = FFTRecursive(array2);
			Complex[] array5 = new Complex[num];
			for (int j = 0; j < num / 2; j++)
			{
				Complex complex = Complex.FromPolar(1.0, Math.PI * -2.0 * (double)j / (double)num) * array4[j];
				array5[j] = array3[j] + complex;
				array5[j + num / 2] = array3[j] - complex;
			}
			return array5;
		}

		public static float[] GetMagnitudeSpectrum(Complex[] fftResult)
		{
			float[] array = new float[fftResult.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (float)fftResult[i].Magnitude;
			}
			return array;
		}
	}
}
