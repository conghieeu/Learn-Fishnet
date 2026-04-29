using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class TorusHitCheckDrawInfo : HitCheckDrawInfo, IMemoryPackable<TorusHitCheckDrawInfo>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class TorusHitCheckDrawInfoFormatter : MemoryPackFormatter<TorusHitCheckDrawInfo>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TorusHitCheckDrawInfo value)
		{
			TorusHitCheckDrawInfo.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref TorusHitCheckDrawInfo value)
		{
			TorusHitCheckDrawInfo.Deserialize(ref reader, ref value);
		}
	}

	public float InnerRad { get; set; }

	public float OuterRad { get; set; }

	public float Height { get; set; }

	static TorusHitCheckDrawInfo()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<TorusHitCheckDrawInfo>())
		{
			MemoryPackFormatterProvider.Register(new TorusHitCheckDrawInfoFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TorusHitCheckDrawInfo[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<TorusHitCheckDrawInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckShapeType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<HitCheckShapeType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TorusHitCheckDrawInfo? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<int, Vector3, Rotator, HitCheckShapeType, float, float, float>(7, value.actorID, value.center, value.rotation, value.shapeType, value.InnerRad, value.OuterRad, value.Height);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref TorusHitCheckDrawInfo? value)
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
		float value6;
		float value7;
		float value8;
		if (memberCount == 7)
		{
			if (value != null)
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.InnerRad;
				value7 = value.OuterRad;
				value8 = value.Height;
				reader.ReadUnmanaged<int>(out value2);
				reader.ReadUnmanaged<Vector3>(out value3);
				reader.ReadUnmanaged<Rotator>(out value4);
				reader.ReadUnmanaged<HitCheckShapeType>(out value5);
				reader.ReadUnmanaged<float>(out value6);
				reader.ReadUnmanaged<float>(out value7);
				reader.ReadUnmanaged<float>(out value8);
				goto IL_0188;
			}
			reader.ReadUnmanaged<int, Vector3, Rotator, HitCheckShapeType, float, float, float>(out value2, out value3, out value4, out value5, out value6, out value7, out value8);
		}
		else
		{
			if (memberCount > 7)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(TorusHitCheckDrawInfo), 7, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = 0;
				value3 = default(Vector3);
				value4 = default(Rotator);
				value5 = HitCheckShapeType.Invalid;
				value6 = 0f;
				value7 = 0f;
				value8 = 0f;
			}
			else
			{
				value2 = value.actorID;
				value3 = value.center;
				value4 = value.rotation;
				value5 = value.shapeType;
				value6 = value.InnerRad;
				value7 = value.OuterRad;
				value8 = value.Height;
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
								reader.ReadUnmanaged<float>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<float>(out value7);
									if (memberCount != 6)
									{
										reader.ReadUnmanaged<float>(out value8);
										_ = 7;
									}
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0188;
			}
		}
		value = new TorusHitCheckDrawInfo
		{
			actorID = value2,
			center = value3,
			rotation = value4,
			shapeType = value5,
			InnerRad = value6,
			OuterRad = value7,
			Height = value8
		};
		return;
		IL_0188:
		value.actorID = value2;
		value.center = value3;
		value.rotation = value4;
		value.shapeType = value5;
		value.InnerRad = value6;
		value.OuterRad = value7;
		value.Height = value8;
	}
}
