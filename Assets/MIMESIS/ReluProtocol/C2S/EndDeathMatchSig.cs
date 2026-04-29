using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class EndDeathMatchSig : IMsg, IMemoryPackable<EndDeathMatchSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EndDeathMatchSigFormatter : MemoryPackFormatter<EndDeathMatchSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndDeathMatchSig value)
			{
				EndDeathMatchSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EndDeathMatchSig value)
			{
				EndDeathMatchSig.Deserialize(ref reader, ref value);
			}
		}

		public Dictionary<int, DeathMatchPlayerResult> deathMatchPlayerResults { get; set; } = new Dictionary<int, DeathMatchPlayerResult>();

		public EndDeathMatchSig()
			: base(MsgType.C2S_EndDeathMatchSig)
		{
			base.reliable = true;
		}

		static EndDeathMatchSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EndDeathMatchSig>())
			{
				MemoryPackFormatterProvider.Register(new EndDeathMatchSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EndDeathMatchSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EndDeathMatchSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, DeathMatchPlayerResult>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, DeathMatchPlayerResult>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndDeathMatchSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
			writer.WriteValue<Dictionary<int, DeathMatchPlayerResult>>(value.deathMatchPlayerResults);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EndDeathMatchSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			Dictionary<int, DeathMatchPlayerResult> value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.deathMatchPlayerResults;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadValue(ref value4);
					goto IL_00c3;
				}
				reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
				value4 = reader.ReadValue<Dictionary<int, DeathMatchPlayerResult>>();
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EndDeathMatchSig), 3, memberCount);
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
					value4 = value.deathMatchPlayerResults;
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
			value = new EndDeathMatchSig
			{
				msgType = value2,
				hashCode = value3,
				deathMatchPlayerResults = value4
			};
			return;
			IL_00c3:
			value.msgType = value2;
			value.hashCode = value3;
			value.deathMatchPlayerResults = value4;
		}
	}
}
