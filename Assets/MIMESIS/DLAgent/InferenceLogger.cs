using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DLAgent
{
	public class InferenceLogger
	{
		private List<InferenceLogElement> logEntries = new List<InferenceLogElement>();

		private List<float> currentObservation;

		private List<float> currentAction;

		public string fileName = "inference_log.csv";

		public void RecordObservation(List<float> observation)
		{
			currentObservation = observation;
		}

		public void RecordAction(List<float> action)
		{
			currentAction = action;
			int num = 0;
			InferenceLogElement inferenceLogElement = new InferenceLogElement();
			inferenceLogElement.Action = currentAction;
			inferenceLogElement.Observation = currentObservation;
			inferenceLogElement.index = num;
			logEntries.Add(inferenceLogElement);
			num++;
			currentObservation = null;
			currentAction = null;
		}

		public void SaveToFile()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ReluGames", "Mimesis", "InferenceLog");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			text = Path.Combine(text, fileName);
			using (StreamWriter streamWriter = new StreamWriter(text))
			{
				streamWriter.WriteLine("Observation,Action");
				foreach (InferenceLogElement logEntry in logEntries)
				{
					string text2 = string.Join(",", logEntry.Observation);
					string text3 = string.Join(",", logEntry.Action);
					streamWriter.WriteLine(text2 + "," + text3);
				}
			}
			Debug.Log("Inference log saved to " + text);
		}
	}
}
