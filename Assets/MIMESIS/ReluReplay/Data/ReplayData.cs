using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using MemoryPack;
using Mimic.Voice.SpeechSystem;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluReplay.Serializer;

namespace ReluReplay.Data
{
	public class ReplayData
	{
		public enum REPLAY_DATA_TYPE
		{
			PLAY = 0,
			VOICE = 1
		}

		public struct PlayLoopData
		{
			public REPLAY_DATA_TYPE Type;

			public int index;
		}

		public enum E_GAME_MODE
		{
			NONE = 0,
			INGAME = 1,
			DEATHMATCH = 2
		}

		public static readonly string ReplayFilePrefix = "Replay";

		public static readonly string ReplayFileExt = "replay";

		public static readonly string ReplayVoiceFilePostfix = "snd";

		private E_GAME_MODE _gameModeState;

		private IReplayHeader _replayHeader;

		private const int MAX_FLUSH_PLAY_DATA_COUNT = 500;

		private const int MAX_FLUSH_VOICE_DATA_COUNT = 10;

		private readonly string _baseFilePath;

		private string _playFilePath;

		private string _voiceFilePath;

		private List<MsgWithTime> _playData;

		private List<SndWithTime> _voiceData;

		private SortedDictionary<long, List<PlayLoopData>> _totalData;

		private List<long> _totalDataTimeList = new List<long>();

		private bool _isLoaded;

		private List<MsgWithTime> _startSpawnData = new List<MsgWithTime>();

		private HashSet<int> _uniquePlayerActorIDs = new HashSet<int>();

		private List<MsgWithTime> _debugMimicVoiceEmotionData = new List<MsgWithTime>();

		private ReplayMetaData _replayMetaDataForRecord;

		private HashSet<int> _spawnedPlayerIds;

		private HashSet<int> _spawnedMimicIds;

		private HashSet<int> _spawnedMonsterIds;

		private StringBuilder _sb = new StringBuilder();

		private HashSet<char> _invalidFileNameChars;

		public E_GAME_MODE GameModeState => _gameModeState;

		public string BaseFilePath => _baseFilePath;

		public string PlayFilePath => _playFilePath;

		public string SndFilePath => _voiceFilePath;

		public bool IsLoaded => _isLoaded;

		public List<MsgWithTime> StartSpawnData => _startSpawnData;

		public List<MsgWithTime> DebugMimicVoiceEmotionData => _debugMimicVoiceEmotionData;

		public int PlayerCount => _uniquePlayerActorIDs.Count;

		public int PlayLoopDataCount => _totalDataTimeList.Count;

		public int NextDungeonMasterID => _replayHeader?.GetDungeonMasterID() ?? (-1);

		public int RandDungeonSeed => _replayHeader?.GetDungeonRandSeed() ?? (-1);

		public long RecordStartTime => _replayHeader?.GetReplayRecordStartTime() ?? (-1);

		public long RecordEndTime => _replayHeader?.GetReplayRecordEndTime() ?? (-1);

		public ReplaySaveInfo SaveInfo => _replayHeader?.GetSaveInfo();

		public ReplayData(in string playFilePath)
		{
			_baseFilePath = Path.GetDirectoryName(playFilePath);
			_playFilePath = playFilePath;
			_voiceFilePath = GetVoiceFilePath(in playFilePath);
			SetupGameModeFromFileName(in playFilePath);
		}

		private void SetupGameModeFromFileName(in string fileName)
		{
			string text = fileName.Split("_")[1];
			if (text.Equals("DeathMatch"))
			{
				_gameModeState = E_GAME_MODE.DEATHMATCH;
			}
			else if (text.Equals("InGame"))
			{
				_gameModeState = E_GAME_MODE.INGAME;
			}
		}

		public void LoadReplayData(Action onDataLoadCompleteCallBack = null)
		{
			if (_isLoaded)
			{
				return;
			}
			FileStream fileStream = new FileStream(_playFilePath, FileMode.Open, FileAccess.Read);
			_replayHeader = LoadReplayHeaderData(in fileStream);
			try
			{
				LoadReplayDataAsync(fileStream).ContinueWith(delegate
				{
					onDataLoadCompleteCallBack?.Invoke();
					fileStream.Close();
				});
			}
			catch (Exception ex)
			{
				Logger.RError("Failed to load replay data: " + ex.Message);
				fileStream.Close();
			}
		}

