using System;
using System.Collections.Generic;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.Enum;

public class StatManager
{
	private Queue<VActorEventArgs> _deferedDamageEvents;

	private VCreature _self;

	private CommonStats _totalStats;

	private MappedStats _mappedStats;

	private EventStats _eventStats;

	private CommonStats _debugStats;

	private Dictionary<MutableStatType, StatsElement> _mutableStats = new Dictionary<MutableStatType, StatsElement>();

	private long _contaIncreaseIdleElapsed;

	private long _contaIncreaseRunElapsed;

	private long _groggyDecayElapsed;

	private long _staminaRegenElapsed;

	private bool _sprintRecentlyEnded;

	private long _soundResonanceOccuredTime;

	private bool _isSprintMode;

	private Dictionary<StatType, bool> _totalStatCollected;

	private Queue<MutableStatChangeInfo> _mutableStatChangeInfoQueue = new Queue<MutableStatChangeInfo>();

	private bool _deferedDying;

	private ApplyDamageArgs? _deferedDyingReserved;

	private GroggyState _groggyState;

	private int _moveSpeedDecreaseRateByWeight;

	private int _pendedMoveSpeedDecreaseRateByWeight;

	private AbnormalController? _abnormalController;

	public long LastDamageElapsed { get; private set; }

	public StatManager(VCreature self)
	{
		_self = self;
		_totalStats = new CommonStats(StatCategory.Total);
		_mappedStats = new MappedStats();
		_eventStats = new EventStats();
		_debugStats = new CommonStats(StatCategory.Cheat);
		foreach (MutableStatType value in Enum.GetValues(typeof(MutableStatType)))
		{
			_mutableStats[value] = new StatsElement(0L);
		}
		_deferedDamageEvents = new Queue<VActorEventArgs>();
		_deferedDying = false;
		_deferedDyingReserved = null;
		_totalStatCollected = new Dictionary<StatType, bool>();
	}

	public bool Initialize()
	{
		return true;
	}

	public void WaitInitDone()
	{
		_abnormalController = _self.AbnormalControlUnit;
	}

	public void InitMutableStats()
	{
		FillHP(0L, full: true);
		ChargeStamina(GetSpecificStatValue(StatType.Stamina));
		long contaValue = _self.VRoom.GetContaValue(_self);
		IncreaseConta(contaValue);
	}

	public bool LoadAllStat(bool reload = false)
	{
		LoadMappedStat();
		LoadEventStats();
		LoadImmutableStats();
		InitMutableStats();
		return true;
	}

	public CommonStats GetStats()
	{
		return _totalStats;
	}

	public long GetSpecificStatValue(StatType type)
	{
		return _totalStats.GetStatValue(type);
	}

	public long GetCurrentHP()
	{
		return _mutableStats[MutableStatType.HP].Value;
	}

	public long GetCurrentVoiceResonance()
	{
		return _mutableStats[MutableStatType.VoiceResonance].Value;
	}

	public long GetMoveSpeed(ActorMoveType moveType)
	{
		switch (moveType)
		{
		case ActorMoveType.Walk:
			return GetSpecificStatValue(StatType.MoveSpeedWalk);
		case ActorMoveType.Run:
			return GetSpecificStatValue(StatType.MoveSpeedRun);
		case ActorMoveType.Attached:
			return GetSpecificStatValue(StatType.MoveSpeedWalk);
		case ActorMoveType.FallDown:
			return 0L;
		default:
			Logger.RWarn("Invalid MoveType");
			return 0L;
		}
	}

	public void GetFullStats(ref StatCollection info)
	{
		foreach (StatType value in Enum.GetValues(typeof(StatType)))
		{
			if (value != StatType.Invalid)
			{
				info.ImmutableStats[value] = _totalStats.GetStatValue(value);
			}
		}
		foreach (MutableStatType value2 in Enum.GetValues(typeof(MutableStatType)))
		{
			if (value2 != MutableStatType.Invalid)
			{
				info.mutableStats[value2] = _mutableStats[value2].Value;
			}
		}
	}

