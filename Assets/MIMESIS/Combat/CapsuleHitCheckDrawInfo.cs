using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class CapsuleHitCheckDrawInfo : HitCheckDrawInfo, IMemoryPackable<CapsuleHitCheckDrawInfo>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class CapsuleHitCheckDrawInfoFormatter : MemoryPackFormatter<CapsuleHitCheckDrawInfo>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CapsuleHitCheckDrawInfo value)
		{
			CapsuleHitCheckDrawInfo.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref CapsuleHitCheckDrawInfo value)
		{
			CapsuleHitCheckDrawInfo.Deserialize(ref reader, ref value);
		}
	}

	public double Radius { get; set; }

	public double Length { get; set; }

	static CapsuleHitCheckDrawInfo()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<CapsuleHitCheckDrawInfo>())
		{
			MemoryPackFormatterProvider.Register(new CapsuleHitCheckDrawInfoFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<CapsuleHitCheckDrawInfo[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<CapsuleHitCheckDrawInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckShapeType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<HitCheckShapeType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CapsuleHitCheckDrawInfo? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<int, Vector3, Rotator, HitCheckShapeType, double, double>(6, value.actorID, value.center, value.rotation, value.shapeType, value.Radius, value.Length);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref CapsuleHitCheckDrawInfo? value)
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
		double value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.Radius;
				value7 = value.Length;
				reader.ReadUnmanaged<int>(out value2);
				reader.ReadUnmanaged<Vector3>(out value3);
				reader.ReadUnmanaged<Rotator>(out value4);
				reader.ReadUnmanaged<HitCheckShapeType>(out value5);
				reader.ReadUnmanaged<double>(out value6);
				reader.ReadUnmanaged<double>(out value7);
				goto IL_0161;
			}
			reader.ReadUnmanaged<int, Vector3, Rotator, HitCheckShapeType, double, double>(out value2, out value3, out value4, out value5, out value6, out value7);
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CapsuleHitCheckDrawInfo), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = 0;
				value3 = default(Vector3);
				value4 = default(Rotator);
				value5 = HitCheckShapeType.Invalid;
				value6 = 0.0;
				value7 = 0.0;
			}
			else
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.Radius;
				value7 = value.Length;
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
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<double>(out value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0161;
			}
		}
		value = new CapsuleHitCheckDrawInfo
		{
			actorID = value2,
			center = value3,
			rotation = value4,
			shapeType = value5,
			Radius = value6,
			Length = value7
		};
		return;
		IL_0161:
		value.actorID = value2;
		value.center = value3;
		value.rotation = value4;
		value.shapeType = value5;
		value.Radius = value6;
		value.Length = value7;
	}
}
