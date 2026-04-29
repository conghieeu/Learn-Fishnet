using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class LevelLoadCompleteRes : IResMsg, IMemoryPackable<LevelLoadCompleteRes>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class LevelLoadCompleteResFormatter : MemoryPackFormatter<LevelLoadCompleteRes>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LevelLoadCompleteRes value)
			{
				LevelLoadCompleteRes.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref LevelLoadCompleteRes value)
			{
				LevelLoadCompleteRes.Deserialize(ref reader, ref value);
			}
		}

		public PlayerInfo selfInfo { get; set; } = new PlayerInfo();

		public List<LevelObjectInfo> levelObjects { get; set; } = new List<LevelObjectInfo>();

		public int targetCurrency { get; set; }

		public TimeSpan currentTime { get; set; }

		public bool firstEnterMap { get; set; }

		public (int, float) boostedItem { get; set; } = (0, 0f);

		public int sessionCount { get; set; }

		public int dayCount { get; set; }

		public Dictionary<int, ItemInfo> stashes { get; set; } = new Dictionary<int, ItemInfo>();

		public List<int> tramUpgradeList { get; set; } = new List<int>();

		[MemoryPackConstructor]
		public LevelLoadCompleteRes(int hashCode)
			: base(MsgType.C2S_LevelLoadCompleteRes, hashCode)
		{
			base.reliable = true;
		}

		public LevelLoadCompleteRes()
			: this(0)
		{
		}

		static LevelLoadCompleteRes()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<LevelLoadCompleteRes>())
			{
				MemoryPackFormatterProvider.Register(new LevelLoadCompleteResFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<LevelLoadCompleteRes[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<LevelLoadCompleteRes>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<LevelObjectInfo>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<LevelObjectInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<(int, float)>())
			{
				MemoryPackFormatterProvider.Register(new ValueTupleFormatter<int, float>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, ItemInfo>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, ItemInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LevelLoadCompleteRes? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(13, value.msgType, value.hashCode, value.errorCode);
			writer.WritePackable<PlayerInfo>(value.selfInfo);
			ListFormatter.SerializePackable(ref writer, value.levelObjects);
			writer.WriteUnmanaged<int, TimeSpan, bool, (int, float), int, int>(in ILSpyHelper_AsRefReadOnly(value.targetCurrency), in ILSpyHelper_AsRefReadOnly(value.currentTime), in ILSpyHelper_AsRefReadOnly(value.firstEnterMap), in ILSpyHelper_AsRefReadOnly(value.boostedItem), in ILSpyHelper_AsRefReadOnly(value.sessionCount), in ILSpyHelper_AsRefReadOnly(value.dayCount));
			writer.WriteValue<Dictionary<int, ItemInfo>>(value.stashes);
			writer.WriteValue<List<int>>(value.tramUpgradeList);
			static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
			{
				//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
				return ref temp;
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref LevelLoadCompleteRes? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			MsgErrorCode value4;
			PlayerInfo value5;
			List<LevelObjectInfo> value6;
			int value7;
			TimeSpan value8;
			bool value9;
			(int, float) value10;
			int value11;
			int value12;
			Dictionary<int, ItemInfo> value13;
			List<int> value14;
			if (memberCount == 13)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
					value5 = reader.ReadPackable<PlayerInfo>();
					value6 = ListFormatter.DeserializePackable<LevelObjectInfo>(ref reader);
					reader.ReadUnmanaged<int, TimeSpan, bool, (int, float), int, int>(out value7, out value8, out value9, out value10, out value11, out value12);
					value13 = reader.ReadValue<Dictionary<int, ItemInfo>>();
					value14 = reader.ReadValue<List<int>>();
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.selfInfo;
					value6 = value.levelObjects;
					value7 = value.targetCurrency;
					value8 = value.currentTime;
					value9 = value.firstEnterMap;
					value10 = value.boostedItem;
					value11 = value.sessionCount;
					value12 = value.dayCount;
					value13 = value.stashes;
					value14 = value.tramUpgradeList;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<MsgErrorCode>(out value4);
					reader.ReadPackable(ref value5);
					ListFormatter.DeserializePackable(ref reader, ref value6);
					reader.ReadUnmanaged<int>(out value7);
					reader.ReadUnmanaged<TimeSpan>(out value8);
					reader.ReadUnmanaged<bool>(out value9);
					reader.ReadUnmanaged<(int, float)>(out value10);
					reader.ReadUnmanaged<int>(out value11);
					reader.ReadUnmanaged<int>(out value12);
					reader.ReadValue(ref value13);
					reader.ReadValue(ref value14);
				}
			}
			else
			{
				if (memberCount > 13)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(LevelLoadCompleteRes), 13, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = MsgErrorCode.Success;
					value5 = null;
					value6 = null;
					value7 = 0;
					value8 = default(TimeSpan);
					value9 = false;
					value10 = default((int, float));
					value11 = 0;
					value12 = 0;
					value13 = null;
					value14 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.selfInfo;
					value6 = value.levelObjects;
					value7 = value.targetCurrency;
					value8 = value.currentTime;
					value9 = value.firstEnterMap;
					value10 = value.boostedItem;
					value11 = value.sessionCount;
					value12 = value.dayCount;
					value13 = value.stashes;
					value14 = value.tramUpgradeList;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<MsgErrorCode>(out value4);
							if (memberCount != 3)
							{
								reader.ReadPackable(ref value5);
								if (memberCount != 4)
								{
									ListFormatter.DeserializePackable(ref reader, ref value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<TimeSpan>(out value8);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<bool>(out value9);
												if (memberCount != 8)
												{
													reader.ReadUnmanaged<(int, float)>(out value10);
													if (memberCount != 9)
													{
														reader.ReadUnmanaged<int>(out value11);
														if (memberCount != 10)
														{
															reader.ReadUnmanaged<int>(out value12);
															if (memberCount != 11)
															{
																reader.ReadValue(ref value13);
																if (memberCount != 12)
																{
																	reader.ReadValue(ref value14);
																	_ = 13;
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
					}
				}
				_ = value;
			}
			value = new LevelLoadCompleteRes(value3)
			{
				msgType = value2,
				errorCode = value4,
				selfInfo = value5,
				levelObjects = value6,
				targetCurrency = value7,
				currentTime = value8,
				firstEnterMap = value9,
				boostedItem = value10,
				sessionCount = value11,
				dayCount = value12,
				stashes = value13,
				tramUpgradeList = value14
			};
		}
	}
}
