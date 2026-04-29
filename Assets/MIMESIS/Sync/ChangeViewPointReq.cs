using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ChangeViewPointReq : IMsg, IMemoryPackable<ChangeViewPointReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ChangeViewPointReqFormatter : MemoryPackFormatter<ChangeViewPointReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeViewPointReq value)
		{
			ChangeViewPointReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ChangeViewPointReq value)
		{
			ChangeViewPointReq.Deserialize(ref reader, ref value);
		}
	}

	public float pitch { get; set; }

	public float angle { get; set; }

	public ChangeViewPointReq()
		: base(MsgType.C2S_ChangeViewPointReq)
	{
	}

	static ChangeViewPointReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeViewPointReq>())
		{
			MemoryPackFormatterProvider.Register(new ChangeViewPointReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeViewPointReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ChangeViewPointReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeViewPointReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, float, float>(4, value.msgType, value.hashCode, value.pitch, value.angle);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ChangeViewPointReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		float value4;
		float value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.pitch;
				value5 = value.angle;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<float>(out value4);
				reader.ReadUnmanaged<float>(out value5);
				goto IL_00f4;
			}
			reader.ReadUnmanaged<MsgType, int, float, float>(out value2, out value3, out value4, out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ChangeViewPointReq), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0f;
				value5 = 0f;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.pitch;
				value5 = value.angle;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<float>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<float>(out value5);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00f4;
			}
		}
		value = new ChangeViewPointReq
		{
			msgType = value2,
			hashCode = value3,
			pitch = value4,
			angle = value5
		};
		return;
		IL_00f4:
		value.msgType = value2;
		value.hashCode = value3;
		value.pitch = value4;
		value.angle = value5;
	}
}
