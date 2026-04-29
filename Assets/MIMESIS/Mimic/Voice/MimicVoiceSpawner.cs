using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DLAgent;
using FishNet;
using FishNet.Managing.Server;
using FishNet.Transporting;
using Mimic.Actors;
using Mimic.Voice.SpeechSystem;
using ReluNetwork.ConstEnum;
using ReluProtocol.Enum;
using UnityEngine;

namespace Mimic.Voice
{
	[RequireComponent(typeof(VoiceManager))]
	public class MimicVoiceSpawner : MonoBehaviour
	{
		public class MimicContext
		{
			public int MimicActorID;

			public int MimicMonsterMasterID = -1;

			public string MimickingPlayerId = string.Empty;

			public string MimickingPlayerIDByVoiceRule = string.Empty;

			public float NextSpawnTime;

			public BTVoiceRule VoiceRule;

			public int ContinuousSpeechCount;

			public float LastSpeechRecordedTime;

			public MimicContext(int mimicActorID, int mimicMonsterMasterID, float nextSpawnTime)
			{
				MimicActorID = mimicActorID;
				MimicMonsterMasterID = mimicMonsterMasterID;
				MimickingPlayerId = string.Empty;
				MimickingPlayerIDByVoiceRule = string.Empty;
				NextSpawnTime = nextSpawnTime;
				VoiceRule = BTVoiceRule.Default;
				ContinuousSpeechCount = 0;
			}
		}

		private static readonly float UpdateInterval = 1f;

		[SerializeField]
		[Tooltip("미믹 음성을 발생하기 위해 필요한 최소 음성 기록의 갯수를 설정합니다.")]
		private int minRequiredSpeechs = 3;

		private CancellationTokenSource? spawnCts;

		private Dictionary<int, MimicContext> actorIdToMimicContextDict = new Dictionary<int, MimicContext>();

		private float _playTimeRandomInterval;

		public VoiceManager voiceManager => GetComponent<VoiceManager>();

		private void Start()
		{
			ServerManager serverManager = InstanceFinder.ServerManager;
			if (serverManager != null)
			{
				serverManager.OnServerConnectionState += OnServerConnectionState;
			}
		}

		private void OnDestroy()
		{
			ServerManager serverManager = InstanceFinder.ServerManager;
			if (serverManager != null)
			{
				serverManager.OnServerConnectionState -= OnServerConnectionState;
			}
		}

		public void ClearAllContexts()
		{
			actorIdToMimicContextDict.Clear();
		}

		private async UniTaskVoid StartSpawningAsync()
		{
			StopSpawning();
			spawnCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
			while (!spawnCts.IsCancellationRequested && voiceManager != null && voiceManager.GetTotalRandomPoolSize() < minRequiredSpeechs)
			{
				await UniTask.WaitForSeconds(1f, ignoreTimeScale: false, PlayerLoopTiming.Update, spawnCts.Token);
			}
			while (!spawnCts.IsCancellationRequested)
			{
				Dictionary<int, ProtoActor> mimicActors = GetAllMimicActors();
				Dictionary<string, FishNetDissonancePlayer> dissonancePlayers = GetAllDissonancePlayers();
				AddOrRemoveContexts(ref mimicActors, ref dissonancePlayers);
				foreach (MimicContext value in actorIdToMimicContextDict.Values)
				{
					TrySpawnVoiceByContext(value, periodic: true);
				}
				await UniTask.WaitForSeconds(UpdateInterval, ignoreTimeScale: false, PlayerLoopTiming.Update, spawnCts.Token);
			}
		}

		private void StopSpawning()
		{
			if (spawnCts != null)
			{
				spawnCts.Cancel();
				spawnCts.Dispose();
				spawnCts = null;
			}
		}

