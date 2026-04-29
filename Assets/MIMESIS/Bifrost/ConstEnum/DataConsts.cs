using System;
using Bifrost.Const;

namespace Bifrost.ConstEnum
{
	public class DataConsts
	{
		public const int CC_HIGHEST_PRIORITY = 9999999;

		public readonly int c_DefaultAIPauseTimeOnActionAbnormal = 100;

		public readonly long C_WaitTimeContaSpawnMimic;

		public readonly long C_RunStaminaConsumeValue;

		public readonly long C_RunContaIncreasePeriod;

		public readonly long C_StaminaRegenValue;

		public readonly long C_IdleContaIncreasePeriod;

		public readonly long C_StaminaRegenDelayRemain;

		public readonly long C_StaminaRegenDelayEmpty;

		public readonly long C_RunStaminaConsumePeriod;

		public readonly long C_StaminaRegenPeriod;

		public readonly long C_GroggyThresholdConsumeValue;

		public readonly long C_GroggyThresholdConsumePeriod;

		[Obsolete]
		public readonly int C_EquipItemGaugeConsumePeriod;

		public readonly long C_GameTimeScaleFactor;

		public readonly long C_AvailableDayPerSession;

		[Obsolete]
		public readonly int C_InitialQuota;

		public readonly int C_QuotaMultiplierPerSessionRate;

		public readonly int C_InitialMoney;

		public readonly int C_WaitTimeWaitingRoomSettlement;

		public readonly int C_WaitTimeGameRoomResulting;

		public readonly int C_WaitTimeGameRoomSuccess;

		public readonly int C_WaitTimeGameRoomFailed;

		public readonly long C_AlarmTimeWeatherEventMessage;

		public readonly long C_MaxContaValue;

		public readonly long C_MaxStaminaValue;

		public readonly int C_MaxCarryWeight;

		public readonly int C_MinThresholdMoveSpeedRate;

		public readonly int C_InitialDungeonSkipMoney;

		public readonly int C_IncrementDungeonSkipMoney;

		public readonly long C_IncreaseAggroTick;

		public readonly long C_DecreaseAggroTick;

		public readonly long C_NoAggroDuration;

		public readonly long C_DecreaseAggroRatePerTick;

		public readonly long C_BTChangeTargetThreshold;

		public readonly long C_ContaRecoveryValue;

		public readonly long C_FallSafeDistance;

		public readonly long C_FallHazardDistance;

		public readonly long C_FallInstanceDeathDistance;

		public readonly long C_NavYThreshold;

		public readonly long C_WaitTimeDecisionGamesuccess;

		public readonly long C_WaitTimeDecisionGameover;

		public readonly int C_ContaScreenEffectId;

		public readonly long C_ContaScreenEffectStartValue;

		public readonly long C_AutoObservingTargetChangeTime;

		public readonly long C_VoicePitchMax;

		public readonly long C_VoicePitchMin;

		public readonly long C_BonusItemAppearRate;

		public readonly long C_BonusItemRateMin;

		public readonly long C_BonusItemRateMax;

		public readonly long C_BlackoutDurationMin;

		public readonly long C_BlackoutDurationMax;

		public readonly long C_DeathmatchRoomContaValue;

		public readonly int C_MaintenanceRoomMapMasterId;

		public readonly int C_WaitingRoomMapMasterId;

		public readonly int C_DeathmatchRoomMapMasterId;

		public readonly int C_WeightLimitTrapWeightMin;

		public readonly int C_WeightLimitTrapWeightMax;

		public readonly int C_DeathmatchRoomSpawnableItemId;

		public readonly int C_DeathmatchRoomSpawnableItemCount;

		public readonly int C_DeathmatchRoomSpawnableMonsterId;

		public readonly int C_DeathmatchRoomSpawnableMonsterCount;

		public readonly int C_UI_PopupDurationDungeonStartLever;

