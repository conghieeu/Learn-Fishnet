using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using Mimic.Actors;
using UnityEngine;

namespace Mimic.Voice.SpeechSystem
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public class SpeechEventAdditionalGameData : IMemoryPackable<SpeechEventAdditionalGameData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class SpeechEventAdditionalGameDataFormatter : MemoryPackFormatter<SpeechEventAdditionalGameData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SpeechEventAdditionalGameData value)
			{
				SpeechEventAdditionalGameData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref SpeechEventAdditionalGameData value)
			{
				SpeechEventAdditionalGameData.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackIgnore]
		private static readonly int weightPlayerCount;

		[MemoryPackIgnore]
		private static readonly int weightArea;

		[MemoryPackIgnore]
		private static readonly int weightPlayTime;

		[MemoryPackIgnore]
		private static readonly int weightFacingPlayerCount;

		[MemoryPackIgnore]
		private static readonly int weightOneHandScrapObject;

		[MemoryPackIgnore]
		private static readonly int weightTwoHandScrapObject;

		[MemoryPackIgnore]
		private static readonly int weightMonsters;

		[MemoryPackIgnore]
		private static readonly int weightTeleporter;

		[MemoryPackIgnore]
		private static readonly int weightCharger;

		[MemoryPackIgnore]
		private static readonly int weightCrowShop;

		[MemoryPackIgnore]
		private static readonly int topK;

		[MemoryPackIgnore]
		private static readonly int maxRandomPool;

		[MemoryPackIgnore]
		private static readonly float playTimeInterval;

		[MemoryPackIgnore]
		private static readonly float deathMatchPlayTimeInterval;

		public SpeechType_AdjacentPlayerCount AdjacentPlayerCount;

		public SpeechType_Area Area;

		public SpeechType_GameTime GameTime;

		public SpeechType_FacingPlayerCount FacingPlayerCount;

		public List<int> ScrapObjects;

		public List<int> Monsters;

		public SpeechType_Teleporter Teleporter;

		public SpeechType_IndoorEntered IndoorEntered;

		public SpeechType_Charger Charger;

		public SpeechType_CrowShop CrowShop;

		public List<IncomingEvent> IncomingEvent;

		public SpeechEventAdditionalGameData()
		{
			AdjacentPlayerCount = SpeechType_AdjacentPlayerCount.Monologue;
			Area = SpeechType_Area.None;
			GameTime = SpeechType_GameTime.First10;
			FacingPlayerCount = SpeechType_FacingPlayerCount.None;
			ScrapObjects = new List<int>();
			Monsters = new List<int>();
			Teleporter = SpeechType_Teleporter.None;
			Charger = SpeechType_Charger.None;
			IndoorEntered = SpeechType_IndoorEntered.None;
			CrowShop = SpeechType_CrowShop.None;
			IncomingEvent = new List<IncomingEvent>();
		}

		[MemoryPackConstructor]
		public SpeechEventAdditionalGameData(SpeechType_AdjacentPlayerCount adjacentPlayerCount, SpeechType_Area area, SpeechType_GameTime gameTime, SpeechType_FacingPlayerCount facingPlayerCount, List<int> scrapObjects, List<int> monsters, SpeechType_Teleporter teleporter, SpeechType_IndoorEntered indoorEntered, SpeechType_Charger charger, SpeechType_CrowShop crowShop, List<IncomingEvent> incomingEvent)
		{
			AdjacentPlayerCount = adjacentPlayerCount;
			Area = area;
			GameTime = gameTime;
			FacingPlayerCount = facingPlayerCount;
			if (scrapObjects == null)
			{
				ScrapObjects = new List<int>();
			}
			else
			{
				ScrapObjects = new List<int>(scrapObjects);
			}
			if (monsters == null)
			{
				Monsters = new List<int>();
			}
			else
			{
				Monsters = new List<int>(monsters);
			}
			Teleporter = teleporter;
			IndoorEntered = indoorEntered;
			Charger = charger;
			CrowShop = crowShop;
			if (incomingEvent == null)
			{
				IncomingEvent = new List<IncomingEvent>();
			}
			else
			{
				IncomingEvent = new List<IncomingEvent>(incomingEvent);
			}
		}

		private static float CalcSimTerm(float a, float b, float maxDiff)
		{
			if (maxDiff <= 0f)
			{
				return 0f;
			}
			float num = Math.Abs(a - b) / maxDiff;
			float num2 = 1f - num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			else if (num2 > 1f)
			{
				num2 = 1f;
			}
			return num2;
		}

		public static bool PickTransmitterVoice(List<(string playerID, SpeechEvent evt)> allEvents, out SpeechEvent? speechEvent, out string mimickingPlayerID)
		{
			speechEvent = null;
			mimickingPlayerID = string.Empty;
			List<(string, SpeechEvent)> list = allEvents.OrderBy<(string, SpeechEvent), float>(((string playerID, SpeechEvent evt) item) => UnityEngine.Random.value).ToList().Where<(string, SpeechEvent)>(delegate((string playerID, SpeechEvent evt) pair)
			{
				SpeechEventAdditionalGameData gameData = pair.evt.GameData;
				if (gameData == null)
				{
					return false;
				}
				return !((float)Hub.s.timeutil.GetCurrentTickSec() - pair.evt.LastPlayedTime < playTimeInterval) && gameData.Area == SpeechType_Area.Transmitter;
			})
				.ToList();
			if (list.Count == 0)
			{
				return false;
			}
			int index = UnityEngine.Random.Range(0, list.Count);
			(string, SpeechEvent) tuple = list[index];
			speechEvent = tuple.Item2;
			(mimickingPlayerID, _) = tuple;
			return true;
		}

		public static float CalculateSimilarity(SpeechEventAdditionalGameData sEventGameData, SpeechEventAdditionalGameData curGameData)
		{
			float a = ((sEventGameData.Monsters.Count < 3) ? ((float)sEventGameData.Monsters.Count) : 3f);
			float b = ((curGameData.Monsters.Count < 3) ? ((float)curGameData.Monsters.Count) : 3f);
			float num = CalcSimTerm(a, b, 3f);
			float num2 = 0f + (float)weightMonsters * num;
			float a2 = (float)sEventGameData.AdjacentPlayerCount;
			float b2 = (float)curGameData.AdjacentPlayerCount;
			float num3 = CalcSimTerm(a2, b2, 2f);
			float num4 = num2 + (float)weightPlayerCount * num3;
			float a3 = ((sEventGameData.FacingPlayerCount == SpeechType_FacingPlayerCount.None) ? 0f : 1f);
			float b3 = ((curGameData.FacingPlayerCount == SpeechType_FacingPlayerCount.None) ? 0f : 1f);
			float num5 = CalcSimTerm(a3, b3, 1f);
			float num6 = num4 + (float)weightFacingPlayerCount * num5;
			float a4 = ((sEventGameData.Teleporter == SpeechType_Teleporter.None) ? 0f : 1f);
			float b4 = ((curGameData.Teleporter == SpeechType_Teleporter.None) ? 0f : 1f);
			float num7 = CalcSimTerm(a4, b4, 1f);
			float num8 = num6 + (float)weightTeleporter * num7;
			float a5 = (float)sEventGameData.GameTime;
			float b5 = (float)curGameData.GameTime;
			float num9 = CalcSimTerm(a5, b5, 2f);
			float num10 = num8 + (float)weightPlayTime * num9;
			List<int> list = sEventGameData.ScrapObjects.Where((int x) => IsOneHandScrap(x)).ToList();
			List<int> list2 = curGameData.ScrapObjects.Where((int x) => IsOneHandScrap(x)).ToList();
			float a6 = ((list.Count <= 2) ? ((float)list.Count) : 3f);
			float b6 = ((list2.Count <= 2) ? ((float)list2.Count) : 3f);
			float num11 = CalcSimTerm(a6, b6, 3f);
			float num12 = num10 + (float)weightOneHandScrapObject * num11;
			List<int> list3 = sEventGameData.ScrapObjects.Where((int x) => !IsOneHandScrap(x)).ToList();
			List<int> list4 = curGameData.ScrapObjects.Where((int x) => !IsOneHandScrap(x)).ToList();
			float a7 = ((list3.Count <= 1) ? ((float)list3.Count) : 2f);
			float b7 = ((list4.Count <= 1) ? ((float)list4.Count) : 2f);
			float num13 = CalcSimTerm(a7, b7, 2f);
			float num14 = num12 + (float)weightTwoHandScrapObject * num13;
			float a8 = ((sEventGameData.Charger == SpeechType_Charger.None) ? 0f : 1f);
			float b8 = ((curGameData.Charger == SpeechType_Charger.None) ? 0f : 1f);
			float num15 = CalcSimTerm(a8, b8, 1f);
			float num16 = num14 + (float)weightCharger * num15;
			float a9 = ((sEventGameData.CrowShop == SpeechType_CrowShop.None) ? 0f : 1f);
			float b9 = ((curGameData.CrowShop == SpeechType_CrowShop.None) ? 0f : 1f);
			float num17 = CalcSimTerm(a9, b9, 1f);
			return num16 + (float)weightCrowShop * num17;
		}

		private static Dictionary<string, float> CalculateFactorContributions(SpeechEventAdditionalGameData a, SpeechEventAdditionalGameData b)
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			float a2 = Math.Min(a.Monsters.Count, 3);
			float b2 = Math.Min(b.Monsters.Count, 3);
			float value = CalcSimTerm(a2, b2, 3f) * (float)weightMonsters;
			dictionary["Monsters"] = value;
			float a3 = (float)a.AdjacentPlayerCount;
			float b3 = (float)b.AdjacentPlayerCount;
			float value2 = CalcSimTerm(a3, b3, 2f) * (float)weightPlayerCount;
			dictionary["AdjacentPlayerCount"] = value2;
			float a4 = ((a.FacingPlayerCount == SpeechType_FacingPlayerCount.None) ? 0f : 1f);
			float b4 = ((b.FacingPlayerCount == SpeechType_FacingPlayerCount.None) ? 0f : 1f);
			float value3 = CalcSimTerm(a4, b4, 1f) * (float)weightFacingPlayerCount;
			dictionary["FacingPlayerCount"] = value3;
			float a5 = ((a.Teleporter == SpeechType_Teleporter.None) ? 0f : 1f);
			float b5 = ((b.Teleporter == SpeechType_Teleporter.None) ? 0f : 1f);
			float value4 = CalcSimTerm(a5, b5, 1f) * (float)weightTeleporter;
			dictionary["Teleporter"] = value4;
			float a6 = (float)a.GameTime;
			float b6 = (float)b.GameTime;
			float value5 = CalcSimTerm(a6, b6, 2f) * (float)weightPlayTime;
			dictionary["GameTime"] = value5;
			int val = a.ScrapObjects.Count((int x) => IsOneHandScrap(x));
			float value6 = CalcSimTerm(b: Math.Min(b.ScrapObjects.Count((int x) => IsOneHandScrap(x)), 3), a: Math.Min(val, 3), maxDiff: 3f) * (float)weightOneHandScrapObject;
			dictionary["OneHandScrap"] = value6;
			int val2 = a.ScrapObjects.Count((int x) => !IsOneHandScrap(x));
			float value7 = CalcSimTerm(b: Math.Min(b.ScrapObjects.Count((int x) => !IsOneHandScrap(x)), 2), a: Math.Min(val2, 2), maxDiff: 2f) * (float)weightTwoHandScrapObject;
			dictionary["TwoHandScrap"] = value7;
			float a7 = ((a.Charger == SpeechType_Charger.None) ? 0f : 1f);
			float b7 = ((b.Charger == SpeechType_Charger.None) ? 0f : 1f);
			float value8 = CalcSimTerm(a7, b7, 1f) * (float)weightCharger;
			dictionary["Charger"] = value8;
			float a8 = ((a.CrowShop == SpeechType_CrowShop.None) ? 0f : 1f);
			float b8 = ((b.CrowShop == SpeechType_CrowShop.None) ? 0f : 1f);
			float value9 = CalcSimTerm(a8, b8, 1f) * (float)weightCrowShop;
			dictionary["CrowShop"] = value9;
			return dictionary;
		}

		private static string GetTopFactor(SpeechEventAdditionalGameData a, SpeechEventAdditionalGameData b)
		{
			return (from kv in CalculateFactorContributions(a, b)
				orderby kv.Value descending
				select kv).First().Key;
		}

		private static bool TryPickFromObservation(List<(string playerID, SpeechEvent evt)> allEvents, SpeechEventAdditionalGameData curGameData, bool periodic, out SpeechEvent? speechEvent, out string mimickingPlayerID, out string pickReason)
		{
			speechEvent = null;
			mimickingPlayerID = string.Empty;
			pickReason = string.Empty;
			List<(string, SpeechEvent, float)> list = (from item in allEvents.Select<(string, SpeechEvent), (string, SpeechEvent, float)>(((string playerID, SpeechEvent evt) pair) => (playerId: pair.playerID, evt: pair.evt, similarity: CalculateSimilarity(pair.evt.GameData, curGameData))).ToList()
				orderby item.similarity descending
				select item).Take(topK).ToList();
			if (list.Count <= 0)
			{
				return false;
			}
			(string, SpeechEvent, float) tuple = list.OrderBy<(string, SpeechEvent, float), float>(((string playerId, SpeechEvent evt, float similarity) evt) => evt.evt.LastPlayedTime).First();
			speechEvent = tuple.Item2;
			mimickingPlayerID = tuple.Item1;
			pickReason = GetTopFactor(tuple.Item2.GameData, curGameData);
			return true;
		}

		public static bool PickBestMatch(MimicVoiceSpawner.MimicContext context, List<(string playerID, SpeechEvent evt)> allEvents, SpeechEventAdditionalGameData curGameData, bool periodic, int pickCount, float playTimeIntervalRandom, out SpeechEvent? speechEvent, out string mimickingPlayerID, out string pickReason)
		{
			speechEvent = null;
			mimickingPlayerID = string.Empty;
			pickReason = string.Empty;
			float playTimeIntervalTemp = 0f;
			if (curGameData.Area == SpeechType_Area.DeathMatch)
			{
				playTimeIntervalTemp = deathMatchPlayTimeInterval + playTimeIntervalRandom;
			}
			else
			{
				playTimeIntervalTemp = playTimeInterval;
			}
			int timeIntervalFailedCount = 0;
			List<(string, SpeechEvent)> list = allEvents.Where<(string, SpeechEvent)>(delegate((string playerID, SpeechEvent evt) pair)
			{
				if (pair.evt.GameData == null)
				{
					return false;
				}
				if ((float)Hub.s.timeutil.GetCurrentTickSec() - pair.evt.LastPlayedTime < playTimeIntervalTemp)
				{
					timeIntervalFailedCount++;
					return false;
				}
				return true;
			}).ToList();
			if (list.Count == 0)
			{
				return false;
			}
			List<(string, SpeechEvent)> list2 = (from _ in list.Where<(string, SpeechEvent)>(delegate((string playerID, SpeechEvent evt) pair)
				{
					SpeechEventAdditionalGameData gameData = pair.evt.GameData;
					if (gameData == null)
					{
						return false;
					}
					return gameData.IndoorEntered == curGameData.IndoorEntered && gameData.Area == curGameData.Area;
				})
				orderby UnityEngine.Random.value
				select _).ToList();
			if (list2.Count == 0)
			{
				return false;
			}
			if (list2.Count <= pickCount)
			{
				(string, SpeechEvent) tuple = list2.OrderBy<(string, SpeechEvent), float>(((string playerID, SpeechEvent evt) evt) => evt.evt.LastPlayedTime).First();
				speechEvent = tuple.Item2;
				mimickingPlayerID = tuple.Item1;
				pickReason = $"Area({curGameData.Area}), Indoor({curGameData.IndoorEntered})";
				return true;
			}
			if (curGameData.IncomingEvent == null || curGameData.IncomingEvent.Count == 0)
			{
				List<(string, SpeechEvent)> list3 = list2.Where<(string, SpeechEvent)>(((string playerID, SpeechEvent evt) pair) => pair.evt.GameData.IncomingEvent == null || pair.evt.GameData.IncomingEvent.Count == 0).ToList();
				if (list3.Count == 0)
				{
					return false;
				}
				if (list3.Count <= pickCount)
				{
					(string, SpeechEvent) tuple2 = list3.OrderBy<(string, SpeechEvent), float>(((string playerID, SpeechEvent evt) evt) => evt.evt.LastPlayedTime).First();
					speechEvent = tuple2.Item2;
					mimickingPlayerID = tuple2.Item1;
					pickReason = "NoIncomingEvents";
					return true;
				}
				return TryPickFromObservation(list3, curGameData, periodic, out speechEvent, out mimickingPlayerID, out pickReason);
			}
			SpeechEvent_IncomingType[] obj = new SpeechEvent_IncomingType[21]
			{
				SpeechEvent_IncomingType.Monster,
				SpeechEvent_IncomingType.User,
				SpeechEvent_IncomingType.OnDamaged,
				SpeechEvent_IncomingType.HandHeldItem,
				SpeechEvent_IncomingType.TimeBombWarning,
				SpeechEvent_IncomingType.SprinklerActivated,
				SpeechEvent_IncomingType.Lightning,
				SpeechEvent_IncomingType.HeliumGasActivated,
				SpeechEvent_IncomingType.InvisibleMine,
				SpeechEvent_IncomingType.GrabSkill,
				SpeechEvent_IncomingType.CorridorSwitches,
				SpeechEvent_IncomingType.Paintspot,
				SpeechEvent_IncomingType.Paintball,
				SpeechEvent_IncomingType.ScrapObject,
				SpeechEvent_IncomingType.Charger,
				SpeechEvent_IncomingType.CrowShop,
				SpeechEvent_IncomingType.Teleporter,
				SpeechEvent_IncomingType.ClosedRoom,
				SpeechEvent_IncomingType.SquallWarning,
				SpeechEvent_IncomingType.Blackout,
				SpeechEvent_IncomingType.ChargeCompleted
			};
			HashSet<SpeechEvent_IncomingType> hashSet = curGameData.IncomingEvent.Select((IncomingEvent evt) => evt.EventType).Distinct().ToHashSet();
			List<(string, SpeechEvent)> list4 = new List<(string, SpeechEvent)>();
			HashSet<SpeechEvent_IncomingType> hashSet2 = new HashSet<SpeechEvent_IncomingType>();
			SpeechEvent_IncomingType[] array = obj;
			foreach (SpeechEvent_IncomingType incType in array)
			{
				if (!hashSet.Contains(incType))
				{
					continue;
				}
				HashSet<int> eventIDsForThisType = (from e in curGameData.IncomingEvent
					where e.EventType == incType
					select e.EventID).Distinct().ToHashSet();
				foreach (var item3 in list2)
				{
					string item = item3.Item1;
					SpeechEvent item2 = item3.Item2;
					bool flag = item2.GameData.IncomingEvent.Any((IncomingEvent e) => e.EventType == incType && eventIDsForThisType.Contains(e.EventID));
					if (incType == SpeechEvent_IncomingType.Monster && eventIDsForThisType.Contains(20000010))
					{
						bool flag2 = item2.GameData.IncomingEvent?.Any((IncomingEvent e) => e.EventType == SpeechEvent_IncomingType.GrabSkill && e.EventID == 20000007) ?? false;
						flag = flag || flag2;
						if (flag2)
						{
							hashSet2.Add(SpeechEvent_IncomingType.GrabSkill);
						}
					}
					if (flag)
					{
						list4.Add((item, item2));
						hashSet2.Add(incType);
					}
				}
				if (list4.Count >= maxRandomPool)
				{
					break;
				}
			}
			if (list4.Count > 0)
			{
				(string, SpeechEvent) tuple3 = list4.OrderBy(((string, SpeechEvent) evt) => evt.Item2.LastPlayedTime).First();
				speechEvent = tuple3.Item2;
				mimickingPlayerID = tuple3.Item1;
				pickReason = "Incoming:" + string.Join(", ", hashSet2);
				return true;
			}
			return false;
		}

		public static SpeechType_GameTime GetGameTime()
		{
			if (Hub.s?.dataman?.ExcelDataManager == null)
			{
				return SpeechType_GameTime.First10;
			}
			DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(Hub.s.pdata.dungeonMasterID);
			if (dungeonInfo == null)
			{
				return SpeechType_GameTime.First10;
			}
			(long, long) tuple = VWorldUtil.ConvertTimeToSeconds(dungeonInfo.StartDisplayTime, dungeonInfo.EndTime);
			long item = tuple.Item1;
			float num = tuple.Item2 - item;
			float num2 = ((float)Hub.s.pdata.main.CurrentTime.TotalSeconds - (float)item) / num;
			if (num2 < 0.1f)
			{
				return SpeechType_GameTime.First10;
			}
			if (num2 >= 0.9f)
			{
				return SpeechType_GameTime.Last10;
			}
			return SpeechType_GameTime.Middle;
		}

		private static IEnumerable<ProtoActor> GetAllActors()
		{
			return UnityEngine.Object.FindObjectsByType<ProtoActor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		}

		public static ProtoActor? GetMyActor()
		{
			return GetAllActors().FirstOrDefault((ProtoActor a) => a.controlMode == ProtoActor.ControlMode.Manual);
		}

		private static bool IsOneHandScrap(int itemID)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemID);
			if (itemInfo != null && itemInfo.ItemType == ItemType.Miscellany)
			{
				if (!(itemInfo as ItemMiscellanyInfo).ForbidChange)
				{
					return true;
				}
			}
			else if (itemInfo != null && itemInfo.ItemType == ItemType.Equipment)
			{
				if (!(itemInfo as ItemEquipmentInfo).IsTwoHanded)
				{
					return true;
				}
			}
			else if (itemInfo != null && itemInfo.ItemType == ItemType.Consumable)
			{
				return true;
			}
			return false;
		}

		static SpeechEventAdditionalGameData()
		{
			weightPlayerCount = 1;
			weightArea = 1;
			weightPlayTime = 1;
			weightFacingPlayerCount = 2;
			weightOneHandScrapObject = 1;
			weightTwoHandScrapObject = 1;
			weightMonsters = 1;
			weightTeleporter = 1;
			weightCharger = 1;
			weightCrowShop = 1;
			topK = 5;
			maxRandomPool = 5;
			playTimeInterval = 60f;
			deathMatchPlayTimeInterval = 3f;
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechEventAdditionalGameData>())
			{
				MemoryPackFormatterProvider.Register(new SpeechEventAdditionalGameDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechEventAdditionalGameData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<SpeechEventAdditionalGameData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_AdjacentPlayerCount>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_AdjacentPlayerCount>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_Area>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_Area>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_GameTime>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_GameTime>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_FacingPlayerCount>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_FacingPlayerCount>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_Teleporter>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_Teleporter>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_IndoorEntered>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_IndoorEntered>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_Charger>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_Charger>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechType_CrowShop>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechType_CrowShop>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<IncomingEvent>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<IncomingEvent>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SpeechEventAdditionalGameData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(11, in value.AdjacentPlayerCount, in value.Area, in value.GameTime, in value.FacingPlayerCount);
			writer.WriteValue(in value.ScrapObjects);
			writer.WriteValue(in value.Monsters);
			writer.WriteUnmanaged(in value.Teleporter, in value.IndoorEntered, in value.Charger, in value.CrowShop);
			ListFormatter.SerializePackable(ref writer, value.IncomingEvent);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref SpeechEventAdditionalGameData? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			SpeechType_AdjacentPlayerCount value2;
			SpeechType_Area value3;
			SpeechType_GameTime value4;
			SpeechType_FacingPlayerCount value5;
			List<int> value6;
			List<int> value7;
			SpeechType_Teleporter value8;
			SpeechType_IndoorEntered value9;
			SpeechType_Charger value10;
			SpeechType_CrowShop value11;
			List<IncomingEvent> value12;
			if (memberCount == 11)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<SpeechType_AdjacentPlayerCount, SpeechType_Area, SpeechType_GameTime, SpeechType_FacingPlayerCount>(out value2, out value3, out value4, out value5);
					value6 = reader.ReadValue<List<int>>();
					value7 = reader.ReadValue<List<int>>();
					reader.ReadUnmanaged<SpeechType_Teleporter, SpeechType_IndoorEntered, SpeechType_Charger, SpeechType_CrowShop>(out value8, out value9, out value10, out value11);
					value12 = ListFormatter.DeserializePackable<IncomingEvent>(ref reader);
				}
				else
				{
					value2 = value.AdjacentPlayerCount;
					value3 = value.Area;
					value4 = value.GameTime;
					value5 = value.FacingPlayerCount;
					value6 = value.ScrapObjects;
					value7 = value.Monsters;
					value8 = value.Teleporter;
					value9 = value.IndoorEntered;
					value10 = value.Charger;
					value11 = value.CrowShop;
					value12 = value.IncomingEvent;
					reader.ReadUnmanaged<SpeechType_AdjacentPlayerCount>(out value2);
					reader.ReadUnmanaged<SpeechType_Area>(out value3);
					reader.ReadUnmanaged<SpeechType_GameTime>(out value4);
					reader.ReadUnmanaged<SpeechType_FacingPlayerCount>(out value5);
					reader.ReadValue(ref value6);
					reader.ReadValue(ref value7);
					reader.ReadUnmanaged<SpeechType_Teleporter>(out value8);
					reader.ReadUnmanaged<SpeechType_IndoorEntered>(out value9);
					reader.ReadUnmanaged<SpeechType_Charger>(out value10);
					reader.ReadUnmanaged<SpeechType_CrowShop>(out value11);
					ListFormatter.DeserializePackable(ref reader, ref value12);
				}
			}
			else
			{
				if (memberCount > 11)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SpeechEventAdditionalGameData), 11, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = SpeechType_AdjacentPlayerCount.Monologue;
					value3 = SpeechType_Area.None;
					value4 = SpeechType_GameTime.First10;
					value5 = SpeechType_FacingPlayerCount.None;
					value6 = null;
					value7 = null;
					value8 = SpeechType_Teleporter.None;
					value9 = SpeechType_IndoorEntered.None;
					value10 = SpeechType_Charger.None;
					value11 = SpeechType_CrowShop.None;
					value12 = null;
				}
				else
				{
					value2 = value.AdjacentPlayerCount;
					value3 = value.Area;
					value4 = value.GameTime;
					value5 = value.FacingPlayerCount;
					value6 = value.ScrapObjects;
					value7 = value.Monsters;
					value8 = value.Teleporter;
					value9 = value.IndoorEntered;
					value10 = value.Charger;
					value11 = value.CrowShop;
					value12 = value.IncomingEvent;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<SpeechType_AdjacentPlayerCount>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<SpeechType_Area>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<SpeechType_GameTime>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<SpeechType_FacingPlayerCount>(out value5);
								if (memberCount != 4)
								{
									reader.ReadValue(ref value6);
									if (memberCount != 5)
									{
										reader.ReadValue(ref value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<SpeechType_Teleporter>(out value8);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<SpeechType_IndoorEntered>(out value9);
												if (memberCount != 8)
												{
													reader.ReadUnmanaged<SpeechType_Charger>(out value10);
													if (memberCount != 9)
													{
														reader.ReadUnmanaged<SpeechType_CrowShop>(out value11);
														if (memberCount != 10)
														{
															ListFormatter.DeserializePackable(ref reader, ref value12);
															_ = 11;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				_ = value;
			}
			value = new SpeechEventAdditionalGameData(value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12);
		}
	}
}
