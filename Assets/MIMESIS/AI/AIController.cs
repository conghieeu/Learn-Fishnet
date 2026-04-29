using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using DLAgent;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;
using UnityEngine.AI;

public class AIController : IVActorController, IDisposable
{
	private VCreature _creature;

	public long PauseTime;

	private BehaviorTreeState? _btState;

	private BTTargetPickRule _pickRule;

	private AIRangeType _aiRangeType;

	private Dictionary<int, long> _btNodeCoolTime = new Dictionary<int, long>();

	private Dictionary<int, AggroObject> m_AggroDict = new Dictionary<int, AggroObject>();

	private int _candidateFactionTier;

	private AggroRemoveCause _lastAggroRemoveCause;

	private long _stateElapsedTime;

	private long _elpasedDelta;

	private Vector3 _battleStartPosition;

	private DLMovementAgent? _dlMovementAgent;

	private DLDecisionAgent? _dlDecisionAgent;

	public VActorControllerType type { get; } = VActorControllerType.AI;

	public string AIDataName { get; private set; } = string.Empty;

	public string PendingAIDataName { get; private set; } = string.Empty;

	public string CurrentBTTemplateName { get; private set; } = string.Empty;

	public string PendingBTTemplateName { get; private set; } = string.Empty;

	public bool IsPaused => !_creature.VRoom.EnableAIController;

	public bool Terminated { get; private set; }

	public int TriggerSkillMasterID { get; private set; }

	public float SightRange { get; private set; }

	public float SightHeightRange { get; private set; }

	public float AttackRange { get; private set; }

	public bool IsFirstAttack { get; private set; }

	public float ChaseRange { get; private set; }

	public ChaseType ChaseType { get; private set; }

	public int PhaseID { get; private set; }

	public bool PhaseChanged { get; private set; }

	public bool RecoveryEnabled { get; private set; }

	public BlackBoard BlackBoard { get; private set; } = new BlackBoard();

	public AIState State { get; private set; }

	public AIController(VActor actor)
	{
		if (!(actor is VCreature creature))
		{
			throw new ArgumentException("AIController must be created with a Creature");
		}
		_creature = creature;
		Reset();
		Terminated = false;
		BlackBoard = new BlackBoard();
		SightRange = 1000000f;
		SightHeightRange = 2000f;
		IsFirstAttack = false;
		ChaseType = ChaseType.BattleStartPosition;
	}

	public void Initialize()
	{
		if (_creature is VMonster)
		{
			MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(_creature.MasterID);
			if (monsterInfo == null)
			{
				Logger.RError("MonsterInfo is null. MasterID : " + _creature.MasterID);
			}
			else
			{
				_pickRule = monsterInfo.TargetPickRule;
			}
		}
	}

	public void Update(long delta)
	{
		if (Terminated || IsPaused)
		{
			return;
		}
		AbnormalController? abnormalControlUnit = _creature.AbnormalControlUnit;
		if (abnormalControlUnit != null && abnormalControlUnit.IsAIPaused())
		{
			return;
		}
		_elpasedDelta += delta;
		_stateElapsedTime += delta;
		if (_elpasedDelta < 200)
		{
			return;
		}
		UpdateAggroTable();
		if (UpdateAIState() || _btState == null)
		{
			return;
		}
		if (PauseTime > 0)
		{
			PauseTime -= delta;
			return;
		}
		switch (ApplyChangeBT())
		{
		case AIChangeResult.Changed:
			_creature.MovementControlUnit?.SetMoveType(_creature.MovementControlUnit.IsMoving ? ActorMoveType.Walk : ActorMoveType.None);
			_creature.VRoom.OnActorChangeBT(_creature);
			_creature.SendDebugBTStateInfo();
			if (PhaseChanged)
			{
				_creature.SyncChangePhase();
			}
			PhaseChanged = false;
			break;
		case AIChangeResult.Fail:
			Reset();
			break;
		}
		_btState?.Update(_elpasedDelta);
		_elpasedDelta = 0L;
	}

	public void Dispose()
	{
	}

	public void SetSightRange(float range)
	{
		if (range <= 0f)
		{
			SightRange = 1000000f;
		}
		else
		{
			SightRange = range;
		}
	}

	public void SetSightHeightRange(float range)
	{
		SightHeightRange = range;
	}

	public void SetAttackRange(float range)
	{
		AttackRange = range;
	}

	public void SetFirstAttack(bool isFirstAttack)
	{
		IsFirstAttack = isFirstAttack;
	}

	public void SetChaseRange(float range)
	{
		ChaseRange = range;
	}

	public void SetChaseType(ChaseType chaseType)
	{
		ChaseType = chaseType;
	}

	public void SetRecoveryEnabled(bool enabled)
	{
		RecoveryEnabled = enabled;
	}

