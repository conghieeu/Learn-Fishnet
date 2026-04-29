using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class ProjectileObjectInfo : ActorBaseInfo, IMemoryPackable<ProjectileObjectInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ProjectileObjectInfoFormatter : MemoryPackFormatter<ProjectileObjectInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ProjectileObjectInfo value)
			{
				ProjectileObjectInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ProjectileObjectInfo value)
			{
				ProjectileObjectInfo.Deserialize(ref reader, ref value);
			}
		}

		public int parentActorID { get; set; }

		public int projectileMasterID { get; set; }

		public long endTime { get; set; }

		static ProjectileObjectInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ProjectileObjectInfo>())
			{
				MemoryPackFormatterProvider.Register(new ProjectileObjectInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ProjectileObjectInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ProjectileObjectInfo>());
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
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ProjectileObjectInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<ActorType, int, int>(10, value.actorType, value.actorID, value.masterID);
			writer.WriteString(value.actorName);
			writer.WritePackable<PosWithRot>(value.position);
			writer.WriteUnmanaged<long, ReasonOfSpawn, int, int, long>(value.UID, value.reasonOfSpawn, value.parentActorID, value.projectileMasterID, value.endTime);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref ProjectileObjectInfo? value)
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
			long value10;
			string text;
			if (memberCount == 10)
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
					value9 = value.projectileMasterID;
					value10 = value.endTime;
					reader.ReadUnmanaged<ActorType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					text = reader.ReadString();
					reader.ReadPackable(ref value5);
					reader.ReadUnmanaged<long>(out value6);
					reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
					reader.ReadUnmanaged<int>(out value8);
					reader.ReadUnmanaged<int>(out value9);
					reader.ReadUnmanaged<long>(out value10);
					goto IL_020a;
				}
				reader.ReadUnmanaged<ActorType, int, int>(out value2, out value3, out value4);
				text = reader.ReadString();
				value5 = reader.ReadPackable<PosWithRot>();
				reader.ReadUnmanaged<long, ReasonOfSpawn, int, int, long>(out value6, out value7, out value8, out value9, out value10);
			}
			else
			{
				if (memberCount > 10)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ProjectileObjectInfo), 10, memberCount);
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
					value10 = 0L;
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
					value9 = value.projectileMasterID;
					value10 = value.endTime;
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
														reader.ReadUnmanaged<long>(out value10);
														_ = 10;
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
					goto IL_020a;
				}
			}
			value = new ProjectileObjectInfo
			{
				actorType = value2,
				actorID = value3,
				masterID = value4,
				actorName = text,
				position = value5,
				UID = value6,
				reasonOfSpawn = value7,
				parentActorID = value8,
				projectileMasterID = value9,
				endTime = value10
			};
			return;
			IL_020a:
			value.actorType = value2;
			value.actorID = value3;
			value.masterID = value4;
			value.actorName = text;
			value.position = value5;
			value.UID = value6;
			value.reasonOfSpawn = value7;
			value.parentActorID = value8;
			value.projectileMasterID = value9;
			value.endTime = value10;
		}
	}
}
