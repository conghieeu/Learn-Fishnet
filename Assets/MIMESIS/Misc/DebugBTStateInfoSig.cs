using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class DebugBTStateInfoSig : IActorMsg, IMemoryPackable<DebugBTStateInfoSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DebugBTStateInfoSigFormatter : MemoryPackFormatter<DebugBTStateInfoSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugBTStateInfoSig value)
		{
			DebugBTStateInfoSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DebugBTStateInfoSig value)
		{
			DebugBTStateInfoSig.Deserialize(ref reader, ref value);
		}
	}

	public string aiDataName { get; set; }

	public string templateName { get; set; }

	public DebugBTStateInfoSig()
		: base(MsgType.C2S_DebugBTStateInfoSig)
	{
	}

	static DebugBTStateInfoSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DebugBTStateInfoSig>())
		{
			MemoryPackFormatterProvider.Register(new DebugBTStateInfoSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DebugBTStateInfoSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DebugBTStateInfoSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugBTStateInfoSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(5, value.msgType, value.hashCode, value.actorID);
		writer.WriteString(value.aiDataName);
		writer.WriteString(value.templateName);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DebugBTStateInfoSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		string text;
		string text2;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				text = value.aiDataName;
				text2 = value.templateName;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				text = reader.ReadString();
				text2 = reader.ReadString();
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			text = reader.ReadString();
			text2 = reader.ReadString();
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DebugBTStateInfoSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				text = null;
				text2 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				text = value.aiDataName;
				text2 = value.templateName;
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
						if (memberCount != 3)
						{
							text = reader.ReadString();
							if (memberCount != 4)
							{
								text2 = reader.ReadString();
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0123;
			}
		}
		value = new DebugBTStateInfoSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			aiDataName = text,
			templateName = text2
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.aiDataName = text;
		value.templateName = text2;
	}
}