	public void SetAiData(string aiDataName, string currentBTTemplateName = "passive")
	{
		AIDataName = aiDataName;
		CurrentBTTemplateName = currentBTTemplateName.ToLower();
	}

	public void SetTriggerSkillInfo(int masterID, long skillSeq)
	{
		TriggerSkillMasterID = masterID;
	}

	public bool HasTarget()
	{
		return _btState?.Target != null;
	}

	public string GetTargetName()
	{
		return _btState?.Target?.ActorName ?? string.Empty;
	}

	public VCreature? GetTarget()
	{
		return _btState?.Target;
	}

	public string GetAIAction()
	{
		return _btState?.TriggeredAction ?? string.Empty;
	}

	public string GetMessage()
	{
		return _btState?.RelatedMessage ?? string.Empty;
	}

	public Vector3 GetChaseBasePosition()
	{
		switch (ChaseType)
		{
		case ChaseType.BattleStartPosition:
			return _battleStartPosition;
		case ChaseType.SpawnPosition:
			return _creature.SpawnPosition;
		default:
			if (State != AIState.Battle)
			{
				return _creature.PositionVector;
			}
			return _battleStartPosition;
		}
	}

	public float GetSightRange(float distanceParam = 0f)
	{
		bool flag = false;
		if (distanceParam < 0f)
		{
			flag = true;
			distanceParam = 0f - distanceParam;
		}
		float num = ((ChaseRange > 0f && State == AIState.Battle) ? ChaseRange : SightRange);
		if (distanceParam > 0f && distanceParam < num)
		{
			if (!flag)
			{
				return distanceParam;
			}
			return 0f - distanceParam;
		}
		if (!flag)
		{
			return num;
		}
		return 0f - num;
	}

	public void Reset()
	{
		_candidateFactionTier = 1;
		PauseTime = 0L;
		PhaseID = 0;
		Terminated = true;
		_btState?.Reset();
		if (_dlMovementAgent != null && _dlDecisionAgent != null)
		{
			ToggleDLAgent(flag: false);
		}
	}

	public void SetPauseTime(long time)
	{
		if (_creature is VMonster)
		{
			PauseTime = time;
		}
	}

	private bool UpdateAIState()
	{
		switch (State)
		{
		case AIState.Battle:
			if (m_AggroDict.Count == 0)
			{
				AIState state = AIState.Recovery;
				if (!RecoveryEnabled)
				{
					state = AIState.Peace;
				}
				else if (_lastAggroRemoveCause == AggroRemoveCause.Dead)
				{
					state = AIState.RecoveryWait;
				}
				SetState(state);
				return true;
			}
			if (ChaseRange > 0f && RecoveryEnabled && Misc.Distance(_creature.PositionVector, GetChaseBasePosition()) > ChaseRange)
			{
				SetState(AIState.Recovery);
				return true;
			}
			break;
		case AIState.Recovery:
			SetState(AIState.Peace);
			return true;
		case AIState.RecoveryWait:
			if (Hub.s.dataman.c_AggroRecoveryWaitTime < _stateElapsedTime)
			{
				SetState(AIState.Recovery);
				return true;
			}
			break;
		}
		return false;
	}

	public bool CreateAI()
	{
		if (string.IsNullOrEmpty(AIDataName))
		{
			return false;
		}
		BTData bT = Hub.s.dataman.AIDataManager.GetBT(AIDataName, CurrentBTTemplateName);
		if (bT == null)
		{
			return false;
		}
		BehaviorTreeState state = new BehaviorTreeState(_creature, BlackBoard, bT);
		PhaseChanged = PhaseID != bT.PhaseID;
		PhaseID = bT.PhaseID;
		if (_btState != null)
		{
			_btState.CopyInternalValue(ref state);
			_btState.Dispose();
		}
		_btState = state;
		return true;
	}

	public void HandleChangeAI(string aiDataName, string btTemplateName)
	{
		PendingAIDataName = aiDataName;
		PendingBTTemplateName = btTemplateName;
	}

	public void HandleChangeBT(string btTemplateName)
	{
		PendingBTTemplateName = btTemplateName;
	}

	private AIChangeResult ApplyChangeBT()
	{
		if (string.IsNullOrEmpty(PendingAIDataName) && string.IsNullOrEmpty(PendingBTTemplateName))
		{
			return AIChangeResult.None;
		}
		string text = (string.IsNullOrEmpty(PendingAIDataName) ? AIDataName : PendingAIDataName);
		string text2 = (string.IsNullOrEmpty(PendingBTTemplateName) ? CurrentBTTemplateName : PendingBTTemplateName);
		PendingAIDataName = string.Empty;
		PendingBTTemplateName = string.Empty;
		if (Hub.s.dataman.AIDataManager.GetBT(text, text2) == null)
		{
			return AIChangeResult.Fail;
		}
		AIDataName = text;
		CurrentBTTemplateName = text2;
		if (!CreateAI())
		{
			return AIChangeResult.Fail;
		}
		ToggleDLAgent(flag: false);
		Flush();
		if (text2 == "passive")
		{
			SetState(AIState.Recovery);
		}
		return AIChangeResult.Changed;
	}

