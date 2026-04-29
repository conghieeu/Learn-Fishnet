using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bifrost.Cooked;
using Dissonance;
using Dissonance.Audio.Codecs.Opus;
using Dissonance.Integrations.FishNet;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using FishNet.Serializing.Generated;
using FishNet.Transporting;
using Mimic.Actors;
using OggVorbis;
using ReluNetwork.ConstEnum;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using UnityEngine;

namespace Mimic.Voice.SpeechSystem
{
	[RequireComponent(typeof(FishNetDissonancePlayer))]
	public class SpeechEventArchive : NetworkBehaviour
	{
		[Header("음성 풀 관리")]
		[SerializeField]
		[Tooltip("보관할 모든 음성 기록의 최대 갯수입니다.(노말 + 데스매치)")]
		private int maxEvents = 128;

		[SerializeField]
		[Tooltip("보관할 데스매치 음성 기록의 최대 갯수입니다.")]
		private int maxDeathMatchEvents = 20;

		[SerializeField]
		[Tooltip("보관할 아웃도어 음성 기록의 최대 갯수입니다.")]
		private int maxOutDoorEvents = 30;

		[SerializeField]
		[Tooltip("음성이 녹음된 이후 사용하기 까지의 유예시간(초)")]
		private float warmUpDuration = 10f;

		[Header("음성 재생 전략")]
		[SerializeField]
		[Tooltip("음성 재생 시 인접한 플레이어(미믹 포함)의 기준이 되는 거리 값")]
		private float adjacentRadius = 10f;

		private List<SpeechEvent> _normalEventsTemp = new List<SpeechEvent>();

		private List<SpeechEvent> _deathMatchEventsTemp = new List<SpeechEvent>();

		private List<SpeechEvent> _outDoorEventsTemp = new List<SpeechEvent>();

