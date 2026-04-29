using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class UseSkillRes : IResMsg, IMemoryPackable<UseSkillRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UseSkillResFormatter : MemoryPackFormatter<UseSkillRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseSkillRes value)
		{
			UseSkillRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UseSkillRes value)
		{
			UseSkillRes.Deserialize(ref reader, ref value);
		}
	}

	public int skillMasterID { get; set; }

	public long skillSyncID { get; set; }

	public PosWithRot startBasePos { get; set; } = new PosWithRot();

	[MemoryPackConstructor]
	public UseSkillRes(int hashCode)
		: base(MsgType.C2S_UseSkillRes, hashCode)
	{
		base.reliable = true;
	}

	public UseSkillRes()
		: this(0)
	{
	}

	static UseSkillRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UseSkillRes>())
		{
			MemoryPackFormatterProvider.Register(new UseSkillResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UseSkillRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UseSkillRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseSkillRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode, int, long>(6, value.msgType, value.hashCode, value.errorCode, value.skillMasterID, value.skillSyncID);
		writer.WritePackable<PosWithRot>(value.startBasePos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref UseSkillRes? value)
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
		long value6;
		PosWithRot value7;
		if (memberCount == 6)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode, int, long>(out value2, out value3, out value4, out value5, out value6);
				value7 = reader.ReadPackable<PosWithRot>();
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.skillMasterID;
				value6 = value.skillSyncID;
				value7 = value.startBasePos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<long>(out value6);
				reader.ReadPackable(ref value7);
			}
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UseSkillRes), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = 0;
				value6 = 0L;
				value7 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.skillMasterID;
				value6 = value.skillSyncID;
				value7 = value.startBasePos;
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
								reader.ReadUnmanaged<long>(out value6);
								if (memberCount != 5)
								{
									reader.ReadPackable(ref value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			_ = value;
		}
		value = new UseSkillRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			skillMasterID = value5,
			skillSyncID = value6,
			startBasePos = value7
		};
	}
}