	public void LoadMappedStat()
	{
		_mappedStats.ClearStats();
		_mappedStats.LoadBaseStats(_self.ActorType, _self.MasterID);
		if (_self.InventoryControlUnit != null)
		{
			List<EquipmentItemElement> allEquipItems = _self.InventoryControlUnit.GetAllEquipItems();
			_mappedStats.LoadEquipStats(allEquipItems);
		}
		_mappedStats.CollectDirtyStats();
	}

	public void LoadEventStats()
	{
		_eventStats.ClearStats();
		_eventStats.RefreshStats();
	}

	public void SetSprintMode(bool flag)
	{
		_isSprintMode = flag;
		if (!flag)
		{
			_contaIncreaseIdleElapsed = 0L;
			_sprintRecentlyEnded = true;
		}
		else if (_self.VRoom is DungeonRoom)
		{
			_contaIncreaseRunElapsed = 0L;
		}
	}

	public void ApplyDamage(ApplyDamageArgs args)
	{
		_deferedDamageEvents.Enqueue(args);
	}

	private void OnGroggyValue(long value)
	{
		if (_self is VMonster && _groggyState == GroggyState.Normal && AddMutableStat(MutableStatType.AbnormalTriggerGauge, value) && _mutableStats[MutableStatType.AbnormalTriggerGauge].Value >= GetSpecificStatValue(StatType.AbnormalTriggerGauge))
		{
			_groggyState = GroggyState.GroggyActivated;
			MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(_self.MasterID);
			if (monsterInfo != null)
			{
				_abnormalController?.AppendAbnormal(_self.ObjectID, monsterInfo.AbnormalTriggerID);
			}
		}
	}

	private void OnDamaged(ApplyDamageArgs args)
	{
		if (args.GrogyValue > 0)
		{
			OnGroggyValue(args.GrogyValue);
		}
		long num = args.Damage;
		if (_deferedDyingReserved == null)
		{
			_ = args.MutableStatChangeCause;
			_self.AIControlUnit?.OnDamaged(args);
			if (_self is VMonster vMonster)
			{
				vMonster.OnDamaged();
			}
			else if (_self is VPlayer && args.Attacker != null && args.Attacker is VPlayer actor)
			{
				_self.VRoom.IncreaseDamageToAlly(actor, num);
			}
			LastDamageElapsed = 0L;
			if (_deferedDying && _mutableStats[MutableStatType.HP].Value <= args.Damage)
			{
				num = _mutableStats[MutableStatType.HP].Value - 1;
				_deferedDyingReserved = args;
			}
			if (num != 0L && AddMutableStat(MutableStatType.HP, -num, args.Attacker, args) && _mutableStats[MutableStatType.HP].Value <= 0)
			{
				_self.MoveToDying(args);
			}
		}
	}

	public void OnUpdate(long delta)
	{
		LastDamageElapsed += delta;
		if (_self is VPlayer)
		{
			UpdateConta(delta);
			RegenerateStamina(delta);
		}
		ApplyDeferedDamage();
		LoadImmutableStats();
		SyncMutableStats();
		SyncImmutableStats();
		HandleGroggyState(delta);
		DecideSoundResonanceReset();
	}

	private void HandleGroggyState(long delta)
	{
		switch (_groggyState)
		{
		case GroggyState.GroggyActivated:
			_groggyState = GroggyState.Recovery;
			_groggyDecayElapsed = 0L;
			break;
		case GroggyState.Recovery:
			_groggyDecayElapsed += delta;
			if (_groggyDecayElapsed <= Hub.s.dataman.ExcelDataManager.Consts.C_GroggyThresholdConsumePeriod)
			{
				break;
			}
			_groggyDecayElapsed = 0L;
			if (AddMutableStat(MutableStatType.AbnormalTriggerGauge, -Hub.s.dataman.ExcelDataManager.Consts.C_GroggyThresholdConsumeValue))
			{
				if (_mutableStats[MutableStatType.AbnormalTriggerGauge].Value == 0L)
				{
					_groggyState = GroggyState.Normal;
				}
				MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(_self.MasterID);
				if (monsterInfo != null)
				{
					_abnormalController?.DispelAbnormal(_self.ObjectID, monsterInfo.AbnormalTriggerID, force: true);
				}
			}
			break;
		}
	}