	public void Flush()
	{
		_creature.MovementControlUnit.StopMove();
		_btState?.Flush();
	}

	public void OnAttaching(int attachingActorID, int attachedActorID)
	{
		if (Terminated || IsPaused)
		{
			return;
		}
		if (m_AggroDict.ContainsKey(attachedActorID))
		{
			m_AggroDict.Remove(attachedActorID);
			_lastAggroRemoveCause = AggroRemoveCause.Attach;
		}
		VActor vActor = _creature.VRoom.FindActorByObjectID(attachingActorID);
		if (vActor != null)
		{
			VActor vActor2 = _creature.VRoom.FindActorByObjectID(attachedActorID);
			if (vActor2 != null && vActor is VCreature attacher && vActor2 is VCreature attached)
			{
				_btState?.OnAttaching(attacher, attached);
			}
		}
	}

	public void OnDetaching(int detachedActorID)
	{
		if (!Terminated && !IsPaused)
		{
			VActor vActor = _creature.VRoom.FindActorByObjectID(detachedActorID);
			if (vActor != null && vActor is VCreature aggroTarget && !m_AggroDict.ContainsKey(detachedActorID))
			{
				m_AggroDict.Add(detachedActorID, new AggroObject(_creature, aggroTarget));
			}
		}
	}

	public void CollectAggroObjectByFaction()
	{
		List<VActor> actorsExcept = _creature.VRoom.GetActorsExcept(_creature, m_AggroDict.Keys.ToList());
		if (actorsExcept.Count == 0)
		{
			return;
		}
		foreach (VActor item in actorsExcept)
		{
			if (item is VCreature vCreature && _creature.IsValidTarget(HitTargetType.Enemy, vCreature) && !m_AggroDict.ContainsKey(item.ObjectID))
			{
				m_AggroDict.Add(item.ObjectID, new AggroObject(_creature, vCreature));
			}
		}
	}

	public void OnDamaged(ApplyDamageArgs args)
	{
		VActor attacker = args.Attacker;
		if (_creature.IsAliveStatus() && !Terminated && !IsPaused && attacker != _creature && args.MutableStatChangeCause == MutableStatChangeCause.ActiveAttack)
		{
			_btState?.OnDamaged();
			_creature.StatControlUnit.GetSpecificStatValue(StatType.HP);
			if (attacker != null)
			{
				int point = (int)args.Damage;
				AddAggroPoint(attacker, point);
			}
		}
	}

	private void SetState(AIState state)
	{
		if (State != state)
		{
			if (State == AIState.Peace && state == AIState.Battle)
			{
				_battleStartPosition = _creature.PositionVector;
			}
			if (state == AIState.Recovery)
			{
				_creature.StatControlUnit?.InstantChargeHP(0L);
			}
			if (State == AIState.RecoveryWait)
			{
				_creature.MovementControlUnit?.SetMoveType(ActorMoveType.None);
			}
			State = state;
			_stateElapsedTime = 0L;
		}
	}

	public bool CanReserveDelayedAction()
	{
		if (State == AIState.Recovery || State == AIState.RecoveryWait)
		{
			return false;
		}
		if (IsPaused)
		{
			return false;
		}
		return true;
	}

	public void AddAggroPoint(VActor actor, int point)
	{
		if (!actor.IsAliveStatus())
		{
			return;
		}
		SetState(AIState.Battle);
		if (m_AggroDict.TryGetValue(actor.ObjectID, out AggroObject value))
		{
			value.AddAggroPointByHit(point);
		}
		else
		{
			if (!(actor is VCreature aggroTarget))
			{
				return;
			}
			AggroObject aggroObject = new AggroObject(_creature, aggroTarget);
			aggroObject.AddAggroPointByHit(point);
			m_AggroDict.Add(actor.ObjectID, aggroObject);
		}
		NotifyAddAggroPoint(actor.ObjectID);
	}

	public void AddSoundAggroPoint(VActor actor, int point)
	{
		if (!actor.IsAliveStatus())
		{
			return;
		}
		MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(_creature.MasterID);
		if (monsterInfo == null)
		{
			Logger.RError("MonsterInfo is null. MasterID : " + _creature.MasterID);
		}
		else
		{
			if (!monsterInfo.AggroInfoDict.ContainsKey(AggroType.Sound) || (double)Misc.Distance(_creature.PositionVector, actor.PositionVector) > monsterInfo.AggroInfoDict[AggroType.Sound].Range)
			{
				return;
			}
			if (!m_AggroDict.TryGetValue(actor.ObjectID, out AggroObject value))
			{
				if (!(actor is VCreature aggroTarget))
				{
					return;
				}
				value = new AggroObject(_creature, aggroTarget);
				m_AggroDict.Add(actor.ObjectID, value);
			}
			value.AddAggroPointBySound(point);
		}
	}

