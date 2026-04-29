using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class AdminCommandReq : IMsg, IMemoryPackable<AdminCommandReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class AdminCommandReqFormatter : MemoryPackFormatter<AdminCommandReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AdminCommandReq value)
		{
			AdminCommandReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref AdminCommandReq value)
		{
			AdminCommandReq.Deserialize(ref reader, ref value);
		}
	}

	public string command { get; set; }

	public string args { get; set; }

	public AdminCommandReq()
		: base(MsgType.C2S_AdminCommandReq)
	{
		base.reliable = true;
	}

	static AdminCommandReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<AdminCommandReq>())
		{
			MemoryPackFormatterProvider.Register(new AdminCommandReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AdminCommandReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<AdminCommandReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AdminCommandReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(4, value.msgType, value.hashCode);
		writer.WriteString(value.command);
		writer.WriteString(value.args);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref AdminCommandReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		string text;
		string text2;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				text = value.command;
				text2 = value.args;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				text = reader.ReadString();
				text2 = reader.ReadString();
				goto IL_00f2;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			text = reader.ReadString();
			text2 = reader.ReadString();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AdminCommandReq), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				text = null;
				text2 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				text = value.command;
				text2 = value.args;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						text = reader.ReadString();
						if (memberCount != 3)
						{
							text2 = reader.ReadString();
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00f2;
			}
		}
		value = new AdminCommandReq
		{
			msgType = value2,
			hashCode = value3,
			command = text,
			args = text2
		};
		return;
		IL_00f2:
		value.msgType = value2;
		value.hashCode = value3;
		value.command = text;
		value.args = text2;
	}
}
