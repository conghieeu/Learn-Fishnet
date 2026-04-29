using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class MoveStartRes : IResMsg, IMemoryPackable<MoveStartRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class MoveStartResFormatter : MemoryPackFormatter<MoveStartRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStartRes value)
		{
			MoveStartRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref MoveStartRes value)
		{
			MoveStartRes.Deserialize(ref reader, ref value);
		}
	}

	public int transID { get; set; }

	[MemoryPackConstructor]
	public MoveStartRes(int hashCode)
		: base(MsgType.C2S_MoveStartRes, hashCode)
	{
	}

	public MoveStartRes()
		: this(0)
	{
	}

	static MoveStartRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStartRes>())
		{
			MemoryPackFormatterProvider.Register(new MoveStartResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStartRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<MoveStartRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStartRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode, int>(4, value.msgType, value.hashCode, value.errorCode, value.transID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref MoveStartRes? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		MsgErrorCode value4;
		int value5;
		if (memberCount == 4)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode, int>(out value2, out value3, out value4, out value5);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.transID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadUnmanaged<int>(out value5);
			}
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(MoveStartRes), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.transID;
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
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<int>(out value5);
							_ = 4;
						}
					}
				}
			}
			_ = value;
		}
		value = new MoveStartRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			transID = value5
		};
	}
}