	public void AddMovementAggroPoint(VActor actor, float distance)
	{
		if (!actor.IsAliveStatus())
		{
			return;
		}
		MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(_creature.MasterID);
		if (monsterInfo == null)
		{
			Logger.RError("MonsterInfo is null. MasterID : " + _creature.MasterID);
		}
		else
		{
			if (!monsterInfo.AggroInfoDict.ContainsKey(AggroType.Sound) || (double)Misc.Distance(_creature.PositionVector, actor.PositionVector) > monsterInfo.AggroInfoDict[AggroType.Sound].Range)
			{
				return;
			}
			if (!m_AggroDict.TryGetValue(actor.ObjectID, out AggroObject value))
			{
				if (!(actor is VCreature aggroTarget))
				{
					return;
				}
				value = new AggroObject(_creature, aggroTarget);
				m_AggroDict.Add(actor.ObjectID, value);
			}
			value.AddAggroPointByMovement(distance);
		}
	}

	public void ResetAggro(bool isDead = false)
	{
		m_AggroDict.Clear();
		_lastAggroRemoveCause = AggroRemoveCause.None;
		_btState?.ResetTargetActor();
	}

	public void ChangeAggroPointToTop(VActor actor)
	{
		if (!(actor is VCreature))
		{
			return;
		}
		if (m_AggroDict.Count == 0)
		{
			AddAggroPoint(actor, 0);
			return;
		}
		AggroObject aggroObject = m_AggroDict.Values.OrderByDescending((AggroObject x) => x.AggroValue).First();
		if (aggroObject.Target != actor)
		{
			AddAggroPoint(actor, aggroObject.AggroValue + 1);
		}
	}

	public bool CheckBTNodeCooltime(int nodeID)
	{
		if (!_btNodeCoolTime.TryGetValue(nodeID, out var value))
		{
			return true;
		}
		if (Hub.s.timeutil.GetCurrentTickMilliSec() < value)
		{
			return false;
		}
		_btNodeCoolTime.Remove(nodeID);
		return true;
	}

	public void SetBTNodeCooltime(int nodeID, long cooltime)
	{
		_btNodeCoolTime[nodeID] = Hub.s.timeutil.GetCurrentTickMilliSec() + cooltime;
	}

	public void OnSightOut(VActor actor)
	{
		if (actor is VCreature actor2)
		{
			if (m_AggroDict.ContainsKey(actor.ObjectID))
			{
				m_AggroDict.Remove(actor.ObjectID);
				_lastAggroRemoveCause = AggroRemoveCause.OutofSight;
			}
			_btState?.OnSightOut(actor2);
		}
	}

	public void OnSightIn(VActor actor)
	{
		if (actor is VCreature vCreature && _creature.IsValidTarget(HitTargetType.Enemy, vCreature) && (_creature.AttachControlUnit == null || (!_creature.AttachControlUnit.IsAttaching() && !_creature.AttachControlUnit.IsAttached())))
		{
			if (!m_AggroDict.ContainsKey(actor.ObjectID))
			{
				m_AggroDict.Add(actor.ObjectID, new AggroObject(_creature, vCreature));
			}
			_btState?.OnSightIn(vCreature);
		}
	}

	public void OnChangeVisible(VActor actor)
	{
		if (!(actor is VCreature vCreature))
		{
			return;
		}
		if (!_creature.IsValidTarget(HitTargetType.Enemy, vCreature))
		{
			if (m_AggroDict.ContainsKey(actor.ObjectID))
			{
				m_AggroDict.Remove(actor.ObjectID);
				if (_btState?.Target?.ObjectID == actor.ObjectID)
				{
					_btState.ResetTargetActor();
				}
				_lastAggroRemoveCause = AggroRemoveCause.Invisible;
			}
			_btState?.OnSightOut(vCreature);
		}
		else
		{
			if (!m_AggroDict.ContainsKey(actor.ObjectID))
			{
				m_AggroDict.Add(actor.ObjectID, new AggroObject(_creature, vCreature));
			}
			_btState?.OnSightIn(vCreature);
		}
	}

