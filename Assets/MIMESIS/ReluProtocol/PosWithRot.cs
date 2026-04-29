using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class PosWithRot : IMemoryPackable<PosWithRot>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class PosWithRotFormatter : MemoryPackFormatter<PosWithRot>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PosWithRot value)
			{
				PosWithRot.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PosWithRot value)
			{
				PosWithRot.Deserialize(ref reader, ref value);
			}
		}

		private Vector3 _pos;

		private Vector3 _rot;

		public Vector3 pos
		{
			get
			{
				return _pos;
			}
			set
			{
				_pos = value;
			}
		}

		public Vector3 rot
		{
			get
			{
				return _rot;
			}
			set
			{
				_rot = value;
			}
		}

		public ref float x => ref _pos.x;

		public ref float y => ref _pos.y;

		public ref float z => ref _pos.z;

		public ref float pitch => ref _rot.x;

		public ref float yaw => ref _rot.y;

		public ref float roll => ref _rot.z;

		public void Clean()
		{
			pos = Vector3.zero;
			rot = Vector3.zero;
		}

		public void CopyTo(PosWithRot to)
		{
			to.pos = pos;
			to.rot = rot;
		}

		public PosWithRot Clone()
		{
			return new PosWithRot
			{
				pos = pos,
				rot = rot
			};
		}

		public override string ToString()
		{
			return $"({pos.x:F3}, {pos.y:F3}, {pos.z:F3} : {rot.x:F1}, {rot.y:F1}, {rot.z:F1})";
		}

		public PosWithRot CreateForwardPosWithRot(float distance = 1f)
		{
			float num = rot.y * (MathF.PI / 180f);
			float num2 = (float)Math.Sin(num);
			float num3 = (float)Math.Cos(num);
			return new PosWithRot
			{
				pos = new Vector3(pos.x + num2 * distance, pos.y, pos.z + num3 * distance),
				rot = rot
			};
		}

		public MMSaveGameDataPosWithRot ToMMSaveGameDataPosWithRot()
		{
			return new MMSaveGameDataPosWithRot
			{
				X = x,
				Y = y,
				Z = z,
				Pitch = pitch,
				Yaw = yaw,
				Roll = roll
			};
		}

		static PosWithRot()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PosWithRot>())
			{
				MemoryPackFormatterProvider.Register(new PosWithRotFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PosWithRot[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PosWithRot>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PosWithRot? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<Vector3, Vector3, float, float, float, float, float, float>(8, value.pos, value.rot, in value.x, in value.y, in value.z, in value.pitch, in value.yaw, in value.roll);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PosWithRot? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Vector3 value2;
			Vector3 value3;
			if (memberCount == 8)
			{
				float value4;
				float value5;
				float value6;
				float value7;
				float value8;
				float value9;
				if (value != null)
				{
					value2 = value.pos;
					value3 = value.rot;
					value4 = value.x;
					value5 = value.y;
					value6 = value.z;
					value7 = value.pitch;
					value8 = value.yaw;
					value9 = value.roll;
					reader.ReadUnmanaged<Vector3>(out value2);
					reader.ReadUnmanaged<Vector3>(out value3);
					reader.ReadUnmanaged<float>(out value4);
					reader.ReadUnmanaged<float>(out value5);
					reader.ReadUnmanaged<float>(out value6);
					reader.ReadUnmanaged<float>(out value7);
					reader.ReadUnmanaged<float>(out value8);
					reader.ReadUnmanaged<float>(out value9);
					goto IL_01cb;
				}
				reader.ReadUnmanaged<Vector3, Vector3, float, float, float, float, float, float>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9);
			}
			else
			{
				if (memberCount > 8)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PosWithRot), 8, memberCount);
					return;
				}
				float value4;
				float value5;
				float value6;
				float value7;
				float value8;
				float value9;
				if (value == null)
				{
					value2 = default(Vector3);
					value3 = default(Vector3);
					value4 = 0f;
					value5 = 0f;
					value6 = 0f;
					value7 = 0f;
					value8 = 0f;
					value9 = 0f;
				}
				else
				{
					value2 = value.pos;
					value3 = value.rot;
					value4 = value.x;
					value5 = value.y;
					value6 = value.z;
					value7 = value.pitch;
					value8 = value.yaw;
					value9 = value.roll;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<Vector3>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<Vector3>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<float>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<float>(out value5);
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
					goto IL_01cb;
				}
			}
			value = new PosWithRot
			{
				pos = value2,
				rot = value3
			};
			return;
			IL_01cb:
			value.pos = value2;
			value.rot = value3;
		}
	}
}
