using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class RollDungeonSig : IMsg, IMemoryPackable<RollDungeonSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class RollDungeonSigFormatter : MemoryPackFormatter<RollDungeonSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RollDungeonSig value)
		{
			RollDungeonSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref RollDungeonSig value)
		{
			RollDungeonSig.Deserialize(ref reader, ref value);
		}
	}

	public (int, int) newDungeonMasterIDs { get; set; } = (0, 0);

	public RollDungeonSig()
		: base(MsgType.C2S_RollDungeonSig)
	{
		base.reliable = true;
	}

	static RollDungeonSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<RollDungeonSig>())
		{
			MemoryPackFormatterProvider.Register(new RollDungeonSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<RollDungeonSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<RollDungeonSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<(int, int)>())
		{
			MemoryPackFormatterProvider.Register(new ValueTupleFormatter<int, int>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RollDungeonSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, (int, int)>((byte)3, in ILSpyHelper_AsRefReadOnly(value.msgType), in ILSpyHelper_AsRefReadOnly(value.hashCode), in ILSpyHelper_AsRefReadOnly(value.newDungeonMasterIDs));
		}
		static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
		{
			//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
			return ref temp;
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref RollDungeonSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		(int, int) value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.newDungeonMasterIDs;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<(int, int)>(out value4);
				goto IL_00c4;
			}
			reader.ReadUnmanaged<MsgType, int, (int, int)>(out value2, out value3, out value4);
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RollDungeonSig), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = default((int, int));
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.newDungeonMasterIDs;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<(int, int)>(out value4);
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00c4;
			}
		}
		value = new RollDungeonSig
		{
			msgType = value2,
			hashCode = value3,
			newDungeonMasterIDs = value4
		};
		return;
		IL_00c4:
		value.msgType = value2;
		value.hashCode = value3;
		value.newDungeonMasterIDs = value4;
	}
}