	public void UpdateAggroTable()
	{
		List<(int, AggroRemoveCause)> list = new List<(int, AggroRemoveCause)>();
		Hub.s.timeutil.GetCurrentTickMilliSec();
		foreach (KeyValuePair<int, AggroObject> item in m_AggroDict)
		{
			VCreature target = item.Value.Target;
			if (!target.IsAliveStatus())
			{
				list.Add((item.Key, AggroRemoveCause.Dead));
			}
			else if (!_creature.IsValidTarget(HitTargetType.Enemy, target))
			{
				list.Add((item.Key, AggroRemoveCause.FactionChanged));
			}
			else
			{
				item.Value.Update();
			}
		}
		foreach (var (num, lastAggroRemoveCause) in list)
		{
			m_AggroDict.Remove(num);
			BehaviorTreeState? btState = _btState;
			if (btState != null && btState.Target?.ObjectID == num)
			{
				_btState.ResetTargetActor();
			}
			_lastAggroRemoveCause = lastAggroRemoveCause;
		}
		if (_btState?.Target == null && m_AggroDict.Count > 0)
		{
			PickTarget(BTTargetPickRule.Invalid, AIRangeType.Invalid, checkHeight: false, reset: false);
		}
	}

	public int GetMaxAggroValue()
	{
		if (m_AggroDict.Count == 0)
		{
			return 0;
		}
		return m_AggroDict.Values.Max((AggroObject x) => x.AggroValue);
	}

	public VCreature? GetMaxAggroActor(bool ignoreReachable = true)
	{
		if (m_AggroDict.Count == 0)
		{
			return null;
		}
		IOrderedEnumerable<AggroObject> orderedEnumerable = from x in m_AggroDict.Values
			where x.CanTarget(checkAggro: true, checkHeight: false, SightRange) && x.Target.isTopFaction(_candidateFactionTier)
			orderby x.AggroValue descending
			select x;
		if (orderedEnumerable.Count() == 0)
		{
			return null;
		}
		if (!ignoreReachable)
		{
			foreach (AggroObject item in orderedEnumerable)
			{
				if (_creature.VRoom.FindPath(_creature.PositionVector, item.Target.PositionVector, out List<Vector3> _))
				{
					return item.Target;
				}
			}
			return null;
		}
		return orderedEnumerable.First().Target;
	}

	public VCreature? GetMinDistanceActor(bool checkHeight, AIRangeType rangeType, bool ignoreReachable = true)
	{
		if (m_AggroDict.Count == 0)
		{
			return null;
		}
		IOrderedEnumerable<AggroObject> orderedEnumerable = from x in m_AggroDict.Values
			where x.CanTarget(checkAggro: false, checkHeight, SightRange) && x.Target.isTopFaction(_candidateFactionTier)
			orderby (rangeType != AIRangeType.ByNavMesh) ? ((double)Misc.Distance(_creature.PositionVector, x.Target.PositionVector)) : _creature.VRoom.GetDistanceByNavMesh(_creature.PositionVector, x.Target.PositionVector)
			select x;
		if (orderedEnumerable.Count() == 0)
		{
			return null;
		}
		if (!ignoreReachable)
		{
			foreach (AggroObject item in orderedEnumerable)
			{
				if (_creature.VRoom.FindPath(_creature.PositionVector, item.Target.PositionVector, out List<Vector3> _))
				{
					return item.Target;
				}
			}
			return null;
		}
		return orderedEnumerable.First().Target;
	}

	public VCreature? GetMaxDistanceActor(bool checkHeight, AIRangeType rangeType, bool ignoreReachable = true)
	{
		if (m_AggroDict.Count == 0)
		{
			return null;
		}
		IOrderedEnumerable<AggroObject> orderedEnumerable = from x in m_AggroDict.Values
			where x.CanTarget(checkAggro: false, checkHeight, SightRange) && x.Target.isTopFaction(_candidateFactionTier)
			orderby (rangeType != AIRangeType.ByNavMesh) ? ((double)Misc.Distance(_creature.PositionVector, x.Target.PositionVector)) : _creature.VRoom.GetDistanceByNavMesh(_creature.PositionVector, x.Target.PositionVector) descending
			select x;
		if (orderedEnumerable.Count() == 0)
		{
			return null;
		}
		if (!ignoreReachable)
		{
			foreach (AggroObject item in orderedEnumerable)
			{
				if (_creature.VRoom.FindPath(_creature.PositionVector, item.Target.PositionVector, out List<Vector3> _))
				{
					return item.Target;
				}
			}
			return null;
		}
		return orderedEnumerable.First().Target;
	}

	public VCreature? GetRandomActor(bool checkHeight, bool ignoreReachable = true)
	{
		if (m_AggroDict.Count == 0)
		{
			return null;
		}
		IOrderedEnumerable<AggroObject> orderedEnumerable = from o in m_AggroDict.Values
			where o.CanTarget(checkAggro: false, checkHeight, SightRange) && o.Target.isTopFaction(_candidateFactionTier)
			orderby Guid.NewGuid()
			select o;
		if (orderedEnumerable.Count() == 0)
		{
			return null;
		}
		if (!ignoreReachable)
		{
			foreach (AggroObject item in orderedEnumerable)
			{
				if (_creature.VRoom.FindPath(_creature.PositionVector, item.Target.PositionVector, out List<Vector3> _))
				{
					return item.Target;
				}
			}
			return null;
		}
		return orderedEnumerable.First().Target;
	}

