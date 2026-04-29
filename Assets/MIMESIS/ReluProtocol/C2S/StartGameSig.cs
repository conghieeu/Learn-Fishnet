using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class StartGameSig : IMsg, IMemoryPackable<StartGameSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class StartGameSigFormatter : MemoryPackFormatter<StartGameSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartGameSig value)
			{
				StartGameSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref StartGameSig value)
			{
				StartGameSig.Deserialize(ref reader, ref value);
			}
		}

		public int selectedDungeonMasterID { get; set; }

		public int randDungeonSeed { get; set; }

		public StartGameSig()
			: base(MsgType.C2S_StartGameSig)
		{
			base.reliable = true;
		}

		static StartGameSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<StartGameSig>())
			{
				MemoryPackFormatterProvider.Register(new StartGameSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<StartGameSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<StartGameSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartGameSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int>(4, value.msgType, value.hashCode, value.selectedDungeonMasterID, value.randDungeonSeed);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref StartGameSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			int value4;
			int value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.selectedDungeonMasterID;
					value5 = value.randDungeonSeed;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					goto IL_00e9;
				}
				reader.ReadUnmanaged<MsgType, int, int, int>(out value2, out value3, out value4, out value5);
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(StartGameSig), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = 0;
					value5 = 0;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.selectedDungeonMasterID;
					value5 = value.randDungeonSeed;
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
			value = new StartGameSig
			{
				msgType = value2,
				hashCode = value3,
				selectedDungeonMasterID = value4,
				randDungeonSeed = value5
			};
			return;
			IL_00e9:
			value.msgType = value2;
			value.hashCode = value3;
			value.selectedDungeonMasterID = value4;
			value.randDungeonSeed = value5;
		}
	}
}