		public readonly int C_UI_PopupDurationSurvivorReport;

		public readonly int C_DeathmatchRoomMVPItemGroupId;

		public readonly float C_SpawnUnableFromRange;

		public readonly long C_MovementAggroCapturePeriod;

		public readonly float C_MovementAggroMinDistanceThreshold;

		public readonly int C_UI_PopupDurationDeathmatchReport;

		public readonly SkyAndWeatherSystem.eWeatherPreset C_DefaultWeather_Maintenance;

		public readonly SkyAndWeatherSystem.eWeatherPreset C_DefaultWeather_InTramWaiting;

		public readonly SkyAndWeatherSystem.eWeatherPreset C_DefaultWeather_Dungeon;

		public readonly SkyAndWeatherSystem.eWeatherPreset C_DefaultWeather_DeathMatch;

		public readonly int C_RandomTeleportEndRangeMin;

		public readonly int C_ReportMinValueScrapCount;

		public readonly int C_ReportMinValueFriendlyAttack;

		public readonly int C_ReportMinValueMetMimesis;

		public readonly int C_ReportMinValueStayTram;

		public readonly int C_FallDamageSkillTargetEffectId;

		public readonly int C_FallDeadSkillTargetEffectId;

		public readonly int C_RandomMatchingWaitingTime;

		public readonly int C_MaxPlayerCount;

		public readonly long C_ScrapBoostValue;

		public readonly int C_ScrapBoostMaxSession;