	public int GetAggroPlayerCount(bool checkHeight, float range = 0f)
	{
		if (range >= 0f)
		{
			return m_AggroDict.Values.Where((AggroObject x) => x.CanTarget(checkAggro: false, checkHeight, SightRange) && Misc.Distance(_creature.PositionVector, x.Target.PositionVector) <= range && x.Target is VPlayer).Count();
		}
		return m_AggroDict.Values.Where((AggroObject x) => x.CanTarget(checkAggro: false, checkHeight, SightRange) && x.Target is VPlayer).Count();
	}

	public List<VCreature> GetAggroActors(bool checkHeight, float range = 0f, int factionTier = 1)
	{
		if (range >= 0f)
		{
			return (from x in m_AggroDict.Values
				where x.CanTarget(checkAggro: false, checkHeight, SightRange) && x.Target.isTopFaction(factionTier)
				select x.Target into x
				where Misc.Distance(_creature.PositionVector, x.PositionVector) <= range
				select x).ToList();
		}
		return (from x in m_AggroDict.Values
			where x.CanTarget(checkAggro: false, checkHeight, SightRange)
			select x.Target).ToList();
	}

	public void IterateAggroActor(Action<VCreature> action)
	{
		foreach (AggroObject value in m_AggroDict.Values)
		{
			action(value.Target);
		}
	}

	public void IterateAggroActorOrderByPoint(Action<VCreature> action)
	{
		foreach (AggroObject item in m_AggroDict.Values.OrderByDescending((AggroObject x) => x.AggroValue))
		{
			action(item.Target);
		}
	}

	private (int, int) GetSkillAggroDefault(int skillSeqMasterID)
	{
		return (0, 0);
	}

	public void OnChangeTarget(VActor? target)
	{
		_creature.SendInSight(new ChangeAITargetSig
		{
			actorID = _creature.ObjectID,
			targetID = (target?.ObjectID ?? 0)
		});
	}

	public void CopyAggroTableFrom(VCreature sourceActor)
	{
		if (m_AggroDict.Count > 0 || sourceActor.AIControlUnit == null || sourceActor.AIControlUnit.m_AggroDict.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<int, AggroObject> item in sourceActor.AIControlUnit.m_AggroDict)
		{
			m_AggroDict.Add(item.Key, item.Value.Clone());
			NotifyAddAggroPoint(item.Key);
		}
		UpdateAggroTable();
		if (m_AggroDict.Count > 0)
		{
			SetState(AIState.Battle);
		}
	}

	public void OnDeath()
	{
		ResetAggro(isDead: true);
		if (_dlMovementAgent != null && _dlDecisionAgent != null)
		{
			ToggleDLAgent(flag: false);
		}
	}

	private void NotifyAddAggroPoint(int actorID)
	{
	}

	public void WaitInitDone()
	{
	}

	public void PostUpdate(long deltaTime)
	{
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		return MsgErrorCode.Success;
	}

	public void SetVoiceRule(BTVoiceRule rule)
	{
		if (_dlDecisionAgent == null)
		{
			DLDecisionAgent dLDecisionAgentByActorID = Hub.s.dLAcademyManager.GetDLDecisionAgentByActorID(_creature.ObjectID);
			if (dLDecisionAgentByActorID != null)
			{
				dLDecisionAgentByActorID.SetCreature(_creature);
				dLDecisionAgentByActorID.SetVoiceRule(rule);
			}
		}
		else
		{
			_dlDecisionAgent.SetVoiceRule(rule);
		}
	}

	public void SetChargingCompleted()
	{
		if (_dlDecisionAgent == null)
		{
			DLDecisionAgent dLDecisionAgentByActorID = Hub.s.dLAcademyManager.GetDLDecisionAgentByActorID(_creature.ObjectID);
			if (dLDecisionAgentByActorID != null)
			{
				dLDecisionAgentByActorID.SetChargingCompleted();
			}
		}
		else
		{
			_dlDecisionAgent.SetChargingCompleted();
		}
	}

	public void ToggleDLAgent(bool flag)
	{
		if (flag)
		{
			if (_dlMovementAgent == null)
			{
				Hub.s.dLAcademyManager.ActivateDLAgentByActorID(_creature, 5f);
				_dlMovementAgent = Hub.s.dLAcademyManager.GetDLMovementAgentByActorID(_creature.ObjectID);
			}
			if (_dlDecisionAgent == null)
			{
				_dlDecisionAgent = Hub.s.dLAcademyManager.GetDLDecisionAgentByActorID(_creature.ObjectID);
			}
			if (_btState?.Target != null && !Hub.s.dLAcademyManager.RegisterTargetActor(_creature.ObjectID, _btState.Target))
			{
				Logger.RError("RegisterTargetActor failed");
			}
		}
		else if (!(_dlMovementAgent == null) && !(_dlDecisionAgent == null))
		{
			Hub.s.dLAcademyManager.DeactivateDLAgentByActorID(_creature.ObjectID);
			_dlMovementAgent = null;
			_dlDecisionAgent = null;
		}
	}

