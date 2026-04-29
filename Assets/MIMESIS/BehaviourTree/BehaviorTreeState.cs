using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;
using ReluProtocol;
using UnityEngine;

public class BehaviorTreeState : IBehaviorTreeState
{
	public readonly VCreature Self;

	private readonly IComposite Root;

	private Stack<IComposite> _compositeStack = new Stack<IComposite>();

	private Stack<IComposite> _delayedCompositeStack = new Stack<IComposite>();

	private IComposite? DelayActionComposite;

	private Queue<string> _triggerActionQueue = new Queue<string>();

	private List<(long, string)> _relatedMessageQueue = new List<(long, string)>();

	public readonly StatController BTStatController;

	public readonly MovementController BTMovementController;

	public readonly AIController BTAIController;

	public readonly SkillController BTSkillController;

	public readonly CooltimeController BTCooltimeController;

	public readonly InventoryController BTInventoryController;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	private SortedSet<(BehaviorDelayedAction, long)> _reservedDelayActions;

	private Dictionary<GroupComposite, int> _groupCompositeIndex = new Dictionary<GroupComposite, int>();

	private long _usingSkillSyncID;

	private int _failOccuredCount;

	private MapMarker_TeleportStartPoint? _pickedTeleportPoint;

	private Dictionary<int, int> _decoratorActivatedCountDict = new Dictionary<int, int>();

	private Dictionary<int, long> _decoratorActivatedTimeDict = new Dictionary<int, long>();

	private string btKey;

	public BlackBoard BlackBoard { get; private set; }

	public VCreature? Target { get; private set; }

	public PosWithRot? TargetPosition { get; private set; }

	public ILevelObjectInfo? TargetLevelObject { get; private set; }

	public long TargetPickedTime { get; private set; }

	public long TargetLevelObjectPickedTime { get; private set; }

	public long TargetPositionPickedTime { get; private set; }

	public long LastTargetCalculatedTick { get; private set; }

	public List<VCreature> HealTargetList { get; private set; } = new List<VCreature>();

	public List<int> MultiTargetActorID { get; private set; } = new List<int>();

	public VCreature? TracingTarget { get; private set; }

	public long ElapsedTime { get; private set; }

	public long UpTime { get; private set; }

	public bool Damaged { get; private set; }

	public int UsableSkillMasterID { get; private set; }

	public string TriggeredAction => string.Join(",", _triggerActionQueue);

	public string ActivatedActionType { get; set; } = string.Empty;

	public string ActivatedAction { get; set; } = string.Empty;

	public string ActivatedActionParam { get; set; } = string.Empty;

	public string RelatedMessage => string.Join(",", _relatedMessageQueue.Select<(long, string), string>(((long, string) x) => x.Item2 ?? ""));

	public bool FixLookAtTarget { get; private set; }

	public MapMarker_TeleportStartPoint? PickedTeleportPoint => _pickedTeleportPoint;

