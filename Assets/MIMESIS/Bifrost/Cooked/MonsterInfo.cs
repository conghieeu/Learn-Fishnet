using System.Collections.Immutable;
using Bifrost.ConstEnum;
using Bifrost.MonsterData;

namespace Bifrost.Cooked
{
	public class MonsterInfo
	{
		public readonly int MasterID;

		public readonly string Name;

		public readonly string BTName;

		public readonly string PuppetName;

		public ImmutableList<int> Factions = ImmutableList<int>.Empty;

		public readonly long HP;

		public readonly long AttackPower;

		public readonly long Defense;

		public readonly long MoveSpeedRun;

		public readonly long MoveSpeedWalk;

		public readonly long AbnormalTriggerThreshold;

		public readonly int AbnormalTriggerID;

		public ImmutableList<int> SkillList = ImmutableList<int>.Empty;

		public readonly int AggroHuddleForTargeting;

		public ImmutableDictionary<AggroType, AggroInfo> AggroInfoDict = ImmutableDictionary<AggroType, AggroInfo>.Empty;

		public BTTargetPickRule TargetPickRule;

		public readonly MonsterType MonsterType;

		public readonly int ThreatValue;

		public readonly int AbnormalMasterIDOnSpawn;

		public readonly int ItemDropMasterID;

		public readonly int AuraMasterID;

		public readonly bool EnableRecovery;

		public readonly float MoveCollisionRadius;

		public readonly float HitCollisionRadius;

		public readonly bool RemainAfterDeath;

		public readonly long SpawningWaitTime;

		public readonly long DyingWaittime;

		public MonsterInfo(MonsterData_MasterData masterData)
		{
			MasterID = masterData.id;
			Name = masterData.name;
			BTName = masterData.btname;
			PuppetName = masterData.model_path;
			HP = masterData.hp;
			AttackPower = masterData.attack_power;
			Defense = masterData.defense;
			MoveSpeedWalk = masterData.move_speed_walk;
			MoveSpeedRun = masterData.move_speed_run;
			MoveCollisionRadius = masterData.move_radius;
			HitCollisionRadius = masterData.hit_radius;
			RemainAfterDeath = masterData.remain_after_death;
			SpawningWaitTime = masterData.spawn_wait_time;
			DyingWaittime = masterData.despawn_delay;
			ImmutableList<int>.Builder builder = ImmutableList.CreateBuilder<int>();
			foreach (int faction in masterData.factions)
			{
				builder.Add(faction);
			}
			Factions = builder.ToImmutable();
			ImmutableList<int>.Builder builder2 = ImmutableList.CreateBuilder<int>();
			foreach (int default_skill_id in masterData.default_skill_ids)
			{
				builder2.Add(default_skill_id);
			}
			SkillList = builder2.ToImmutable();
			ImmutableDictionary<AggroType, AggroInfo>.Builder builder3 = ImmutableDictionary.CreateBuilder<AggroType, AggroInfo>();
			foreach (MonsterData_aggro item in masterData.MonsterData_aggroval)
			{
				builder3.Add((AggroType)item.type, new AggroInfo((AggroType)item.type, item.weight, item.range, item.increase_score_per_distance));
			}
			AggroInfoDict = builder3.ToImmutable();
			AbnormalTriggerThreshold = masterData.abnormal_trigger_threshold;
			AbnormalTriggerID = masterData.abnormal_trigger_id;
			AggroHuddleForTargeting = masterData.targeting_aggro_score;
			TargetPickRule = (BTTargetPickRule)masterData.pick_target;
			MonsterType = (MonsterType)masterData.monster_type;
			ThreatValue = masterData.threat_value;
			AbnormalMasterIDOnSpawn = masterData.inborn_abnormal;
			ItemDropMasterID = masterData.drop_id;
			AuraMasterID = masterData.start_aura_skill_id;
			EnableRecovery = masterData.recover_mode == 1;
		}

		public bool IsMimic()
		{
			return MonsterType == MonsterType.Mimic;
		}
	}
}
