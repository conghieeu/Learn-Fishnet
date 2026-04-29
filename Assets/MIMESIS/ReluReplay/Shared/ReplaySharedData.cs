using System;
using System.Collections.Generic;
using System.IO;
using ReluReplay.Data;
using UnityEngine;

namespace ReluReplay.Shared
{
	public static class ReplaySharedData
	{
		public enum E_MODE
		{
			NONE = 0,
			RECORD = 1,
			PLAY = 2
		}

		public enum E_PLAY_MODE
		{
			NORMAL = 0,
			REPEAT = 1,
			PLAY_AND_EXTRACT = 2
		}

		public enum E_EVENT
		{
			DEAD_FROM_MIMIC = 0,
			INCOMING_MIMIC = 1,
			USE_SWITCH = 2
		}

		public struct EmotionData
		{
			public float TimeRatio;

			public string ActorName;

			public E_EVENT Emotion;
		}

		private struct ReplayFileInfo
		{
			public string FileName;

			public string FilePath;

			public long FileSize;

			public long StartGameTime;

			public long EndGameTime;

			public List<int> PlayerIds;

			public List<string> PlayerNames;

			public List<int> MapInfo;

			public ReplaySaveInfo SaveInfo;
		}

		private static E_MODE _mode = E_MODE.NONE;

		private static List<ReplayFileInfo> _replayFileList = new List<ReplayFileInfo>();

		private static int _currentSelectedReplayFileIdx = -1;

		private static string _baseReplaySaveFilePath = string.Empty;

		private static E_PLAY_MODE _PlayMode = E_PLAY_MODE.NORMAL;

		private static float _resorvedTimeSliderValue = 0f;

		private static float _timeSliderValue = 0f;

		private static float _timeSliderMaxValue = 0f;

		private static string _clickedActorName = string.Empty;

		private static List<EmotionData> _replayEmotionData = new List<EmotionData>();

		public static Action<string, float> OnReservedTime;

		public static Action OnReady;

		public static bool IsReplayPlayMode => _mode == E_MODE.PLAY;

		public static bool IsReplayPlayNormalMode()
		{
			return _PlayMode == E_PLAY_MODE.NORMAL;
		}

		public static bool GetRepeatPlayMode()
		{
			return _PlayMode == E_PLAY_MODE.REPEAT;
		}

		public static void SetRepeatPlayMode(bool value)
		{
			_PlayMode = (value ? E_PLAY_MODE.REPEAT : E_PLAY_MODE.NORMAL);
		}

		public static void SetPlayMode(E_PLAY_MODE playMode)
		{
			_PlayMode = playMode;
		}

		public static bool IsPlayAndExtractMode()
		{
			return _PlayMode == E_PLAY_MODE.PLAY_AND_EXTRACT;
		}

		public static float GetReservedTimeSliderValue()
		{
			return _resorvedTimeSliderValue;
		}

		public static void SetReservedTimeSliderValue(float value)
		{
			_resorvedTimeSliderValue = value;
		}

		public static void SetTimeSliderValue(float value)
		{
			_timeSliderValue = value;
		}

		public static float GetTimeSliderValue()
		{
			return _timeSliderValue;
		}

		public static void SetTimeSliderMaxValue(float value)
		{
			_timeSliderMaxValue = value;
		}

		public static float GetTimeSliderMaxValue()
		{
			return _timeSliderMaxValue;
		}

		public static float GetProgressBarValue()
		{
			if (!(_timeSliderMaxValue > 0f))
			{
				return 0f;
			}
			return _timeSliderValue / _timeSliderMaxValue;
		}

		public static string GetClickedActorName()
		{
			return _clickedActorName;
		}

		public static void SetClickedActorName(string actorName)
		{
			_clickedActorName = actorName;
		}

		public static void RegisterBaseSaveReplayFilePath(string path = null)
		{
			if (string.IsNullOrEmpty(path))
			{
				_baseReplaySaveFilePath = Path.Combine(Application.persistentDataPath, "Replay");
				if (!Directory.Exists(_baseReplaySaveFilePath))
				{
					Directory.CreateDirectory(_baseReplaySaveFilePath);
				}
			}
			else
			{
				_baseReplaySaveFilePath = path;
			}
		}

		public static string GetBaseReplaySaveFilePath()
		{
			if (string.IsNullOrEmpty(_baseReplaySaveFilePath))
			{
				RegisterBaseSaveReplayFilePath();
			}
			return _baseReplaySaveFilePath;
		}

		public static void SetReplayFilePath(string replayFilePath, string replayFileName, long replayFileSize)
		{
			if (!string.IsNullOrEmpty(replayFilePath) && !string.IsNullOrEmpty(replayFileName) && replayFileSize > 0)
			{
				IReplayHeader replayHeader = ReplayData.LoadReplayHeaderData(replayFilePath);
				if (replayHeader != null)
				{
					_replayFileList.Add(new ReplayFileInfo
					{
						FilePath = replayFilePath,
						FileName = replayFileName,
						FileSize = replayFileSize,
						StartGameTime = replayHeader.GetReplayRecordStartTime(),
						EndGameTime = replayHeader.GetReplayRecordEndTime(),
						PlayerIds = (replayHeader.GetPlayerActorIDs()?.Clone() ?? new List<int>()),
						PlayerNames = (replayHeader.GetPlayerActorNames()?.Clone() ?? new List<string>()),
						MapInfo = (replayHeader.GetMapInfos()?.Clone() ?? new List<int>()),
						SaveInfo = new ReplaySaveInfo(replayHeader.GetSaveInfo())
					});
				}
			}
		}

