using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class SyncSkillMoveRes : IResMsg, IMemoryPackable<SyncSkillMoveRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SyncSkillMoveResFormatter : MemoryPackFormatter<SyncSkillMoveRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncSkillMoveRes value)
		{
			SyncSkillMoveRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SyncSkillMoveRes value)
		{
			SyncSkillMoveRes.Deserialize(ref reader, ref value);
		}
	}

	public long skillSyncID { get; set; }

	public int skillMasterID { get; set; }

	[MemoryPackConstructor]
	public SyncSkillMoveRes(int hashCode)
		: base(MsgType.C2S_SyncSkillMoveRes, hashCode)
	{
	}

	public SyncSkillMoveRes()
		: this(0)
	{
	}

	static SyncSkillMoveRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SyncSkillMoveRes>())
		{
			MemoryPackFormatterProvider.Register(new SyncSkillMoveResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SyncSkillMoveRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SyncSkillMoveRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncSkillMoveRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode, long, int>(5, value.msgType, value.hashCode, value.errorCode, value.skillSyncID, value.skillMasterID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SyncSkillMoveRes? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		MsgErrorCode value4;
		long value5;
		int value6;
		if (memberCount == 5)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode, long, int>(out value2, out value3, out value4, out value5, out value6);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.skillSyncID;
				value6 = value.skillMasterID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadUnmanaged<long>(out value5);
				reader.ReadUnmanaged<int>(out value6);
			}
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SyncSkillMoveRes), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = 0L;
				value6 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.skillSyncID;
				value6 = value.skillMasterID;
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
							reader.ReadUnmanaged<long>(out value5);
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
		value = new SyncSkillMoveRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			skillSyncID = value5,
			skillMasterID = value6
		};
	}
}