	private void SyncMutableStats()
	{
		foreach (MutableStatType value2 in Enum.GetValues(typeof(MutableStatType)))
		{
			if (!_mutableStats[value2].IsDirty)
			{
				continue;
			}
			AnnounceMutableStatSig announceMutableStatSig = new AnnounceMutableStatSig
			{
				actorID = _self.ObjectID,
				type = value2,
				remainValue = _mutableStats[value2].Value
			};
			if (IsMutableStatNotDefinedStat(value2))
			{
				if (value2 == MutableStatType.Conta)
				{
					announceMutableStatSig.maxValue = Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue;
				}
			}
			else
			{
				StatType statType = Hub.s.dataman.ExcelDataManager.toImmutableStatType(value2);
				if (statType == StatType.Invalid)
				{
					continue;
				}
				if (!Hub.s.dataman.ExcelDataManager.MutableStatInfos.TryGetValue(statType, out MutableStatInfo value))
				{
					_mutableStats[value2].Sync();
					continue;
				}
				announceMutableStatSig.maxValue = GetSpecificStatValue(value.StatType);
			}
			if (Hub.s.dataman.ExcelDataManager.OtherActorSyncMutableStats.Contains(value2))
			{
				_self.SendInSight(announceMutableStatSig, includeSelf: true);
			}
			else
			{
				_self.SendToMe(announceMutableStatSig);
			}
			_mutableStats[value2].Sync();
		}
	}

	public void SyncImmutableStats()
	{
		if (_totalStats.AlteredStatTypes.Count == 0)
		{
			return;
		}
		if (!_self.InGrid())
		{
			_totalStats.OnSyncComplete();
			return;
		}
		SyncImmutableStatSig syncImmutableStatSig = new SyncImmutableStatSig();
		AnnounceImmutableStatSig announceImmutableStatSig = new AnnounceImmutableStatSig
		{
			actorID = _self.ObjectID
		};
		foreach (StatType alteredStatType in _totalStats.AlteredStatTypes)
		{
			long statValue = _totalStats.GetStatValue(alteredStatType);
			syncImmutableStatSig.ImmutableStats.Add(alteredStatType, statValue);
			if (Hub.s.dataman.ExcelDataManager.OtherActorSyncStats.Contains(alteredStatType))
			{
				announceImmutableStatSig.ImmutableStats.Add(alteredStatType, statValue);
			}
		}
		_self.SendInSight(announceImmutableStatSig);
		_self.SendToMe(syncImmutableStatSig);
		_totalStats.OnSyncComplete();
		_self.MovementControlUnit?.OnChangeImmutableStats();
	}

	public void LoadImmutableStats()
	{
		_eventStats.RefreshStats();
		if (_mappedStats.AlteredStatTypes.Count != 0 || _eventStats.AlteredStatTypes.Count != 0 || _debugStats.AlteredStatTypes.Count != 0 || _pendedMoveSpeedDecreaseRateByWeight != _moveSpeedDecreaseRateByWeight)
		{
			CalcImmutableStats();
			ProcessMutableStatChange();
			_totalStats.CollectDirtyStats();
			_mappedStats.OnSyncComplete();
			_eventStats.OnSyncComplete();
			_debugStats.OnSyncComplete();
		}
	}

	private void ProcessMutableStatChange()
	{
		Dictionary<MutableStatType, long> dictionary = new Dictionary<MutableStatType, long>();
		MutableStatChangeInfo result;
		while (_mutableStatChangeInfoQueue.TryDequeue(out result))
		{
			if (!dictionary.TryGetValue(result.MutableStatType, out var value))
			{
				value = _mutableStats[result.MutableStatType].Value;
			}
			switch (result.MethodType)
			{
			case MutableStatChangeMethodType.Add:
				if (result.OldImmutableValue < result.NewImmutableValue)
				{
					value += result.NewImmutableValue - result.OldImmutableValue;
				}
				else if (result.NewImmutableValue < value)
				{
					value = result.NewImmutableValue;
				}
				break;
			case MutableStatChangeMethodType.Multiply:
				if (result.OldImmutableValue == 0L)
				{
					continue;
				}
				value = value * result.NewImmutableValue / result.OldImmutableValue;
				break;
			}
			dictionary[result.MutableStatType] = value;
		}
		foreach (KeyValuePair<MutableStatType, long> item in dictionary)
		{
			AddMutableStat(item.Key, item.Value - _mutableStats[item.Key].Value);
		}
	}

