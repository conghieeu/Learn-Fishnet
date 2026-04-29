using System;

namespace Mimic.Voice.SpeechSystem
{
	[Serializable]
	public struct Complex
	{
		public double Real { get; set; }

		public double Imaginary { get; set; }

		public double Magnitude => Math.Sqrt(Real * Real + Imaginary * Imaginary);

		public double Phase => Math.Atan2(Imaginary, Real);

		public Complex(double real, double imaginary)
		{
			Real = real;
			Imaginary = imaginary;
		}

		public static Complex FromPolar(double magnitude, double phase)
		{
			return new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
		}

		public static Complex operator +(Complex a, Complex b)
		{
			return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
		}

		public static Complex operator -(Complex a, Complex b)
		{
			return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
		}

		public static Complex operator *(Complex a, Complex b)
		{
			return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary, a.Real * b.Imaginary + a.Imaginary * b.Real);
		}
	}
}
