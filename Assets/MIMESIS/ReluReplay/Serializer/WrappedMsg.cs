using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;

namespace ReluReplay.Serializer
{
	[MemoryPackable(GenerateType.Object)]
	public class WrappedMsg : IMemoryPackable<WrappedMsg>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class WrappedMsgFormatter : MemoryPackFormatter<WrappedMsg>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref WrappedMsg value)
			{
				WrappedMsg.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref WrappedMsg value)
			{
				WrappedMsg.Deserialize(ref reader, ref value);
			}
		}

		public string TypeName { get; set; }

		public byte[] Data { get; set; }

		public long Time { get; set; }

		public WrappedMsg(string typeName, long time, byte[] data)
		{
			TypeName = typeName;
			Data = data;
			Time = time;
		}

		public static string GetTypeNameFromMsg(IMsg msg)
		{
			return msg.GetType().AssemblyQualifiedName;
		}

		public static byte[] GetDataFromMsg(IMsg msg)
		{
			return MemoryPackSerializer.Serialize(msg.GetType(), msg);
		}

		public IMsg GetMessage()
		{
			return (IMsg)MemoryPackSerializer.Deserialize(Type.GetType(TypeName), Data);
		}

		static WrappedMsg()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<WrappedMsg>())
			{
				MemoryPackFormatterProvider.Register(new WrappedMsgFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<WrappedMsg[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<WrappedMsg>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<byte[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<byte>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref WrappedMsg? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(3);
			writer.WriteString(value.TypeName);
			writer.WriteUnmanagedArray(value.Data);
			writer.WriteUnmanaged<long>(value.Time);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref WrappedMsg? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			byte[] value2;
			long value3;
			string typeName;
			if (memberCount == 3)
			{
				if (value == null)
				{
					typeName = reader.ReadString();
					value2 = reader.ReadUnmanagedArray<byte>();
					reader.ReadUnmanaged<long>(out value3);
				}
				else
				{
					typeName = value.TypeName;
					value2 = value.Data;
					value3 = value.Time;
					typeName = reader.ReadString();
					reader.ReadUnmanagedArray(ref value2);
					reader.ReadUnmanaged<long>(out value3);
				}
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(WrappedMsg), 3, memberCount);
					return;
				}
				if (value == null)
				{
					typeName = null;
					value2 = null;
					value3 = 0L;
				}
				else
				{
					typeName = value.TypeName;
					value2 = value.Data;
					value3 = value.Time;
				}
				if (memberCount != 0)
				{
					typeName = reader.ReadString();
					if (memberCount != 1)
					{
						reader.ReadUnmanagedArray(ref value2);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<long>(out value3);
							_ = 3;
						}
					}
				}
				_ = value;
			}
			value = new WrappedMsg(typeName, value3, value2);
		}
	}
}
