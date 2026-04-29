using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Mimic.GRPC.NET;
using Mimic.Voice.SpeechSystem;
using ReluProtocol;
using ReluProtocol.Enum;
using ReluReplay.Data;
using ReluReplay.Shared;
using Relugames.Replay;
using UnityEngine;

namespace ReluReplay.Recorder
{
	public class ReplayRecorder
	{
		private enum E_STATE
		{
			NONE = 0,
			READY = 1,
			RECORDING = 2
		}

		private static readonly HashSet<Type> s_capturedPackets = new HashSet<Type>();

		private E_STATE _recordState;

		private ReplayManager _replayManager;

		private ReplayData _replayData;

		private IMsg _prevSendCapturedMsg;

		private long _prevSendTime;

		private string _serverUrl;

		private GrpcChannel _channel;

		private ReplayTransfer.ReplayTransferClient _client;

		public bool UseRecord => ReplaySharedData.GetMode() == ReplaySharedData.E_MODE.RECORD;

		public ReplayRecorder(ReplayManager manager)
		{
			_replayManager = manager;
		}

		private bool IsRecordingState()
		{
			return _recordState == E_STATE.RECORDING;
		}

		public static void AddCapturedPacket(Type type)
		{
			s_capturedPackets.Add(type);
		}

		private bool CheckCapturedPacket(Type type)
		{
			return s_capturedPackets.Contains(type);
		}

		private bool CanRecordReplayData(IMsg msg)
		{
			if (IsRecordingState())
			{
				return CheckCapturedPacket(msg.GetType());
			}
			return false;
		}

		private bool CheckBroadcastPacket(IMsg msg, long time)
		{
			if (msg.msgType != _prevSendCapturedMsg?.msgType)
			{
				return false;
			}
			if (msg == _prevSendCapturedMsg)
			{
				return true;
			}
			if (msg is IActorMsg actorMsg)
			{
				if (!(_prevSendCapturedMsg is IActorMsg actorMsg2))
				{
					return false;
				}
				if (actorMsg.actorID == actorMsg2.actorID && time - _prevSendTime < 100)
				{
					return true;
				}
			}
			return false;
		}

		public bool CapturePacket(IMsg msg)
		{
			if (!UseRecord || msg == null)
			{
				return false;
			}
			if (!CanRecordReplayData(msg))
			{
				return false;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			_prevSendCapturedMsg = msg;
			if (!_replayData.AddPacketMsg(msg, currentTickMilliSec))
			{
				OnCancelRecording();
				return false;
			}
			return true;
		}

		public void ReadyRecordingForDeathMatch()
		{
			_recordState = E_STATE.READY;
			ReplayMetaData metaData = new ReplayMetaData
			{
				FilePath = ReplaySharedData.GetBaseReplaySaveFilePath(),
				NextDungeonMasterID = 0,
				RandDungeonSeed = 0,
				RecordStartTime = Hub.s.timeutil.GetCurrentTickMilliSec(),
				PlayerInfoList = new List<PlayerMetaInfo>(4),
				MapInfoDanger = 0,
				MapInfoToxicity = 0,
				MapInfoReward = 0,
				TramUpgradeIDs = null
			};
			if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.TramUpgradeIDs != null)
			{
				metaData.TramUpgradeIDs = new List<int>(Hub.s.pdata.TramUpgradeIDs);
			}
			_replayData = new ReplayData(in metaData, ReplayData.E_GAME_MODE.DEATHMATCH);
			InitSendStorage();
		}

		public void ReadyRecording(int nextDungeonMasterID, int randDungeonSeed, int inMapDanger, int inMapToxicity, int inMapReward)
		{
			_recordState = E_STATE.READY;
			ReplayMetaData metaData = new ReplayMetaData
			{
				FilePath = ReplaySharedData.GetBaseReplaySaveFilePath(),
				NextDungeonMasterID = nextDungeonMasterID,
				RandDungeonSeed = randDungeonSeed,
				RecordStartTime = Hub.s.timeutil.GetCurrentTickMilliSec(),
				PlayerInfoList = new List<PlayerMetaInfo>(4),
				MapInfoDanger = inMapDanger,
				MapInfoToxicity = inMapToxicity,
				MapInfoReward = inMapReward,
				TramUpgradeIDs = null
			};
			if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.TramUpgradeIDs != null)
			{
				metaData.TramUpgradeIDs = new List<int>(Hub.s.pdata.TramUpgradeIDs);
			}
			try
			{
				_replayData = new ReplayData(in metaData, ReplayData.E_GAME_MODE.INGAME);
			}
			catch
			{
				Hub.s.replayManager.OnCancelRecording();
				Logger.RError("[ReplayData] ReplayData - Create ReplayData Exception");
				return;
			}
			InitSendStorage();
		}

		public void StopRecording()
		{
			_recordState = E_STATE.NONE;
			if (_replayData != null)
			{
				bool flag = _replayData.OnStopRecording();
				int valueOrDefault = (Hub.s?.pdata?.main?.GetPlayersCount()).GetValueOrDefault();
				bool flag2 = false;
				flag2 = UnityEngine.Random.Range(0, 10000) < Mathf.Clamp(25, 0, 10000);
				if (CheckUploadForce() || (flag && flag2 && 4 <= valueOrDefault))
				{
					UploadReplayDataToStorage(_replayData.PlayFilePath).Forget();
					UploadReplayDataToStorage(_replayData.SndFilePath).Forget();
				}
				else
				{
					DeleteSendedFile(_replayData.PlayFilePath);
					DeleteSendedFile(_replayData.SndFilePath);
				}
				_replayData = null;
			}
		}