	public BehaviorTreeState(VCreature self, BlackBoard blackBoard, BTData btData)
	{
		Self = self;
		Root = btData.CompositeRoot.Clone();
		btKey = btData.Key.ToString();
		BTStatController = self.StatControlUnit;
		BTMovementController = self.MovementControlUnit;
		BTAIController = self.AIControlUnit;
		BTSkillController = self.SkillControlUnit;
		BTCooltimeController = self.CooltimeControlUnit;
		BTInventoryController = self.InventoryControlUnit;
		BlackBoard = blackBoard;
		Comparer<(BehaviorDelayedAction, long)> comparer = Comparer<(BehaviorDelayedAction, long)>.Create(((BehaviorDelayedAction, long) x, (BehaviorDelayedAction, long) y) => x.Item2.CompareTo(y.Item2));
		_reservedDelayActions = new SortedSet<(BehaviorDelayedAction, long)>(comparer);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && _disposed.On())
		{
			Reset();
		}
	}

	public void Reset()
	{
		Flush();
		_reservedDelayActions.Clear();
		ElapsedTime = 0L;
		Damaged = false;
	}

	public void Flush()
	{
		try
		{
			throw new Exception();
		}
		catch (Exception)
		{
		}
		_compositeStack.Clear();
		_delayedCompositeStack.Clear();
		_decoratorActivatedCountDict.Clear();
		_decoratorActivatedTimeDict.Clear();
	}

	private bool HandleDelayedComposite()
	{
		if (DelayActionComposite == null)
		{
			return false;
		}
		if (_delayedCompositeStack.Count > 0)
		{
			while (_delayedCompositeStack.Count > 0)
			{
				IComposite composite = _delayedCompositeStack.Peek();
				if (composite.Behave(this) == BehaviorResult.RUNNING)
				{
					break;
				}
				if (composite == DelayActionComposite)
				{
					_delayedCompositeStack.Pop();
					DelayActionComposite = null;
					break;
				}
			}
		}
		else
		{
			_delayedCompositeStack.Push(DelayActionComposite);
			while (_delayedCompositeStack.Count > 0)
			{
				IComposite composite2 = _delayedCompositeStack.Peek();
				if (composite2.Behave(this) == BehaviorResult.RUNNING)
				{
					break;
				}
				if (composite2 == DelayActionComposite)
				{
					_delayedCompositeStack.Pop();
					DelayActionComposite = null;
					break;
				}
			}
		}
		return true;
	}

	public void Update(long delta)
	{
		ElapsedTime += delta;
		UpTime += delta;
		if (HandleDelayedComposite())
		{
			return;
		}
		if (_delayedCompositeStack.Count == 0 && DelayActionComposite == null && _compositeStack.Count == 0 && _reservedDelayActions.Count > 0)
		{
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (_reservedDelayActions.Count > 0)
			{
				(BehaviorDelayedAction, long) min = _reservedDelayActions.Min;
				if (currentTickMilliSec >= min.Item2)
				{
					_reservedDelayActions.Remove(min);
					(DelayActionComposite, _) = min;
				}
			}
			if (HandleDelayedComposite())
			{
				return;
			}
		}
		BehaviorResult behaviorResult = BehaviorResult.NONE;
		if (_compositeStack.Count == 0)
		{
			behaviorResult = Root.Behave(this);
			if (behaviorResult == BehaviorResult.RUNNING)
			{
				return;
			}
		}
		else
		{
			BehaviorResult nestedResult = BehaviorResult.NONE;
			int num = 0;
			while (_compositeStack.Count > 0)
			{
				int count = _compositeStack.Count;
				behaviorResult = _compositeStack.Peek().Behave(this, nestedResult);
				if (behaviorResult == BehaviorResult.RUNNING || _compositeStack.Count == 0)
				{
					break;
				}
				nestedResult = behaviorResult;
				int count2 = _compositeStack.Count;
				num = ((count == count2) ? (num + 1) : 0);
				if (num > 100)
				{
					Logger.RError("LoopCount is over 100. BT is currupted while currentStackCount == afterCount");
					break;
				}
			}
		}
		if (behaviorResult == BehaviorResult.FAILURE && Target != null)
		{
			_failOccuredCount++;
			if (_failOccuredCount > 10)
			{
				VWorld? vworld = Hub.s.vworld;
				if (vworld != null && !vworld.FindPath(Self.PositionVector, Target.PositionVector, out List<Vector3> _))
				{
					Vector3? reachableRandomPosForTeleport = Misc.GetReachableRandomPosForTeleport(Self.PositionVector, 0.0, 0.5);
					if (reachableRandomPosForTeleport.HasValue)
					{
						Self.Teleport(reachableRandomPosForTeleport.Value.toPosWithRot(Self.Position.yaw), TeleportReason.ForceMoveSync);
						_failOccuredCount = 0;
					}
					else
					{
						Logger.RError($"Teleport failed, targetPos is null. _failOccuredCount: {_failOccuredCount}");
					}
				}
				else
				{
					_failOccuredCount = 0;
				}
			}
		}
		else
		{
			_failOccuredCount = 0;
		}
		ElapsedTime = 0L;
		Damaged = false;
		FlushCompositeTrack();
	}

	public long GetElapsedTime()
	{
		return ElapsedTime;
	}

	public void OnAttaching(VCreature attacher, VCreature attached)
	{
		if (Target == attached && attacher != Self)
		{
			Target = null;
		}
		if (TracingTarget == attached)
		{
			TracingTarget = null;
		}
		if (HealTargetList.Contains(attached))
		{
			HealTargetList.Remove(attached);
		}
		if (MultiTargetActorID.Contains(attached.ObjectID))
		{
			MultiTargetActorID.Remove(attached.ObjectID);
		}
	}

	public void OnSightIn(VCreature actor)
	{
		if (Target == null && actor.IsAliveStatus())
		{
			SetTargetActor(actor);
		}
	}

	public void OnSightOut(VCreature actor)
	{
		if (Target == actor)
		{
			Target = null;
		}
		if (TracingTarget == actor)
		{
			TracingTarget = null;
		}
		if (HealTargetList.Contains(actor))
		{
			HealTargetList.Remove(actor);
		}
		if (MultiTargetActorID.Contains(actor.ObjectID))
		{
			MultiTargetActorID.Remove(actor.ObjectID);
		}
	}

	public void OnDamaged()
	{
		Damaged = true;
	}

	public void CopyInternalValue(ref BehaviorTreeState state)
	{
		state.Target = Target;
		state.TargetPosition = TargetPosition;
		state.TargetLevelObject = TargetLevelObject;
		state.TargetPickedTime = TargetPickedTime;
		state.TargetLevelObjectPickedTime = TargetLevelObjectPickedTime;
		state.TargetPositionPickedTime = TargetPositionPickedTime;
		state.LastTargetCalculatedTick = LastTargetCalculatedTick;
		state.HealTargetList = new List<VCreature>(HealTargetList);
		state.MultiTargetActorID = new List<int>(MultiTargetActorID);
		state.TracingTarget = TracingTarget;
		state.UsableSkillMasterID = UsableSkillMasterID;
		state._triggerActionQueue = new Queue<string>(_triggerActionQueue);
	}

	public void ResetTargetActor()
	{
		if (Target != null)
		{
			BTMovementController.SetTargetID(0);
			BTAIController.OnChangeTarget(null);
		}
		if (BTMovementController.IsMoving)
		{
			BTMovementController.StopMove();
		}
		Target = null;
		FixLookAtTarget = false;
		TargetPickedTime = 0L;
	}

	public void SetTargetActor(VCreature target, List<int>? multiTargetActorIDs = null)
	{
		if (target.IsAliveStatus() && Target != target)
		{
			if (multiTargetActorIDs != null)
			{
				MultiTargetActorID.Clear();
				MultiTargetActorID.AddRange(multiTargetActorIDs);
			}
			Target = target;
			TargetPickedTime = Hub.s.timeutil.GetCurrentTickMilliSec();
			BTMovementController.SetTargetID(Target.ObjectID);
			BTAIController.OnChangeTarget(target);
		}
	}

	public void SetTargetLevelObject(ILevelObjectInfo? levelObject)
	{
		TargetLevelObject = levelObject;
		TargetLevelObjectPickedTime = ((levelObject == null) ? 0 : Hub.s.timeutil.GetCurrentTickMilliSec());
	}

	public void SetTeleportPoint(MapMarker_TeleportStartPoint? point)
	{
		_pickedTeleportPoint = point;
	}

	public void SetTargetPosition(PosWithRot? pos)
	{
		TargetPosition = pos;
		TargetPositionPickedTime = ((pos != null) ? Hub.s.timeutil.GetCurrentTickMilliSec() : 0);
		if (_pickedTeleportPoint != null && pos == null)
		{
			_pickedTeleportPoint = null;
		}
	}

	private void FlushCompositeTrack()
	{
		if (_relatedMessageQueue.Count != 0)
		{
			long currentTick = Hub.s.timeutil.GetCurrentTickMilliSec();
			int num = _relatedMessageQueue.FindIndex(((long, string) x) => currentTick - x.Item1 < 5000);
			if (num == -1)
			{
				_relatedMessageQueue.Clear();
				_relatedMessageQueue.RemoveRange(0, num - 1);
			}
		}
	}

	public bool SetUsableSkillMasterID(int skillMasterID)
	{
		if (skillMasterID > 0)
		{
			SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
			if (skillInfo == null)
			{
				return false;
			}
			if (skillInfo.IsGrabSkill() && Self.AttachControlUnit.IsAttaching())
			{
				return false;
			}
		}
		UsableSkillMasterID = skillMasterID;
		return true;
	}

	public bool CheckBTNodeCooltime(int nodeId)
	{
		return BTAIController.CheckBTNodeCooltime(nodeId);
	}

	public void LogBTCompositeTrack(string message)
	{
		_relatedMessageQueue.Add((Hub.s.timeutil.GetCurrentTickMilliSec(), ActivatedAction + "(" + ActivatedActionParam + "): " + message));
		if (_relatedMessageQueue.Count > 5)
		{
			_relatedMessageQueue.RemoveAt(0);
		}
	}

	public bool ReserveDealyedAction(BehaviorDelayedAction action, long delayTime)
	{
		if (!BTAIController.CanReserveDelayedAction())
		{
			return false;
		}
		_reservedDelayActions.Add((action, Hub.s.timeutil.GetCurrentTickMilliSec() + delayTime));
		return true;
	}

	public void SnapBTComposite(string actionName, string[]? actionParams, string actionType)
	{
		ActivatedActionType = actionType;
		ActivatedAction = actionName;
		if (actionParams != null)
		{
			ActivatedActionParam = string.Join(",", actionParams);
		}
		else
		{
			ActivatedActionParam = string.Empty;
		}
		_ = Hub.s.gameConfig.playerActor.btDebugEnable;
		_triggerActionQueue.Enqueue(actionName + "/" + string.Join(",", actionParams ?? new string[0]) + "/\n");
		if (_triggerActionQueue.Count > 10)
		{
			_triggerActionQueue.Dequeue();
		}
	}

	public bool PushComposite(IComposite composite)
	{
		if (DelayActionComposite != null)
		{
			if (_delayedCompositeStack.Contains(composite))
			{
				return false;
			}
			_delayedCompositeStack.Push(composite);
			return true;
		}
		if (_compositeStack.Contains(composite))
		{
			return false;
		}
		_compositeStack.Push(composite);
		return true;
	}

	public bool PopComposite(IComposite composite)
	{
		if (DelayActionComposite != null)
		{
			if (_delayedCompositeStack.Peek() != composite)
			{
				return false;
			}
			_delayedCompositeStack.Pop();
			if (composite is GroupComposite key && _groupCompositeIndex.ContainsKey(key))
			{
				_groupCompositeIndex.Remove(key);
			}
			return true;
		}
		if (_compositeStack.Peek() != composite)
		{
			return false;
		}
		_compositeStack.Pop();
		if (composite is GroupComposite key2 && _groupCompositeIndex.ContainsKey(key2))
		{
			_groupCompositeIndex.Remove(key2);
		}
		return true;
	}

	public bool SaveChildIndex(GroupComposite composite, int index)
	{
		if (_groupCompositeIndex.ContainsKey(composite))
		{
			_groupCompositeIndex[composite] = index;
		}
		else
		{
			_groupCompositeIndex.Add(composite, index);
		}
		return true;
	}

	public int GetSavedChildIndex(GroupComposite composite)
	{
		if (_groupCompositeIndex.ContainsKey(composite))
		{
			return _groupCompositeIndex[composite];
		}
		return 100000;
	}

	public void SetFixLookAtTarget(bool fix)
	{
		if (!fix || Target != null)
		{
			FixLookAtTarget = fix;
		}
	}

	public long GetUsingSkillSyncID()
	{
		return _usingSkillSyncID;
	}

	public void SetUsingSkillSyncID(long syncID)
	{
		_usingSkillSyncID = syncID;
	}

	public void IncreaseDecoratorActivatedCount(int decoratorID)
	{
		if (_decoratorActivatedCountDict.ContainsKey(decoratorID))
		{
			_decoratorActivatedCountDict[decoratorID]++;
		}
		else
		{
			_decoratorActivatedCountDict.Add(decoratorID, 1);
		}
	}

	public void RegisterDecoratorActivatedCount(int decoratorID)
	{
		if (!_decoratorActivatedCountDict.ContainsKey(decoratorID))
		{
			_decoratorActivatedCountDict.Add(decoratorID, 0);
		}
	}

	public int GetDecoratorActivatedCount(int decoratorID)
	{
		if (_decoratorActivatedCountDict.ContainsKey(decoratorID))
		{
			return _decoratorActivatedCountDict[decoratorID];
		}
		return -1;
	}

	public long GetDecoratorActivatedTime(int decoratorID)
	{
		if (_decoratorActivatedTimeDict.ContainsKey(decoratorID))
		{
			return _decoratorActivatedTimeDict[decoratorID];
		}
		return -1L;
	}

	public void RegisterDecoratorActivatedTime(int decoratorID)
	{
		if (!_decoratorActivatedTimeDict.ContainsKey(decoratorID))
		{
			_decoratorActivatedTimeDict.Add(decoratorID, 0L);
		}
	}

	public void AllocateDecoratorActivatedTime(int decoratorID)
	{
		if (_decoratorActivatedTimeDict.ContainsKey(decoratorID))
		{
			_decoratorActivatedTimeDict[decoratorID] = Hub.s.timeutil.GetCurrentTickMilliSec();
		}
	}

	public void RemoveDecoratorActivatedTime(int decoratorID)
	{
		if (_decoratorActivatedTimeDict.ContainsKey(decoratorID))
		{
			_decoratorActivatedTimeDict.Remove(decoratorID);
		}
	}

	public void RemoveDecoratorActivatedCount(int decoratorID)
	{
		if (_decoratorActivatedCountDict.ContainsKey(decoratorID))
		{
			_decoratorActivatedCountDict.Remove(decoratorID);
		}
	}
}