	public bool GetTargetPosWithDLAgent(out PosWithRot resultPos, out ActorMoveType moveType, out bool calcAngle)
	{
		moveType = ActorMoveType.Walk;
		calcAngle = false;
		if (_dlMovementAgent == null)
		{
			resultPos = new PosWithRot();
			return false;
		}
		if (!_dlMovementAgent.DLAgentMovementOutput.worldPosDiff.HasValue || !_dlMovementAgent.DLAgentMovementOutput.yawDiff.HasValue || !_dlMovementAgent.DLAgentMovementOutput.pitchDiff.HasValue)
		{
			resultPos = new PosWithRot();
			return false;
		}
		moveType = ((_dlMovementAgent.DLAgentMovementOutput.isRunning != true) ? ActorMoveType.Walk : ActorMoveType.Run);
		calcAngle = _dlMovementAgent.DLAgentMovementOutput.calcAngle == true;
		resultPos = VWorldUtil.GetTargetPos(_creature.Position, _dlMovementAgent.DLAgentMovementOutput.worldPosDiff.Value, _dlMovementAgent.DLAgentMovementOutput.yawDiff.Value, _dlMovementAgent.DLAgentMovementOutput.pitchDiff.Value);
		if (NavMesh.SamplePosition(new Vector3(resultPos.x, resultPos.y, resultPos.z), out var hit, 0.5f, -1))
		{
			Vector3 position = hit.position;
			resultPos.x = position.x;
			resultPos.y = position.y;
			resultPos.z = position.z;
		}
		_dlMovementAgent.DLAgentMovementOutput.isConsumed = true;
		return true;
	}

	public bool GetDLAgentDecisionResult(out DLAgentDecisionOutput? output)
	{
		if (_dlDecisionAgent == null)
		{
			output = null;
			return false;
		}
		output = _dlDecisionAgent.DLAgentDecisionOutput;
		return true;
	}

	public bool ResetDLDecision()
	{
		if (_dlDecisionAgent == null)
		{
			DLDecisionAgent dLDecisionAgentByActorID = Hub.s.dLAcademyManager.GetDLDecisionAgentByActorID(_creature.ObjectID);
			if (dLDecisionAgentByActorID != null)
			{
				dLDecisionAgentByActorID.ResetDLDecision();
			}
		}
		else
		{
			_dlDecisionAgent.ResetDLDecision();
		}
		return true;
	}

	public bool ResetDLTimeToAttackTimer()
	{
		if (_dlDecisionAgent == null)
		{
			DLDecisionAgent dLDecisionAgentByActorID = Hub.s.dLAcademyManager.GetDLDecisionAgentByActorID(_creature.ObjectID);
			if (dLDecisionAgentByActorID != null)
			{
				dLDecisionAgentByActorID.ResetDLTimeToAttackTimer();
			}
		}
		else
		{
			_dlDecisionAgent.ResetDLTimeToAttackTimer();
		}
		return true;
	}

	public bool ResetUseScrapScanner()
	{
		if (_dlDecisionAgent == null)
		{
			DLDecisionAgent dLDecisionAgentByActorID = Hub.s.dLAcademyManager.GetDLDecisionAgentByActorID(_creature.ObjectID);
			if (dLDecisionAgentByActorID != null)
			{
				dLDecisionAgentByActorID.SetJustUsedScrapScanner();
			}
		}
		else
		{
			_dlDecisionAgent.SetJustUsedScrapScanner();
		}
		return true;
	}

	public bool CopyInventory(BTTargetPickRule rule)
	{
		switch (rule)
		{
		case BTTargetPickRule.MaxDistance:
		{
			VCreature maxDistanceActor = GetMaxDistanceActor(checkHeight: false, AIRangeType.Absolute);
			if (maxDistanceActor == null)
			{
				return false;
			}
			Dictionary<int, ItemElement> dictionary3 = maxDistanceActor.InventoryControlUnit?.GetAllItemElements();
			if (dictionary3 == null)
			{
				return false;
			}
			return _creature.InventoryControlUnit?.CloneInven(dictionary3) ?? false;
		}
		case BTTargetPickRule.Random:
		{
			VCreature randomActor = GetRandomActor(checkHeight: false);
			if (randomActor == null)
			{
				return false;
			}
			Dictionary<int, ItemElement> dictionary2 = randomActor.InventoryControlUnit?.GetAllItemElements();
			if (dictionary2 == null)
			{
				return false;
			}
			return _creature.InventoryControlUnit?.CloneInven(dictionary2) ?? false;
		}
		case BTTargetPickRule.MinDistance:
		{
			VCreature minDistanceActor = GetMinDistanceActor(checkHeight: false, AIRangeType.Absolute);
			if (minDistanceActor == null)
			{
				return false;
			}
			Dictionary<int, ItemElement> dictionary = minDistanceActor.InventoryControlUnit?.GetAllItemElements();
			if (dictionary == null)
			{
				return false;
			}
			return _creature.InventoryControlUnit?.CloneInven(dictionary) ?? false;
		}
		default:
			throw new ArgumentException($"Invalid rule {rule}");
		}
	}

