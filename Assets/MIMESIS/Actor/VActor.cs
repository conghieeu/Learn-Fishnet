using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.Faction;
using Cysharp.Threading.Tasks;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using ReluServerBase.Threading;
using UnityEngine;

public abstract class VActor : ISpaceActor, IActor, IDisposable
{
	protected ControllerManager _controllerManager = new ControllerManager();

	protected AtomicFlag _disposed = new AtomicFlag(value: false);

	protected CommandExecutor _commandExecutor;

	public readonly ActorType ActorType;

	public readonly int MasterID;

	public readonly string ActorName;

	public readonly long UID;

	public readonly ReasonOfSpawn ReasonOfSpawn = ReasonOfSpawn.Spawn;

	public readonly IVroom VRoom;

	protected Dictionary<int, HashSet<int>> m_Factions = new Dictionary<int, HashSet<int>>();

	protected HashSet<int> m_DefaultFactions = new HashSet<int>();

	protected HashSet<int> m_AllyFactions = new HashSet<int>();

	protected HashSet<int> m_EnemyFactions = new HashSet<int>();

	protected HashSet<int> m_NeutralFactions = new HashSet<int>();

	public readonly Vector3 SpawnPosition;

	protected PosWithRot _currentPosition = new PosWithRot();

	public readonly Dictionary<VActorEventType, OnActorEventDelegate?> EventHandlers = new Dictionary<VActorEventType, OnActorEventDelegate>();

	private long _debugCollectLastTick;

	protected bool _isIndoor;

	protected List<IGameEventLog> _logDumps = new List<IGameEventLog>();

	public int ObjectID { get; }

	public float MoveCollisionRadius { get; protected set; }

	public float HitCollisionRadius { get; protected set; }

	public float Height { get; protected set; }

	public Vector3 LastTickPosition { get; protected set; }

	public VSpace? VSpace { get; protected set; }

	public bool Visible { get; protected set; } = true;

	public UntargetReason UntargetReason { get; protected set; }

	public PosWithRot Position => _currentPosition;

	public PosWithRot SavedPosition { get; protected set; } = new PosWithRot();

	public Vector3 PositionVector => Position.pos;

	public float Angle => Position.rot.y;

	public AIController? AIControlUnit => _controllerManager.GetController(VActorControllerType.AI) as AIController;

	public StatController? StatControlUnit => _controllerManager.GetController(VActorControllerType.Stat) as StatController;

	public MovementController? MovementControlUnit => _controllerManager.GetController(VActorControllerType.Movement) as MovementController;

	public SkillController? SkillControlUnit => _controllerManager.GetController(VActorControllerType.Skill) as SkillController;

	public CooltimeController? CooltimeControlUnit => _controllerManager.GetController(VActorControllerType.Cooltime) as CooltimeController;

	public InventoryController? InventoryControlUnit => _controllerManager.GetController(VActorControllerType.Inventory) as InventoryController;

	public AbnormalController? AbnormalControlUnit => _controllerManager.GetController(VActorControllerType.Abnormal) as AbnormalController;

	public AttachController? AttachControlUnit => _controllerManager.GetController(VActorControllerType.Attach) as AttachController;

	public EmotionController? EmotionControlUnit => _controllerManager.GetController(VActorControllerType.Emotion) as EmotionController;

	public AuraController? AuraControlUnit => _controllerManager.GetController(VActorControllerType.Aura) as AuraController;

	public ScrapMotionController? ScrapMotionController => _controllerManager.GetController(VActorControllerType.ScrapMotion) as ScrapMotionController;

	public int SpawnPointIndex { get; private set; }

	public bool IsIndoor => _isIndoor;

	public VActor(ActorType actorType, int actorID, int masterID, string actorName, PosWithRot position, bool isIndoor, IVroom room, long UID, ReasonOfSpawn reasonOfSpawn)
	{
		ActorType = actorType;
		ObjectID = actorID;
		MasterID = masterID;
		ActorName = actorName;
		this.UID = UID;
		ReasonOfSpawn = reasonOfSpawn;
		SpawnPosition = position.toVector3();
		position.CopyTo(_currentPosition);
		VRoom = room;
		_commandExecutor = room.GetCommandExecutor();
		_isIndoor = isIndoor;
	}

	public abstract SendResult SendToMe(IMsg msg);

