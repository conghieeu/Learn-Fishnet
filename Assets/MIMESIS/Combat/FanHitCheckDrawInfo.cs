using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class FanHitCheckDrawInfo : HitCheckDrawInfo, IMemoryPackable<FanHitCheckDrawInfo>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class FanHitCheckDrawInfoFormatter : MemoryPackFormatter<FanHitCheckDrawInfo>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FanHitCheckDrawInfo value)
		{
			FanHitCheckDrawInfo.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref FanHitCheckDrawInfo value)
		{
			FanHitCheckDrawInfo.Deserialize(ref reader, ref value);
		}
	}

	public float InnerRad { get; set; }

	public float OuterRad { get; set; }

	public float Height { get; set; }

	public float Angle { get; set; }

	static FanHitCheckDrawInfo()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<FanHitCheckDrawInfo>())
		{
			MemoryPackFormatterProvider.Register(new FanHitCheckDrawInfoFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<FanHitCheckDrawInfo[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<FanHitCheckDrawInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitCheckShapeType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<HitCheckShapeType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FanHitCheckDrawInfo? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<int, Vector3, Rotator, HitCheckShapeType, float, float, float, float>(8, value.actorID, value.center, value.rotation, value.shapeType, value.InnerRad, value.OuterRad, value.Height, value.Angle);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref FanHitCheckDrawInfo? value)
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
		float value9;
		if (memberCount == 8)
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
				value9 = value.Angle;
				reader.ReadUnmanaged<int>(out value2);
				reader.ReadUnmanaged<Vector3>(out value3);
				reader.ReadUnmanaged<Rotator>(out value4);
				reader.ReadUnmanaged<HitCheckShapeType>(out value5);
				reader.ReadUnmanaged<float>(out value6);
				reader.ReadUnmanaged<float>(out value7);
				reader.ReadUnmanaged<float>(out value8);
				reader.ReadUnmanaged<float>(out value9);
				goto IL_01b7;
			}
			reader.ReadUnmanaged<int, Vector3, Rotator, HitCheckShapeType, float, float, float, float>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9);
		}
		else
		{
			if (memberCount > 8)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(FanHitCheckDrawInfo), 8, memberCount);
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
				value9 = 0f;
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
				value9 = value.Angle;
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
										if (memberCount != 7)
										{
											reader.ReadUnmanaged<float>(out value9);
											_ = 8;
										}
									}
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_01b7;
			}
		}
		value = new FanHitCheckDrawInfo
		{
			actorID = value2,
			center = value3,
			rotation = value4,
			shapeType = value5,
			InnerRad = value6,
			OuterRad = value7,
			Height = value8,
			Angle = value9
		};
		return;
		IL_01b7:
		value.actorID = value2;
		value.center = value3;
		value.rotation = value4;
		value.shapeType = value5;
		value.InnerRad = value6;
		value.OuterRad = value7;
		value.Height = value8;
		value.Angle = value9;
	}
}
