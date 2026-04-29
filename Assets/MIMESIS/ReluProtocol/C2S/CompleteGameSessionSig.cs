using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class CompleteGameSessionSig : IMsg, IMemoryPackable<CompleteGameSessionSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CompleteGameSessionSigFormatter : MemoryPackFormatter<CompleteGameSessionSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CompleteGameSessionSig value)
			{
				CompleteGameSessionSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CompleteGameSessionSig value)
			{
				CompleteGameSessionSig.Deserialize(ref reader, ref value);
			}
		}

		public bool success { get; set; }

		public int nextTargetCurrency { get; set; }

		public CompleteGameSessionSig()
			: base(MsgType.C2S_CompleteGameSessionSig)
		{
			base.reliable = true;
		}

		static CompleteGameSessionSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CompleteGameSessionSig>())
			{
				MemoryPackFormatterProvider.Register(new CompleteGameSessionSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CompleteGameSessionSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CompleteGameSessionSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CompleteGameSessionSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, bool, int>(4, value.msgType, value.hashCode, value.success, value.nextTargetCurrency);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CompleteGameSessionSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			bool value4;
			int value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.success;
					value5 = value.nextTargetCurrency;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<bool>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					goto IL_00e9;
				}
				reader.ReadUnmanaged<MsgType, int, bool, int>(out value2, out value3, out value4, out value5);
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CompleteGameSessionSig), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = false;
					value5 = 0;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.success;
					value5 = value.nextTargetCurrency;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<bool>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<int>(out value5);
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00e9;
				}
			}
			value = new CompleteGameSessionSig
			{
				msgType = value2,
				hashCode = value3,
				success = value4,
				nextTargetCurrency = value5
			};
			return;
			IL_00e9:
			value.msgType = value2;
			value.hashCode = value3;
			value.success = value4;
			value.nextTargetCurrency = value5;
		}
	}
}
