using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class IResMsg : IMsg, IMemoryPackable<IResMsg>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class IResMsgFormatter : MemoryPackFormatter<IResMsg>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref IResMsg value)
			{
				IResMsg.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref IResMsg value)
			{
				IResMsg.Deserialize(ref reader, ref value);
			}
		}

		public MsgErrorCode errorCode { get; set; }

		[MemoryPackConstructor]
		public IResMsg(MsgType msgType, int hashCode)
			: base(msgType, hashCode)
		{
		}

		public IResMsg(MsgType msgType)
			: this(msgType, 0)
		{
		}

		static IResMsg()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<IResMsg>())
			{
				MemoryPackFormatterProvider.Register(new IResMsgFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<IResMsg[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<IResMsg>());
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
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref IResMsg? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(3, value.msgType, value.hashCode, value.errorCode);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref IResMsg? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			MsgErrorCode value4;
			if (memberCount == 3)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<MsgErrorCode>(out value4);
				}
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(IResMsg), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = MsgErrorCode.Success;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
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
							_ = 3;
						}
					}
				}
				_ = value;
			}
			value = new IResMsg(value2, value3)
			{
				errorCode = value4
			};
		}
	}
}
