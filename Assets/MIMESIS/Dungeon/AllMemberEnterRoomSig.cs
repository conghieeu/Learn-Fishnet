using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class AllMemberEnterRoomSig : IMsg, IMemoryPackable<AllMemberEnterRoomSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class AllMemberEnterRoomSigFormatter : MemoryPackFormatter<AllMemberEnterRoomSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AllMemberEnterRoomSig value)
		{
			AllMemberEnterRoomSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref AllMemberEnterRoomSig value)
		{
			AllMemberEnterRoomSig.Deserialize(ref reader, ref value);
		}
	}

	public List<string> enterCutsceneNames { get; set; } = new List<string>();

	public AllMemberEnterRoomSig()
		: base(MsgType.C2S_AllMemberEnterRoomSig)
	{
		base.reliable = true;
	}

	static AllMemberEnterRoomSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<AllMemberEnterRoomSig>())
		{
			MemoryPackFormatterProvider.Register(new AllMemberEnterRoomSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AllMemberEnterRoomSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<AllMemberEnterRoomSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<string>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<string>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AllMemberEnterRoomSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
		writer.WriteValue<List<string>>(value.enterCutsceneNames);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref AllMemberEnterRoomSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		List<string> value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.enterCutsceneNames;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadValue(ref value4);
				goto IL_00c3;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			value4 = reader.ReadValue<List<string>>();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AllMemberEnterRoomSig), 3, memberCount);
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
				value4 = value.enterCutsceneNames;
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
		value = new AllMemberEnterRoomSig
		{
			msgType = value2,
			hashCode = value3,
			enterCutsceneNames = value4
		};
		return;
		IL_00c3:
		value.msgType = value2;
		value.hashCode = value3;
		value.enterCutsceneNames = value4;
	}
}
