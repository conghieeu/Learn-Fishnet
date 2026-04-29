using System.Collections.Generic;

namespace DLAgent
{
	public class InferenceLogElement
	{
		public int index;

		public List<float> Observation { get; set; }

		public List<float> Action { get; set; }

		public InferenceLogElement()
		{
			Observation = new List<float>();
			Action = new List<float>();
		}
	}
}
