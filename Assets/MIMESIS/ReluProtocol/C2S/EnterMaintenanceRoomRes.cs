using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class EnterMaintenanceRoomRes : IResMsg, IMemoryPackable<EnterMaintenanceRoomRes>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EnterMaintenanceRoomResFormatter : MemoryPackFormatter<EnterMaintenanceRoomRes>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterMaintenanceRoomRes value)
			{
				EnterMaintenanceRoomRes.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EnterMaintenanceRoomRes value)
			{
				EnterMaintenanceRoomRes.Deserialize(ref reader, ref value);
			}
		}

		public PlayerInfo playerInfo { get; set; } = new PlayerInfo();

		public List<PlayerSessionInfo> memberList { get; set; } = new List<PlayerSessionInfo>();

		public int currency { get; set; }

		public int cycleCount { get; set; }

		public int dayCount { get; set; }

		public bool repaired { get; set; }

		public Dictionary<int, int> itemPrices { get; set; } = new Dictionary<int, int>();

		public bool inPlaying { get; set; }

		public string roomSessionID { get; set; } = string.Empty;

		public (int leftPanel, int rightPanel) tramUpgradeCandidate { get; set; } = (leftPanel: 0, rightPanel: 0);

		[MemoryPackConstructor]
		public EnterMaintenanceRoomRes(int hashCode)
			: base(MsgType.C2S_EnterMaintenenceRoomRes, hashCode)
		{
			base.reliable = true;
		}

		public EnterMaintenanceRoomRes()
			: this(0)
		{
		}

		static EnterMaintenanceRoomRes()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EnterMaintenanceRoomRes>())
			{
				MemoryPackFormatterProvider.Register(new EnterMaintenanceRoomResFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EnterMaintenanceRoomRes[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EnterMaintenanceRoomRes>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<PlayerSessionInfo>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<PlayerSessionInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, int>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<(int, int)>())
			{
				MemoryPackFormatterProvider.Register(new ValueTupleFormatter<int, int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterMaintenanceRoomRes? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(13, value.msgType, value.hashCode, value.errorCode);
			writer.WritePackable<PlayerInfo>(value.playerInfo);
			ListFormatter.SerializePackable(ref writer, value.memberList);
			writer.WriteUnmanaged<int, int, int, bool>(value.currency, value.cycleCount, value.dayCount, value.repaired);
			writer.WriteValue<Dictionary<int, int>>(value.itemPrices);
			writer.WriteUnmanaged<bool>(value.inPlaying);
			writer.WriteString(value.roomSessionID);
			writer.WriteUnmanaged<(int, int)>(in ILSpyHelper_AsRefReadOnly(value.tramUpgradeCandidate));
			static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
			{
				//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
				return ref temp;
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EnterMaintenanceRoomRes? value)
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
			List<PlayerSessionInfo> value6;
			int value7;
			int value8;
			int value9;
			bool value10;
			Dictionary<int, int> value11;
			bool value12;
			(int, int) value13;
			string text;
			if (memberCount == 13)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
					value5 = reader.ReadPackable<PlayerInfo>();
					value6 = ListFormatter.DeserializePackable<PlayerSessionInfo>(ref reader);
					reader.ReadUnmanaged<int, int, int, bool>(out value7, out value8, out value9, out value10);
					value11 = reader.ReadValue<Dictionary<int, int>>();
					reader.ReadUnmanaged<bool>(out value12);
					text = reader.ReadString();
					reader.ReadUnmanaged<(int, int)>(out value13);
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.playerInfo;
					value6 = value.memberList;
					value7 = value.currency;
					value8 = value.cycleCount;
					value9 = value.dayCount;
					value10 = value.repaired;
					value11 = value.itemPrices;
					value12 = value.inPlaying;
					text = value.roomSessionID;
					value13 = value.tramUpgradeCandidate;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<MsgErrorCode>(out value4);
					reader.ReadPackable(ref value5);
					ListFormatter.DeserializePackable(ref reader, ref value6);
					reader.ReadUnmanaged<int>(out value7);
					reader.ReadUnmanaged<int>(out value8);
					reader.ReadUnmanaged<int>(out value9);
					reader.ReadUnmanaged<bool>(out value10);
					reader.ReadValue(ref value11);
					reader.ReadUnmanaged<bool>(out value12);
					text = reader.ReadString();
					reader.ReadUnmanaged<(int, int)>(out value13);
				}
			}
			else
			{
				if (memberCount > 13)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EnterMaintenanceRoomRes), 13, memberCount);
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
					value8 = 0;
					value9 = 0;
					value10 = false;
					value11 = null;
					value12 = false;
					text = null;
					value13 = default((int, int));
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.playerInfo;
					value6 = value.memberList;
					value7 = value.currency;
					value8 = value.cycleCount;
					value9 = value.dayCount;
					value10 = value.repaired;
					value11 = value.itemPrices;
					value12 = value.inPlaying;
					text = value.roomSessionID;
					value13 = value.tramUpgradeCandidate;
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
											reader.ReadUnmanaged<int>(out value8);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<int>(out value9);
												if (memberCount != 8)
												{
													reader.ReadUnmanaged<bool>(out value10);
													if (memberCount != 9)
													{
														reader.ReadValue(ref value11);
														if (memberCount != 10)
														{
															reader.ReadUnmanaged<bool>(out value12);
															if (memberCount != 11)
															{
																text = reader.ReadString();
																if (memberCount != 12)
																{
																	reader.ReadUnmanaged<(int, int)>(out value13);
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
			value = new EnterMaintenanceRoomRes(value3)
			{
				msgType = value2,
				errorCode = value4,
				playerInfo = value5,
				memberList = value6,
				currency = value7,
				cycleCount = value8,
				dayCount = value9,
				repaired = value10,
				itemPrices = value11,
				inPlaying = value12,
				roomSessionID = text,
				tramUpgradeCandidate = value13
			};
		}
	}
}