		private void AddOrRemoveContexts(ref Dictionary<int, ProtoActor> mimicActors, ref Dictionary<string, FishNetDissonancePlayer> dissonancePlayers)
		{
			HashSet<int> hashSet = mimicActors.Keys.ToHashSet();
			HashSet<int> hashSet2 = actorIdToMimicContextDict.Keys.ToHashSet();
			foreach (int item in hashSet2.Except(hashSet))
			{
				actorIdToMimicContextDict.Remove(item);
			}
			foreach (int item2 in hashSet.Except(hashSet2))
			{
				float nextSpawnTime = 0f;
				if (TryGetMimicMonsterTableRow(mimicActors[item2].monsterMasterID, out MMMimicMonsterTable.Row monsterTableRow) && monsterTableRow != null)
				{
					nextSpawnTime = (float)Hub.s.timeutil.GetCurrentTickSec() + monsterTableRow.InitializeInterval.RandomValue;
				}
				actorIdToMimicContextDict.Add(item2, new MimicContext(item2, mimicActors[item2].monsterMasterID, nextSpawnTime));
			}
		}

		private bool TrySpawnVoiceByContext(MimicContext context, bool periodic, SpeechType_Area speechType_Area = SpeechType_Area.None)
		{
			if (!TryGetMimicMonsterTableRow(context.MimicMonsterMasterID, out MMMimicMonsterTable.Row monsterTableRow))
			{
				return false;
			}
			if (periodic && context.NextSpawnTime > (float)Hub.s.timeutil.GetCurrentTickSec())
			{
				return false;
			}
			if (!TryPickHeuristicSpeechEvent(context, periodic, speechType_Area, out SpeechEvent speechEvent))
			{
				return false;
			}
			if (!voiceManager.TryGetSpeechEventArchive(context.MimickingPlayerId, out var archive))
			{
				return false;
			}
			float num = PickRandomInterval(periodic, monsterTableRow, speechType_Area);
			context.NextSpawnTime = (float)Hub.s.timeutil.GetCurrentTickSec() + speechEvent.Duration + num;
			speechEvent.LastPlayedTime = Hub.s.timeutil.GetCurrentTickSec();
			if (Hub.s.replayManager.IsRecordMode())
			{
				Hub.s.replayManager.OnRecordVoiceFile(context.MimicActorID, speechEvent);
			}
			archive.ObserverRpcPlayOnActor(speechEvent.Id, context.MimicActorID, monsterTableRow.muteLocalPlayerVoice);
			return true;
		}

		private float PickRandomInterval(bool periodic, MMMimicMonsterTable.Row monsterTableRow, SpeechType_Area speechType_Area = SpeechType_Area.None)
		{
			float num = 0f;
			if (periodic)
			{
				if (speechType_Area == SpeechType_Area.DeathMatch)
				{
					return Mathf.Max(0f, Random.Range(monsterTableRow.deathMatchInterval.minValue, monsterTableRow.deathMatchInterval.maxValue));
				}
				return Mathf.Max(0f, Random.Range(monsterTableRow.interval.minValue, monsterTableRow.interval.maxValue));
			}
			return Random.Range(2f, 4f);
		}

		private Dictionary<int, ProtoActor> GetAllMimicActors()
		{
			Dictionary<int, ProtoActor> result = new Dictionary<int, ProtoActor>();
			if (Hub.s == null)
			{
				return result;
			}
			if (Hub.s.pdata == null)
			{
				return result;
			}
			if (Hub.s.pdata.main == null)
			{
				return result;
			}
			result = Hub.s.pdata.main.GetProtoActorMap();
			return result.Where((KeyValuePair<int, ProtoActor> a) => a.Value.ActorType == ActorType.Monster && a.Value.IsMimic()).ToDictionary((KeyValuePair<int, ProtoActor> e) => e.Key, (KeyValuePair<int, ProtoActor> e) => e.Value);
		}

