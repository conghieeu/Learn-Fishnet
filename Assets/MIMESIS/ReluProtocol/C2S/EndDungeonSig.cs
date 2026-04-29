using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class EndDungeonSig : IMsg, IMemoryPackable<EndDungeonSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EndDungeonSigFormatter : MemoryPackFormatter<EndDungeonSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndDungeonSig value)
			{
				EndDungeonSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EndDungeonSig value)
			{
				EndDungeonSig.Deserialize(ref reader, ref value);
			}
		}

		public GameRoomResult result { get; set; } = new GameRoomResult();

		public EndDungeonSig()
			: base(MsgType.C2S_EndDungeonSig)
		{
			base.reliable = true;
		}

		static EndDungeonSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EndDungeonSig>())
			{
				MemoryPackFormatterProvider.Register(new EndDungeonSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EndDungeonSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EndDungeonSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndDungeonSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
			writer.WritePackable<GameRoomResult>(value.result);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EndDungeonSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			GameRoomResult value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.result;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadPackable(ref value4);
					goto IL_00c3;
				}
				reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
				value4 = reader.ReadPackable<GameRoomResult>();
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EndDungeonSig), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.result;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadPackable(ref value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00c3;
				}
			}
			value = new EndDungeonSig
			{
				msgType = value2,
				hashCode = value3,
				result = value4
			};
			return;
			IL_00c3:
			value.msgType = value2;
			value.hashCode = value3;
			value.result = value4;
		}
	}
}
