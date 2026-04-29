using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class ProjectileHitTargetSig : IMsg, IMemoryPackable<ProjectileHitTargetSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ProjectileHitTargetSigFormatter : MemoryPackFormatter<ProjectileHitTargetSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ProjectileHitTargetSig value)
		{
			ProjectileHitTargetSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ProjectileHitTargetSig value)
		{
			ProjectileHitTargetSig.Deserialize(ref reader, ref value);
		}
	}

	public int projectileActorID { get; set; }

	public int projectileMasterID { get; set; }

	public List<TargetHitInfo> targetHitInfos { get; set; } = new List<TargetHitInfo>();

	public Vector3 hitPos { get; set; }

	public Vector3 surfaceNormal { get; set; }

	public ProjectileHitTargetSig()
		: base(MsgType.C2S_ProjectileHitTargetSig)
	{
		base.reliable = true;
	}

	static ProjectileHitTargetSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ProjectileHitTargetSig>())
		{
			MemoryPackFormatterProvider.Register(new ProjectileHitTargetSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ProjectileHitTargetSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ProjectileHitTargetSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<TargetHitInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<TargetHitInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ProjectileHitTargetSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int>(7, value.msgType, value.hashCode, value.projectileActorID, value.projectileMasterID);
		ListFormatter.SerializePackable(ref writer, value.targetHitInfos);
		writer.WriteUnmanaged<Vector3, Vector3>(value.hitPos, value.surfaceNormal);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ProjectileHitTargetSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		int value5;
		List<TargetHitInfo> value6;
		Vector3 value7;
		Vector3 value8;
		if (memberCount == 7)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.projectileActorID;
				value5 = value.projectileMasterID;
				value6 = value.targetHitInfos;
				value7 = value.hitPos;
				value8 = value.surfaceNormal;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				ListFormatter.DeserializePackable(ref reader, ref value6);
				reader.ReadUnmanaged<Vector3>(out value7);
				reader.ReadUnmanaged<Vector3>(out value8);
				goto IL_0186;
			}
			reader.ReadUnmanaged<MsgType, int, int, int>(out value2, out value3, out value4, out value5);
			value6 = ListFormatter.DeserializePackable<TargetHitInfo>(ref reader);
			reader.ReadUnmanaged<Vector3, Vector3>(out value7, out value8);
		}
		else
		{
			if (memberCount > 7)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ProjectileHitTargetSig), 7, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = null;
				value7 = default(Vector3);
				value8 = default(Vector3);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.projectileActorID;
				value5 = value.projectileMasterID;
				value6 = value.targetHitInfos;
				value7 = value.hitPos;
				value8 = value.surfaceNormal;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<int>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<int>(out value5);
							if (memberCount != 4)
							{
								ListFormatter.DeserializePackable(ref reader, ref value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<Vector3>(out value7);
									if (memberCount != 6)
									{
										reader.ReadUnmanaged<Vector3>(out value8);
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
				goto IL_0186;
			}
		}
		value = new ProjectileHitTargetSig
		{
			msgType = value2,
			hashCode = value3,
			projectileActorID = value4,
			projectileMasterID = value5,
			targetHitInfos = value6,
			hitPos = value7,
			surfaceNormal = value8
		};
		return;
		IL_0186:
		value.msgType = value2;
		value.hashCode = value3;
		value.projectileActorID = value4;
		value.projectileMasterID = value5;
		value.targetHitInfos = value6;
		value.hitPos = value7;
		value.surfaceNormal = value8;
	}
}
