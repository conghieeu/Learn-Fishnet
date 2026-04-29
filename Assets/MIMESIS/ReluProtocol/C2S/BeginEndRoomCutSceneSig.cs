using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class BeginEndRoomCutSceneSig : IMsg, IMemoryPackable<BeginEndRoomCutSceneSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class BeginEndRoomCutSceneSigFormatter : MemoryPackFormatter<BeginEndRoomCutSceneSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BeginEndRoomCutSceneSig value)
			{
				BeginEndRoomCutSceneSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref BeginEndRoomCutSceneSig value)
			{
				BeginEndRoomCutSceneSig.Deserialize(ref reader, ref value);
			}
		}

		public List<string> exitCutsceneNames { get; set; } = new List<string>();

		public BeginEndRoomCutSceneSig()
			: base(MsgType.C2S_BeginEndRoomCutSceneSig)
		{
			base.reliable = true;
		}

		static BeginEndRoomCutSceneSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<BeginEndRoomCutSceneSig>())
			{
				MemoryPackFormatterProvider.Register(new BeginEndRoomCutSceneSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<BeginEndRoomCutSceneSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<BeginEndRoomCutSceneSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<string>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<string>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BeginEndRoomCutSceneSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
			writer.WriteValue<List<string>>(value.exitCutsceneNames);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref BeginEndRoomCutSceneSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			List<string> value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.exitCutsceneNames;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadValue(ref value4);
					goto IL_00c3;
				}
				reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
				value4 = reader.ReadValue<List<string>>();
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BeginEndRoomCutSceneSig), 3, memberCount);
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
					value4 = value.exitCutsceneNames;
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
			value = new BeginEndRoomCutSceneSig
			{
				msgType = value2,
				hashCode = value3,
				exitCutsceneNames = value4
			};
			return;
			IL_00c3:
			value.msgType = value2;
			value.hashCode = value3;
			value.exitCutsceneNames = value4;
		}
	}
}