	private void ApplyDeferedDamage()
	{
		VActorEventArgs result;
		while (_deferedDamageEvents.TryDequeue(out result))
		{
			if (_self.IsAliveStatus() && GetCurrentHP() >= 0)
			{
				if (result is ApplyDamageArgs args)
				{
					OnDamaged(args);
				}
				if (!_deferedDying && _deferedDyingReserved != null)
				{
					ApplyDamageArgs deferedDyingReserved = _deferedDyingReserved;
					_deferedDyingReserved = null;
					AddMutableStat(MutableStatType.HP, _mutableStats[MutableStatType.HP].Value, deferedDyingReserved.Attacker);
					_self.MoveToDying(deferedDyingReserved);
				}
			}
		}
	}

	private void CalcImmutableStats()
	{
		_totalStatCollected.Clear();
		foreach (StatType alteredStatType in _mappedStats.AlteredStatTypes)
		{
			if (!_totalStatCollected.ContainsKey(alteredStatType))
			{
				AccumulateStat(alteredStatType);
				_totalStatCollected.Add(alteredStatType, value: true);
			}
		}
		foreach (StatType alteredStatType2 in _eventStats.AlteredStatTypes)
		{
			if (!_totalStatCollected.ContainsKey(alteredStatType2))
			{
				AccumulateStat(alteredStatType2);
				_totalStatCollected.Add(alteredStatType2, value: true);
			}
		}
		foreach (StatType alteredStatType3 in _debugStats.AlteredStatTypes)
		{
			if (!_totalStatCollected.ContainsKey(alteredStatType3))
			{
				AccumulateStat(alteredStatType3);
				_totalStatCollected.Add(alteredStatType3, value: true);
			}
		}
		if (_pendedMoveSpeedDecreaseRateByWeight != _moveSpeedDecreaseRateByWeight)
		{
			AccumulateStat(StatType.MoveSpeedWalk);
			AccumulateStat(StatType.MoveSpeedRun);
			_moveSpeedDecreaseRateByWeight = _pendedMoveSpeedDecreaseRateByWeight;
			_totalStats.elements[StatType.MoveSpeedWalk].Set((long)((double)(GetSpecificStatValue(StatType.MoveSpeedWalk) * (10000 - _moveSpeedDecreaseRateByWeight)) * 0.0001));
			_totalStats.elements[StatType.MoveSpeedRun].Set((long)((double)(GetSpecificStatValue(StatType.MoveSpeedRun) * (10000 - _moveSpeedDecreaseRateByWeight)) * 0.0001));
		}
		_totalStats.elements[StatType.Conta].Set(Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue);
	}

	public void AccumulateStat(StatType type, MutableStatChangeMethodType changeType = MutableStatChangeMethodType.Add)
	{
		long statValue = _totalStats.GetStatValue(type);
		_totalStats.elements[type].Set(_mappedStats.GetStatValue(type) + _eventStats.GetStatValue(type) + _debugStats.GetStatValue(type));
		long statValue2 = _totalStats.GetStatValue(type);
		if (statValue != statValue2)
		{
			OnChangeImmutableStat(type, statValue, statValue2, changeType);
		}
	}

	public void SetStatSpecificValue(StatType type, long oldVal, long newVal, MutableStatChangeMethodType changeType)
	{
		_totalStats.elements[type].Set(newVal);
		if (oldVal != newVal)
		{
			OnChangeImmutableStat(type, oldVal, newVal, changeType);
		}
	}

	public void FillHP(long hp, bool full = false)
	{
		if (full)
		{
			VActorEventArgs result;
			while (_deferedDamageEvents.TryDequeue(out result))
			{
			}
			_mutableStats[MutableStatType.HP].Set(GetSpecificStatValue(StatType.HP));
		}
		else
		{
			_mutableStats[MutableStatType.HP].Set(Math.Min(hp + _mutableStats[MutableStatType.HP].Value, GetSpecificStatValue(StatType.HP)));
		}
	}