		public static IReplayHeader LoadReplayHeaderData(string playFilePath)
		{
			if (!File.Exists(playFilePath))
			{
				Logger.RError("Replay file not found: " + playFilePath);
				return null;
			}
			using FileStream fileStream = new FileStream(playFilePath, FileMode.Open, FileAccess.Read);
			return LoadReplayHeaderData(in fileStream);
		}

		private static IReplayHeader LoadReplayHeaderData(in FileStream fileStream)
		{
			byte[] array = new byte[4];
			fileStream.Read(array, 0, array.Length);
			if (Encoding.UTF8.GetString(array) != "RELU")
			{
				fileStream.Seek(0L, SeekOrigin.Begin);
				return LoadReplayLegacyHeaderData(in fileStream);
			}
			return LoadReplayNewHeaderData(in fileStream);
		}

		private static IReplayHeader LoadReplayNewHeaderData(in FileStream fileStream)
		{
			byte[] array = new byte[4];
			fileStream.Read(array, 0, array.Length);
			byte[] array2 = new byte[BitConverter.ToInt32(array, 0)];
			fileStream.Read(array2, 0, array2.Length);
			IReplayHeader result = null;
			try
			{
				result = MemoryPackSerializer.Deserialize<ReplayHeader>(array2);
			}
			catch (Exception ex)
			{
				Logger.RError("Failed to deserialize replay header: " + ex.Message);
			}
			return result;
		}

		private static IReplayHeader LoadReplayLegacyHeaderData(in FileStream fileStream)
		{
			byte[] array = new byte[4];
			fileStream.Read(array, 0, array.Length);
			byte[] array2 = new byte[BitConverter.ToInt32(array, 0)];
			fileStream.Read(array2, 0, array2.Length);
			IReplayHeader result = null;
			try
			{
				result = MemoryPackSerializer.Deserialize<LegacyReplayHeader>(array2);
			}
			catch (Exception ex)
			{
				Logger.RError("Failed to deserialize legacy replay header: " + ex.Message);
			}
			return result;
		}

		public async UniTask LoadReplayDataAsync(FileStream fileStream)
		{
			UniTask<List<MsgWithTime>> task = UniTask.RunOnThreadPool(() => LoadPlayDataAsync(fileStream));
			UniTask<List<SndWithTime>> task2 = UniTask.RunOnThreadPool((Func<UniTask<List<SndWithTime>>>)LoadVoiceDataAsync, configureAwait: true, default(CancellationToken));
			(List<MsgWithTime>, List<SndWithTime>) tuple = await UniTask.WhenAll(task, task2);
			_isLoaded = true;
			(_playData, _voiceData) = tuple;
			await MakePlayDatas();
		}

		private async UniTask<List<MsgWithTime>> LoadPlayDataAsync(FileStream fileStream)
		{
			return await ReplaySerializer.DeserializeMsgFromFileAsync(fileStream);
		}

		private async UniTask<List<SndWithTime>> LoadVoiceDataAsync()
		{
			if (File.Exists(_voiceFilePath))
			{
				return await ReplaySerializer.DeserializeSndFromFileAsync(_voiceFilePath);
			}
			return new List<SndWithTime>();
		}

		public MsgWithTime GetPlayDataByIndex(int index)
		{
			if (_playData == null)
			{
				return null;
			}
			return _playData[index];
		}

		public SndWithTime GetVoiceDataByIndex(int index)
		{
			if (_voiceData == null)
			{
				return null;
			}
			return _voiceData[index];
		}

		public long GetLastPlayLoopDataTime()
		{
			if (_totalDataTimeList == null || _totalDataTimeList.Count == 0)
			{
				return -1L;
			}
			List<long> totalDataTimeList = _totalDataTimeList;
			return totalDataTimeList[totalDataTimeList.Count - 1];
		}

		public (long, List<PlayLoopData>) GetPlayLoopDataByIndex(int index)
		{
			if (_totalData == null || index < 0 || index >= _totalDataTimeList.Count)
			{
				return (-1L, null);
			}
			long num = _totalDataTimeList[index];
			return (num, GetPlayLoopDataByTimeKey(num));
		}

