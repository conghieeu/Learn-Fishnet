using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class NetworkGradeSig : IMsg, IMemoryPackable<NetworkGradeSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class NetworkGradeSigFormatter : MemoryPackFormatter<NetworkGradeSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref NetworkGradeSig value)
		{
			NetworkGradeSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref NetworkGradeSig value)
		{
			NetworkGradeSig.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<string, NetworkGrade> grades { get; set; } = new Dictionary<string, NetworkGrade>();

	public NetworkGradeSig()
		: base(MsgType.C2S_NetworkGradeSig)
	{
	}

	static NetworkGradeSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<NetworkGradeSig>())
		{
			MemoryPackFormatterProvider.Register(new NetworkGradeSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<NetworkGradeSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<NetworkGradeSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<string, NetworkGrade>>())
		{
			MemoryPackFormatterProvider.Register(new DictionaryFormatter<string, NetworkGrade>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<NetworkGrade>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<NetworkGrade>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref NetworkGradeSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
		writer.WriteValue<Dictionary<string, NetworkGrade>>(value.grades);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref NetworkGradeSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		Dictionary<string, NetworkGrade> value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.grades;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadValue(ref value4);
				goto IL_00c3;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			value4 = reader.ReadValue<Dictionary<string, NetworkGrade>>();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(NetworkGradeSig), 3, memberCount);
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
				value4 = value.grades;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadValue(ref value4);
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00c3;
			}
		}
		value = new NetworkGradeSig
		{
			msgType = value2,
			hashCode = value3,
			grades = value4
		};
		return;
		IL_00c3:
		value.msgType = value2;
		value.hashCode = value3;
		value.grades = value4;
	}
}
