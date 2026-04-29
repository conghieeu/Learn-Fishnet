using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class LeaveServerSig : IMsg, IMemoryPackable<LeaveServerSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class LeaveServerSigFormatter : MemoryPackFormatter<LeaveServerSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LeaveServerSig value)
			{
				LeaveServerSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref LeaveServerSig value)
			{
				LeaveServerSig.Deserialize(ref reader, ref value);
			}
		}

		public ulong steamID { get; set; }

		public string nickName { get; set; } = string.Empty;

		public LeaveServerSig()
			: base(MsgType.C2S_LeaveServerSig)
		{
			base.reliable = true;
		}

		static LeaveServerSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<LeaveServerSig>())
			{
				MemoryPackFormatterProvider.Register(new LeaveServerSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<LeaveServerSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<LeaveServerSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LeaveServerSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, ulong>(4, value.msgType, value.hashCode, value.steamID);
			writer.WriteString(value.nickName);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref LeaveServerSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			ulong value4;
			string text;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.steamID;
					text = value.nickName;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<ulong>(out value4);
					text = reader.ReadString();
					goto IL_00f0;
				}
				reader.ReadUnmanaged<MsgType, int, ulong>(out value2, out value3, out value4);
				text = reader.ReadString();
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(LeaveServerSig), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = 0uL;
					text = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.steamID;
					text = value.nickName;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<ulong>(out value4);
							if (memberCount != 3)
							{
								text = reader.ReadString();
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00f0;
				}
			}
			value = new LeaveServerSig
			{
				msgType = value2,
				hashCode = value3,
				steamID = value4,
				nickName = text
			};
			return;
			IL_00f0:
			value.msgType = value2;
			value.hashCode = value3;
			value.steamID = value4;
			value.nickName = text;
		}
	}
}
