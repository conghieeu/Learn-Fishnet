using System;
using System.Collections.Generic;
using ReluProtocol;
using ReluProtocol.Enum;

public class StatController : IVActorController, IDisposable
{
	private readonly StatManager StatManager;

	public VActorControllerType type { get; } = VActorControllerType.Stat;

	public VCreature Self { get; private set; }

	public StatController(VCreature creature)
	{
		Self = creature;
		StatManager = new StatManager(creature);
	}

	public void Initialize()
	{
		StatManager.Initialize();
	}

	public void OnDeath()
	{
	}

	public void WaitInitDone()
	{
		StatManager.WaitInitDone();
		LoadStats(reload: true);
	}

	public void Update(long deltaTime)
	{
		StatManager.OnUpdate(deltaTime);
		if (Self is VPlayer vPlayer)
		{
			ApplyVoiceResonance(vPlayer.GetVoicePeak());
		}
	}

	public bool LoadStats(bool reload)
	{
		StatManager.LoadAllStat(reload);
		return true;
	}

	public void ApplyDamage(ApplyDamageArgs args)
	{
		if (Self.IsAliveStatus())
		{
			StatManager.ApplyDamage(args);
		}
	}

	public void AdjustHP(long delta, bool full = false)
	{
		StatManager.SetMutableStat(MutableStatType.HP, full ? GetSpecificStatValue(StatType.HP) : (GetCurrentHP() + delta));
	}

	public void AdjustConta(int percent)
	{
		StatManager.SetMutableStat(MutableStatType.Conta, (long)((double)(Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue * percent) * 0.01));
	}

	public DamageAttribute EstimateDamage(VActor? attacker, MutableStatChangeCause cause, int skillSeqID)
	{
		StatManager.GetStats();
		if (cause != MutableStatChangeCause.ActiveAttack)
		{
			return new DamageAttribute(0L, 0L, critical: false);
		}
		return VWorldCombatUtil.CalculateDamage(Self, attacker, skillSeqID);
	}

	public CommonStats GetStatInfo()
	{
		return StatManager.GetStats();
	}

	public long GetSpecificStatValue(StatType type)
	{
		return StatManager.GetSpecificStatValue(type);
	}

	public bool GetStatCollection(ref StatCollection statCollection)
	{
		StatManager.LoadImmutableStats();
		StatManager.GetFullStats(ref statCollection);
		return true;
	}

	public bool GetSimpleStatValue(ref SimpleStatInfo simpleStatInfo)
	{
		StatManager.GetSimpleStat(ref simpleStatInfo);
		return true;
	}

	public long GetCurrentHP()
	{
		return StatManager.GetCurrentHP();
	}

	public float GetMoveSpeed()
	{
		ActorMoveType actorMoveType = Self.MovementControlUnit?.GetMoveType() ?? ActorMoveType.None;
		if (actorMoveType == ActorMoveType.None)
		{
			actorMoveType = ActorMoveType.Walk;
		}
		return (float)StatManager.GetMoveSpeed(actorMoveType) * 0.01f;
	}

	public void InstantChargeHP(long value, bool full = true)
	{
		StatManager.FillHP(value, full);
	}

	public bool IsHPFull()
	{
		return StatManager.IsHPFull();
	}

	public void OnEnterChannel()
	{
	}

	public int AddPartsStats(int partsMasterID, long amount)
	{
		return 0;
	}

	public void SyncChangedImmutableStats()
	{
		StatManager.SyncImmutableStats();
	}

	public long GetMutableStatPercent(MutableStatType statType)
	{
		return StatManager.GetMutableStatPercent(statType);
	}

	public long GetLastDamageElapsedTime()
	{
		return StatManager.LastDamageElapsed;
	}

	public void SetDeferedDying(bool enabled)
	{
		StatManager.SetDeferedDying(enabled);
	}

	public long GetCurrentConta()
	{
		return StatManager.GetCurrentConta();
	}

	public void RecoverConta(long amount)
	{
		StatManager.RecoverConta(amount);
	}

	public void IncreaseConta(long amount)
	{
		StatManager.IncreaseConta(amount);
	}

	public void SetSprintMode(bool flag)
	{
		StatManager.SetSprintMode(flag);
	}

	public bool IsAllMutableStatsFullCharged(List<MutableStatType> statTypes)
	{
		return StatManager.IsAllMutableStatFullCharged(statTypes);
	}

	public Dictionary<StatType, long> GetImmutableTotalStats()
	{
		return StatManager.GetImmutableTotalStats();
	}

	public void PostUpdate(long deltaTime)
	{
	}

	public void Dispose()
	{
		StatManager.ResetLastDamageElapsedTime();
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		return MsgErrorCode.Success;
	}

	public void SetMoveSpeedDecreaseRateByWeight(int rate)
	{
		StatManager.SetMoveSpeedDecreaseRateByWeight(rate);
	}

	public void OnChangeAbnormalStats(VActorEventArgs args)
	{
		StatManager.OnChangeAbnormalStat(args);
	}

	public long GetCurrentStamina()
	{
		return StatManager.GetCurrentStamina();
	}

	public void ConsumeStamina(long amount)
	{
		StatManager.ConsumeStamina(amount);
	}

	public void RecoverStamina(long amount)
	{
		StatManager.ChargeStamina(amount);
	}

	public long GetCurrentVoiceResonance()
	{
		return StatManager.GetCurrentVoiceResonance();
	}

	public void ApplyVoiceResonance(long amount)
	{
		StatManager.SetVoiceResonance(amount);
	}

	public string GetDebugString()
	{
		return $"CurrentHP : {StatManager.GetCurrentHP()}";
	}

	public void AddDebugStat(StatType type, long value)
	{
		StatManager.AddDebugStat(type, value);
	}

	public void ResetDebugStat()
	{
		StatManager.ResetDebugStats();
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}
}
