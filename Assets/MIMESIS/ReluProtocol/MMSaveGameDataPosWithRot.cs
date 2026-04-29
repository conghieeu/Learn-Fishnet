using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[Serializable]
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class MMSaveGameDataPosWithRot : IMemoryPackable<MMSaveGameDataPosWithRot>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class MMSaveGameDataPosWithRotFormatter : MemoryPackFormatter<MMSaveGameDataPosWithRot>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameDataPosWithRot value)
			{
				MMSaveGameDataPosWithRot.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref MMSaveGameDataPosWithRot value)
			{
				MMSaveGameDataPosWithRot.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public float X { get; set; }

		[MemoryPackOrder(1)]
		public float Y { get; set; }

		[MemoryPackOrder(2)]
		public float Z { get; set; }

		[MemoryPackOrder(3)]
		public float Pitch { get; set; }

		[MemoryPackOrder(4)]
		public float Yaw { get; set; }

		[MemoryPackOrder(5)]
		public float Roll { get; set; }

		public PosWithRot ToPosWitRot()
		{
			PosWithRot posWithRot = new PosWithRot();
			posWithRot.x = X;
			posWithRot.y = Y;
			posWithRot.z = Z;
			posWithRot.pitch = Pitch;
			posWithRot.yaw = Yaw;
			posWithRot.roll = Roll;
			return posWithRot;
		}

		static MMSaveGameDataPosWithRot()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameDataPosWithRot>())
			{
				MemoryPackFormatterProvider.Register(new MMSaveGameDataPosWithRotFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameDataPosWithRot[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<MMSaveGameDataPosWithRot>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameDataPosWithRot? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(6);
			writer.WriteVarInt(Unsafe.SizeOf<float>());
			writer.WriteVarInt(Unsafe.SizeOf<float>());
			writer.WriteVarInt(Unsafe.SizeOf<float>());
			writer.WriteVarInt(Unsafe.SizeOf<float>());
			writer.WriteVarInt(Unsafe.SizeOf<float>());
			writer.WriteVarInt(Unsafe.SizeOf<float>());
			writer.WriteUnmanaged<float, float, float, float, float, float>(value.X, value.Y, value.Z, value.Pitch, value.Yaw, value.Roll);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref MMSaveGameDataPosWithRot? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Span<int> span = stackalloc int[(int)memberCount];
			for (int i = 0; i < memberCount; i++)
			{
				span[i] = reader.ReadVarIntInt32();
			}
			int num = 6;
			float value2;
			float value3;
			float value4;
			float value5;
			float value6;
			float value7;
			if (memberCount == 6)
			{
				if (value != null)
				{
					value2 = value.X;
					value3 = value.Y;
					value4 = value.Z;
					value5 = value.Pitch;
					value6 = value.Yaw;
					value7 = value.Roll;
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<float>(out value2);
					}
					if (span[1] != 0)
					{
						reader.ReadUnmanaged<float>(out value3);
					}
					if (span[2] != 0)
					{
						reader.ReadUnmanaged<float>(out value4);
					}
					if (span[3] != 0)
					{
						reader.ReadUnmanaged<float>(out value5);
					}
					if (span[4] != 0)
					{
						reader.ReadUnmanaged<float>(out value6);
					}
					if (span[5] != 0)
					{
						reader.ReadUnmanaged<float>(out value7);
					}
					goto IL_02a3;
				}
				if (span[0] == 0)
				{
					value2 = 0f;
				}
				else
				{
					reader.ReadUnmanaged<float>(out value2);
				}
				if (span[1] == 0)
				{
					value3 = 0f;
				}
				else
				{
					reader.ReadUnmanaged<float>(out value3);
				}
				if (span[2] == 0)
				{
					value4 = 0f;
				}
				else
				{
					reader.ReadUnmanaged<float>(out value4);
				}
				if (span[3] == 0)
				{
					value5 = 0f;
				}
				else
				{
					reader.ReadUnmanaged<float>(out value5);
				}
				if (span[4] == 0)
				{
					value6 = 0f;
				}
				else
				{
					reader.ReadUnmanaged<float>(out value6);
				}
				if (span[5] == 0)
				{
					value7 = 0f;
				}
				else
				{
					reader.ReadUnmanaged<float>(out value7);
				}
			}
			else
			{
				if (value == null)
				{
					value2 = 0f;
					value3 = 0f;
					value4 = 0f;
					value5 = 0f;
					value6 = 0f;
					value7 = 0f;
				}
				else
				{
					value2 = value.X;
					value3 = value.Y;
					value4 = value.Z;
					value5 = value.Pitch;
					value6 = value.Yaw;
					value7 = value.Roll;
				}
				if (memberCount != 0)
				{
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<float>(out value2);
					}
					if (memberCount != 1)
					{
						if (span[1] != 0)
						{
							reader.ReadUnmanaged<float>(out value3);
						}
						if (memberCount != 2)
						{
							if (span[2] != 0)
							{
								reader.ReadUnmanaged<float>(out value4);
							}
							if (memberCount != 3)
							{
								if (span[3] != 0)
								{
									reader.ReadUnmanaged<float>(out value5);
								}
								if (memberCount != 4)
								{
									if (span[4] != 0)
									{
										reader.ReadUnmanaged<float>(out value6);
									}
									if (memberCount != 5)
									{
										if (span[5] != 0)
										{
											reader.ReadUnmanaged<float>(out value7);
										}
										_ = 6;
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_02a3;
				}
			}
			value = new MMSaveGameDataPosWithRot
			{
				X = value2,
				Y = value3,
				Z = value4,
				Pitch = value5,
				Yaw = value6,
				Roll = value7
			};
			goto IL_030e;
			IL_02a3:
			value.X = value2;
			value.Y = value3;
			value.Z = value4;
			value.Pitch = value5;
			value.Yaw = value6;
			value.Roll = value7;
			goto IL_030e;
			IL_030e:
			if (memberCount != num)
			{
				for (int j = num; j < memberCount; j++)
				{
					reader.Advance(span[j]);
				}
			}
		}
	}
}