		private Dictionary<string, FishNetDissonancePlayer> GetAllDissonancePlayers()
		{
			IEnumerable<FishNetDissonancePlayer> enumerable = from e in Object.FindObjectsByType<FishNetDissonancePlayer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
				where e != null && !string.IsNullOrEmpty(e.PlayerId)
				select e;
			Dictionary<string, FishNetDissonancePlayer> dictionary = new Dictionary<string, FishNetDissonancePlayer>();
			foreach (FishNetDissonancePlayer item in enumerable)
			{
				if (!dictionary.ContainsKey(item.PlayerId))
				{
					dictionary.Add(item.PlayerId, item);
				}
				else
				{
					Debug.LogWarning("중복된 PlayerId 발견: " + item.PlayerId);
				}
			}
			return dictionary;
		}

		private bool TryGetMimicMonsterTableRow(int monsterMasterID, out MMMimicMonsterTable.Row? monsterTableRow)
		{
			monsterTableRow = null;
			if (Hub.s != null && Hub.s.tableman != null && Hub.s.tableman.mimicMonster != null)
			{
				return Hub.s.tableman.mimicMonster.TryGetRow(monsterMasterID, out monsterTableRow);
			}
			return false;
		}

		private void OnServerConnectionState(ServerConnectionStateArgs args)
		{
			if (args.ConnectionState == LocalConnectionState.Started)
			{
				StartSpawningAsync().Forget();
			}
			else if (args.ConnectionState == LocalConnectionState.Stopping)
			{
				StopSpawning();
			}
		}

		public bool TryPickHeuristicSpeechEvent(MimicContext context, bool periodic, SpeechType_Area speechType_Area, out SpeechEvent? speechEvent)
		{
			speechEvent = null;
			if (Hub.s == null)
			{
				return false;
			}
			if (Hub.s.pdata == null)
			{
				return false;
			}
			if (Hub.s.pdata.main == null)
			{
				return false;
			}
			ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(context.MimicActorID);
			if (actorByActorID == null)
			{
				return false;
			}
			if (speechType_Area != SpeechType_Area.Transmitter)
			{
				List<ProtoActor> allActorsInRange = Hub.s.pdata.main.GetAllActorsInRange(actorByActorID.transform.position, 15f, ActorType.Player);
				if (allActorsInRange == null || allActorsInRange.Count == 0)
				{
					return false;
				}
			}
			BTVoiceRule bTVoiceRule = actorByActorID.gameObject.GetComponentInChildren<DLDecisionAgent>()?.VoiceRule ?? BTVoiceRule.Default;
			List<(string, SpeechEvent)> list = new List<(string, SpeechEvent)>();
			string MimickingPlayerIDByVoiceRule;
			foreach (SpeechEventArchive allSpeechEventArchive in voiceManager.GetAllSpeechEventArchives(bTVoiceRule, actorByActorID.transform.position, context, out MimickingPlayerIDByVoiceRule))
			{
				foreach (SpeechEvent item in allSpeechEventArchive.GetSpeechEventRandomPool())
				{
					list.Add((allSpeechEventArchive.PlayerId, item));
				}
			}
			string mimickingPlayerID = string.Empty;
			bool flag = false;
			if (context.ContinuousSpeechCount > 0)
			{
				flag = PickEarliestWithinWindow(list, context.LastSpeechRecordedTime, 2f, out speechEvent);
				mimickingPlayerID = context.MimickingPlayerId;
				if (flag)
				{
					context.ContinuousSpeechCount--;
				}
				else
				{
					context.ContinuousSpeechCount = 0;
				}
			}
			else
			{
				switch (speechType_Area)
				{
				case SpeechType_Area.Transmitter:
					flag = SpeechEventAdditionalGameData.PickTransmitterVoice(list, out speechEvent, out mimickingPlayerID);
					break;
				default:
					return false;
				case SpeechType_Area.None:
				{
					SpeechEventAdditionalGameData curGameData = new SpeechEventAdditionalGameData(actorByActorID.GetAdjacentPlayerCount(), actorByActorID.GetAreaType(actorByActorID.transform.position), SpeechEventAdditionalGameData.GetGameTime(), actorByActorID.GetFacingPlayerCount(), actorByActorID.GetScrapObjects(), actorByActorID.GetMonsters(), actorByActorID.GetTeleporter(), actorByActorID.GetIndoorEntered(), actorByActorID.GetCharger(), actorByActorID.GetCrowShop(), actorByActorID.GetIncomingEvents());
					int pickCount = 1;
					if (context.VoiceRule == bTVoiceRule)
					{
						pickCount = 3;
					}
					string pickReason = string.Empty;
					flag = SpeechEventAdditionalGameData.PickBestMatch(context, list, curGameData, periodic, pickCount, _playTimeRandomInterval, out speechEvent, out mimickingPlayerID, out pickReason);
					SendVoicePickReason(context.MimicActorID, pickReason);
					break;
				}
				}
				if (flag)
				{
					_playTimeRandomInterval = Random.Range(-0.7f, 0.7f);
				}
			}
			context.VoiceRule = bTVoiceRule;
			context.MimickingPlayerIDByVoiceRule = MimickingPlayerIDByVoiceRule;
			context.MimickingPlayerId = mimickingPlayerID;
			if (speechEvent != null)
			{
				context.LastSpeechRecordedTime = speechEvent.RecordedTime + speechEvent.Duration;
			}
			else
			{
				context.LastSpeechRecordedTime = 0f;
			}
			return flag;
		}