		public static void RemoveReplayFilePath(int replayFileListIdx)
		{
			if (replayFileListIdx >= 0 && replayFileListIdx < _replayFileList.Count)
			{
				_replayFileList.RemoveAt(replayFileListIdx);
			}
		}

		public static int GetCurrentSelectedReplayFileIdx()
		{
			return _currentSelectedReplayFileIdx;
		}

		public static void SetCurrentSelectedReplayFileIdx(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				_currentSelectedReplayFileIdx = -1;
			}
			else
			{
				_currentSelectedReplayFileIdx = replayFileListIdx;
			}
		}

		public static string GetSelectedReplayFilePath()
		{
			return GetReplayFilePath(_currentSelectedReplayFileIdx);
		}

		public static string GetNextSelectedReplayFilePath()
		{
			_currentSelectedReplayFileIdx++;
			return GetReplayFilePath(_currentSelectedReplayFileIdx);
		}

		public static string GetReplayFilePath(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				return string.Empty;
			}
			return _replayFileList[replayFileListIdx].FilePath;
		}

		public static string GetReplayFileName(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				return string.Empty;
			}
			return _replayFileList[replayFileListIdx].FileName;
		}

		public static long GetReplayFileSize(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				return -1L;
			}
			return _replayFileList[replayFileListIdx].FileSize;
		}

		public static string GetReplayFileEndGameTime(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				return string.Empty;
			}
			long endGameTime = _replayFileList[replayFileListIdx].EndGameTime;
			if (endGameTime <= 0)
			{
				return string.Empty;
			}
			endGameTime -= _replayFileList[replayFileListIdx].StartGameTime;
			return DateTimeOffset.FromUnixTimeMilliseconds(endGameTime).DateTime.ToString("HH:mm:ss");
		}

		public static string GetReplayFileMapInfo(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				return "-/-/-";
			}
			if (_replayFileList[replayFileListIdx].MapInfo == null)
			{
				return "-/-/-";
			}
			if (_replayFileList[replayFileListIdx].MapInfo.Count == 0 && _replayFileList[replayFileListIdx].MapInfo.Count != 3)
			{
				return "-/-/-";
			}
			List<int> mapInfo = _replayFileList[replayFileListIdx].MapInfo;
			return $"{mapInfo[0]}/{mapInfo[1]}/{mapInfo[2]}";
		}

		public static List<int> GetReplayFilePlayerIds(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				return new List<int>();
			}
			return _replayFileList[replayFileListIdx].PlayerIds;
		}

		public static List<string> GetReplayFilePlayerNames(int replayFileListIdx)
		{
			if (replayFileListIdx < 0 || replayFileListIdx >= _replayFileList.Count)
			{
				return new List<string>();
			}
			return _replayFileList[replayFileListIdx].PlayerNames;
		}

		public static bool IsEmpty()
		{
			return _replayFileList.Count <= 0;
		}

		public static int GetReplayFileListCount()
		{
			return _replayFileList.Count;
		}

		public static void Clear()
		{
			_currentSelectedReplayFileIdx = -1;
			ClearTimeProgress();
			_replayFileList.Clear();
			SetNormalMode();
		}

		public static void ClearTimeProgress()
		{
			_timeSliderValue = 0f;
			_timeSliderMaxValue = 0f;
		}

		public static E_MODE GetMode()
		{
			return _mode;
		}

		public static void SetPlayMode()
		{
			_mode = E_MODE.PLAY;
		}

		public static void SetRecordMode()
		{
			_mode = E_MODE.RECORD;
		}

		public static void SetNormalMode()
		{
			_mode = E_MODE.NONE;
		}

		public static void SetEmotionData(List<MsgWithTime> data)
		{
			_replayEmotionData.Clear();
			if (data == null || data.Count <= 0 || _currentSelectedReplayFileIdx < 0 || _currentSelectedReplayFileIdx >= _replayFileList.Count)
			{
				return;
			}
			long startGameTime = _replayFileList[_currentSelectedReplayFileIdx].StartGameTime;
			long endGameTime = _replayFileList[_currentSelectedReplayFileIdx].EndGameTime;
			for (int i = 0; i < data.Count; i++)
			{
				if (data[i].msg is DebugMimicVoiceEmotionSig debugMimicVoiceEmotionSig)
				{
					_replayEmotionData.Add(new EmotionData
					{
						TimeRatio = (float)(data[i].time - startGameTime) / (float)(endGameTime - startGameTime),
						ActorName = debugMimicVoiceEmotionSig.ActorName,
						Emotion = (E_EVENT)debugMimicVoiceEmotionSig.EmotionData
					});
				}
			}
		}

		public static List<EmotionData> GetEmotionData()
		{
			return _replayEmotionData;
		}
	}
}