		[AllowMutableSyncType]
		[SerializeField]
		public SyncList<SpeechEvent> events = new SyncList<SpeechEvent>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.OwnerOnly, 999999f, Channel.Reliable));

		[Header("청킹 동기화 설정")]
		private int chunkSize = 3;

		private float chunkInterval = 1f;

		private bool isInitialSyncComplete;

		private bool NetworkInitialize___EarlyMimic_002EVoice_002ESpeechSystem_002ESpeechEventArchiveAssembly_002DCSharp_002Edll_Excuted;

		private bool NetworkInitialize__LateMimic_002EVoice_002ESpeechSystem_002ESpeechEventArchiveAssembly_002DCSharp_002Edll_Excuted;

		public FishNetDissonancePlayer Player => GetComponent<FishNetDissonancePlayer>();

		public string PlayerId => Player.PlayerId;

		public long PlayerUID => Player.PlayerUID;

		public bool IsLocal => Player.IsOwner;

		public int RandomPoolSize => GetSpeechEventRandomPool().Count;

		public int SpeechEventPoolSize => events.Count;

		private DissonanceComms comms => DissonanceFishNetComms.Instance.Comms;

		private VoiceManager voiceman
		{
			get
			{
				if (!(Hub.s != null))
				{
					return null;
				}
				return Hub.s.voiceman;
			}
		}

		private SpeechEventRecorder eventRecorder
		{
			get
			{
				if (!(Hub.s != null) || !(Hub.s.voiceman != null))
				{
					return null;
				}
				return Hub.s.voiceman.speechEventRecorder;
			}
		}

		public override void OnStartClient()
		{
			voiceman.RegisterClient(this, Player);
			events.OnChange += events_onChange;
			if (!base.IsOwner)
			{
				if (!isInitialSyncComplete)
				{
					ServerRpcRequestInitialSync();
				}
			}
			else
			{
				eventRecorder.OnSpeechEventRecorded += OnSpeechEventRecorded;
			}
		}

		public override void OnStopClient()
		{
			if (voiceman != null)
			{
				voiceman.UnregisterClient(this, Player);
			}
			if (eventRecorder != null)
			{
				eventRecorder.OnSpeechEventRecorded -= OnSpeechEventRecorded;
			}
			isInitialSyncComplete = false;
			events.OnChange -= events_onChange;
		}

		public bool TryPlayRandomEventAtPoint(Vector3 position)
		{
			if (!TryGetRandomSpeechEvent(position, out var speechEvent))
			{
				return false;
			}
			AudioClip clip = CreateAudioClip(speechEvent);
			bool isSpectatorMode = Hub.s.cameraman.IsSpectatorMode;
			Hub.s.legacyAudio.PlayVoiceOneShotAtPoint(clip, position, isSpectatorMode);
			return true;
		}

		public bool TryPlayRandomEventInsideCircle(Vector3 center, float circleRadius)
		{
			Vector3 position = center;
			if (circleRadius > 0f)
			{
				Vector3 vector = UnityEngine.Random.insideUnitCircle * circleRadius;
				position += vector;
			}
			return TryPlayRandomEventAtPoint(position);
		}

		public void SendToServerVoiceEmotion(int inActorID, string inActorName, ReplaySharedData.E_EVENT inEmotion)
		{
			if (base.IsOwner)
			{
				SendToServerVoiceEmotionServerRpc(inActorID, inActorName, (byte)inEmotion);
			}
		}

		public bool IsFullSpeechEventPool()
		{
			return maxEvents - maxDeathMatchEvents <= events.Count;
		}

		[ObserversRpc]
		internal void ObserverRpcPlayOnActor(long speechEventId, int actorID, bool muteLocalPlayerVoice)
		{
			RpcWriter___Observers_ObserverRpcPlayOnActor_1543699021(speechEventId, actorID, muteLocalPlayerVoice);
		}

		[ServerRpc(RequireOwnership = false)]
		public void ServerRpcPlayRandomEventOnActor(long speechEventId, int actorID, bool muteLocalPlayerVoice)
		{
			RpcWriter___Server_ServerRpcPlayRandomEventOnActor_1543699021(speechEventId, actorID, muteLocalPlayerVoice);
		}

		private List<long> AddEvent(SpeechEvent speechEvent)
		{
			events.Add(speechEvent);
			return RemoveLowerValueEventsIfExceeded();
		}

		private List<long> RemoveLowerValueEventsIfExceeded()
		{
			_normalEventsTemp.Clear();
			_deathMatchEventsTemp.Clear();
			_outDoorEventsTemp.Clear();
			for (int i = 0; i < events.Count; i++)
			{
				if (events[i].GameData.Area == SpeechType_Area.DeathMatch)
				{
					_deathMatchEventsTemp.Add(events[i]);
				}
				else if (events[i].GameData.Area == SpeechType_Area.Outdoor || events[i].GameData.Area == SpeechType_Area.Tram)
				{
					_outDoorEventsTemp.Add(events[i]);
				}
				else
				{
					_normalEventsTemp.Add(events[i]);
				}
			}
			List<long> list = new List<long>();
			int num = maxEvents - maxDeathMatchEvents - maxOutDoorEvents;
			if (_normalEventsTemp.Count > num)
			{
				int count = _normalEventsTemp.Count - num;
				list.AddRange(from e in _normalEventsTemp.OrderByDescending((SpeechEvent e) => EvaluateValue(in e)).TakeLast(count)
					select e.Id);
			}
			if (_outDoorEventsTemp.Count > maxOutDoorEvents)
			{
				int count2 = _outDoorEventsTemp.Count - maxOutDoorEvents;
				list.AddRange(from e in _outDoorEventsTemp.OrderByDescending((SpeechEvent e) => EvaluateValue(in e)).TakeLast(count2)
					select e.Id);
			}
			if (_deathMatchEventsTemp.Count > maxDeathMatchEvents)
			{
				int count3 = _deathMatchEventsTemp.Count - maxDeathMatchEvents;
				list.AddRange(from e in _deathMatchEventsTemp.OrderByDescending((SpeechEvent e) => EvaluateValue(in e)).TakeLast(count3)
					select e.Id);
			}
			foreach (long id in list)
			{
				events.RemoveAll((SpeechEvent e) => e.Id == id);
			}
			return list;
		}

		private bool TryGetRandomSpeechEvent(Vector3 pos, out SpeechEvent speechEvent)
		{
			List<SpeechEvent> speechEventRandomPool = GetSpeechEventRandomPool();
			Hub.s.dLAcademyManager.GetAreaForDL(pos, out var areaType);
			return speechEventRandomPool.Where((SpeechEvent e) => e.GameData != null && e.GameData.Area == areaType).ToList().TryPickRandom(out speechEvent);
		}

		public List<SpeechEvent> GetSpeechEventRandomPool()
		{
			float timeNow = Hub.s.timeutil.GetCurrentTickSec();
			return events.Where((SpeechEvent e) => timeNow - e.RecordedTime > warmUpDuration).ToList();
		}

		public List<SpeechEvent> GetSpeechEventPool()
		{
			return events.ToList();
		}

		private AudioClip CreateAudioClip(SpeechEvent speechEvent)
		{
			if (OpusAudioUtility.IsOpusData(speechEvent.CompressedAudioData))
			{
				return OpusAudioUtility.ToAudioClip(speechEvent.CompressedAudioData, speechEvent.Id.ToString());
			}
			return null;
		}

		public void CreateOggFile(SpeechEvent speechEvent)
		{
			try
			{
				AudioClip audioClip = null;
				if (OpusAudioUtility.IsOpusData(speechEvent.CompressedAudioData))
				{
					audioClip = OpusAudioUtility.ToAudioClip(speechEvent.CompressedAudioData, speechEvent.Id.ToString());
				}
				if (audioClip == null)
				{
					Logger.RError($"Failed to create audio clip for speech event: {speechEvent.Id}");
					return;
				}
				string text = Application.persistentDataPath + "/OggFiles";
				string path = speechEvent.Id + ".ogg";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				VorbisPlugin.Save(Path.Combine(text, path), audioClip);
			}
			catch (Exception arg)
			{
				Logger.RError($"Failed to create ogg file for speech event: {speechEvent.Id}\nException: {arg}");
			}
		}

		private float EvaluateValue(in SpeechEvent speechEvent)
		{
			return -speechEvent.AudioPlayedCount;
		}

		private void OnSpeechEventRecorded(SpeechEvent speechEvent, bool isForce)
		{
			if (speechEvent.PlayerName != comms.LocalPlayerName)
			{
				return;
			}
			List<long> list = AddEvent(speechEvent);
			if (Hub.s != null && Hub.s.pdata != null && (Hub.s.pdata.serverRoomState == Hub.PersistentData.eServerRoomState.InGame || isForce))
			{
				ServerRpcBroadcastNewEventWithRemoval(speechEvent, list.ToArray());
			}
			else
			{
				Logger.RWarn($"[DEBUG] NOT calling ObserversRpc. Hub.s={Hub.s != null}, pdata={Hub.s?.pdata != null}, State={Hub.s?.pdata?.serverRoomState}", sendToLogServer: false, useConsoleOut: true, "mimicvoice");
			}
			if (0 >= speechEvent.GameData.IncomingEvent.Count)
			{
				return;
			}
			foreach (IncomingEvent item in speechEvent.GameData.IncomingEvent)
			{
				_ = item.EventType;
				_ = 1;
			}
		}

		private void events_onChange(SyncListOperation op, int index, SpeechEvent oldItem, SpeechEvent newItem, bool asServer)
		{
			if (base.IsHostStarted && !asServer)
			{
				return;
			}
			SpeechEvent speechEvent = ((op == SyncListOperation.Add) ? newItem : oldItem);
			switch (op)
			{
			case SyncListOperation.Add:
				if (newItem != null)
				{
					OnAddSpeechEvent(newItem, asServer);
				}
				break;
			case SyncListOperation.RemoveAt:
				if (oldItem != null)
				{
					SendSpeechEventsChanged(op, speechEvent);
				}
				break;
			}
		}

		private void OnAddSpeechEvent(SpeechEvent speechEvent, bool asServer)
		{
			if (asServer && Hub.s.voiceman.SpawnMimicVoicEventOnce(PlayerUID))
			{
				SendSpeechEventsChanged(SyncListOperation.Add, speechEvent);
			}
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene != null && gamePlayScene.DlHelper != null)
			{
				gamePlayScene.DlHelper.RecordVoiceFile(PlayerUID, speechEvent);
			}
		}

		private void SendSpeechEventsChanged(SyncListOperation op, SpeechEvent speechEvent)
		{
			if (Hub.s == null || Hub.s.pdata == null || Hub.s.replayManager == null || Hub.s.pdata.ClientMode != NetworkClientMode.Host)
			{
				return;
			}
			int num = ((op == SyncListOperation.Add) ? 1 : (-1));
			if (Hub.s.replayManager.UseRecordMode)
			{
				ProtoActor actorByPlayerUID = voiceman.GetActorByPlayerUID(PlayerUID);
				if (!(actorByPlayerUID == null))
				{
					Hub.s.replayManager.OnCapturePacketOnlyReplay(new DebugSpeechEventDeltaSig
					{
						actorID = actorByPlayerUID.ActorID,
						delta = (sbyte)num,
						adjacentCount = speechEvent.GameData.AdjacentPlayerCount,
						area = speechEvent.GameData.Area,
						gameTime = speechEvent.GameData.GameTime,
						facingCount = speechEvent.GameData.FacingPlayerCount,
						teleporter = speechEvent.GameData.Teleporter,
						indoorEntered = speechEvent.GameData.IndoorEntered,
						charger = speechEvent.GameData.Charger,
						crowShop = speechEvent.GameData.CrowShop,
						incomingEvents = (speechEvent.GameData.IncomingEvent?.ToArray() ?? Array.Empty<IncomingEvent>()),
						scrapObjects = (speechEvent.GameData.ScrapObjects?.ToArray() ?? Array.Empty<int>()),
						monsters = (speechEvent.GameData.Monsters?.ToArray() ?? Array.Empty<int>())
					});
				}
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void ServerRpcRequestInitialSync(NetworkConnection conn = null)
		{
			RpcWriter___Server_ServerRpcRequestInitialSync_328543758(conn);
		}

		private IEnumerator CoSendInitialChunks(NetworkConnection conn)
		{
			_ = conn.ClientId;
			int totalEvents = events.Count;
			int sentCount = 0;
			while (sentCount < totalEvents)
			{
				int num = Mathf.Min(chunkSize, totalEvents - sentCount);
				SpeechEvent[] array = new SpeechEvent[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = events[sentCount + i];
				}
				bool flag = sentCount + num >= totalEvents;
				TargetRpcReceiveChunk(conn, array, flag, sentCount, totalEvents);
				sentCount += num;
				if (!flag)
				{
					yield return new WaitForSeconds(chunkInterval);
				}
			}
			if (totalEvents == 0)
			{
				TargetRpcReceiveChunk(conn, new SpeechEvent[0], isFinalChunk: true, 0, 0);
			}
		}

		[TargetRpc]
		private void TargetRpcReceiveChunk(NetworkConnection conn, SpeechEvent[] chunk, bool isFinalChunk, int currentCount, int totalCount)
		{
			RpcWriter___Target_TargetRpcReceiveChunk_3872555705(conn, chunk, isFinalChunk, currentCount, totalCount);
		}

		[ObserversRpc]
		private void ObserversRpcReceiveNewEventWithRemoval(SpeechEvent newEvent, long[] idsToRemove)
		{
			RpcWriter___Observers_ObserversRpcReceiveNewEventWithRemoval_1161837147(newEvent, idsToRemove);
		}

		[ServerRpc]
		private void ServerRpcBroadcastNewEventWithRemoval(SpeechEvent newEvent, long[] idsToRemove)
		{
			RpcWriter___Server_ServerRpcBroadcastNewEventWithRemoval_1161837147(newEvent, idsToRemove);
		}

		[ServerRpc]
		private void SendToServerVoiceEmotionServerRpc(int inActorID, string inActorName, byte inEmotionData)
		{
			RpcWriter___Server_SendToServerVoiceEmotionServerRpc_4002388585(inActorID, inActorName, inEmotionData);
		}

		public virtual void NetworkInitialize___Early()
		{
			if (!NetworkInitialize___EarlyMimic_002EVoice_002ESpeechSystem_002ESpeechEventArchiveAssembly_002DCSharp_002Edll_Excuted)
			{
				NetworkInitialize___EarlyMimic_002EVoice_002ESpeechSystem_002ESpeechEventArchiveAssembly_002DCSharp_002Edll_Excuted = true;
				events.InitializeEarly(this, 0u, isSyncObject: true);
				RegisterObserversRpc(0u, RpcReader___Observers_ObserverRpcPlayOnActor_1543699021);
				RegisterServerRpc(1u, RpcReader___Server_ServerRpcPlayRandomEventOnActor_1543699021);
				RegisterServerRpc(2u, RpcReader___Server_ServerRpcRequestInitialSync_328543758);
				RegisterTargetRpc(3u, RpcReader___Target_TargetRpcReceiveChunk_3872555705);
				RegisterObserversRpc(4u, RpcReader___Observers_ObserversRpcReceiveNewEventWithRemoval_1161837147);
				RegisterServerRpc(5u, RpcReader___Server_ServerRpcBroadcastNewEventWithRemoval_1161837147);
				RegisterServerRpc(6u, RpcReader___Server_SendToServerVoiceEmotionServerRpc_4002388585);
			}
		}

		public virtual void NetworkInitialize__Late()
		{
			if (!NetworkInitialize__LateMimic_002EVoice_002ESpeechSystem_002ESpeechEventArchiveAssembly_002DCSharp_002Edll_Excuted)
			{
				NetworkInitialize__LateMimic_002EVoice_002ESpeechSystem_002ESpeechEventArchiveAssembly_002DCSharp_002Edll_Excuted = true;
				events.InitializeLate();
			}
		}

		public override void NetworkInitializeIfDisabled()
		{
			NetworkInitialize___Early();
			NetworkInitialize__Late();
		}

		private void RpcWriter___Observers_ObserverRpcPlayOnActor_1543699021(long speechEventId, int actorID, bool muteLocalPlayerVoice)
		{
			if (!base.IsServerInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteInt64(speechEventId);
			pooledWriter.WriteInt32(actorID);
			pooledWriter.WriteBoolean(muteLocalPlayerVoice);
			SendObserversRpc(0u, pooledWriter, channel, DataOrderType.Default, bufferLast: false, excludeServer: false, excludeOwner: false);
			pooledWriter.Store();
		}

		internal void RpcLogic___ObserverRpcPlayOnActor_1543699021(long speechEventId, int actorID, bool muteLocalPlayerVoice)
		{
			if (voiceman == null)
			{
				Logger.RError("Voice manager is not set.");
				return;
			}
			SpeechEvent speechEvent = events.Find((SpeechEvent e) => e.Id == speechEventId);
			if (speechEvent == null)
			{
				return;
			}
			ProtoActor actorByPlayerUID = voiceman.GetActorByPlayerUID(PlayerUID);
			bool flag = voiceman.voiceMode == VoiceMode.Observer;
			if (!Hub.s.replayManager.IsReplayPlayMode && ((!flag && muteLocalPlayerVoice && IsLocal) || (flag && muteLocalPlayerVoice && actorByPlayerUID != null && Hub.s.cameraman.IsCurrentSpectatorTarget(actorByPlayerUID.ActorID))))
			{
				return;
			}
			ProtoActor protoActor = UnityEngine.Object.FindObjectsByType<ProtoActor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault((ProtoActor a) => a.ActorID == actorID);
			if (protoActor == null)
			{
				return;
			}
			AudioClip clip = CreateAudioClip(speechEvent);
			if (speechEvent.GameData.Area == SpeechType_Area.Transmitter)
			{
				if (!(Hub.s.pdata.main is GamePlayScene gamePlayScene))
				{
					return;
				}
				if (Hub.s.cameraman.IsSpectatorMode)
				{
					if (!Hub.s.cameraman.TryGetCurrentSpectatorTarget(out ProtoActor target) || !gamePlayScene.CheckActorIsIndoor(target))
					{
						return;
					}
					protoActor.PlayVoiceOnActor(clip, isTransmitter: true, isMimicVoiceEcho: true);
				}
				else
				{
					if (!gamePlayScene.IsAvatarIndoor)
					{
						return;
					}
					protoActor.PlayVoiceOnActor(clip, isTransmitter: true, isMimicVoiceEcho: false);
				}
			}
			else if (protoActor.IsMimic())
			{
				protoActor.PlayVoiceOnActor(clip, isTransmitter: false, Hub.s.cameraman.IsSpectatorMode);
			}
			else
			{
				protoActor.PlayVoiceOnActor(clip, isTransmitter: false, isMimicVoiceEcho: false);
			}
			speechEvent.AudioPlayedCount++;
			if (protoActor.ActorType == ActorType.Monster)
			{
				MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(protoActor.monsterMasterID);
				if (monsterInfo != null && monsterInfo.IsMimic())
				{
					protoActor.SetVoiceTypeDebugText(speechEvent);
				}
			}
		}

		private void RpcReader___Observers_ObserverRpcPlayOnActor_1543699021(PooledReader PooledReader0, Channel channel)
		{
			long speechEventId = PooledReader0.ReadInt64();
			int actorID = PooledReader0.ReadInt32();
			bool muteLocalPlayerVoice = PooledReader0.ReadBoolean();
			if (base.IsClientInitialized)
			{
				RpcLogic___ObserverRpcPlayOnActor_1543699021(speechEventId, actorID, muteLocalPlayerVoice);
			}
		}

		private void RpcWriter___Server_ServerRpcPlayRandomEventOnActor_1543699021(long speechEventId, int actorID, bool muteLocalPlayerVoice)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteInt64(speechEventId);
			pooledWriter.WriteInt32(actorID);
			pooledWriter.WriteBoolean(muteLocalPlayerVoice);
			SendServerRpc(1u, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		public void RpcLogic___ServerRpcPlayRandomEventOnActor_1543699021(long speechEventId, int actorID, bool muteLocalPlayerVoice)
		{
			ObserverRpcPlayOnActor(speechEventId, actorID, muteLocalPlayerVoice);
		}

		private void RpcReader___Server_ServerRpcPlayRandomEventOnActor_1543699021(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			long speechEventId = PooledReader0.ReadInt64();
			int actorID = PooledReader0.ReadInt32();
			bool muteLocalPlayerVoice = PooledReader0.ReadBoolean();
			if (base.IsServerInitialized)
			{
				RpcLogic___ServerRpcPlayRandomEventOnActor_1543699021(speechEventId, actorID, muteLocalPlayerVoice);
			}
		}

		private void RpcWriter___Server_ServerRpcRequestInitialSync_328543758(NetworkConnection conn = null)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			SendServerRpc(2u, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		private void RpcLogic___ServerRpcRequestInitialSync_328543758(NetworkConnection conn = null)
		{
			if (conn == null)
			{
				Logger.RError("[SpeechEventArchive] ServerRpcRequestInitialSync: conn is null");
				return;
			}
			_ = conn.ClientId;
			StartCoroutine(CoSendInitialChunks(conn));
		}

		private void RpcReader___Server_ServerRpcRequestInitialSync_328543758(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			if (base.IsServerInitialized)
			{
				RpcLogic___ServerRpcRequestInitialSync_328543758(conn);
			}
		}

		private void RpcWriter___Target_TargetRpcReceiveChunk_3872555705(NetworkConnection conn, SpeechEvent[] chunk, bool isFinalChunk, int currentCount, int totalCount)
		{
			if (!base.IsServerInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_005B_005DFishNet_002ESerializing_002EGenerated(pooledWriter, chunk);
			pooledWriter.WriteBoolean(isFinalChunk);
			pooledWriter.WriteInt32(currentCount);
			pooledWriter.WriteInt32(totalCount);
			SendTargetRpc(3u, pooledWriter, channel, DataOrderType.Default, conn, excludeServer: false);
			pooledWriter.Store();
		}

		private void RpcLogic___TargetRpcReceiveChunk_3872555705(NetworkConnection conn, SpeechEvent[] chunk, bool isFinalChunk, int currentCount, int totalCount)
		{
			if (chunk == null)
			{
				Logger.RError("[SpeechEventArchive] Received null chunk.");
				return;
			}
			foreach (SpeechEvent speechEvent in chunk)
			{
				if (!events.Any((SpeechEvent e) => e.Id == speechEvent.Id))
				{
					events.Collection.Add(speechEvent);
				}
			}
			if (isFinalChunk)
			{
				isInitialSyncComplete = true;
			}
		}

		private void RpcReader___Target_TargetRpcReceiveChunk_3872555705(PooledReader PooledReader0, Channel channel)
		{
			SpeechEvent[] chunk = GeneratedReaders___Internal.GRead___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_005B_005DFishNet_002ESerializing_002EGenerateds(PooledReader0);
			bool isFinalChunk = PooledReader0.ReadBoolean();
			int currentCount = PooledReader0.ReadInt32();
			int totalCount = PooledReader0.ReadInt32();
			if (base.IsClientInitialized)
			{
				RpcLogic___TargetRpcReceiveChunk_3872555705(base.LocalConnection, chunk, isFinalChunk, currentCount, totalCount);
			}
		}

		private void RpcWriter___Observers_ObserversRpcReceiveNewEventWithRemoval_1161837147(SpeechEvent newEvent, long[] idsToRemove)
		{
			if (!base.IsServerInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because server is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteSpeechEvent(newEvent);
			GeneratedWriters___Internal.GWrite___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerated(pooledWriter, idsToRemove);
			SendObserversRpc(4u, pooledWriter, channel, DataOrderType.Default, bufferLast: false, excludeServer: false, excludeOwner: false);
			pooledWriter.Store();
		}

		private void RpcLogic___ObserversRpcReceiveNewEventWithRemoval_1161837147(SpeechEvent newEvent, long[] idsToRemove)
		{
			if (base.IsOwner)
			{
				return;
			}
			if (!events.Any((SpeechEvent e) => e.Id == newEvent.Id))
			{
				events.Collection.Add(newEvent);
				OnAddSpeechEvent(newEvent, base.IsHostStarted);
			}
			if (idsToRemove == null || idsToRemove.Length == 0 || idsToRemove == null || idsToRemove.Length == 0)
			{
				return;
			}
			for (int num = events.Count - 1; num >= 0; num--)
			{
				if (idsToRemove.Contains(events[num].Id))
				{
					SpeechEvent speechEvent = events[num];
					events.Collection.RemoveAt(num);
					SendSpeechEventsChanged(SyncListOperation.RemoveAt, speechEvent);
				}
			}
		}

		private void RpcReader___Observers_ObserversRpcReceiveNewEventWithRemoval_1161837147(PooledReader PooledReader0, Channel channel)
		{
			SpeechEvent newEvent = PooledReader0.ReadSpeechEvent();
			long[] idsToRemove = GeneratedReaders___Internal.GRead___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerateds(PooledReader0);
			if (base.IsClientInitialized)
			{
				RpcLogic___ObserversRpcReceiveNewEventWithRemoval_1161837147(newEvent, idsToRemove);
			}
		}

		private void RpcWriter___Server_ServerRpcBroadcastNewEventWithRemoval_1161837147(SpeechEvent newEvent, long[] idsToRemove)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			if (!base.IsOwner)
			{
				NetworkManager networkManager2 = base.NetworkManager;
				networkManager2.LogWarning("Cannot complete action because you are not the owner of this object. .");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteSpeechEvent(newEvent);
			GeneratedWriters___Internal.GWrite___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerated(pooledWriter, idsToRemove);
			SendServerRpc(5u, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		private void RpcLogic___ServerRpcBroadcastNewEventWithRemoval_1161837147(SpeechEvent newEvent, long[] idsToRemove)
		{
			ObserversRpcReceiveNewEventWithRemoval(newEvent, idsToRemove);
		}

		private void RpcReader___Server_ServerRpcBroadcastNewEventWithRemoval_1161837147(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			SpeechEvent newEvent = PooledReader0.ReadSpeechEvent();
			long[] idsToRemove = GeneratedReaders___Internal.GRead___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerateds(PooledReader0);
			if (base.IsServerInitialized && OwnerMatches(conn))
			{
				RpcLogic___ServerRpcBroadcastNewEventWithRemoval_1161837147(newEvent, idsToRemove);
			}
		}

		private void RpcWriter___Server_SendToServerVoiceEmotionServerRpc_4002388585(int inActorID, string inActorName, byte inEmotionData)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			if (!base.IsOwner)
			{
				NetworkManager networkManager2 = base.NetworkManager;
				networkManager2.LogWarning("Cannot complete action because you are not the owner of this object. .");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteInt32(inActorID);
			pooledWriter.WriteString(inActorName);
			pooledWriter.WriteUInt8Unpacked(inEmotionData);
			SendServerRpc(6u, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		private void RpcLogic___SendToServerVoiceEmotionServerRpc_4002388585(int inActorID, string inActorName, byte inEmotionData)
		{
			if (Hub.s.replayManager.OnCapturePacketOnlyReplay(new DebugMimicVoiceEmotionSig
			{
				ActorID = inActorID,
				ActorName = inActorName,
				EmotionData = inEmotionData
			}))
			{
				Hub.s.replayManager.AddEmotionDataCount();
				Debug.Log($"SendToServerVoiceEmotionServerRpc: {inActorID}, {inActorName}, {inEmotionData}");
			}
		}

		private void RpcReader___Server_SendToServerVoiceEmotionServerRpc_4002388585(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			int inActorID = PooledReader0.ReadInt32();
			string inActorName = PooledReader0.ReadString();
			byte inEmotionData = PooledReader0.ReadUInt8Unpacked();
			if (base.IsServerInitialized && OwnerMatches(conn))
			{
				RpcLogic___SendToServerVoiceEmotionServerRpc_4002388585(inActorID, inActorName, inEmotionData);
			}
		}

		public virtual void Awake()
		{
			NetworkInitialize___Early();
			NetworkInitialize__Late();
		}
	}
}
