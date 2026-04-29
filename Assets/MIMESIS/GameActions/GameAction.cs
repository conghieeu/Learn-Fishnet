using Bifrost.ConstEnum;

public abstract class GameAction : IGameAction
{
	private GameActionComplete? _deleFunc;

	public DefAction ActionType { get; private set; }

	public GameActionState State { get; protected set; }

	public GameAction(DefAction actionType)
	{
		ActionType = actionType;
	}

	public abstract bool Correct(IGameAction action);

	public abstract void Clone(ref IGameAction? action);

	public abstract IGameAction Clone();

	public virtual string GetActionName()
	{
		return ActionType.ToString();
	}

	public virtual bool Progress()
	{
		if (State != GameActionState.Ready)
		{
			return false;
		}
		State = GameActionState.Progress;
		return true;
	}

	public virtual void SetComplete()
	{
		if (State != GameActionState.Failed && State != GameActionState.Complete)
		{
			State = GameActionState.Complete;
		}
	}

	public virtual void SetFailed()
	{
		if (State != GameActionState.Complete && State != GameActionState.Failed)
		{
			State = GameActionState.Failed;
		}
	}

	public virtual bool IsComplete()
	{
		return _deleFunc?.Invoke() ?? true;
	}

	public GameActionParamType GetLinkedParamType()
	{
		switch (ActionType)
		{
		case DefAction.SPAWN_FIELD_SKILL:
		case DefAction.SPAWN_FIELD_SKILL_RANDOM:
		case DefAction.REMOVE_FIELD_SKILL_NEARBY:
			return GameActionParamType.Position;
		case DefAction.SESSION_DECISION:
		case DefAction.CHANGE_MUTABLE_STAT:
		case DefAction.CHANGE_MUTABLE_STAT_RANDOM:
		case DefAction.DECREASE_ITEM_COUNT:
		case DefAction.ADD_ABNORMAL:
		case DefAction.TELEPORT:
		case DefAction.HANDLE_LEVELOBJECT:
		case DefAction.INSTANT_KILL:
		case DefAction.CHARGE_ITEM:
		case DefAction.RANDOM_TELEPORT:
		case DefAction.SETINVINCIBLE:
			return GameActionParamType.Actor;
		default:
			return GameActionParamType.None;
		}
	}

	public virtual void RegisterCompleteDelegate(GameActionComplete deleFunc)
	{
		_deleFunc = deleFunc;
	}

	public bool HasCompleteChecker()
	{
		return _deleFunc != null;
	}
}
