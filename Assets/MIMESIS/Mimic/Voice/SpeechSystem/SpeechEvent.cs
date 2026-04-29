using System;
using System.Buffers;
using System.Linq;
using System.Text;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace Mimic.Voice.SpeechSystem
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public class SpeechEvent : IEquatable<SpeechEvent>, IMemoryPackable<SpeechEvent>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class SpeechEventFormatter : MemoryPackFormatter<SpeechEvent>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SpeechEvent value)
			{
				SpeechEvent.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref SpeechEvent value)
			{
				SpeechEvent.Deserialize(ref reader, ref value);
			}
		}

		public readonly long Id;

		public string PlayerName;

		public readonly float RecordedTime;

		public readonly int Channels;

		public readonly int SampleRate;

		[MemoryPackIgnore]
		public readonly byte[] CompressedAudioData;

		public readonly int OriginalAudioDataLength;

		public readonly float AverageAmplitude;

		[MemoryPackAllowSerialize]
		public readonly SpeechEventAdditionalGameData GameData;

		public float LastPlayedTime;

		public int AudioPlayedCount;

		public float Duration
		{
			get
			{
				if (!((float)(Channels * SampleRate) > 0f))
				{
					return 0f;
				}
				return (float)OriginalAudioDataLength / (float)(Channels * SampleRate);
			}
		}

		public SpeechEvent(string playerName, float recordedTime, int channels, int sampleRate, byte[] compressedAudioData, int originalAudioDataLength, float averageAmplitude, DateTime recordedDateTime, SpeechEventAdditionalGameData gameData, float lastPlayedTime)
		{
			Id = recordedDateTime.Ticks;
			PlayerName = playerName;
			RecordedTime = recordedTime;
			Channels = channels;
			SampleRate = sampleRate;
			CompressedAudioData = compressedAudioData;
			OriginalAudioDataLength = originalAudioDataLength;
			AverageAmplitude = averageAmplitude;
			GameData = gameData;
			LastPlayedTime = lastPlayedTime;
		}

		public SpeechEvent(long id, string playerName, float recordedTime, int channels, int sampleRate, byte[] compressedAudioData, int originalAudioDataLength, float averageAmplitude, SpeechEventAdditionalGameData gameData, float lastPlayedTime)
		{
			Id = id;
			PlayerName = playerName;
			RecordedTime = recordedTime;
			Channels = channels;
			SampleRate = sampleRate;
			CompressedAudioData = compressedAudioData;
			OriginalAudioDataLength = originalAudioDataLength;
			AverageAmplitude = averageAmplitude;
			GameData = gameData;
			LastPlayedTime = lastPlayedTime;
		}

		[MemoryPackConstructor]
		public SpeechEvent(long id, string playerName, float recordedTime, int channels, int sampleRate, int originalAudioDataLength, float averageAmplitude, SpeechEventAdditionalGameData gameData, float lastPlayedTime)
		{
			Id = id;
			PlayerName = playerName;
			RecordedTime = recordedTime;
			Channels = channels;
			SampleRate = sampleRate;
			OriginalAudioDataLength = originalAudioDataLength;
			AverageAmplitude = averageAmplitude;
			GameData = gameData;
			LastPlayedTime = lastPlayedTime;
		}

		public bool Equals(SpeechEvent other)
		{
			if (other != null)
			{
				return other.Id == Id;
			}
			return false;
		}

		public override string ToString()
		{
			return $"SpeechEvent(Id={Id}, PlayerName={PlayerName}, RecordedTime={RecordedTime}, Channels={Channels}, SampleRate={SampleRate}, AverageAmplitude={AverageAmplitude}, " + $"AdjacentPlayer={GameData.AdjacentPlayerCount}, Area={GameData.Area}, GameTime={GameData.GameTime}, FacingPlayer={GameData.FacingPlayerCount}) " + string.Format("LevelObjects=[{0}], Monsters=[{1}], Teleporter={2}) ", string.Join(", ", GameData.ScrapObjects), string.Join(", ", GameData.Monsters), GameData.Teleporter) + string.Format("IndoorEntered={0}, Charger={1}, IncomingEvents=[{2}]", GameData.IndoorEntered, GameData.Charger, string.Join(", ", GameData.Monsters));
		}

		public string ToStringSimple()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (GameData.IncomingEvent != null && GameData.IncomingEvent.Count > 0)
			{
				stringBuilder.Append("IEV:[" + string.Join(", ", GameData.IncomingEvent.Select((IncomingEvent x) => x.EventType)) + "]");
			}
			stringBuilder.Append($"AP:{GameData.AdjacentPlayerCount}, A:{GameData.Area}, GT:{GameData.GameTime}, FP:{GameData.FacingPlayerCount}");
			if (GameData.ScrapObjects != null && GameData.ScrapObjects.Count > 0)
			{
				stringBuilder.Append(", Scrap:[" + string.Join(", ", GameData.ScrapObjects) + "]");
			}
			if (GameData.Monsters != null && GameData.Monsters.Count > 0)
			{
				stringBuilder.Append(", MO:[" + string.Join(", ", GameData.Monsters) + "]");
			}
			stringBuilder.Append($", TE:{GameData.Teleporter}");
			stringBuilder.Append($", I:{GameData.IndoorEntered}");
			stringBuilder.Append($", C:{GameData.Charger}");
			stringBuilder.Append($", CS:{GameData.CrowShop}");
			return stringBuilder.ToString();
		}

		public void ToStringIncomingEvent(StringBuilder inSb, float inIntervalTime)
		{
			bool flag = (float)Hub.s.timeutil.GetCurrentTickSec() - LastPlayedTime > inIntervalTime;
			if (GameData.IncomingEvent != null && GameData.IncomingEvent.Count > 0)
			{
				inSb.AppendLine(string.Format("[intervalPass({0})][Area({1})][Event({2})][Interval({3})]", flag, GameData.Area, string.Join(", ", GameData.IncomingEvent.Select((IncomingEvent x) => x.EventType)), inIntervalTime));
			}
			else
			{
				inSb.AppendLine($"[intervalPass({flag})][Area({GameData.Area})][Event(null)][Interval({inIntervalTime})]");
			}
		}

		static SpeechEvent()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechEvent>())
			{
				MemoryPackFormatterProvider.Register(new SpeechEventFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechEvent[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<SpeechEvent>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SpeechEvent? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader(11, in value.Id);
			writer.WriteString(value.PlayerName);
			writer.WriteUnmanaged(in value.RecordedTime, in value.Channels, in value.SampleRate, in value.OriginalAudioDataLength, in value.AverageAmplitude);
			writer.WritePackable(in value.GameData);
			writer.WriteUnmanaged<float, int, float>(in value.LastPlayedTime, in value.AudioPlayedCount, value.Duration);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref SpeechEvent? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			long value2;
			float value3;
			int value4;
			int value5;
			int value6;
			float value7;
			SpeechEventAdditionalGameData value8;
			float value9;
			int value10;
			string playerName;
			if (memberCount == 11)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<long>(out value2);
					playerName = reader.ReadString();
					reader.ReadUnmanaged<float, int, int, int, float>(out value3, out value4, out value5, out value6, out value7);
					value8 = reader.ReadPackable<SpeechEventAdditionalGameData>();
					reader.ReadUnmanaged<float, int, float>(out value9, out value10, out var _);
				}
				else
				{
					value2 = value.Id;
					playerName = value.PlayerName;
					value3 = value.RecordedTime;
					value4 = value.Channels;
					value5 = value.SampleRate;
					value6 = value.OriginalAudioDataLength;
					value7 = value.AverageAmplitude;
					value8 = value.GameData;
					value9 = value.LastPlayedTime;
					value10 = value.AudioPlayedCount;
					float value11 = value.Duration;
					reader.ReadUnmanaged<long>(out value2);
					playerName = reader.ReadString();
					reader.ReadUnmanaged<float>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					reader.ReadUnmanaged<int>(out value6);
					reader.ReadUnmanaged<float>(out value7);
					reader.ReadPackable(ref value8);
					reader.ReadUnmanaged<float>(out value9);
					reader.ReadUnmanaged<int>(out value10);
					reader.ReadUnmanaged<float>(out value11);
				}
			}
			else
			{
				if (memberCount > 11)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SpeechEvent), 11, memberCount);
					return;
				}
				float value11;
				if (value == null)
				{
					value2 = 0L;
					playerName = null;
					value3 = 0f;
					value4 = 0;
					value5 = 0;
					value6 = 0;
					value7 = 0f;
					value8 = null;
					value9 = 0f;
					value10 = 0;
					value11 = 0f;
				}
				else
				{
					value2 = value.Id;
					playerName = value.PlayerName;
					value3 = value.RecordedTime;
					value4 = value.Channels;
					value5 = value.SampleRate;
					value6 = value.OriginalAudioDataLength;
					value7 = value.AverageAmplitude;
					value8 = value.GameData;
					value9 = value.LastPlayedTime;
					value10 = value.AudioPlayedCount;
					value11 = value.Duration;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<long>(out value2);
					if (memberCount != 1)
					{
						playerName = reader.ReadString();
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<float>(out value3);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<int>(out value4);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<int>(out value5);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value6);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<float>(out value7);
											if (memberCount != 7)
											{
												reader.ReadPackable(ref value8);
												if (memberCount != 8)
												{
													reader.ReadUnmanaged<float>(out value9);
													if (memberCount != 9)
													{
														reader.ReadUnmanaged<int>(out value10);
														if (memberCount != 10)
														{
															reader.ReadUnmanaged<float>(out value11);
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
			value = new SpeechEvent(value2, playerName, value3, value4, value5, value6, value7, value8, value9)
			{
				AudioPlayedCount = value10
			};
		}
	}
}
