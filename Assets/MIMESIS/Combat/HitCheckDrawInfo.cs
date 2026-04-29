using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class HitCheckDrawInfo : IMemoryPackable<HitCheckDrawInfo>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class HitCheckDrawInfoFormatter : MemoryPackFormatter<HitCheckDrawInfo>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HitCheckDrawInfo value)
		{
			HitCheckDrawInfo.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref HitCheckDrawInfo value)
		{
			HitCheckDrawInfo.Deserialize(ref reader, ref value);
		}
	}

	public int actorID { get; set; }

	public Vector3 center { get; set; }

	public Rotator rotation { get; set; }

	public HitCheckShapeType shapeType { get; set; }

	static HitCheckDrawInfo()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckDrawInfo>())
		{
			MemoryPackFormatterProvider.Register(new HitCheckDrawInfoFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckDrawInfo[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<HitCheckDrawInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckShapeType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<HitCheckShapeType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HitCheckDrawInfo? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<int, Vector3, Rotator, HitCheckShapeType>(4, value.actorID, value.center, value.rotation, value.shapeType);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref HitCheckDrawInfo? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		int value2;
		Vector3 value3;
		Rotator value4;
		HitCheckShapeType value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				reader.ReadUnmanaged<int>(out value2);
				reader.ReadUnmanaged<Vector3>(out value3);
				reader.ReadUnmanaged<Rotator>(out value4);
				reader.ReadUnmanaged<HitCheckShapeType>(out value5);
				goto IL_00f8;
			}
			reader.ReadUnmanaged<int, Vector3, Rotator, HitCheckShapeType>(out value2, out value3, out value4, out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(HitCheckDrawInfo), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = 0;
				value3 = default(Vector3);
				value4 = default(Rotator);
				value5 = HitCheckShapeType.Invalid;
			}
			else
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<int>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<Vector3>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<Rotator>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<HitCheckShapeType>(out value5);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00f8;
			}
		}
		value = new HitCheckDrawInfo
		{
			actorID = value2,
			center = value3,
			rotation = value4,
			shapeType = value5
		};
		return;
		IL_00f8:
		value.actorID = value2;
		value.center = value3;
		value.rotation = value4;
		value.shapeType = value5;
	}
}
