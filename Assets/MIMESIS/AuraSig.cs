using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class AuraSig : IActorMsg, IMemoryPackable<AuraSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class AuraSigFormatter : MemoryPackFormatter<AuraSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AuraSig value)
		{
			AuraSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref AuraSig value)
		{
			AuraSig.Deserialize(ref reader, ref value);
		}
	}

	public List<int> addedAuraMasterIDs { get; set; } = new List<int>();

	public List<int> removedAuraMasterIDs { get; set; } = new List<int>();

	public AuraSig()
		: base(MsgType.C2S_AuraSig)
	{
	}

	static AuraSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<AuraSig>())
		{
			MemoryPackFormatterProvider.Register(new AuraSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AuraSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<AuraSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<int>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AuraSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(5, value.msgType, value.hashCode, value.actorID);
		writer.WriteValue<List<int>>(value.addedAuraMasterIDs);
		writer.WriteValue<List<int>>(value.removedAuraMasterIDs);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref AuraSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		List<int> value5;
		List<int> value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.addedAuraMasterIDs;
				value6 = value.removedAuraMasterIDs;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadValue(ref value5);
				reader.ReadValue(ref value6);
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadValue<List<int>>();
			value6 = reader.ReadValue<List<int>>();
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AuraSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
				value6 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.addedAuraMasterIDs;
				value6 = value.removedAuraMasterIDs;
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
							reader.ReadValue(ref value5);
							if (memberCount != 4)
							{
								reader.ReadValue(ref value6);
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
		value = new AuraSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			addedAuraMasterIDs = value5,
			removedAuraMasterIDs = value6
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.addedAuraMasterIDs = value5;
		value.removedAuraMasterIDs = value6;
	}
}
