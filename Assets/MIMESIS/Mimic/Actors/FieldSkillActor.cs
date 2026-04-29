using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

namespace Mimic.Actors
{
	public class FieldSkillActor : MonoBehaviour, IActor
	{
		private FieldSkillMemberInfo currentMemberInfo;

		private GameObject spawnedFieldSkillEffect;

		public ActorType ActorType { get; } = ActorType.FieldSkill;

		public int ActorID { get; private set; }

		public int ParentActorId { get; private set; }

		public int FieldSkillMasterId { get; private set; }

		public int FieldSkillIndex { get; private set; }

		public long EndTimeMSec { get; private set; }

		public Vector3 spawnPointSurfaceNormal { get; private set; } = Vector3.zero;

		public float Spawn(FieldSkillObjectInfo fieldSkillObjectInfo)
		{
			ActorID = fieldSkillObjectInfo.actorID;
			ParentActorId = fieldSkillObjectInfo.parentActorID;
			FieldSkillMasterId = fieldSkillObjectInfo.masterID;
			FieldSkillIndex = fieldSkillObjectInfo.fieldSkillIndex;
			base.transform.position = fieldSkillObjectInfo.position.toVector3();
			base.transform.rotation = fieldSkillObjectInfo.position.toRotation().ToQuaternion();
			float num = 0.1f;
			FieldSkillInfo fieldSkillData = Hub.s.dataman.ExcelDataManager.GetFieldSkillData(fieldSkillObjectInfo.fieldSkillMasterID);
			if (fieldSkillData != null)
			{
				FieldSkillMemberInfo fieldSkillMemberInfo = fieldSkillData.FieldSkillMemberInfos[fieldSkillObjectInfo.fieldSkillIndex];
				if (fieldSkillMemberInfo != null)
				{
					if (Hub.s.tableman.fieldSkills.TryGet(fieldSkillMemberInfo.PrefabName, out MMFieldSkillTable.Row row))
					{
						Vector3 position = fieldSkillObjectInfo.position.toVector3();
						Quaternion rotation = fieldSkillObjectInfo.position.toRotation();
						spawnedFieldSkillEffect = Object.Instantiate(row.prefab, base.transform);
						if (spawnedFieldSkillEffect != null)
						{
							spawnedFieldSkillEffect.transform.position = position;
							spawnedFieldSkillEffect.transform.rotation = rotation;
							num = (float)fieldSkillMemberInfo.PrefabDisplayDuration / 1000f;
							Object.Destroy(spawnedFieldSkillEffect, num);
						}
					}
					if (fieldSkillMemberInfo.DecalId != null && fieldSkillMemberInfo.DecalId != "")
					{
						Quaternion quaternion = Quaternion.Euler(fieldSkillObjectInfo.position.pitch, fieldSkillObjectInfo.position.yaw, fieldSkillObjectInfo.position.roll);
						if (quaternion == Quaternion.identity)
						{
							quaternion = Quaternion.LookRotation(Vector3.down);
						}
						Vector3 normalized = (quaternion * Vector3.forward).normalized;
						Vector3 value = fieldSkillObjectInfo.position.toVector3();
						Vector3 vector = fieldSkillObjectInfo.surfaceNormal;
						if (vector == Vector3.zero)
						{
							vector = Vector3.up;
						}
						DecalManager.DecalData decalData = DecalManager.DecalData.CreateDecalData(fieldSkillMemberInfo.DecalId, fieldSkillMemberInfo.DecalColorId, 0f, fieldSkillMemberInfo.IsDecalRotateRandomly, fieldSkillMemberInfo.DecalDurationMSec, fieldSkillMemberInfo.DecalFadeoutMSec, fieldSkillMemberInfo.DecalDistanceFromSpawnPoint, normalized, vector);
						Hub.s.decalManamger.SpawnDecal(decalData, null, value);
					}
				}
			}
			return num;
		}

		public void ShowDebugText(string debugText)
		{
		}

		[PacketHandler(false)]
		private void OnDebugInfoSig(DebugInfoSig packet)
		{
			if (packet.actorID != ActorID)
			{
				Logger.RError($"[DebugInfoSig] FieldSkillActor. packet.actorID != ActorID. {packet.actorID} != {ActorID}");
				return;
			}
			if (!string.IsNullOrEmpty(packet.debugInfo))
			{
				ShowDebugText(packet.debugInfo);
			}
			Hub.s.UpdateHitCheckVisualizations(packet.actorID, packet.hitCheckDrawInfos);
		}
	}
}
