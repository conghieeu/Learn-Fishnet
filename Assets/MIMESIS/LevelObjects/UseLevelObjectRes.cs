using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class UseLevelObjectRes : IResMsg, IMemoryPackable<UseLevelObjectRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UseLevelObjectResFormatter : MemoryPackFormatter<UseLevelObjectRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseLevelObjectRes value)
		{
			UseLevelObjectRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UseLevelObjectRes value)
		{
			UseLevelObjectRes.Deserialize(ref reader, ref value);
		}
	}

	public int fromState { get; set; }

	public int toState { get; set; }

	[MemoryPackConstructor]
	public UseLevelObjectRes(int hashCode)
		: base(MsgType.C2S_UseLevelObjectRes, hashCode)
	{
	}

	public UseLevelObjectRes()
		: this(0)
	{
		base.reliable = true;
	}

	static UseLevelObjectRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UseLevelObjectRes>())
		{
			MemoryPackFormatterProvider.Register(new UseLevelObjectResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UseLevelObjectRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UseLevelObjectRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseLevelObjectRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode, int, int>(5, value.msgType, value.hashCode, value.errorCode, value.fromState, value.toState);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref UseLevelObjectRes? value)
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
		int value6;
		if (memberCount == 5)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode, int, int>(out value2, out value3, out value4, out value5, out value6);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.fromState;
				value6 = value.toState;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<int>(out value6);
			}
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UseLevelObjectRes), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = 0;
				value6 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.fromState;
				value6 = value.toState;
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
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<int>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			_ = value;
		}
		value = new UseLevelObjectRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			fromState = value5,
			toState = value6
		};
	}
}
