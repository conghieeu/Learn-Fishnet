using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class CubeHitCheckDrawInfo : HitCheckDrawInfo, IMemoryPackable<CubeHitCheckDrawInfo>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class CubeHitCheckDrawInfoFormatter : MemoryPackFormatter<CubeHitCheckDrawInfo>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CubeHitCheckDrawInfo value)
		{
			CubeHitCheckDrawInfo.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref CubeHitCheckDrawInfo value)
		{
			CubeHitCheckDrawInfo.Deserialize(ref reader, ref value);
		}
	}

	public Vector3 Extent { get; set; }

	static CubeHitCheckDrawInfo()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<CubeHitCheckDrawInfo>())
		{
			MemoryPackFormatterProvider.Register(new CubeHitCheckDrawInfoFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<CubeHitCheckDrawInfo[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<CubeHitCheckDrawInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckShapeType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<HitCheckShapeType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CubeHitCheckDrawInfo? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<int, Vector3, Rotator, HitCheckShapeType, Vector3>(5, value.actorID, value.center, value.rotation, value.shapeType, value.Extent);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref CubeHitCheckDrawInfo? value)
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
		Vector3 value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.Extent;
				reader.ReadUnmanaged<int>(out value2);
				reader.ReadUnmanaged<Vector3>(out value3);
				reader.ReadUnmanaged<Rotator>(out value4);
				reader.ReadUnmanaged<HitCheckShapeType>(out value5);
				reader.ReadUnmanaged<Vector3>(out value6);
				goto IL_0128;
			}
			reader.ReadUnmanaged<int, Vector3, Rotator, HitCheckShapeType, Vector3>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CubeHitCheckDrawInfo), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = 0;
				value3 = default(Vector3);
				value4 = default(Rotator);
				value5 = HitCheckShapeType.Invalid;
				value6 = default(Vector3);
			}
			else
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.Extent;
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
								reader.ReadUnmanaged<Vector3>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0128;
			}
		}
		value = new CubeHitCheckDrawInfo
		{
			actorID = value2,
			center = value3,
			rotation = value4,
			shapeType = value5,
			Extent = value6
		};
		return;
		IL_0128:
		value.actorID = value2;
		value.center = value3;
		value.rotation = value4;
		value.shapeType = value5;
		value.Extent = value6;
	}
}
