using System.Collections.Immutable;
using Bifrost.Cooked;
using ReluProtocol.Enum;
using UnityEngine;

namespace Mimic.Actors
{
	public class AuraSkillActor : MonoBehaviour
	{
		public int MasterID;

		public string AuraPrefabName;

		public long AuraPrefabDuration;

		public IHitCheck HitBoxShape;

		public Vector3 AuraOffset;

		public long AuraDuration;

		public ImmutableArray<HitTargetType> HitTargetTypes = ImmutableArray<HitTargetType>.Empty;

		public int AbnormalMasterID;

		public int LinkSkillTargetEffectDataID;

		private GameObject spawnedAura;

		public ActorType ActorType { get; } = ActorType.AuraSkill;

		public ProtoActor Owner { get; private set; }

		public float Spawn(AuraInfo auraInfo, ProtoActor owner)
		{
			Owner = owner;
			MasterID = auraInfo.MasterID;
			AuraPrefabName = auraInfo.AuraPrefabName;
			AuraPrefabDuration = auraInfo.AuraPrefabDuration;
			HitBoxShape = auraInfo.HitCheck;
			AuraOffset = auraInfo.AuraOffset;
			AuraDuration = auraInfo.AuraDuration;
			HitTargetTypes = auraInfo.HitTargetTypes;
			AbnormalMasterID = auraInfo.AbnormalMasterID;
			LinkSkillTargetEffectDataID = auraInfo.LinkSkillTargetEffectDataID;
			base.transform.position = owner.transform.position + owner.transform.rotation * AuraOffset;
			base.transform.rotation = owner.transform.rotation;
			base.transform.SetParent(owner.transform);
			float num = (float)AuraPrefabDuration / 1000f;
			if (Hub.s.tableman.auraSkills.TryGet(AuraPrefabName, out MMAuraSkillTable.Row row))
			{
				spawnedAura = Object.Instantiate(row.prefab, base.transform);
				if (spawnedAura == null)
				{
					Logger.RError("[AuraSkillActor] Failed to instantiate aura prefab: " + AuraPrefabName, sendToLogServer: false, "aura");
				}
			}
			if (LinkSkillTargetEffectDataID != 0)
			{
				bool playAnimation = true;
				if (auraInfo.HitTargetTypes.Contains(HitTargetType.ALL) || auraInfo.HitTargetTypes.Contains(HitTargetType.Self))
				{
					AbnormalInfo abnormalInfo = Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(AbnormalMasterID);
					if (abnormalInfo != null)
					{
						SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(abnormalInfo.SkillTargetEffectId);
						if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.animation.Length <= 0)
						{
							playAnimation = false;
						}
					}
				}
				owner.PlaySkillHitEffect(LinkSkillTargetEffectDataID, playAnimation);
			}
			Object.Destroy(base.gameObject, num);
			return num;
		}

		public void ShowDebugText(string debugText)
		{
		}
	}
}