	private bool IsMutableStatNotDefinedStat(MutableStatType type)
	{
		if (type != MutableStatType.Conta && type != MutableStatType.AggroGauge)
		{
			return type == MutableStatType.VoiceResonance;
		}
		return true;
	}

	private bool ValidateAndClampMutableStat(MutableStatType type, ref long value, out bool isValidOperation)
	{
		isValidOperation = true;
		if (value < 0)
		{
			value = 0L;
		}
		if (type == MutableStatType.VoiceResonance)
		{
			return true;
		}
		if (!(_self is VPlayer) && type == MutableStatType.Conta)
		{
			isValidOperation = false;
			return false;
		}
		if (IsMutableStatNotDefinedStat(type))
		{
			if (type == MutableStatType.Conta && value > Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue)
			{
				value = Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue;
			}
			return true;
		}
		StatType statType = Hub.s.dataman.ExcelDataManager.toImmutableStatType(type);
		if (statType == StatType.Invalid)
		{
			isValidOperation = false;
			return false;
		}
		if (!Hub.s.dataman.ExcelDataManager.MutableStatInfos.TryGetValue(statType, out MutableStatInfo value2))
		{
			return true;
		}
		long specificStatValue = GetSpecificStatValue(value2.StatType);
		if (value > specificStatValue)
		{
			value = specificStatValue;
		}
		return true;
	}

	private void ApplyMutableStatChange(MutableStatType type, long newValue, VActor? occuredActor, VActorEventArgs? args)
	{
		if (newValue != _mutableStats[type].Value)
		{
			long value = _mutableStats[type].Value;
			_mutableStats[type].Set(newValue);
			OnChangeMutableStatValue(type, value, newValue, occuredActor, args);
		}
	}

	public void SetMutableStat(MutableStatType type, long value, VActor? occuredActor = null, VActorEventArgs? args = null)
	{
		if (ValidateAndClampMutableStat(type, ref value, out var _))
		{
			ApplyMutableStatChange(type, value, occuredActor, args);
		}
	}

	public bool AddMutableStat(MutableStatType type, long delta, VActor? occuredActor = null, VActorEventArgs? args = null)
	{
		long value = _mutableStats[type].Value + delta;
		if (!ValidateAndClampMutableStat(type, ref value, out var isValidOperation))
		{
			return !isValidOperation;
		}
		ApplyMutableStatChange(type, value, occuredActor, args);
		return true;
	}

	private void OnChangeMutableStatValue(MutableStatType type, long oldVal, long newVal, VActor? occuredActor, VActorEventArgs? args)
	{
		switch (type)
		{
		case MutableStatType.HP:
			if (_mutableStats[MutableStatType.HP].Value <= 0)
			{
				if (args != null && args is ApplyDamageArgs args2)
				{
					_self.MoveToDying(args2);
				}
				else
				{
					_self.MoveToDying(new ApplyDamageArgs(occuredActor, _self, MutableStatChangeCause.SystemNormal, 0L, 0L));
				}
			}
			break;
		case MutableStatType.Conta:
			if (IsContaExceeded())
			{
				_mutableStats[MutableStatType.HP].Set(0L);
				SyncMutableStats();
				_self.MoveToDying(new ApplyDamageArgs(null, _self, MutableStatChangeCause.Conta, 0L, 0L));
				_self.VRoom.ReserveCreateMonster(Hub.s.dataman.ExcelDataManager.Consts.C_WaitTimeContaSpawnMimic, _self.Position, 20000001, _self.IsIndoor, ReasonOfSpawn.ActorDying);
			}
			break;
		}
	}

	private void OnChangeImmutableStat(StatType type, long oldVal, long newVal, MutableStatChangeMethodType changeType, VActor? occuredActor = null)
	{
		if (!Hub.s.dataman.ExcelDataManager.MutableStatInfos.TryGetValue(type, out MutableStatInfo value) || value == MutableStatInfo.InvalidMutableStat)
		{
			return;
		}
		if (value.IsNoAffectMutableStat)
		{
			if (_mutableStats[value.MutableStatType].Value - newVal > 0)
			{
				AddMutableStat(value.MutableStatType, -(_mutableStats[value.MutableStatType].Value - newVal), occuredActor);
			}
		}
		else
		{
			_mutableStatChangeInfoQueue.Enqueue(new MutableStatChangeInfo(value.MutableStatType, changeType, oldVal, newVal));
		}
	}