		public void TrySpawnMimicVoiceEventOnce(Vector3 playerPos)
		{
			if (Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.main == null)
			{
				return;
			}
			foreach (MimicContext value in actorIdToMimicContextDict.Values)
			{
				ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(value.MimicActorID);
				if (!(actorByActorID == null) && !(Vector3.Distance(playerPos, actorByActorID.transform.position) > 20f))
				{
					TrySpawnVoiceByContext(value, periodic: false);
				}
			}
		}

		public bool TrySpawnMimicVoiceEventOnceWithArea(int mimicActorID, SpeechType_Area speechType_Area)
		{
			if (Hub.s == null)
			{
				return false;
			}
			if (Hub.s.pdata == null)
			{
				return false;
			}
			if (Hub.s.pdata.main == null)
			{
				return false;
			}
			if (!actorIdToMimicContextDict.TryGetValue(mimicActorID, out MimicContext value))
			{
				return false;
			}
			if (Hub.s.pdata.main.GetActorByActorID(mimicActorID) == null)
			{
				return false;
			}
			return TrySpawnVoiceByContext(value, periodic: false, speechType_Area);
		}

		private void SendVoicePickReason(int actorID, string pickReason)
		{
			if (!(Hub.s == null) && Hub.s.pdata != null && !(Hub.s.pdata.main == null) && Hub.s.pdata.ClientMode == NetworkClientMode.Host)
			{
				DebugMimicVoiceInfoSig msg = new DebugMimicVoiceInfoSig
				{
					actorID = actorID,
					VoicePickReason = pickReason
				};
				Hub.s.vworld?.BroadcastToAll(msg);
			}
		}

		private bool PickEarliestWithinWindow(List<(string playerID, SpeechEvent evt)> allEvents, float referenceTime, float windowSeconds, out SpeechEvent? speechEvent)
		{
			speechEvent = null;
			List<(string, SpeechEvent)> list = allEvents.Where<(string, SpeechEvent)>(((string playerID, SpeechEvent evt) pair) => pair.evt.RecordedTime >= referenceTime && pair.evt.RecordedTime <= referenceTime + windowSeconds).ToList();
			if (list.Count == 0)
			{
				return false;
			}
			speechEvent = list.OrderBy<(string, SpeechEvent), float>(((string playerID, SpeechEvent evt) pair) => pair.evt.RecordedTime).First().Item2;
			return true;
		}
	}
}