	public abstract bool IsAliveStatus();

	public abstract void FillSightInSig(ref SightInSig sig);

	public abstract void CollectDebugInfo(ref DebugInfoSig sig);

	protected void CollectHitCheckInfo(IHitCheck hitcheck, ref DebugInfoSig sig)
	{
		_controllerManager.CollectDebugInfo(ref sig);
		VWorldUtil.CollectHitCheckDebugInfo(ObjectID, Position, hitcheck, ref sig);
	}

	public virtual void Update(long deltaTick)
	{
		if (IsAliveStatus())
		{
			_controllerManager.Update(deltaTick);
			if (VRoom.TurnOnDebug && Hub.s.timeutil.GetCurrentTickMilliSec() - _debugCollectLastTick > VRoom.DebugIntervalMillisec)
			{
				_debugCollectLastTick = Hub.s.timeutil.GetCurrentTickMilliSec();
				DebugInfoSig sig = new DebugInfoSig
				{
					actorID = ObjectID
				};
				CollectDebugInfo(ref sig);
				SendInSight(sig, includeSelf: true);
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void SetSpawnPointIndex(int index)
	{
		SpawnPointIndex = index;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_controllerManager.Dispose();
		}
	}

	public virtual void OnEnterSpace(VSpace space)
	{
		VSpace = space;
		SightInSector(space.AroundSpaces, SightReason.Spawned);
	}

	public virtual void OnExitSpace(VSpace space, bool exclude = false)
	{
		if (!exclude)
		{
			VSpace = null;
		}
		SightOutSector(space.AroundSpaces, SightReason.Despawned);
	}

	protected virtual void SightInSector(VSpace[] sectors, SightReason reason, bool mySightOnly = false)
	{
		SightInSig sig = new SightInSig
		{
			sightReason = reason
		};
		FillSightInSig(ref sig);
		bool found = false;
		SightInSig mySig = new SightInSig
		{
			sightReason = reason
		};
		for (int i = 0; i < sectors.Length; i++)
		{
			sectors[i].IterateObject(delegate(ISpaceActor sectorObject)
			{
				if (sectorObject is VActor vActor && sectorObject != this)
				{
					if (vActor.Visible && this is VPlayer)
					{
						found = true;
						vActor.FillSightInSig(ref mySig);
					}
					if (vActor is VPlayer vPlayer2 && Visible && !mySightOnly)
					{
						vPlayer2.SendToMe(sig);
					}
					AIControlUnit?.OnSightIn(vActor);
					vActor.AIControlUnit?.OnSightIn(this);
				}
			});
		}
		if (found)
		{
			SendToMe(mySig);
			if (this is VPlayer { OtherGameEntered: false } vPlayer)
			{
				vPlayer.SetOtherGameEntered();
			}
		}
	}

	protected virtual void SightOutSector(VSpace[] sectors, SightReason reason, bool includeSelf = false)
	{
		SightOutSig sig = new SightOutSig
		{
			sightReason = reason
		};
		sig.actorIDs.Add(ObjectID);
		SightOutSig mySig = new SightOutSig
		{
			sightReason = reason
		};
		for (int i = 0; i < sectors.Length; i++)
		{
			sectors[i].IterateObject(delegate(ISpaceActor sectorObject)
			{
				if (sectorObject is VActor vActor && sectorObject != this)
				{
					mySig.actorIDs.Add(vActor.ObjectID);
					if (sectorObject is VPlayer vPlayer)
					{
						vPlayer.SendToMe(sig);
					}
					if (sectorObject is VMonster)
					{
						vActor.AIControlUnit?.OnSightOut(this);
					}
				}
			});
		}
		if (includeSelf && mySig.actorIDs.Count > 0)
		{
			SendToMe(mySig);
		}
	}

	public List<VActor> GetAroundActors()
	{
		List<VActor> actors = new List<VActor>();
		VSpace?.IterateAroundObject(delegate(ISpaceActor actor)
		{
			if (actor is VActor vActor && vActor.ObjectID != ObjectID && VWorldUtil.Distance(PositionVector, vActor.PositionVector, ignoreHeight: true) < 2000.0)
			{
				actors.Add(vActor);
			}
		});
		return actors;
	}

	public void IterateAroundActors(Action<VActor> action, bool includeSelf = false)
	{
		VSpace?.IterateAroundObject(delegate(ISpaceActor sectorObject)
		{
			if (sectorObject is VActor vActor && (vActor != this || includeSelf))
			{
				action(vActor);
			}
		});
	}

	public void IterateActorsInRange(Action<VActor> action, double range, bool includeSelf = false)
	{
		VRoom.IterateAllActor(delegate(VActor actor)
		{
			if ((actor != this || includeSelf) && !((double)Vector3.Distance(actor.PositionVector, PositionVector) > range))
			{
				action(actor);
			}
		});
	}

	private void SyncVisible()
	{
		_ = this is VPlayer;
	}

	public bool InGrid()
	{
		return VSpace != null;
	}

	public void SendInSight(IMsg msg, bool includeSelf = false)
	{
		if (msg is IActorMsg actorMsg)
		{
			actorMsg.actorID = ObjectID;
		}
		VSpace?.IterateAroundObject(delegate(ISpaceActor ISpaceActor)
		{
			if (ISpaceActor is VActor vActor && (vActor != this || includeSelf))
			{
				vActor.SendToMe(msg);
			}
		});
	}

	public void SetPitch(float pitch)
	{
		if (float.IsNaN(pitch))
		{
			Logger.RError($"Set Relative Pitch to nan, position : {Position}, MasterID : {MasterID}, ZoneMasterID : {VRoom.MasterID}");
		}
		else
		{
			Position.pitch = pitch;
		}
	}

	public void SetAngle(float angle)
	{
		if (float.IsNaN(angle))
		{
			Logger.RError($"Set Relative Angle to nan, position : {Position}, MasterID : {MasterID}, ZoneMasterID : {VRoom.MasterID}");
		}
		else
		{
			Position.yaw = angle;
		}
	}

	public override string ToString()
	{
		return $"ActorID : {ObjectID}, Name : {ActorName}, Type : {ActorType}, ReasonOfSpawn : {ReasonOfSpawn}, Position : {Position}, Channel : {VRoom}";
	}

	public void SetRotation(Vector3 rotation)
	{
		Position.rot = rotation;
	}

	public virtual void SetPosition(PosWithRot pos, ActorMoveCause cause, bool toSave = false)
	{
		if (VWorldUtil.IsNaN(pos))
		{
			Logger.RError($"Set Absolute Position to nan, Position : {pos}, Cause : {cause}");
		}
		else
		{
			if (pos == _currentPosition)
			{
				return;
			}
			Vector3 positionVector = PositionVector;
			_currentPosition.x = pos.x;
			_currentPosition.y = pos.y;
			_currentPosition.z = pos.z;
			SetPitch(pos.pitch);
			SetAngle(pos.yaw);
			if (toSave)
			{
				OverwritePosition();
			}
			if (!VRoom.MoveObject(this, positionVector, PositionVector, cause))
			{
				Logger.RError($"MoveObject Failed on SetAbsolutePositionAndAngle, ActorID : {ObjectID}, Position : {PositionVector}, Cause : {cause}");
			}
			else if (this is VPlayer)
			{
				if (cause != ActorMoveCause.Emotion)
				{
					EmotionControlUnit?.OnMove();
				}
				if (cause != ActorMoveCause.ScrapMotion)
				{
					ScrapMotionController?.OnMove();
				}
			}
		}
	}

	public void OverwritePosition()
	{
		Position.CopyTo(SavedPosition);
	}

	public void Teleport(PosWithRot pos, TeleportReason reason)
	{
		MoveToPosition(pos, ActorMoveCause.Teleport);
		MovementControlUnit?.ForceSyncPosition(reason);
	}

	public virtual void OnMovePosition(Vector3 before, Vector3 after, ActorMoveCause cause)
	{
	}

	public virtual void OnMoveFailed(Vector3 oldPos, Vector3 newPos)
	{
		Logger.RError($"OnMoveFailed, ActorID : {ObjectID}, OldPos : {oldPos}, NewPos : {newPos}");
	}

	public void MoveToPosition(PosWithRot pos, ActorMoveCause cause)
	{
		MovementControlUnit?.StopMove(sync: false);
		SkillControlUnit?.CancelSkill(SkillCancelType.All, 0L);
		SetPosition(pos, cause, toSave: true);
	}

	public PosWithRot GetPosition()
	{
		return Position.Clone();
	}

	public void AddEvent(VActorEventType type, OnActorEventDelegate handler)
	{
		Dictionary<VActorEventType, OnActorEventDelegate> eventHandlers = EventHandlers;
		eventHandlers[type] = (OnActorEventDelegate)Delegate.Combine(eventHandlers[type], handler);
	}

	public void RemoveEvent(VActorEventType type, OnActorEventDelegate handler)
	{
		Dictionary<VActorEventType, OnActorEventDelegate> eventHandlers = EventHandlers;
		eventHandlers[type] = (OnActorEventDelegate)Delegate.Remove(eventHandlers[type], handler);
	}

	public void SetVisible(bool visible = true, bool sync = true)
	{
		bool visible2 = Visible;
		Visible = visible;
		if (!Visible)
		{
			AddUntargetReason(UntargetReason.Invisible);
		}
		else
		{
			RemoveUntargetReason(UntargetReason.Invisible);
		}
		if (!sync)
		{
			return;
		}
		IterateAroundActors(delegate(VActor actor)
		{
			if (actor is VCreature vCreature)
			{
				vCreature.AIControlUnit?.OnChangeVisible(this);
			}
		});
		if (visible2 != Visible)
		{
			SyncVisible();
		}
	}

	public void AddUntargetReason(UntargetReason reason)
	{
		UntargetReason |= reason;
		_ = 2;
	}

	public void RemoveUntargetReason(UntargetReason reason)
	{
		UntargetReason &= ~reason;
	}

	public virtual void LateUpdate(long delta)
	{
	}

	public virtual void SendToChannel(IMsg msg)
	{
		VRoom.SendToAll(msg);
	}

	public virtual void SendToOtherInChannel(IMsg msg)
	{
		VRoom.SendToOther(msg, this);
	}

	public virtual SendResult SendError<T>(MsgErrorCode errorCode) where T : IResMsg, new()
	{
		throw new NotImplementedException();
	}

	public virtual UniTask<TResponse> SendRequestAsync<TResponse>(IMsg req, string file = "", int line = 0) where TResponse : IResMsg, new()
	{
		throw new NotImplementedException();
	}

	public bool isTopFaction(int groupID)
	{
		return (from x in m_Factions
			where x.Value.Count > 0
			select x.Key).Max() == groupID;
	}

	public bool HasFaction(int id)
	{
		using (Dictionary<int, HashSet<int>>.Enumerator enumerator = m_Factions.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.Contains(id))
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	public HashSet<int> GetDefaultFactions()
	{
		return m_DefaultFactions;
	}

	public bool AddFaction(int id, bool defaultFlag = false, bool sync = false)
	{
		Faction_MasterData faction = Hub.s.dataman.ExcelDataManager.GetFaction(id);
		if (faction == null)
		{
			return false;
		}
		if (m_Factions.ContainsKey(faction.group) && m_Factions[faction.group].Contains(faction.id))
		{
			return true;
		}
		m_Factions[faction.group].Add(faction.id);
		if (defaultFlag)
		{
			m_DefaultFactions.Add(faction.id);
		}
		RefreshFaction();
		if (sync)
		{
			SyncFaction();
		}
		return true;
	}

	public bool AddFactions(ImmutableArray<int> ids, bool defaultFlag = false, bool sync = false)
	{
		ImmutableArray<int>.Enumerator enumerator = ids.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_ = enumerator.Current;
			Faction faction = new Faction();
			if (faction == null)
			{
				return false;
			}
			m_Factions[faction.category].Add(faction.id);
			if (defaultFlag)
			{
				m_DefaultFactions.Add(faction.id);
			}
		}
		RefreshFaction();
		if (sync)
		{
			SyncFaction();
		}
		return true;
	}

	public void SyncFaction()
	{
		ChangeFactionSig changeFactionSig = new ChangeFactionSig
		{
			actorID = ObjectID
		};
		foreach (KeyValuePair<int, HashSet<int>> faction in m_Factions)
		{
			changeFactionSig.factions.AddRange(faction.Value);
		}
		SendInSight(changeFactionSig, includeSelf: true);
	}

	public bool RemoveFaction(int id)
	{
		Faction_MasterData faction = Hub.s.dataman.ExcelDataManager.GetFaction(id);
		if (faction == null)
		{
			return false;
		}
		m_Factions[faction.group].Remove(faction.id);
		RefreshFaction();
		SyncFaction();
		return true;
	}

	public bool RemoveFactions(ImmutableArray<int> ids)
	{
		ImmutableArray<int>.Enumerator enumerator = ids.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_ = enumerator.Current;
			Faction faction = new Faction();
			if (faction == null)
			{
				return false;
			}
			m_Factions[faction.category].Remove(faction.id);
		}
		RefreshFaction();
		SyncFaction();
		return true;
	}

	private void RefreshFaction()
	{
		m_AllyFactions.Clear();
		m_NeutralFactions.Clear();
		m_EnemyFactions.Clear();
		foreach (KeyValuePair<int, HashSet<int>> faction2 in m_Factions)
		{
			foreach (int item in faction2.Value)
			{
				Faction_MasterData? faction = Hub.s.dataman.ExcelDataManager.GetFaction(item);
				faction.ally.ForEach(delegate(int allyID)
				{
					m_AllyFactions.Add(allyID);
				});
				faction.neutral.ForEach(delegate(int neutralID)
				{
					m_NeutralFactions.Add(neutralID);
				});
				faction.enemy.ForEach(delegate(int enemyID)
				{
					m_EnemyFactions.Add(enemyID);
				});
			}
		}
	}

	private ActorRelation Relation(VActor target)
	{
		if (Hub.s.dataman.ExcelDataManager.FactionCategory != null)
		{
			foreach (int item in Hub.s.dataman.ExcelDataManager.FactionCategory.Reverse())
			{
				HashSet<int> hashSet = target.m_Factions[item];
				foreach (int allyFaction in m_AllyFactions)
				{
					if (hashSet.Contains(allyFaction))
					{
						return ActorRelation.Ally;
					}
				}
				foreach (int neutralFaction in m_NeutralFactions)
				{
					if (hashSet.Contains(neutralFaction))
					{
						return ActorRelation.Neutral;
					}
				}
				foreach (int enemyFaction in m_EnemyFactions)
				{
					if (hashSet.Contains(enemyFaction))
					{
						return ActorRelation.Enemy;
					}
				}
			}
		}
		return ActorRelation.Neutral;
	}

	private void InitFaction()
	{
		m_Factions.Clear();
		foreach (int item in Hub.s.dataman.ExcelDataManager.GetFactionCategory())
		{
			m_Factions.Add(item, new HashSet<int>());
		}
	}

	public void ResetFaction()
	{
		InitFaction();
		foreach (int defaultFaction in m_DefaultFactions)
		{
			AddFaction(defaultFaction);
		}
	}

	public virtual void MoveToDying(ApplyDamageArgs args)
	{
	}

	protected virtual void GetActorBaseInfo(ref ActorBaseInfo info)
	{
		info.actorType = ActorType;
		info.actorID = ObjectID;
		info.masterID = MasterID;
		info.actorName = ActorName;
		info.position = Position;
		info.UID = UID;
		info.reasonOfSpawn = ReasonOfSpawn;
	}

	public bool IsValidTarget(ImmutableArray<HitTargetType> types, VActor actor)
	{
		ImmutableArray<HitTargetType>.Enumerator enumerator = types.GetEnumerator();
		while (enumerator.MoveNext())
		{
			HitTargetType current = enumerator.Current;
			if (IsValidTarget(current, actor))
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool IsValidTarget(HitTargetType type, VActor actor)
	{
		if (!actor.IsAliveStatus())
		{
			return false;
		}
		if (actor.UntargetReason != UntargetReason.None)
		{
			return false;
		}
		if (type == HitTargetType.ALL)
		{
			return true;
		}
		if (actor == this && type == HitTargetType.Self)
		{
			return true;
		}
		return Relation(actor) switch
		{
			ActorRelation.Ally => type == HitTargetType.Ally, 
			ActorRelation.Enemy => type == HitTargetType.Enemy, 
			ActorRelation.Neutral => type == HitTargetType.Neutral, 
			_ => false, 
		};
	}

	public void SetIsIndoor(bool isIndoor)
	{
		_isIndoor = isIndoor;
	}

	public void AddGameEventLog(IGameEventLog log)
	{
		_logDumps.Add(log);
	}

	public List<IGameEventLog> GetGameEventLogs()
	{
		return _logDumps;
	}
}