		public void OnCancelRecording()
		{
			_recordState = E_STATE.NONE;
			if (_replayData != null)
			{
				DeleteSendedFile(_replayData.PlayFilePath);
				DeleteSendedFile(_replayData.SndFilePath);
			}
			ReplaySharedData.SetNormalMode();
			_replayData = null;
		}

		private void OnApplicationQuit()
		{
			if (_recordState == E_STATE.RECORDING && _replayData != null)
			{
				StopRecording();
				try
				{
					UploadReplayDataToStorageSync(_replayData.PlayFilePath);
					UploadReplayDataToStorageSync(_replayData.SndFilePath);
				}
				catch (Exception)
				{
				}
			}
		}

		public void SetSessionCtxEvent(SessionContext sessionCtx)
		{
			sessionCtx.SetPreSessionCtxSendEvent(OnPreSendHookEvent);
		}

		public void OnPreSendHookEvent(IMsg msg, IContext ctx)
		{
			if (!UseRecord || msg == null || ctx == null || !CanRecordReplayData(msg))
			{
				return;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (!CheckBroadcastPacket(msg, currentTickMilliSec))
			{
				_prevSendCapturedMsg = msg;
				if (!_replayData.AddPacketMsg(msg, currentTickMilliSec))
				{
					OnCancelRecording();
				}
			}
		}

		public bool OnPreDispatcherHookEvent(IMsg msg, IContext ctx)
		{
			if (!UseRecord)
			{
				return true;
			}
			if (CanRecordReplayData(msg))
			{
				long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
				if (!_replayData.AddPacketMsg(msg, currentTickMilliSec))
				{
					OnCancelRecording();
					return false;
				}
				return true;
			}
			if (CheckMsgTypeForStartRecord(msg))
			{
				StartRecording();
			}
			return true;
		}

		public void OnRecordVoiceFile(int actorID, SpeechEvent speechEvent)
		{
			if (_replayData != null)
			{
				long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
				if (!_replayData.AddSndSpeechEvent(actorID, currentTickMilliSec, speechEvent))
				{
					OnCancelRecording();
				}
			}
		}

		private bool CheckMsgTypeForStartRecord(IMsg msg)
		{
			if (_recordState != E_STATE.READY)
			{
				return false;
			}
			if (msg.msgType == MsgType.C2S_LevelLoadCompleteReq)
			{
				return true;
			}
			return false;
		}

		private void StartRecording()
		{
			_recordState = E_STATE.RECORDING;
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (_recordState == E_STATE.RECORDING && _replayData != null)
			{
				_replayData.OnStopRecording();
				Logger.RError($"Exception Recording stop on crash: {e.ExceptionObject}");
				try
				{
					UploadReplayDataToStorageSync(_replayData.PlayFilePath);
					UploadReplayDataToStorageSync(_replayData.SndFilePath);
				}
				catch (Exception)
				{
				}
			}
		}

		private void UploadReplayDataToStorageSync(string filePath)
		{
			UploadReplayDataToStorage(filePath).GetAwaiter().GetResult();
		}

		private bool CheckUploadForce()
		{
			if (Hub.s?.ReplayUploadWhiteList == null || Hub.s?.SteamConnector == null)
			{
				return false;
			}
			if (Hub.s.ReplayUploadWhiteList.Contains(Hub.s.SteamConnector.SteamId))
			{
				return true;
			}
			return false;
		}

		private void InitSendStorage()
		{
			_serverUrl = "https://mimesisapi.relugameservice.com:22443";
			GRPCBestHttpHandler httpHandler = new GRPCBestHttpHandler();
			_channel = GrpcChannel.ForAddress(_serverUrl, new GrpcChannelOptions
			{
				HttpHandler = httpHandler
			});
			_client = new ReplayTransfer.ReplayTransferClient(_channel);
		}

		private async UniTask UploadReplayDataToStorage(string filePath)
		{
			_ = 4;
			try
			{
				if (!File.Exists(filePath))
				{
					return;
				}
				using AsyncClientStreamingCall<UploadReplayDataRequest, UploadReplayDataResponse> call = _client.UploadReplayData();
				string fileName = Path.GetFileName(filePath);
				ReplayMetadata metadata = new ReplayMetadata
				{
					Filename = fileName,
					MimeType = "application/octet-stream",
					TotalSize = (ulong)new FileInfo(filePath).Length
				};
				Logger.RLog($"Starting upload : {DateTime.Now}");
				await call.RequestStream.WriteAsync(new UploadReplayDataRequest
				{
					Metadata = metadata
				});
				using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				byte[] buffer = new byte[65536];
				int count;
				while ((count = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
				{
					await call.RequestStream.WriteAsync(new UploadReplayDataRequest
					{
						Chunk = ByteString.CopyFrom(buffer, 0, count)
					});
				}
				await call.RequestStream.CompleteAsync();
				await call.ResponseAsync;
				Logger.RLog($"Upload completed : {DateTime.Now}");
			}
			catch (RpcException)
			{
			}
			finally
			{
				DeleteSendedFile(filePath);
			}
		}

		public bool DeleteSendedFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				try
				{
					File.Delete(filePath);
				}
				catch (IOException)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}
}
