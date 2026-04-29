using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class JoinServerRes : IResMsg, IMemoryPackable<JoinServerRes>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class JoinServerResFormatter : MemoryPackFormatter<JoinServerRes>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JoinServerRes value)
			{
				JoinServerRes.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref JoinServerRes value)
			{
				JoinServerRes.Deserialize(ref reader, ref value);
			}
		}

		public long playerUID { get; set; }

		public bool isHost { get; set; }

		public MaintenenceRoomInfo roomInfo { get; set; }

		[MemoryPackConstructor]
		public JoinServerRes(int hashCode)
			: base(MsgType.C2S_JoinServerRes, hashCode)
		{
			base.reliable = true;
		}

		public JoinServerRes()
			: this(0)
		{
		}

		static JoinServerRes()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<JoinServerRes>())
			{
				MemoryPackFormatterProvider.Register(new JoinServerResFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<JoinServerRes[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<JoinServerRes>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JoinServerRes? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode, long, bool>(6, value.msgType, value.hashCode, value.errorCode, value.playerUID, value.isHost);
			writer.WritePackable<MaintenenceRoomInfo>(value.roomInfo);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref JoinServerRes? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			MsgErrorCode value4;
			long value5;
			bool value6;
			MaintenenceRoomInfo value7;
			if (memberCount == 6)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<MsgType, int, MsgErrorCode, long, bool>(out value2, out value3, out value4, out value5, out value6);
					value7 = reader.ReadPackable<MaintenenceRoomInfo>();
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.playerUID;
					value6 = value.isHost;
					value7 = value.roomInfo;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<MsgErrorCode>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<bool>(out value6);
					reader.ReadPackable(ref value7);
				}
			}
			else
			{
				if (memberCount > 6)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(JoinServerRes), 6, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = MsgErrorCode.Success;
					value5 = 0L;
					value6 = false;
					value7 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.playerUID;
					value6 = value.isHost;
					value7 = value.roomInfo;
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
								reader.ReadUnmanaged<long>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<bool>(out value6);
									if (memberCount != 5)
									{
										reader.ReadPackable(ref value7);
										_ = 6;
									}
								}
							}
						}
					}
				}
				_ = value;
			}
			value = new JoinServerRes(value3)
			{
				msgType = value2,
				errorCode = value4,
				playerUID = value5,
				isHost = value6,
				roomInfo = value7
			};
		}
	}
}
