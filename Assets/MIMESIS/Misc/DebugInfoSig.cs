using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class DebugInfoSig : IActorMsg, IMemoryPackable<DebugInfoSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DebugInfoSigFormatter : MemoryPackFormatter<DebugInfoSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugInfoSig value)
		{
			DebugInfoSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DebugInfoSig value)
		{
			DebugInfoSig.Deserialize(ref reader, ref value);
		}
	}

	public string debugInfo { get; set; }

	public List<HitCheckDrawInfo> hitCheckDrawInfos { get; set; } = new List<HitCheckDrawInfo>();

	public DebugInfoSig()
		: base(MsgType.C2S_DebugInfoSig)
	{
	}

	static DebugInfoSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DebugInfoSig>())
		{
			MemoryPackFormatterProvider.Register(new DebugInfoSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DebugInfoSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DebugInfoSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<HitCheckDrawInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<HitCheckDrawInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugInfoSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(5, value.msgType, value.hashCode, value.actorID);
		writer.WriteString(value.debugInfo);
		ListFormatter.SerializePackable(ref writer, value.hitCheckDrawInfos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DebugInfoSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		List<HitCheckDrawInfo> value5;
		string text;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				text = value.debugInfo;
				value5 = value.hitCheckDrawInfos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				text = reader.ReadString();
				ListFormatter.DeserializePackable(ref reader, ref value5);
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			text = reader.ReadString();
			value5 = ListFormatter.DeserializePackable<HitCheckDrawInfo>(ref reader);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DebugInfoSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				text = null;
				value5 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				text = value.debugInfo;
				value5 = value.hitCheckDrawInfos;
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
								ListFormatter.DeserializePackable(ref reader, ref value5);
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
		value = new DebugInfoSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			debugInfo = text,
			hitCheckDrawInfos = value5
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.debugInfo = text;
		value.hitCheckDrawInfos = value5;
	}
}