	public bool IsHPFull()
	{
		return GetSpecificStatValue(StatType.HP) <= GetCurrentHP();
	}

	public bool IsContaExceeded()
	{
		if (_self.VRoom is MaintenanceRoom)
		{
			return false;
		}
		return _mutableStats[MutableStatType.Conta].Value >= Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue;
	}

	public void UpdateConta(long delta)
	{
		if (!_self.IsAliveStatus())
		{
			return;
		}
		_contaIncreaseIdleElapsed += delta;
		if (_contaIncreaseIdleElapsed > Hub.s.dataman.ExcelDataManager.Consts.C_IdleContaIncreasePeriod)
		{
			int contaValue = _self.VRoom.GetContaValue(_self, isRun: false);
			IncreaseConta(contaValue);
			_contaIncreaseIdleElapsed = 0L;
		}
		if (!_isSprintMode)
		{
			return;
		}
		MovementController? movementControlUnit = _self.MovementControlUnit;
		if (movementControlUnit != null && !movementControlUnit.IsMoving)
		{
			_contaIncreaseRunElapsed = 0L;
			return;
		}
		_contaIncreaseRunElapsed += delta;
		if (_contaIncreaseRunElapsed > Hub.s.dataman.ExcelDataManager.Consts.C_RunContaIncreasePeriod)
		{
			int contaValue2 = _self.VRoom.GetContaValue(_self, isRun: true);
			IncreaseConta(contaValue2);
			_contaIncreaseRunElapsed = 0L;
		}
	}

	private void IncreaseConta(int value)
	{
		AddMutableStat(MutableStatType.Conta, value);
	}

	public void GetMutableStat(ref StatCollection collection)
	{
		foreach (MutableStatType value in Enum.GetValues(typeof(MutableStatType)))
		{
			collection.mutableStats.Add(value, _mutableStats[value].Value);
		}
	}

	public void GetSimpleStat(ref SimpleStatInfo simpleStatInfo)
	{
		foreach (StatType otherActorSyncStat in Hub.s.dataman.ExcelDataManager.OtherActorSyncStats)
		{
			simpleStatInfo.ImmutableStats.Add(otherActorSyncStat, GetSpecificStatValue(otherActorSyncStat));
		}
		foreach (MutableStatType otherActorSyncMutableStat in Hub.s.dataman.ExcelDataManager.OtherActorSyncMutableStats)
		{
			simpleStatInfo.MutableStats.Add(otherActorSyncMutableStat, _mutableStats[otherActorSyncMutableStat].Value);
		}
	}

	public long GetMutableStatPercent(MutableStatType type)
	{
		StatType statType = Hub.s.dataman.ExcelDataManager.toImmutableStatType(type);
		if (statType == StatType.Invalid)
		{
			return 0L;
		}
		if (!Hub.s.dataman.ExcelDataManager.MutableStatInfos.TryGetValue(statType, out MutableStatInfo value))
		{
			return 0L;
		}
		return (long)Math.Round(10000f * (float)(_mutableStats[type].Value / GetSpecificStatValue(value.StatType)));
	}

	private bool IsMutableStatFullCharged(MutableStatType type)
	{
		StatType statType = Hub.s.dataman.ExcelDataManager.toImmutableStatType(type);
		if (statType == StatType.Invalid)
		{
			return false;
		}
		if (!Hub.s.dataman.ExcelDataManager.MutableStatInfos.TryGetValue(statType, out MutableStatInfo value))
		{
			return false;
		}
		return _mutableStats[type].Value >= GetSpecificStatValue(value.StatType);
	}

	public void SetDeferedDying(bool flag)
	{
		_deferedDying = flag;
	}

	public long GetCurrentConta()
	{
		return _mutableStats[MutableStatType.Conta].Value;
	}

	public void RecoverConta(long amount)
	{
		AddMutableStat(MutableStatType.Conta, -amount);
	}

	public void IncreaseConta(long amount)
	{
		AddMutableStat(MutableStatType.Conta, amount);
	}

