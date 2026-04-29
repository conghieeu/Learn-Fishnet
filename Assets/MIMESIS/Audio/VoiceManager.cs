using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dissonance;
using Dissonance.Audio.Codecs.Opus;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.FishNet;
using FishNet;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using FishySteamworks;
using Mimic.Actors;
using Mimic.Voice;
using Mimic.Voice.SpeechSystem;
using ReluReplay.Shared;
using UnityEngine;

[RequireComponent(typeof(SpeechEventRecorder))]
[RequireComponent(typeof(MimicVoiceSpawner))]
public class VoiceManager : MonoBehaviour
{
	[Header("서버 설정")]
	[SerializeField]
	[Tooltip("포트를 명시적으로 지정하지 않은 경우 사용할 기본 포트입니다.")]
	private ushort defaultServerPort = 20402;

	[Header("채널 설정")]
	[SerializeField]
	[Tooltip("플레이어 음성을 송신하는 컴포넌트입니다.")]
	private VoiceBroadcastTrigger playerBroadcastTrigger;

	[SerializeField]
	[Tooltip("플레이어 음성을 수신하는 컴포넌트입니다.")]
	private VoiceReceiptTrigger playerReceiptTrigger;

	[SerializeField]
	[Tooltip("관찰자 음성을 송신하는 컴포넌트입니다.")]
	private VoiceBroadcastTrigger observerBroadcastTrigger;

	[SerializeField]
	[Tooltip("관찰자 음성을 수신하는 컴포넌트입니다.")]
	private VoiceReceiptTrigger observerReceiptTrigger;

	[SerializeField]
	[Tooltip("송출기 음성을 송신하는 컴포넌트입니다.")]
	private VoiceBroadcastTrigger transmitterBroadcastTrigger;

	[SerializeField]
	[Tooltip("송출기 음성을 수신하는 컴포넌트입니다.")]
	private VoiceReceiptTrigger transmitterReceiptTrigger;

	[SerializeField]
	[Tooltip("미믹이 송출기 음성을 사용할 때 등록될 Root(2D만 적용가능)")]
	private GameObject mimicTransmitterAudioRoot;

	private List<SpeechEventArchive> speechEventArchives = new List<SpeechEventArchive>();

	private List<FishNetDissonancePlayer> players = new List<FishNetDissonancePlayer>();

	[SerializeField]
	[Tooltip("내 목소리를 효과 적용해서 들리게 하는 기능. Voice Effect.")]
	private MyVoiceProcessor MyVoiceProcessor_VoiceEffect;

	private bool _isSilence;

	private bool _isTransmitter;

	private bool _asServer;

	private float _lastMimicVoiceTime = -1f;

	[SerializeField]
	private float MimicVoiceCooldown = 3f;

	[SerializeField]
	private float MimicVoiceDelay = 0.2f;

	public Transform MimicTransmitterAudioRootTransform => mimicTransmitterAudioRoot?.transform;

	public VoiceMode voiceMode { get; private set; }

	public SpeechEventRecorder speechEventRecorder => GetComponent<SpeechEventRecorder>();

	public MimicVoiceSpawner mimicVoiceSpawner => GetComponent<MimicVoiceSpawner>();

	public List<FishNetDissonancePlayer> Players => players;

	public bool IsTransmitterSpeaking => transmitterBroadcastTrigger.enabled;

	public VoiceBroadcastTrigger GetPlayerBroadcastTrigger()
	{
		return playerBroadcastTrigger;
	}

	public VoiceBroadcastTrigger GetObserverBroadcastTrigger()
	{
		return observerBroadcastTrigger;
	}

	public VoiceBroadcastTrigger GetTransmitterBroadcastTrigger()
	{
		return transmitterBroadcastTrigger;
	}

	private void Start()
	{
		DissonanceFishNetComms.Instance.Comms.OnPlayerJoinedSession += OnPlayerJoinedSession;
		DissonanceFishNetComms.Instance.Comms.OnPlayerEnteredRoom += OnPlayerEnteredRoom;
		SetVoiceMode(VoiceMode.Off);
		if (mimicTransmitterAudioRoot == null)
		{
			Logger.RError("mimicTransmitterAudioRoot is null");
		}
	}