		public List<PlayLoopData> GetPlayLoopDataByTimeKey(long timeKey)
		{
			if (_totalData == null)
			{
				return null;
			}
			if (_totalData.TryGetValue(timeKey, out var value))
			{
				return value;
			}
			return null;
		}

		public List<PlayLoopData> GetNextPlayLoopDataByTime(long time)
		{
			if (_totalData == null)
			{
				return null;
			}
			if (_totalData.TryGetValue(time, out var value))
			{
				return value;
			}
			return null;
		}

		public long? GetFirstLargerTimeKey(long time)
		{
			foreach (long totalDataTime in _totalDataTimeList)
			{
				if (totalDataTime > time)
				{
					return totalDataTime;
				}
			}
			return null;
		}

		private async UniTask MakePlayDatas()
		{
			if (_totalData == null)
			{
				_totalData = new SortedDictionary<long, List<PlayLoopData>>();
			}
			else
			{
				_totalData.Clear();
			}
			UniTask<(SortedDictionary<long, List<PlayLoopData>> localTotalData, List<long> localTotalDataTimeList, HashSet<int> localUniquePlayerActorIDs, List<MsgWithTime> localStartSpawnData, List<MsgWithTime> debugMimicVoiceEmotionData)> task = UniTask.RunOnThreadPool(delegate
			{
				SortedDictionary<long, List<PlayLoopData>> sortedDictionary = new SortedDictionary<long, List<PlayLoopData>>();
				List<long> list = new List<long>();
				HashSet<int> hashSet = new HashSet<int>();
				List<MsgWithTime> list2 = new List<MsgWithTime>();
				bool flag = false;
				bool flag2 = false;
				int num = -1;
				int num2 = 500;
				List<SightInSig> list3 = new List<SightInSig>();
				List<int> list4 = new List<int>();
				List<MsgWithTime> list5 = new List<MsgWithTime>();
				for (int i = 0; i < _playData.Count; i++)
				{
					MsgWithTime msgWithTime = _playData[i];
					if (msgWithTime.msg.msgType != MsgType.C2S_MoveStopSig)
					{
						if (msgWithTime.msg.msgType == MsgType.C2S_DebugMimicVoiceEmotionSig)
						{
							list5.Add(msgWithTime);
						}
						else
						{
							if (i < num2)
							{
								if (msgWithTime.msg.msgType == MsgType.C2S_SightInSig)
								{
									if (msgWithTime.msg is SightInSig sightInSig)
									{
										if (sightInSig.playerInfos.Count == 1)
										{
											int actorID = sightInSig.playerInfos[0].actorID;
											hashSet.Add(actorID);
											if (num == actorID)
											{
												for (int j = 0; j < list3.Count; j++)
												{
													if (list3[j].playerInfos[0].actorID == actorID)
													{
														list3[j].reliable = sightInSig.reliable;
														list3[j].parsingType = sightInSig.parsingType;
														break;
													}
												}
												continue;
											}
											list3.Add(sightInSig);
										}
										else if (sightInSig.playerInfos.Count > 1)
										{
											foreach (PlayerInfo playerInfo in sightInSig.playerInfos)
											{
												hashSet.Add(playerInfo.actorID);
											}
											list3.Add(sightInSig);
										}
										if (sightInSig.lootingObjectInfos.Count > 0 && !flag)
										{
											flag = true;
											list2.Add(msgWithTime);
											continue;
										}
									}
								}
								else
								{
									if (msgWithTime.msg.msgType == MsgType.C2S_LevelLoadCompleteRes && !flag2)
									{
										list2.Add(msgWithTime);
										if (msgWithTime.msg is LevelLoadCompleteRes levelLoadCompleteRes)
										{
											SightInSig sightInSig2 = new SightInSig
											{
												sightReason = SightReason.Spawned,
												playerInfos = new List<PlayerInfo>
												{
													new PlayerInfo
													{
														actorType = levelLoadCompleteRes.selfInfo.actorType,
														actorLifeCycle = levelLoadCompleteRes.selfInfo.actorLifeCycle,
														actorName = levelLoadCompleteRes.selfInfo.actorName,
														position = levelLoadCompleteRes.selfInfo.position.Clone(),
														UID = levelLoadCompleteRes.selfInfo.UID,
														masterID = levelLoadCompleteRes.selfInfo.masterID,
														actorID = levelLoadCompleteRes.selfInfo.actorID,
														factions = levelLoadCompleteRes.selfInfo.factions.Clone(),
														attachingActorIDs = levelLoadCompleteRes.selfInfo.attachingActorIDs.Clone(),
														attachedActorID = levelLoadCompleteRes.selfInfo.attachedActorID,
														statInfoCollection = new StatCollection
														{
															mutableStats = levelLoadCompleteRes.selfInfo.statInfoCollection.mutableStats.Clone(),
															ImmutableStats = levelLoadCompleteRes.selfInfo.statInfoCollection.ImmutableStats.Clone()
														},
														onHandItem = new ItemInfo
														{
															itemID = levelLoadCompleteRes.selfInfo.onHandItem.itemID,
															itemType = levelLoadCompleteRes.selfInfo.onHandItem.itemType,
															itemMasterID = levelLoadCompleteRes.selfInfo.onHandItem.itemMasterID,
															stackCount = levelLoadCompleteRes.selfInfo.onHandItem.stackCount,
															durability = levelLoadCompleteRes.selfInfo.onHandItem.durability,
															remainGauge = levelLoadCompleteRes.selfInfo.onHandItem.remainGauge,
															isTurnOn = levelLoadCompleteRes.selfInfo.onHandItem.isTurnOn,
															isFake = levelLoadCompleteRes.selfInfo.onHandItem.isFake
														},
														inventories = levelLoadCompleteRes.selfInfo.inventories.Clone(),
														currentInventorySlot = levelLoadCompleteRes.selfInfo.currentInventorySlot
													}
												}
											};
											num = levelLoadCompleteRes.selfInfo.actorID;
											list3.Add(sightInSig2);
											list2.Add(new MsgWithTime
											{
												time = msgWithTime.time,
												msg = sightInSig2
											});
										}
										flag2 = true;
										continue;
									}
									if (msgWithTime.msg.msgType == MsgType.C2S_MoveStartSig && msgWithTime.msg is MoveStartSig { actorID: >0 } moveStartSig && !list4.Contains(moveStartSig.actorID))
									{
										list4.Add(moveStartSig.actorID);
										foreach (SightInSig item2 in list3)
										{
											foreach (PlayerInfo playerInfo2 in item2.playerInfos)
											{
												if (playerInfo2.actorID == moveStartSig.actorID)
												{
													playerInfo2.position = moveStartSig.basePositionCurr;
												}
											}
										}
									}
								}
							}
							PlayLoopData item = new PlayLoopData
							{
								Type = REPLAY_DATA_TYPE.PLAY,
								index = i
							};
							if (sortedDictionary.TryGetValue(msgWithTime.time, out var value))
							{
								value.Add(item);
							}
							else
							{
								sortedDictionary[msgWithTime.time] = new List<PlayLoopData> { item };
								list.Add(msgWithTime.time);
							}
						}
					}
				}
				return (localTotalData: sortedDictionary, localTotalDataTimeList: list, localUniquePlayerActorIDs: hashSet, localStartSpawnData: list2, debugMimicVoiceEmotionData: list5);
			});
			UniTask<(SortedDictionary<long, List<PlayLoopData>>, List<long>)> task2 = UniTask.RunOnThreadPool(delegate
			{
				SortedDictionary<long, List<PlayLoopData>> sortedDictionary = new SortedDictionary<long, List<PlayLoopData>>();
				List<long> list = new List<long>();
				foreach (SndWithTime voiceDatum in _voiceData)
				{
					PlayLoopData item = new PlayLoopData
					{
						Type = REPLAY_DATA_TYPE.VOICE,
						index = _voiceData.IndexOf(voiceDatum)
					};
					int num = voiceDatum.SpeechEvent.Channels * voiceDatum.SpeechEvent.SampleRate;
					long num2 = 0L;
					if (num > 0)
					{
						num2 = (long)(voiceDatum.SpeechEvent.Duration * 1000f);
					}
					long num3 = voiceDatum.Time - num2;
					if (sortedDictionary.TryGetValue(num3, out var value))
					{
						value.Add(item);
					}
					else
					{
						sortedDictionary[num3] = new List<PlayLoopData> { item };
						list.Add(num3);
					}
				}
				return (localTotalData: sortedDictionary, localTotalDataTimeList: list);
			});
			((SortedDictionary<long, List<PlayLoopData>>, List<long>, HashSet<int>, List<MsgWithTime>, List<MsgWithTime>), (SortedDictionary<long, List<PlayLoopData>>, List<long>)) obj = await UniTask.WhenAll(task, task2);
			var (playTotalData, playTotalDataTimeList, uniquePlayerActorIDs, startSpawnData, debugMimicVoiceEmotionData) = obj.Item1;
			var (voiceTotalData, voiceTotalDataTimeList) = obj.Item2;
			await UniTask.WhenAll(UniTask.RunOnThreadPool(delegate
			{
				foreach (KeyValuePair<long, List<PlayLoopData>> item3 in playTotalData)
				{
					if (_totalData.TryGetValue(item3.Key, out var value))
					{
						lock (value)
						{
							value.AddRange(item3.Value);
						}
					}
					else
					{
						lock (_totalData)
						{
							_totalData[item3.Key] = item3.Value;
						}
					}
				}
			}), UniTask.RunOnThreadPool(delegate
			{
				foreach (KeyValuePair<long, List<PlayLoopData>> item4 in voiceTotalData)
				{
					if (_totalData.TryGetValue(item4.Key, out var value))
					{
						lock (value)
						{
							value.AddRange(item4.Value);
						}
					}
					else
					{
						lock (_totalData)
						{
							_totalData[item4.Key] = item4.Value;
						}
					}
				}
			}));
			_totalDataTimeList.AddRange(playTotalDataTimeList);
			_totalDataTimeList.AddRange(voiceTotalDataTimeList);
			_totalDataTimeList.Sort();
			_uniquePlayerActorIDs = uniquePlayerActorIDs;
			_startSpawnData = startSpawnData;
			_debugMimicVoiceEmotionData = debugMimicVoiceEmotionData;
		}