	public bool IsAllMutableStatFullCharged(List<MutableStatType> statTypes)
	{
		foreach (MutableStatType statType in statTypes)
		{
			if (!IsMutableStatFullCharged(statType))
			{
				return false;
			}
		}
		return true;
	}

	public Dictionary<StatType, long> GetImmutableTotalStats()
	{
		Dictionary<StatType, long> dictionary = new Dictionary<StatType, long>();
		foreach (StatType value in Enum.GetValues(typeof(StatType)))
		{
			if (value != StatType.Invalid)
			{
				dictionary.Add(value, GetSpecificStatValue(value));
			}
		}
		return dictionary;
	}

	public void ResetLastDamageElapsedTime()
	{
		LastDamageElapsed = 0L;
	}

	public void SetMoveSpeedDecreaseRateByWeight(int rate)
	{
		_pendedMoveSpeedDecreaseRateByWeight = rate;
	}

	private long ApplyAbnormalModifier(AbnormalStatsCategory category, StatModifyType modifyType, MutableStatType mutableStatType, StatType statType, long value)
	{
		long num = 0L;
		switch (category)
		{
		case AbnormalStatsCategory.MutableStat:
			return (modifyType == StatModifyType.Percent || modifyType == StatModifyType.MultiCustomPercent) ? ((long)Math.Round(0.0001 * (double)value * (double)_mutableStats[mutableStatType].Value)) : value;
		case AbnormalStatsCategory.ImmutableStat:
			return (modifyType == StatModifyType.Percent || modifyType == StatModifyType.MultiCustomPercent) ? ((long)Math.Round(0.0001 * (double)value * (double)GetSpecificStatValue(statType))) : value;
		default:
			Logger.RError($"Invalid AbnormalStatsCategory: {category}");
			return 0L;
		}
	}

	private void OnChangeAbnormalStat(AddImmutableStatsAbnormalArgs addArgs)
	{
		AbnormalController? abnormalController = _abnormalController;
		if (abnormalController == null || !abnormalController.IsStatsImmune(addArgs.StatType))
		{
			long value = ApplyAbnormalModifier(AbnormalStatsCategory.ImmutableStat, addArgs.ModifyType, MutableStatType.Invalid, addArgs.StatType, addArgs.Value);
			_eventStats.AddAbnormalStats(addArgs.SyncID, addArgs.StatType, value, addArgs.Index);
		}
	}

	private void OnChangeAbnormalStat(UpdateImmutableStatsAbnormalArgs updateArgs)
	{
		long value = ApplyAbnormalModifier(AbnormalStatsCategory.ImmutableStat, updateArgs.ModifyType, MutableStatType.Invalid, updateArgs.StatType, updateArgs.Value);
		_eventStats.UpdateAbnormalStats(updateArgs.SyncID, value, updateArgs.Index);
	}

	private void OnChangeAbnormalStat(RemoveImmutableStatsAbnormalArgs removeArgs)
	{
		_eventStats.RemoveAbnormalStats(removeArgs.SyncID);
	}

	private void OnHealed(ApplyHealArgs args)
	{
		if (_deferedDyingReserved == null)
		{
			AddMutableStat(MutableStatType.HP, args.HealAmount, args.Healer);
		}
	}

	private void OnChangeAbnormalStat(ApplyMutableStatsAbnormalArgs mutableArgs)
	{
		long num = ApplyAbnormalModifier(AbnormalStatsCategory.MutableStat, mutableArgs.ModifyType, mutableArgs.StatType, StatType.Invalid, mutableArgs.Value);
		VActor vActor = _self.VRoom.FindActorByObjectID(mutableArgs.CasterActorID);
		if (mutableArgs.StatType == MutableStatType.HP)
		{
			if (!_self.IsAliveStatus() || vActor == null)
			{
				return;
			}
			AbnormalController? abnormalController = _abnormalController;
			if (abnormalController != null && abnormalController.IsImmortal())
			{
				return;
			}
			if (mutableArgs.Value > 0)
			{
				if (IsHPFull())
				{
					return;
				}
				OnHealed(new ApplyHealArgs(vActor, _self, MutableStatChangeCause.AbnormalHeal, num));
			}
			OnDamaged(new ApplyDamageArgs(vActor, _self, MutableStatChangeCause.AbnormalDamage, -num, 0L));
		}
		else
		{
			AddMutableStat(mutableArgs.StatType, num, vActor);
		}
	}

