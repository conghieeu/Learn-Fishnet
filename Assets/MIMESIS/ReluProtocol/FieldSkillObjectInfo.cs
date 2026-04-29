using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;
using UnityEngine;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class FieldSkillObjectInfo : ActorBaseInfo, IMemoryPackable<FieldSkillObjectInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class FieldSkillObjectInfoFormatter : MemoryPackFormatter<FieldSkillObjectInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FieldSkillObjectInfo value)
			{
				FieldSkillObjectInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref FieldSkillObjectInfo value)
			{
				FieldSkillObjectInfo.Deserialize(ref reader, ref value);
			}
		}

		public int parentActorID { get; set; }

		public int fieldSkillMasterID { get; set; }

		public int fieldSkillIndex { get; set; }

		public long endTime { get; set; }

		public Vector3 surfaceNormal { get; set; }

		static FieldSkillObjectInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<FieldSkillObjectInfo>())
			{
				MemoryPackFormatterProvider.Register(new FieldSkillObjectInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<FieldSkillObjectInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<FieldSkillObjectInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ActorType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ActorType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfSpawn>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfSpawn>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FieldSkillObjectInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<ActorType, int, int>(12, value.actorType, value.actorID, value.masterID);
			writer.WriteString(value.actorName);
			writer.WritePackable<PosWithRot>(value.position);
			writer.WriteUnmanaged<long, ReasonOfSpawn, int, int, int, long, Vector3>(value.UID, value.reasonOfSpawn, value.parentActorID, value.fieldSkillMasterID, value.fieldSkillIndex, value.endTime, value.surfaceNormal);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref FieldSkillObjectInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			ActorType value2;
			int value3;
			int value4;
			PosWithRot value5;
			long value6;
			ReasonOfSpawn value7;
			int value8;
			int value9;
			int value10;
			long value11;
			Vector3 value12;
			string text;
			if (memberCount == 12)
			{
				if (value != null)
				{
					value2 = value.actorType;
					value3 = value.actorID;
					value4 = value.masterID;
					text = value.actorName;
					value5 = value.position;
					value6 = value.UID;
					value7 = value.reasonOfSpawn;
					value8 = value.parentActorID;
					value9 = value.fieldSkillMasterID;
					value10 = value.fieldSkillIndex;
					value11 = value.endTime;
					value12 = value.surfaceNormal;
					reader.ReadUnmanaged<ActorType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					text = reader.ReadString();
					reader.ReadPackable(ref value5);
					reader.ReadUnmanaged<long>(out value6);
					reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
					reader.ReadUnmanaged<int>(out value8);
					reader.ReadUnmanaged<int>(out value9);
					reader.ReadUnmanaged<int>(out value10);
					reader.ReadUnmanaged<long>(out value11);
					reader.ReadUnmanaged<Vector3>(out value12);
					goto IL_026d;
				}
				reader.ReadUnmanaged<ActorType, int, int>(out value2, out value3, out value4);
				text = reader.ReadString();
				value5 = reader.ReadPackable<PosWithRot>();
				reader.ReadUnmanaged<long, ReasonOfSpawn, int, int, int, long, Vector3>(out value6, out value7, out value8, out value9, out value10, out value11, out value12);
			}
			else
			{
				if (memberCount > 12)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(FieldSkillObjectInfo), 12, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = ActorType.None;
					value3 = 0;
					value4 = 0;
					text = null;
					value5 = null;
					value6 = 0L;
					value7 = ReasonOfSpawn.None;
					value8 = 0;
					value9 = 0;
					value10 = 0;
					value11 = 0L;
					value12 = default(Vector3);
				}
				else
				{
					value2 = value.actorType;
					value3 = value.actorID;
					value4 = value.masterID;
					text = value.actorName;
					value5 = value.position;
					value6 = value.UID;
					value7 = value.reasonOfSpawn;
					value8 = value.parentActorID;
					value9 = value.fieldSkillMasterID;
					value10 = value.fieldSkillIndex;
					value11 = value.endTime;
					value12 = value.surfaceNormal;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<ActorType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								text = reader.ReadString();
								if (memberCount != 4)
								{
									reader.ReadPackable(ref value5);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<long>(out value6);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<int>(out value8);
												if (memberCount != 8)
												{
													reader.ReadUnmanaged<int>(out value9);
													if (memberCount != 9)
													{
														reader.ReadUnmanaged<int>(out value10);
														if (memberCount != 10)
														{
															reader.ReadUnmanaged<long>(out value11);
															if (memberCount != 11)
															{
																reader.ReadUnmanaged<Vector3>(out value12);
																_ = 12;
															}
														}
													}
												}
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
					goto IL_026d;
				}
			}
			value = new FieldSkillObjectInfo
			{
				actorType = value2,
				actorID = value3,
				masterID = value4,
				actorName = text,
				position = value5,
				UID = value6,
				reasonOfSpawn = value7,
				parentActorID = value8,
				fieldSkillMasterID = value9,
				fieldSkillIndex = value10,
				endTime = value11,
				surfaceNormal = value12
			};
			return;
			IL_026d:
			value.actorType = value2;
			value.actorID = value3;
			value.masterID = value4;
			value.actorName = text;
			value.position = value5;
			value.UID = value6;
			value.reasonOfSpawn = value7;
			value.parentActorID = value8;
			value.fieldSkillMasterID = value9;
			value.fieldSkillIndex = value10;
			value.endTime = value11;
			value.surfaceNormal = value12;
		}
	}
}
