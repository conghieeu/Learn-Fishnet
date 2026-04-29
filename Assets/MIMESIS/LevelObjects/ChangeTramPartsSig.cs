using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ChangeTramPartsSig : IMsg, IMemoryPackable<ChangeTramPartsSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ChangeTramPartsSigFormatter : MemoryPackFormatter<ChangeTramPartsSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeTramPartsSig value)
		{
			ChangeTramPartsSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ChangeTramPartsSig value)
		{
			ChangeTramPartsSig.Deserialize(ref reader, ref value);
		}
	}

	public int sessionCount { get; set; }

	public List<int> upgradeList { get; set; } = new List<int>();

	public ChangeTramPartsSig()
		: base(MsgType.C2S_ChangeTramPartsSig)
	{
		base.reliable = true;
	}

	static ChangeTramPartsSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeTramPartsSig>())
		{
			MemoryPackFormatterProvider.Register(new ChangeTramPartsSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeTramPartsSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ChangeTramPartsSig>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeTramPartsSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.sessionCount);
		writer.WriteValue<List<int>>(value.upgradeList);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ChangeTramPartsSig? value)
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
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.sessionCount;
				value5 = value.upgradeList;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadValue(ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadValue<List<int>>();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ChangeTramPartsSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.sessionCount;
				value5 = value.upgradeList;
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
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00ef;
			}
		}
		value = new ChangeTramPartsSig
		{
			msgType = value2,
			hashCode = value3,
			sessionCount = value4,
			upgradeList = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.sessionCount = value4;
		value.upgradeList = value5;
	}
}
