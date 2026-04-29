using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class TargetInfo : IMemoryPackable<TargetInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class TargetInfoFormatter : MemoryPackFormatter<TargetInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TargetInfo value)
			{
				TargetInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref TargetInfo value)
			{
				TargetInfo.Deserialize(ref reader, ref value);
			}
		}

		public int targetObjectID { get; set; }

		public Vector3 targetPosition { get; set; }

		public Vector3 hitPosition { get; set; }

		static TargetInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<TargetInfo>())
			{
				MemoryPackFormatterProvider.Register(new TargetInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<TargetInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<TargetInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TargetInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<int, Vector3, Vector3>(3, value.targetObjectID, value.targetPosition, value.hitPosition);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref TargetInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			Vector3 value3;
			Vector3 value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.targetObjectID;
					value3 = value.targetPosition;
					value4 = value.hitPosition;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<Vector3>(out value3);
					reader.ReadUnmanaged<Vector3>(out value4);
					goto IL_00ca;
				}
				reader.ReadUnmanaged<int, Vector3, Vector3>(out value2, out value3, out value4);
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(TargetInfo), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = default(Vector3);
					value4 = default(Vector3);
				}
				else
				{
					value2 = value.targetObjectID;
					value3 = value.targetPosition;
					value4 = value.hitPosition;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<Vector3>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<Vector3>(out value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00ca;
				}
			}
			value = new TargetInfo
			{
				targetObjectID = value2,
				targetPosition = value3,
				hitPosition = value4
			};
			return;
			IL_00ca:
			value.targetObjectID = value2;
			value.targetPosition = value3;
			value.hitPosition = value4;
		}
	}
}
