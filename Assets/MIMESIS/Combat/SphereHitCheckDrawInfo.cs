using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class SphereHitCheckDrawInfo : HitCheckDrawInfo, IMemoryPackable<SphereHitCheckDrawInfo>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SphereHitCheckDrawInfoFormatter : MemoryPackFormatter<SphereHitCheckDrawInfo>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SphereHitCheckDrawInfo value)
		{
			SphereHitCheckDrawInfo.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SphereHitCheckDrawInfo value)
		{
			SphereHitCheckDrawInfo.Deserialize(ref reader, ref value);
		}
	}

	public double Radius { get; set; }

	static SphereHitCheckDrawInfo()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SphereHitCheckDrawInfo>())
		{
			MemoryPackFormatterProvider.Register(new SphereHitCheckDrawInfoFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SphereHitCheckDrawInfo[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SphereHitCheckDrawInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckShapeType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<HitCheckShapeType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SphereHitCheckDrawInfo? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<int, Vector3, Rotator, HitCheckShapeType, double>(5, value.actorID, value.center, value.rotation, value.shapeType, value.Radius);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SphereHitCheckDrawInfo? value)
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
		double value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.Radius;
				reader.ReadUnmanaged<int>(out value2);
				reader.ReadUnmanaged<Vector3>(out value3);
				reader.ReadUnmanaged<Rotator>(out value4);
				reader.ReadUnmanaged<HitCheckShapeType>(out value5);
				reader.ReadUnmanaged<double>(out value6);
				goto IL_012b;
			}
			reader.ReadUnmanaged<int, Vector3, Rotator, HitCheckShapeType, double>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SphereHitCheckDrawInfo), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = 0;
				value3 = default(Vector3);
				value4 = default(Rotator);
				value5 = HitCheckShapeType.Invalid;
				value6 = 0.0;
			}
			else
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.Radius;
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
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<double>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_012b;
			}
		}
		value = new SphereHitCheckDrawInfo
		{
			actorID = value2,
			center = value3,
			rotation = value4,
			shapeType = value5,
			Radius = value6
		};
		return;
		IL_012b:
		value.actorID = value2;
		value.center = value3;
		value.rotation = value4;
		value.shapeType = value5;
		value.Radius = value6;
	}
}