	private void OnDestroy()
	{
		DissonanceFishNetComms.Instance.Comms.OnPlayerJoinedSession -= OnPlayerJoinedSession;
	}

	private void OnPlayerJoinedSession(VoicePlayerState state)
	{
		FishNetDissonancePlayer fishNetDissonancePlayer = Players.Find((FishNetDissonancePlayer player) => player.PlayerId == state.Name);
		if (fishNetDissonancePlayer != null)
		{
			fishNetDissonancePlayer.ProtoActorCache?.InitializeVoiceEffecter();
		}
	}

	private void OnPlayerEnteredRoom(VoicePlayerState state, string roomName)
	{
		Debug.Log("[VoiceManager] OnPlayerEnteredRoom: PlayerId=" + state.Name + ", RoomName=" + roomName);
	}

	public bool StartAsServer()
	{
		Transport transport = InstanceFinder.TransportManager.Transport;
		transport.SetServerBindAddress("0.0.0.0", IPAddressType.IPv4);
		transport.SetPort(defaultServerPort);
		_asServer = true;
		return transport.StartConnection(server: true);
	}

	public bool StartAsClient(string address, bool clientWithRelay)
	{
		InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
		InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
		if (InstanceFinder.TransportManager.Transport is Multipass)
		{
			Multipass multipass = (Multipass)InstanceFinder.TransportManager.Transport;
			if (clientWithRelay)
			{
				multipass.SetClientTransport(typeof(global::FishySteamworks.FishySteamworks));
			}
			else
			{
				multipass.SetClientTransport(typeof(Tugboat));
			}
		}
		return InstanceFinder.ClientManager.StartConnection(address, defaultServerPort);
	}

	private void OnClientConnectionState(ClientConnectionStateArgs args)
	{
		if (args.ConnectionState == LocalConnectionState.Stopped)
		{
			Shutdown();
		}
	}

	public void Shutdown()
	{
		SetVoiceMode(VoiceMode.Off);
		InstanceFinder.TransportManager.Transport.Shutdown();
	}

	public bool IsConnected()
	{
		return !InstanceFinder.IsOffline;
	}

	public void SetTalkMode(CommActivationMode mode)
	{
		switch (mode)
		{
		case CommActivationMode.VoiceActivation:
			playerBroadcastTrigger.Mode = CommActivationMode.VoiceActivation;
			observerBroadcastTrigger.Mode = CommActivationMode.VoiceActivation;
			transmitterBroadcastTrigger.Mode = CommActivationMode.VoiceActivation;
			break;
		case CommActivationMode.PushToTalk:
			playerBroadcastTrigger.Mode = CommActivationMode.PushToTalk;
			observerBroadcastTrigger.Mode = CommActivationMode.PushToTalk;
			transmitterBroadcastTrigger.Mode = CommActivationMode.PushToTalk;
			break;
		case CommActivationMode.None:
			playerBroadcastTrigger.Mode = CommActivationMode.None;
			observerBroadcastTrigger.Mode = CommActivationMode.None;
			transmitterBroadcastTrigger.Mode = CommActivationMode.None;
			break;
		case CommActivationMode.Open:
			playerBroadcastTrigger.Mode = CommActivationMode.VoiceActivation;
			observerBroadcastTrigger.Mode = CommActivationMode.VoiceActivation;
			transmitterBroadcastTrigger.Mode = CommActivationMode.VoiceActivation;
			break;
		}
	}