	public void OnChangeAbnormalStat(VActorEventArgs? args)
	{
		if (args == null)
		{
			return;
		}
		if (!(args is AddImmutableStatsAbnormalArgs addArgs))
		{
			if (!(args is UpdateImmutableStatsAbnormalArgs updateArgs))
			{
				if (!(args is RemoveImmutableStatsAbnormalArgs removeArgs))
				{
					if (args is ApplyMutableStatsAbnormalArgs mutableArgs)
					{
						OnChangeAbnormalStat(mutableArgs);
					}
				}
				else
				{
					OnChangeAbnormalStat(removeArgs);
				}
			}
			else
			{
				OnChangeAbnormalStat(updateArgs);
			}
		}
		else
		{
			OnChangeAbnormalStat(addArgs);
		}
	}

	public long GetCurrentStamina()
	{
		return _mutableStats[MutableStatType.Stamina].Value;
	}

	public void ConsumeStamina(long amount)
	{
		AddMutableStat(MutableStatType.Stamina, -amount);
	}

	public void ChargeStamina(long amount)
	{
		AddMutableStat(MutableStatType.Stamina, amount);
	}

	public bool IsStaminaFull()
	{
		return GetSpecificStatValue(StatType.Stamina) <= _mutableStats[MutableStatType.Stamina].Value;
	}

	public void RegenerateStamina(long delta)
	{
		if (!_self.IsAliveStatus())
		{
			return;
		}
		if (Hub.s.dataman.ExcelDataManager.Consts.C_StaminaRegenPeriod > 0 && !IsStaminaFull() && Hub.s.dataman.ExcelDataManager.Consts.C_StaminaRegenValue != 0L)
		{
			MovementController? movementControlUnit = _self.MovementControlUnit;
			if (movementControlUnit == null || !movementControlUnit.IsSprint || !_self.MovementControlUnit.IsMoving)
			{
				_staminaRegenElapsed += delta;
				long num = ((GetCurrentStamina() == 0L) ? Hub.s.dataman.ExcelDataManager.Consts.C_StaminaRegenDelayEmpty : Hub.s.dataman.ExcelDataManager.Consts.C_StaminaRegenDelayRemain);
				if (!_sprintRecentlyEnded || _staminaRegenElapsed >= num)
				{
					_sprintRecentlyEnded = false;
					if (_staminaRegenElapsed >= Hub.s.dataman.ExcelDataManager.Consts.C_StaminaRegenPeriod)
					{
						_staminaRegenElapsed = 0L;
						ChargeStamina(Hub.s.dataman.ExcelDataManager.Consts.C_StaminaRegenValue);
					}
				}
				return;
			}
		}
		_staminaRegenElapsed = 0L;
	}

	public void SetVoiceResonance(long value)
	{
		if (value > 0)
		{
			_soundResonanceOccuredTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		}
		else
		{
			_soundResonanceOccuredTime = 0L;
		}
		SetMutableStat(MutableStatType.VoiceResonance, value);
	}

	public void DecideSoundResonanceReset()
	{
		if (_mutableStats[MutableStatType.VoiceResonance].Value != 0L && _soundResonanceOccuredTime != 0L && Hub.s.timeutil.GetCurrentTickMilliSec() - _soundResonanceOccuredTime > Hub.s.dataman.ExcelDataManager.Consts.C_NoAggroDuration)
		{
			SetVoiceResonance(0L);
		}
	}

	public void AddDebugStat(StatType type, long value)
	{
		if (type != StatType.Invalid)
		{
			_debugStats.elements[type].Add(value);
			_debugStats.CollectDirtyStats();
		}
	}

	public void ResetDebugStats()
	{
		foreach (StatType value in Enum.GetValues(typeof(StatType)))
		{
			if (value != StatType.Invalid)
			{
				_debugStats.elements[value].Set(0L);
			}
		}
		_debugStats.CollectDirtyStats();
	}
}