		public DataConsts(Const_MasterDataHolder holder)
		{
			foreach (Const_MasterData item in holder.dataHolder)
			{
				ConstDataKey result = ConstDataKey.Invalid;
				if (!Enum.TryParse<ConstDataKey>(item.key, ignoreCase: true, out result))
				{
					continue;
				}
				switch (result)
				{
				case ConstDataKey.WAIT_TIME_CONTA_SPAWN_MIMIC:
					C_WaitTimeContaSpawnMimic = item.value1;
					break;
				case ConstDataKey.RUN_STAMINA_CONSUME_VALUE:
					C_RunStaminaConsumeValue = item.value1;
					break;
				case ConstDataKey.STAMINA_REGEN_VALUE:
					C_StaminaRegenValue = item.value1;
					break;
				case ConstDataKey.RUN_CONTA_INCREASE_PERIOD:
					C_RunContaIncreasePeriod = item.value1;
					break;
				case ConstDataKey.IDLE_CONTA_INCREASE_PERIOD:
					C_IdleContaIncreasePeriod = item.value1;
					break;
				case ConstDataKey.STAMINA_REGEN_DELAY_REMAIN:
					C_StaminaRegenDelayRemain = item.value1;
					break;
				case ConstDataKey.STAMINA_REGEN_DELAY_EMPTY:
					C_StaminaRegenDelayEmpty = item.value1;
					break;
				case ConstDataKey.RUN_STAMINA_CONSUME_PERIOD:
					C_RunStaminaConsumePeriod = item.value1;
					break;
				case ConstDataKey.STAMINA_REGEN_PERIOD:
					C_StaminaRegenPeriod = item.value1;
					break;
				case ConstDataKey.GROGGY_THRESHOLD_CONSUME_VALUE:
					C_GroggyThresholdConsumeValue = item.value1;
					break;
				case ConstDataKey.GROGGY_THRESHOLD_CONSUME_PERIOD:
					C_GroggyThresholdConsumePeriod = item.value1;
					break;
				case ConstDataKey.EQUIPITEM_GAUGE_CONSUME_PERIOD:
					C_EquipItemGaugeConsumePeriod = (int)item.value1;
					break;
				case ConstDataKey.GAME_TIME_SCALE_FACTOR:
					C_GameTimeScaleFactor = item.value1;
					break;
				case ConstDataKey.AVAILABLE_DAY_PER_SESSION:
					C_AvailableDayPerSession = item.value1;
					break;
				case ConstDataKey.INITIAL_QUOTA:
					C_InitialQuota = (int)item.value1;
					break;
				case ConstDataKey.QUOTA_MULTIPLIER_PER_SESSION:
					C_QuotaMultiplierPerSessionRate = (int)item.value1;
					break;
				case ConstDataKey.INITIAL_MONEY:
					C_InitialMoney = (int)item.value1;
					break;
				case ConstDataKey.WAIT_TIME_WAITING_ROOM_SETTLEMENT:
					C_WaitTimeWaitingRoomSettlement = (int)item.value1;
					break;
				case ConstDataKey.WAIT_TIME_GAME_ROOM_RESULTING:
					C_WaitTimeGameRoomResulting = (int)item.value1;
					break;
				case ConstDataKey.WAIT_TIME_GAME_ROOM_SUCCESS:
					C_WaitTimeGameRoomSuccess = (int)item.value1;
					break;
				case ConstDataKey.WAIT_TIME_GAME_ROOM_FAILED:
					C_WaitTimeGameRoomFailed = (int)item.value1;
					break;
				case ConstDataKey.ALARM_TIME_WEATHER_EVENT_MESSAGE:
					C_AlarmTimeWeatherEventMessage = item.value1;
					break;
				case ConstDataKey.MAX_CONTA_VALUE:
					C_MaxContaValue = item.value1;
					break;
				case ConstDataKey.MAX_STAMINA_VALUE:
					C_MaxStaminaValue = item.value1;
					break;
				case ConstDataKey.MAX_CARRY_WEIGHT:
					C_MaxCarryWeight = (int)item.value1;
					break;
				case ConstDataKey.MIN_THRESHOLD_MOVESPEED_RATE:
					C_MinThresholdMoveSpeedRate = (int)item.value1;
					break;
				case ConstDataKey.INITIAL_DUNGEON_SKIP_MONEY:
					C_InitialDungeonSkipMoney = (int)item.value1;
					break;
				case ConstDataKey.INCREMENT_DUNGEON_SKIP_MONEY:
					C_IncrementDungeonSkipMoney = (int)item.value1;
					break;
				case ConstDataKey.INCREASE_AGGRO_SCORE_TICK:
					C_IncreaseAggroTick = item.value1;
					break;
				case ConstDataKey.DECREASE_AGGRO_SCORE_TICK:
					C_DecreaseAggroTick = item.value1;
					break;
				case ConstDataKey.NO_AGGRO_DURATION:
					C_NoAggroDuration = item.value1;
					break;
				case ConstDataKey.DECREASE_AGGRO_RATE_PER_TICK:
					C_DecreaseAggroRatePerTick = item.value1;
					break;
				case ConstDataKey.BT_CHANGE_TARGET_THRESHOLD:
					C_BTChangeTargetThreshold = item.value1;
					break;
				case ConstDataKey.CONTA_RECOVERY_VALUE:
					C_ContaRecoveryValue = item.value1;
					break;
				case ConstDataKey.FALL_SAFE_DISTANCE:
					C_FallSafeDistance = item.value1;
					break;
				case ConstDataKey.FALL_HAZARD_DISTANCE:
					C_FallHazardDistance = item.value1;
					break;
				case ConstDataKey.FALL_INSTANCE_DEATH_DISTANCE:
					C_FallInstanceDeathDistance = item.value1;
					break;
				case ConstDataKey.NAV_Y_THRESHOLD:
					C_NavYThreshold = item.value1;
					break;
				case ConstDataKey.WAIT_TIME_DECISION_GAMESUCCESS:
					C_WaitTimeDecisionGamesuccess = item.value1;
					break;
				case ConstDataKey.WAIT_TIME_DECISION_GAMEOVER:
					C_WaitTimeDecisionGameover = item.value1;
					break;
				case ConstDataKey.CONTA_SCREEN_EFFECT_ID:
					C_ContaScreenEffectId = (int)item.value1;
					break;
				case ConstDataKey.CONTA_SCREEN_EFFECT_START_VALUE:
					C_ContaScreenEffectStartValue = item.value1;
					break;
				case ConstDataKey.AUTO_OBSERVING_TARGET_CHANGE_TIME:
					C_AutoObservingTargetChangeTime = item.value1;
					break;
				case ConstDataKey.VOICE_PITCH_MAX:
					C_VoicePitchMax = item.value1;
					break;
				case ConstDataKey.VOICE_PITCH_MIN:
					C_VoicePitchMin = item.value1;
					break;
				case ConstDataKey.BONUS_ITEM_APPEAR_RATE:
					C_BonusItemAppearRate = item.value1;
					break;
				case ConstDataKey.BONUS_ITEM_RATE_MIN:
					C_BonusItemRateMin = item.value1;
					break;
				case ConstDataKey.BONUS_ITEM_RATE_MAX:
					C_BonusItemRateMax = item.value1;
					break;
				case ConstDataKey.BLACKOUT_DURATION_MIN:
					C_BlackoutDurationMin = item.value1;
					break;
				case ConstDataKey.BLACKOUT_DURATION_MAX:
					C_BlackoutDurationMax = item.value1;
					break;
				case ConstDataKey.DEATHMATCH_ROOM_CONTA_VALUE:
					C_DeathmatchRoomContaValue = item.value1;
					break;
				case ConstDataKey.MAINTENANCEROOM_MAP_MASTER_ID:
					C_MaintenanceRoomMapMasterId = (int)item.value1;
					break;
				case ConstDataKey.WAITINGROOM_MAP_MASTER_ID:
					C_WaitingRoomMapMasterId = (int)item.value1;
					break;
				case ConstDataKey.DEATHMATCHROOM_MAP_MASTER_ID:
					C_DeathmatchRoomMapMasterId = (int)item.value1;
					break;
				case ConstDataKey.WEIGHTLIMIT_TRAP_WEIGHT_MIN:
					C_WeightLimitTrapWeightMin = (int)item.value1;
					break;
				case ConstDataKey.WEIGHTLIMIT_TRAP_WEIGHT_MAX:
					C_WeightLimitTrapWeightMax = (int)item.value1;
					break;
				case ConstDataKey.DEATHMATCH_ROOM_SPAWNABLE_ITEM_ID:
					C_DeathmatchRoomSpawnableItemId = (int)item.value1;
					break;
				case ConstDataKey.DEATHMATCH_ROOM_SPAWNABLE_ITEM_COUNT:
					C_DeathmatchRoomSpawnableItemCount = (int)item.value1;
					break;
				case ConstDataKey.DEATHMATCH_ROOM_SPAWNABLE_MONSTER_ID:
					C_DeathmatchRoomSpawnableMonsterId = (int)item.value1;
					break;
				case ConstDataKey.DEATHMATCH_ROOM_SPAWNABLE_MONSTER_COUNT:
					C_DeathmatchRoomSpawnableMonsterCount = (int)item.value1;
					break;
				case ConstDataKey.UI_POPUP_DURATION_DUNGEON_START_LEVER:
					C_UI_PopupDurationDungeonStartLever = (int)item.value1;
					break;
				case ConstDataKey.UI_POPUP_DURATION_SURVIVOR_REPORT:
					C_UI_PopupDurationSurvivorReport = (int)item.value1;
					break;
				case ConstDataKey.DEATHMATCH_ROOM_MVP_ITEM_ID:
					C_DeathmatchRoomMVPItemGroupId = (int)item.value1;
					break;
				case ConstDataKey.SPAWN_UNABLE_FROM_RANGE:
					C_SpawnUnableFromRange = (float)item.value1 * 0.01f;
					break;
				case ConstDataKey.MOVEMENT_AGGRO_CAPTURE_PERIOD:
					C_MovementAggroCapturePeriod = item.value1;
					break;
				case ConstDataKey.MOVEMENT_AGGRO_MIN_DISTANCE_THRESHOLD:
					C_MovementAggroMinDistanceThreshold = (float)item.value1 * 0.01f;
					break;
				case ConstDataKey.UI_POPUP_DURATION_DEATHMATCH_REPORT:
					C_UI_PopupDurationDeathmatchReport = (int)item.value1;
					break;
				case ConstDataKey.DEFAULT_WEATHER_MAINTENANCE:
					if (!Enum.TryParse<SkyAndWeatherSystem.eWeatherPreset>(item.valuestring, ignoreCase: true, out C_DefaultWeather_Maintenance))
					{
						Logger.RError("Invalid item.valuestring : " + item.key + ", " + item.valuestring);
					}
					break;
				case ConstDataKey.DEFAULT_WEATHER_IN_TRAM_WAITING:
					if (!Enum.TryParse<SkyAndWeatherSystem.eWeatherPreset>(item.valuestring, ignoreCase: true, out C_DefaultWeather_InTramWaiting))
					{
						Logger.RError("Invalid item.valuestring : " + item.key + ", " + item.valuestring);
					}
					break;
				case ConstDataKey.DEFAULT_WEATHER_DUNGEON:
					if (!Enum.TryParse<SkyAndWeatherSystem.eWeatherPreset>(item.valuestring, ignoreCase: true, out C_DefaultWeather_Dungeon))
					{
						Logger.RError("Invalid item.valuestring : " + item.key + ", " + item.valuestring);
					}
					break;
				case ConstDataKey.DEFAULT_WEATHER_DEATHMATCH:
					if (!Enum.TryParse<SkyAndWeatherSystem.eWeatherPreset>(item.valuestring, ignoreCase: true, out C_DefaultWeather_DeathMatch))
					{
						Logger.RError("Invalid item.valuestring : " + item.key + ", " + item.valuestring);
					}
					break;
				case ConstDataKey.RANDOMTELEPORT_END_RANGE_MIN:
					C_RandomTeleportEndRangeMin = (int)item.value1;
					break;
				case ConstDataKey.REPORT_MIN_VALUE_SCRAP_COUNT:
					C_ReportMinValueScrapCount = (int)item.value1;
					break;
				case ConstDataKey.REPORT_MIN_VALUE_FRIENDLY_ATTACK:
					C_ReportMinValueFriendlyAttack = (int)item.value1;
					break;
				case ConstDataKey.REPORT_MIN_VALUE_MET_MIMESIS:
					C_ReportMinValueMetMimesis = (int)item.value1;
					break;
				case ConstDataKey.REPORT_MIN_VALUE_STAY_TRAM:
					C_ReportMinValueStayTram = (int)item.value1;
					break;
				case ConstDataKey.FALL_DAMAGE_SKILL_TARGET_EFFECT_ID:
					C_FallDamageSkillTargetEffectId = (int)item.value1;
					break;
				case ConstDataKey.FALL_DEAD_SKILL_TARGET_EFFECT_ID:
					C_FallDeadSkillTargetEffectId = (int)item.value1;
					break;
				case ConstDataKey.RANDOMMATCHING_WAITING_TIME:
					C_RandomMatchingWaitingTime = (int)item.value1;
					break;
				case ConstDataKey.MAX_PLAYER_COUNT:
					C_MaxPlayerCount = (int)item.value1;
					break;
				case ConstDataKey.SCRAP_BOOST_VALUE:
					C_ScrapBoostValue = item.value1;
					break;
				case ConstDataKey.SCRAP_BOOST_MAX_SESSION:
					C_ScrapBoostMaxSession = (int)item.value1;
					break;
				default:
					Logger.RError("Invalid ConstDataKey : " + item.key);
					break;
				}
			}
		}
	}
}