	public void SetVoiceMode(VoiceMode voiceMode)
	{
		_isSilence = false;
		_isTransmitter = false;
		switch (voiceMode)
		{
		case VoiceMode.Off:
			SetPlayerChannel(canSpeak: false, canHear: false);
			SetObserverChannel(canSpeak: false, canHear: false);
			SetTransmitterChannel(canSpeak: false, canHear: false);
			speechEventRecorder.StopRecording();
			break;
		case VoiceMode.PreGame:
			SetPlayerChannel(canSpeak: true, canHear: true);
			SetObserverChannel(canSpeak: false, canHear: false);
			SetTransmitterChannel(canSpeak: false, canHear: false);
			speechEventRecorder.StopRecording();
			mimicVoiceSpawner.ClearAllContexts();
			break;
		case VoiceMode.Player:
			SetPlayerChannel(canSpeak: true, canHear: true);
			SetObserverChannel(canSpeak: false, canHear: false);
			SetTransmitterChannel(canSpeak: false, canHear: false);
			speechEventRecorder.StartRecording();
			break;
		case VoiceMode.Observer:
			SetPlayerChannel(canSpeak: false, canHear: true);
			SetObserverChannel(canSpeak: true, canHear: true);
			SetTransmitterChannel(canSpeak: false, canHear: true);
			speechEventRecorder.StopRecording();
			break;
		default:
			throw new ArgumentOutOfRangeException("voiceMode");
		}
		this.voiceMode = voiceMode;
		MyVoiceProcessor_VoiceEffect.Activate(isActivated: false);
	}

	private void SetPlayerChannel(bool canSpeak, bool canHear)
	{
		playerBroadcastTrigger.enabled = canSpeak;
		playerReceiptTrigger.enabled = canHear;
	}

	private void SetObserverChannel(bool canSpeak, bool canHear)
	{
		observerBroadcastTrigger.enabled = canSpeak;
		observerReceiptTrigger.enabled = canHear;
	}

	private void SetTransmitterChannel(bool canSpeak, bool canHear)
	{
		SetTransmitterChannelSend(canSpeak, isForce: true);
		SetTransmitterChannelRecv(canHear);
	}

	public void EnableSelfVoice(bool inSpeak, bool inHear, bool inTransmitter)
	{
		playerBroadcastTrigger.IsMuted = !inSpeak;
		MyVoiceProcessor_VoiceEffect.Activate(inHear);
		transmitterBroadcastTrigger.enabled = inTransmitter;
		if (_isTransmitter != inTransmitter)
		{
			_isTransmitter = inTransmitter;
			Hub.s.voiceman.speechEventRecorder.OnChangeTransmitterSpeaking(inTransmitter);
		}
	}

	public void SetTransmitterChannelSend(bool canSpeak, bool isForce = false)
	{
		if ((!_isSilence || !canSpeak) && (isForce || _isTransmitter != canSpeak))
		{
			_isTransmitter = canSpeak;
			transmitterBroadcastTrigger.enabled = _isTransmitter;
			if (!isForce)
			{
				Hub.s.voiceman.speechEventRecorder.OnChangeTransmitterSpeaking(canSpeak);
			}
		}
	}

	public void SetTransmitterChannelRecv(bool canHear)
	{
		if (transmitterReceiptTrigger.enabled && !canHear)
		{
			StopAllMimicVoiceIndoor();
		}
		transmitterReceiptTrigger.enabled = canHear;
	}

	public void StopAllMimicVoiceIndoor()
	{
		foreach (ProtoActor value in Hub.s.pdata.main.GetProtoActorMap().Values)
		{
			if (!(value == null) && value.IsMimic() && Hub.s.dLAcademyManager.GetAreaForDL(value.transform.position, out var _) == 0)
			{
				value.StopVoiceOnActor();
			}
		}
	}

	public bool TryPlayHalucinationInsideCircle(Vector3 center, float circleRadius)
	{
		SpeechEventArchive randomOtherSpeechEventArchive = GetRandomOtherSpeechEventArchive();
		if (randomOtherSpeechEventArchive == null)
		{
			return false;
		}
		return randomOtherSpeechEventArchive.TryPlayRandomEventInsideCircle(center, circleRadius);
	}

