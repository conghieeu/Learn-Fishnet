using System;
using System.IO;
using UnityEngine;

namespace DLAgent
{
	[CreateAssetMenu(fileName = "DL_Config", menuName = "MimicDL/DLConfig", order = int.MaxValue)]
	public class DL_Config : ScriptableObject
	{
		[Header("Model Configuration")]
		public string ModelPath;

		[Header("Recording Options")]
		public bool UseRecordPlayData;

		public bool UseRecordVoiceAsFile;

		public string RecordBaseFolderName = "PlayDataLogs";

		public string VoiceRecordFolderName = "SpeechEvents";

		private string _recordBasePath;

		private string _voiceRecordPath;

		[Header("Training Data Options")]
		public bool ExtractTrainingData;

		public string GetRecordBasePath()
		{
			if (string.IsNullOrEmpty(_recordBasePath))
			{
				_recordBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ReluGames", "Mimesis", "PlayLogData");
			}
			return _recordBasePath;
		}

		public string GetVoiceRecordPath()
		{
			if (string.IsNullOrEmpty(_voiceRecordPath))
			{
				_voiceRecordPath = Path.Combine(GetRecordBasePath(), "SpeechEvents");
			}
			return _voiceRecordPath;
		}
	}
}