		public ReplayData(in ReplayMetaData metaData, E_GAME_MODE gameMode)
		{
			_spawnedPlayerIds = new HashSet<int>();
			_spawnedMimicIds = new HashSet<int>();
			_spawnedMonsterIds = new HashSet<int>();
			_playData = new List<MsgWithTime>();
			_voiceData = new List<SndWithTime>();
			_baseFilePath = metaData.FilePath;
			_gameModeState = gameMode;
			switch (gameMode)
			{
			case E_GAME_MODE.INGAME:
				SetupFilePathForInGameMode();
				break;
			case E_GAME_MODE.DEATHMATCH:
				SetupFilePathForDeathMatch();
				break;
			default:
				throw new ArgumentException($"Unsupported game mode: {gameMode}");
			}
			_replayMetaDataForRecord = metaData;
			using (new FileStream(_playFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
			{
			}
			using (new FileStream(_voiceFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
			{
			}
		}

		private string GetGitRevision()
		{
			string text = "919187f474";
			if (string.IsNullOrEmpty(text))
			{
				text = "unknown";
			}
			return text;
		}

		private void SetupFilePathForInGameMode()
		{
			string text = ReplayFilePrefix + "_InGame";
			text = text + "_" + GetGitRevision();
			text += $"_{GetCurrentRealTimeMilliSec() % 10000:D4}";
			_playFilePath = Path.Combine(_baseFilePath, text + "." + ReplayFileExt);
			_voiceFilePath = GetVoiceFilePath(in _playFilePath);
		}

		private void SetupFilePathForDeathMatch()
		{
			string text = ReplayFilePrefix + "_DeathMatch";
			text = text + "_" + GetGitRevision();
			text += $"_{GetCurrentRealTimeMilliSec() % 10000:D4}";
			_playFilePath = Path.Combine(_baseFilePath, text + "." + ReplayFileExt);
			_voiceFilePath = GetVoiceFilePath(in _playFilePath);
		}

		private long GetCurrentRealTimeMilliSec()
		{
			return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		private string GetVoiceFileName(in string playFilePath)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(playFilePath);
			return fileNameWithoutExtension + "_" + ReplayVoiceFilePostfix + "." + ReplayFileExt;
		}

		private string GetVoiceFilePath(in string playFilePath)
		{
			string voiceFileName = GetVoiceFileName(in playFilePath);
			return Path.Combine(_baseFilePath, voiceFileName);
		}

		private void SetReplayHeader(in ReplayMetaData metaData)
		{
			ReplayMepInfo mapInfo = new ReplayMepInfo
			{
				NextDungeonMasterID = metaData.NextDungeonMasterID,
				RandDungeonSeed = metaData.RandDungeonSeed,
				PlayerActorIDs = metaData.PlayerInfoList.ConvertAll((PlayerMetaInfo player) => player.ActorID),
				PlayerActorNames = metaData.PlayerInfoList.ConvertAll((PlayerMetaInfo player) => player.NickName),
				MapInfos = new List<int> { metaData.MapInfoDanger, metaData.MapInfoToxicity, metaData.MapInfoReward }
			};
			ReplaySaveInfo replaySaveInfo = new ReplaySaveInfo
			{
				TramUpgradeIDs = new List<int>()
			};
			if (metaData.TramUpgradeIDs != null)
			{
				replaySaveInfo.TramUpgradeIDs.AddRange(metaData.TramUpgradeIDs);
			}
			ReplayHeaderMeta metaInfos = new ReplayHeaderMeta
			{
				MapInfo = mapInfo,
				RecordStartTime = metaData.RecordStartTime,
				RecordEndTime = metaData.RecordEndTime,
				SaveInfo = replaySaveInfo
			};
			ReplaySerializer.AddHeaderToExistingFile(new ReplayHeader
			{
				Version = REPLAY_HEADER_VERSION.V_1_1,
				MetaInfos = metaInfos
			}, _playFilePath);
		}

		private void ChangeReplayFileName()
		{
			string directoryName = Path.GetDirectoryName(_playFilePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_playFilePath);
			string extension = Path.GetExtension(_playFilePath);
			int randDungeonSeed = _replayMetaDataForRecord.RandDungeonSeed;
			PlayerMetaInfo playerMetaInfo = _replayMetaDataForRecord.PlayerInfoList.Find((PlayerMetaInfo player) => player.ActorID == Hub.s.pdata.main.GetMyAvatar().ActorID);
			string empty = string.Empty;
			int num = 0;
			if (Hub.s != null && Hub.s.replayManager != null)
			{
				num = Hub.s.replayManager.EmotionDataCount;
			}
			string text = DateTime.UtcNow.ToString("yyMMddxHHmmss");
			empty = $"{fileNameWithoutExtension}_{SanitizeFileName(playerMetaInfo.NickName)}_{text}_{randDungeonSeed}_{num}{extension}";
			string playFilePath = Path.Combine(directoryName, empty);
			File.Move(_playFilePath, playFilePath);
			_playFilePath = playFilePath;
			string voiceFilePath = GetVoiceFilePath(in playFilePath);
			File.Move(_voiceFilePath, voiceFilePath);
			_voiceFilePath = voiceFilePath;
		}

		private bool CanFlushPlayData(IMsg msg)
		{
			if (_playData.Count < 500)
			{
				return msg.msgType == MsgType.C2S_EndDungeonSig;
			}
			return true;
		}

		public bool AddPacketMsg(IMsg msg, long time)
		{
			try
			{
				SetMetaDataFromMsg(msg);
				IMsg msg2 = ReplaySerializer.DeepCopy(msg);
				_playData.Add(new MsgWithTime
				{
					msg = msg2,
					time = time
				});
				if (CanFlushPlayData(msg2))
				{
					ReplaySerializer.SerializeMsgToFile(in _playData, in _playFilePath);
					_playData.Clear();
				}
			}
			catch (Exception arg)
			{
				Logger.RError($"[ReplayData] AddPacketMsg - Exception : {arg}");
				return false;
			}
			return true;
		}

		public bool AddSndSpeechEvent(int actorID, long time, SpeechEvent speechEvent)
		{
			try
			{
				_voiceData.Add(new SndWithTime
				{
					ActorID = actorID,
					Time = time,
					SpeechEvent = speechEvent
				});
				if (_voiceData.Count >= 10)
				{
					ReplaySerializer.SerializeSndToFile(in _voiceData, in _voiceFilePath);
					_voiceData.Clear();
				}
			}
			catch (Exception arg)
			{
				Logger.RError($"[ReplayData] AddSndSpeechEvent - Exception : {arg}");
				return false;
			}
			return true;
		}

		public void SetMetaDataFromMsg(IMsg msg)
		{
			switch (msg.msgType)
			{
			case MsgType.C2S_LevelLoadCompleteRes:
				if (msg is LevelLoadCompleteRes levelLoadCompleteRes)
				{
					SetPlayerMetaInfo(levelLoadCompleteRes.selfInfo);
				}
				break;
			case MsgType.C2S_SightInSig:
				if (msg is SightInSig sightInSig)
				{
					SetPlayerMetaInfo(sightInSig.playerInfos);
					SetMonsterMetaInfo(sightInSig.monsterInfos);
				}
				break;
			case MsgType.C2S_EndDungeonSig:
				if (msg is EndDungeonSig)
				{
					_replayMetaDataForRecord.RecordEndTime = Hub.s.timeutil.GetCurrentTickMilliSec();
				}
				else if (msg is EndDeathMatchSig)
				{
					_replayMetaDataForRecord.RecordEndTime = Hub.s.timeutil.GetCurrentTickMilliSec();
				}
				break;
			}
		}

		private void SetMonsterMetaInfo(List<OtherCreatureInfo> monsterInfos)
		{
			foreach (OtherCreatureInfo monsterInfo in monsterInfos)
			{
				if (monsterInfo.masterID == 20000001)
				{
					_spawnedMimicIds.Add(monsterInfo.actorID);
				}
				else
				{
					_spawnedMonsterIds.Add(monsterInfo.actorID);
				}
			}
		}

		private void SetPlayerMetaInfo(PlayerInfo playerInfo)
		{
			if (!CheckPlayerMetaInfoExists(playerInfo.actorID))
			{
				PlayerMetaInfo item = new PlayerMetaInfo
				{
					ActorID = playerInfo.actorID,
					NickName = playerInfo.actorName
				};
				_replayMetaDataForRecord.PlayerInfoList.Add(item);
				_spawnedPlayerIds.Add(playerInfo.actorID);
			}
		}

		private void SetPlayerMetaInfo(List<PlayerInfo> playerInfos)
		{
			foreach (PlayerInfo playerInfo in playerInfos)
			{
				SetPlayerMetaInfo(playerInfo);
			}
		}

		private bool CheckPlayerMetaInfoExists(int actorID)
		{
			return _spawnedPlayerIds.Contains(actorID);
		}

		public bool OnStopRecording()
		{
			try
			{
				FlushRemainDatas();
				_replayMetaDataForRecord.RecordEndTime = Hub.s.timeutil.GetCurrentTickMilliSec();
				_replayMetaDataForRecord.TotalPlayPacketCount = _playData.Count;
				_replayMetaDataForRecord.TotalVoicePacketCount = _voiceData.Count;
				_replayMetaDataForRecord.TotalMonsterCount = _spawnedMonsterIds.Count;
				_replayMetaDataForRecord.TotalMimicCount = _spawnedMimicIds.Count;
				_replayMetaDataForRecord.TotalPlayerCount = _spawnedPlayerIds.Count;
				SetReplayHeader(in _replayMetaDataForRecord);
				ChangeReplayFileName();
				return true;
			}
			catch (Exception arg)
			{
				Logger.RError($"[ReplayData] OnStopRecording - Exception : {arg}");
				return false;
			}
		}

		private void FlushRemainDatas()
		{
			if (_playData.Count > 0)
			{
				ReplaySerializer.SerializeMsgToFile(in _playData, in _playFilePath);
				_playData.Clear();
			}
			if (_voiceData.Count > 0)
			{
				ReplaySerializer.SerializeSndToFile(in _voiceData, in _voiceFilePath);
				_voiceData.Clear();
			}
		}

		private string SanitizeFileName(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return fileName;
			}
			if (_invalidFileNameChars == null || _invalidFileNameChars.Count == 0)
			{
				_invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
			}
			_sb.Clear();
			_sb.EnsureCapacity(fileName.Length);
			foreach (char c in fileName)
			{
				if (_invalidFileNameChars.Contains(c))
				{
					_sb.Append('-');
				}
				else
				{
					_sb.Append(c);
				}
			}
			return _sb.ToString();
		}
	}
}