	public bool TryGetSpeechEventArchive(string playerId, out SpeechEventArchive archive)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			Logger.RError("TryGetSpeechEventArchive called with null or empty playerId!");
			archive = null;
			return false;
		}
		archive = speechEventArchives.FirstOrDefault((SpeechEventArchive e) => e.PlayerId == playerId);
		return archive != null;
	}

	public void TrySendToServerVoiceEmotion(int actorID, ReplaySharedData.E_EVENT inEmotion)
	{
		if (Hub.s.pdata != null && Hub.s.pdata.MyActorID == actorID)
		{
			SpeechEventArchive speechEventArchive = speechEventArchives.FirstOrDefault((SpeechEventArchive e) => e.PlayerUID == Hub.s.pdata.PlayerUID);
			if (speechEventArchive != null && Hub.s.pdata != null)
			{
				speechEventArchive.SendToServerVoiceEmotion(Hub.s.pdata.MyActorID, Hub.s.pdata.MyNickName, inEmotion);
			}
		}
	}

	public List<SpeechEventArchive> GetAllSpeechEventArchives(BTVoiceRule rule, Vector3 position, MimicVoiceSpawner.MimicContext prevContext, out string MimickingPlayerIDByVoiceRule)
	{
		List<SpeechEventArchive> result = new List<SpeechEventArchive>();
		MimickingPlayerIDByVoiceRule = prevContext.MimickingPlayerIDByVoiceRule;
		bool flag = prevContext.ContinuousSpeechCount > 0;
		if (flag)
		{
			MimickingPlayerIDByVoiceRule = prevContext.MimickingPlayerIDByVoiceRule;
			result = speechEventArchives.Where((SpeechEventArchive archive) => archive.PlayerId == prevContext.MimickingPlayerId).ToList();
		}
		switch (rule)
		{
		case BTVoiceRule.Default:
			MimickingPlayerIDByVoiceRule = prevContext.MimickingPlayerId;
			if (!flag)
			{
				result = speechEventArchives;
			}
			break;
		case BTVoiceRule.Dead:
		{
			if (prevContext.VoiceRule == rule)
			{
				MimickingPlayerIDByVoiceRule = prevContext.MimickingPlayerIDByVoiceRule;
				if (!flag)
				{
					result = speechEventArchives.Where((SpeechEventArchive archive) => archive.PlayerId == prevContext.MimickingPlayerIDByVoiceRule).ToList();
				}
				break;
			}
			GamePlayScene gamePlayScene2 = Hub.s.pdata.main as GamePlayScene;
			if (!(gamePlayScene2 != null))
			{
				break;
			}
			long chosenDead = gamePlayScene2.GetRandomDeadActorUID();
			if (chosenDead >= 0)
			{
				List<SpeechEventArchive> list2 = speechEventArchives.Where((SpeechEventArchive archive) => archive.PlayerUID == chosenDead).ToList();
				if (!flag)
				{
					result = list2;
				}
				if (list2.Count > 0)
				{
					MimickingPlayerIDByVoiceRule = list2[0].PlayerId;
				}
				else
				{
					MimickingPlayerIDByVoiceRule = string.Empty;
				}
			}
			break;
		}
		case BTVoiceRule.MaxDistance:
		{
			if (prevContext.VoiceRule == rule)
			{
				MimickingPlayerIDByVoiceRule = prevContext.MimickingPlayerIDByVoiceRule;
				if (!flag)
				{
					result = speechEventArchives.Where((SpeechEventArchive archive) => archive.PlayerId == prevContext.MimickingPlayerIDByVoiceRule).ToList();
				}
				break;
			}
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (!(gamePlayScene != null))
			{
				break;
			}
			long chosenPlayerUID = gamePlayScene.GetMaxDistancePlayerUID(position);
			if (chosenPlayerUID >= 0)
			{
				List<SpeechEventArchive> list = speechEventArchives.Where((SpeechEventArchive archive) => archive.PlayerUID == chosenPlayerUID).ToList();
				if (!flag)
				{
					result = list;
				}
				if (list.Count > 0)
				{
					MimickingPlayerIDByVoiceRule = list[0].PlayerId;
				}
				else
				{
					MimickingPlayerIDByVoiceRule = string.Empty;
				}
			}
			break;
		}
		}
		return result;
	}

	public int GetTotalRandomPoolSize()
	{
		return speechEventArchives.Sum((SpeechEventArchive archive) => archive.RandomPoolSize);
	}

	public void RegisterClient(SpeechEventArchive archive, FishNetDissonancePlayer player)
	{
		if (!speechEventArchives.Contains(archive))
		{
			speechEventArchives.Add(archive);
		}
		else
		{
			Logger.RError($"Failed to register archive: {archive}");
		}
		if (!players.Contains(player))
		{
			players.Add(player);
			if (Hub.s != null && Hub.s.uiman.inGameMenu != null)
			{
				Hub.s.uiman.inGameMenu.isUpdated = true;
			}
		}
		else
		{
			Logger.RError($"Failed to register player: {player}");
		}
	}

	public void UnregisterClient(SpeechEventArchive archive, FishNetDissonancePlayer player)
	{
		if (speechEventArchives.Contains(archive))
		{
			speechEventArchives.Remove(archive);
		}
		else
		{
			Logger.RError($"Failed to unregister archive: {archive}");
		}
		if (players.Contains(player))
		{
			players.Remove(player);
		}
		else
		{
			Logger.RError($"Failed to unregister player: {player}");
		}
	}

	private SpeechEventArchive GetRandomOtherSpeechEventArchive()
	{
		string localPlayerId = DissonanceFishNetComms.Instance.Comms.LocalPlayerName;
		List<SpeechEventArchive> list = speechEventArchives.Where((SpeechEventArchive a) => a.PlayerId != localPlayerId && a.RandomPoolSize > 0).ToList();
		if (list.Count == 0)
		{
			return null;
		}
		int index = new System.Random().Next(list.Count);
		return list[index];
	}

	public string GetLocalPlayerUID()
	{
		return DissonanceFishNetComms.Instance.Comms.LocalPlayerName;
	}

	public bool TryGetVoiceAmplitude(string playerId, out float amplitude, out bool isSpeaking)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			Logger.RError("TryGetVoiceAmplitude called with null or empty playerId!");
			amplitude = 0f;
			isSpeaking = false;
			return false;
		}
		VoicePlayerState voicePlayerState = DissonanceFishNetComms.Instance.Comms.FindPlayer(playerId);
		if (voicePlayerState == null)
		{
			amplitude = 0f;
			isSpeaking = false;
			return false;
		}
		amplitude = voicePlayerState.Amplitude;
		isSpeaking = voicePlayerState.IsSpeaking;
		return true;
	}

	public bool IsPlayerSpeaking(int actorID)
	{
		FishNetDissonancePlayer fishNetDissonancePlayer = players.Find(delegate(FishNetDissonancePlayer p)
		{
			if (p.ProtoActorCache == null)
			{
				ProtoActor protoActor = Hub.s?.pdata?.main?.GetActorByPlayerUID(p.PlayerUID);
				if (protoActor == null)
				{
					return false;
				}
				return protoActor.ActorID == actorID;
			}
			return p.ProtoActorCache.ActorID == actorID;
		});
		if (fishNetDissonancePlayer == null)
		{
			return false;
		}
		return IsPlayerSpeaking(fishNetDissonancePlayer.PlayerId);
	}

	public bool IsPlayerSpeaking(string playerId)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			Logger.RError("IsPlayerSpeaking called with null or empty playerId!");
			return false;
		}
		return DissonanceFishNetComms.Instance.Comms.FindPlayer(playerId)?.IsSpeaking ?? false;
	}

	public void SetPlayerVolume(string playerId, float volume)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			Logger.RError("SetPlayerVolume called with null or empty playerId!");
			return;
		}
		VoicePlayerState voicePlayerState = DissonanceFishNetComms.Instance.Comms.FindPlayer(playerId);
		if (voicePlayerState != null)
		{
			voicePlayerState.Volume = volume;
		}
	}

	public float GetPlayerVolume(string playerId)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			Logger.RError("GetPlayerVolume called with null or empty playerId!");
			return 0f;
		}
		return DissonanceFishNetComms.Instance.Comms.FindPlayer(playerId)?.Volume ?? 0f;
	}

	public AudioSource GetVoicePlaybackAudioSource(long playerUID)
	{
		string text = string.Empty;
		if (players != null)
		{
			FishNetDissonancePlayer fishNetDissonancePlayer = players.Find((FishNetDissonancePlayer p) => p != null && p.PlayerUID == playerUID);
			if (fishNetDissonancePlayer == null)
			{
				return null;
			}
			text = fishNetDissonancePlayer.PlayerId;
		}
		if (string.IsNullOrEmpty(text))
		{
			Logger.RError("There is no voice player. id=" + text);
			return null;
		}
		if (DissonanceFishNetComms.Instance.Comms.FindPlayer(text)?.Playback is CustomVoicePlayback customVoicePlayback)
		{
			return customVoicePlayback.AudioSource;
		}
		return null;
	}

	public ProtoActor? GetActorByPlayerUID(long PlayerUID)
	{
		if (Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.main == null)
		{
			Logger.RError("Failed to get Hub.s.pdata.main! Cannot get actor by player ID!");
			return null;
		}
		return Hub.s.pdata.main.GetActorByPlayerUID(PlayerUID);
	}

	public bool SpawnMimicVoicEventOnce(long playerUID)
	{
		if (!_asServer)
		{
			return false;
		}
		if (mimicVoiceSpawner == null)
		{
			return false;
		}
		if ((float)Hub.s.timeutil.GetCurrentTickSec() - _lastMimicVoiceTime < MimicVoiceCooldown)
		{
			return false;
		}
		ProtoActor actorByPlayerUID = GetActorByPlayerUID(playerUID);
		if (actorByPlayerUID == null)
		{
			return false;
		}
		StartCoroutine(SpawnMimicVoiceWithDelay(actorByPlayerUID.transform.position));
		return true;
	}

	private IEnumerator SpawnMimicVoiceWithDelay(Vector3 position)
	{
		yield return new WaitForSeconds(0.2f);
		mimicVoiceSpawner.TrySpawnMimicVoiceEventOnce(position);
		_lastMimicVoiceTime = Hub.s.timeutil.GetCurrentTickSec();
	}

	public bool TrySpawnMimicTransmitterVoice(int mimicActorID)
	{
		return mimicVoiceSpawner.TrySpawnMimicVoiceEventOnceWithArea(mimicActorID, SpeechType_Area.Transmitter);
	}

	public AudioSource? GetLocalVoiceAudioSources()
	{
		if (MyVoiceProcessor_VoiceEffect == null)
		{
			return null;
		}
		return MyVoiceProcessor_VoiceEffect.GetAudioSource();
	}

	public static AudioClip CreateAudioClip(SpeechEvent speechEvent)
	{
		if (OpusAudioUtility.IsOpusData(speechEvent.CompressedAudioData))
		{
			return OpusAudioUtility.ToAudioClip(speechEvent.CompressedAudioData, speechEvent.Id.ToString());
		}
		return null;
	}

	public AudioClip GetSpeechAudioClipFromSpeechEventArchiveRandom(System.Random random)
	{
		if (random == null)
		{
			return null;
		}
		List<SpeechEventArchive> list = speechEventArchives.Where((SpeechEventArchive a) => a.SpeechEventPoolSize > 0).ToList();
		if (list.Count == 0)
		{
			return null;
		}
		int index = random.Next(0, list.Count);
		List<SpeechEvent> speechEventPool = list[index].GetSpeechEventPool();
		if (speechEventPool.Count == 0)
		{
			return null;
		}
		index = random.Next(0, speechEventPool.Count);
		SpeechEvent speechEvent = speechEventPool[index];
		if (speechEvent == null)
		{
			return null;
		}
		return CreateAudioClip(speechEvent);
	}
}
