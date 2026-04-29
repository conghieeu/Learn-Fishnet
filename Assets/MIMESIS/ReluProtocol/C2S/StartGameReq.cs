using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class StartGameReq : IMsg, IMemoryPackable<StartGameReq>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class StartGameReqFormatter : MemoryPackFormatter<StartGameReq>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartGameReq value)
			{
				StartGameReq.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref StartGameReq value)
			{
				StartGameReq.Deserialize(ref reader, ref value);
			}
		}

		public int selectedDungeonMasterID { get; set; }

		public StartGameReq()
			: base(MsgType.C2S_StartGameReq)
		{
			base.reliable = true;
		}

		static StartGameReq()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<StartGameReq>())
			{
				MemoryPackFormatterProvider.Register(new StartGameReqFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<StartGameReq[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<StartGameReq>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartGameReq? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(3, value.msgType, value.hashCode, value.selectedDungeonMasterID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref StartGameReq? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			int value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.selectedDungeonMasterID;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					goto IL_00be;
				}
				reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(StartGameReq), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = 0;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.selectedDungeonMasterID;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00be;
				}
			}
			value = new StartGameReq
			{
				msgType = value2,
				hashCode = value3,
				selectedDungeonMasterID = value4
			};
			return;
			IL_00be:
			value.msgType = value2;
			value.hashCode = value3;
			value.selectedDungeonMasterID = value4;
		}
	}
}
