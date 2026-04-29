using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class EnterWaitingRoomRes : IResMsg, IMemoryPackable<EnterWaitingRoomRes>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EnterWaitingRoomResFormatter : MemoryPackFormatter<EnterWaitingRoomRes>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterWaitingRoomRes value)
			{
				EnterWaitingRoomRes.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EnterWaitingRoomRes value)
			{
				EnterWaitingRoomRes.Deserialize(ref reader, ref value);
			}
		}

		public PlayerInfo myVPlayerInfo { get; set; } = new PlayerInfo();

		public List<int> nextGameDungeonMasterIDs { get; set; } = new List<int>();

		public List<PlayerSessionInfo> memberList { get; set; } = new List<PlayerSessionInfo>();

		[MemoryPackConstructor]
		public EnterWaitingRoomRes(int hashCode)
			: base(MsgType.C2S_EnterWaitingRoomRes, hashCode)
		{
			base.reliable = true;
		}

		public EnterWaitingRoomRes()
			: this(0)
		{
		}

		static EnterWaitingRoomRes()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EnterWaitingRoomRes>())
			{
				MemoryPackFormatterProvider.Register(new EnterWaitingRoomResFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EnterWaitingRoomRes[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EnterWaitingRoomRes>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<PlayerSessionInfo>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<PlayerSessionInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterWaitingRoomRes? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(6, value.msgType, value.hashCode, value.errorCode);
			writer.WritePackable<PlayerInfo>(value.myVPlayerInfo);
			writer.WriteValue<List<int>>(value.nextGameDungeonMasterIDs);
			ListFormatter.SerializePackable(ref writer, value.memberList);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EnterWaitingRoomRes? value)
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
			List<int> value6;
			List<PlayerSessionInfo> value7;
			if (memberCount == 6)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
					value5 = reader.ReadPackable<PlayerInfo>();
					value6 = reader.ReadValue<List<int>>();
					value7 = ListFormatter.DeserializePackable<PlayerSessionInfo>(ref reader);
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.myVPlayerInfo;
					value6 = value.nextGameDungeonMasterIDs;
					value7 = value.memberList;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<MsgErrorCode>(out value4);
					reader.ReadPackable(ref value5);
					reader.ReadValue(ref value6);
					ListFormatter.DeserializePackable(ref reader, ref value7);
				}
			}
			else
			{
				if (memberCount > 6)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EnterWaitingRoomRes), 6, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = MsgErrorCode.Success;
					value5 = null;
					value6 = null;
					value7 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.myVPlayerInfo;
					value6 = value.nextGameDungeonMasterIDs;
					value7 = value.memberList;
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
									reader.ReadValue(ref value6);
									if (memberCount != 5)
									{
										ListFormatter.DeserializePackable(ref reader, ref value7);
										_ = 6;
									}
								}
							}
						}
					}
				}
				_ = value;
			}
			value = new EnterWaitingRoomRes(value3)
			{
				msgType = value2,
				errorCode = value4,
				myVPlayerInfo = value5,
				nextGameDungeonMasterIDs = value6,
				memberList = value7
			};
		}
	}
}