	public bool PickTarget(BTTargetPickRule rule = BTTargetPickRule.Invalid, AIRangeType rangeType = AIRangeType.Invalid, bool checkHeight = false, bool reset = true, int factionTier = 0)
	{
		if (!reset && _btState?.Target != null)
		{
			return true;
		}
		BTTargetPickRule bTTargetPickRule = ((rule == BTTargetPickRule.Invalid) ? _pickRule : rule);
		if (rangeType == AIRangeType.Invalid)
		{
			_ = _aiRangeType;
		}
		if (bTTargetPickRule == BTTargetPickRule.Invalid)
		{
			if (_btState?.Target != null)
			{
				_btState.ResetTargetActor();
			}
			return false;
		}
		if (factionTier > 0)
		{
			_candidateFactionTier = factionTier;
		}
		switch (bTTargetPickRule)
		{
		case BTTargetPickRule.MaxAggro:
		{
			VCreature maxAggroActor = GetMaxAggroActor(ignoreReachable: false);
			if (maxAggroActor != null)
			{
				if (_btState?.Target != null && m_AggroDict.TryGetValue(_btState.Target.ObjectID, out AggroObject value) && (double)(value.AggroValue * Hub.s.dataman.ExcelDataManager.Consts.C_BTChangeTargetThreshold) * 0.0001 < (double)m_AggroDict[maxAggroActor.ObjectID].AggroValue)
				{
					_btState?.SetTargetActor(maxAggroActor);
				}
				break;
			}
			return false;
		}
		case BTTargetPickRule.MinDistance:
		{
			VCreature minDistanceActor = GetMinDistanceActor(checkHeight, rangeType, ignoreReachable: false);
			if (minDistanceActor != null)
			{
				if (minDistanceActor == _btState?.Target)
				{
					return true;
				}
				if (_btState?.Target != null && (double)Misc.Distance(_creature.PositionVector, _btState.Target.PositionVector) < (double)(Misc.Distance(_creature.PositionVector, minDistanceActor.PositionVector) * (float)Hub.s.dataman.ExcelDataManager.Consts.C_BTChangeTargetThreshold) * 0.0001)
				{
					return false;
				}
				_btState?.SetTargetActor(minDistanceActor);
				break;
			}
			return false;
		}
		case BTTargetPickRule.MaxDistance:
		{
			VCreature maxDistanceActor = GetMaxDistanceActor(checkHeight, rangeType, ignoreReachable: false);
			if (maxDistanceActor != null)
			{
				if (maxDistanceActor == _btState?.Target)
				{
					return true;
				}
				if (_btState?.Target != null && (double)(Misc.Distance(_creature.PositionVector, _btState.Target.PositionVector) * (float)Hub.s.dataman.ExcelDataManager.Consts.C_BTChangeTargetThreshold) * 0.0001 > (double)Misc.Distance(_creature.PositionVector, maxDistanceActor.PositionVector))
				{
					return false;
				}
				_btState?.SetTargetActor(maxDistanceActor);
				break;
			}
			return false;
		}
		case BTTargetPickRule.Random:
		{
			VCreature randomActor = GetRandomActor(checkHeight, ignoreReachable: false);
			if (randomActor == _btState?.Target)
			{
				return true;
			}
			if (randomActor != null)
			{
				_btState?.SetTargetActor(randomActor);
				break;
			}
			return false;
		}
		default:
			throw new ArgumentException($"Invalid rule {_pickRule}");
		}
		if (rule != BTTargetPickRule.Invalid)
		{
			_pickRule = rule;
		}
		if (rangeType != AIRangeType.Invalid)
		{
			_aiRangeType = rangeType;
		}
		return true;
	}

	public long GetBTActivateTime()
	{
		return _btState?.UpTime ?? 0;
	}

	public string GetDebugString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("AggroList: \n");
		int num = 1;
		foreach (AggroObject item in m_AggroDict.Values.OrderByDescending((AggroObject x) => x.AggroValue))
		{
			stringBuilder.Append($"{num++}. {item.Target.ActorName}({item.AggroValue}) \n");
		}
		if (_btState != null)
		{
			stringBuilder.Append("BTState: " + _btState.TriggeredAction);
		}
		return stringBuilder.ToString();
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}
}
