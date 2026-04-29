using System;
using ReluProtocol;
using ReluProtocol.Enum;

public class ScrapMotionController : IVActorController, IDisposable
{
	private VCreature _self;

	private int _playedMiscItemMasterID;

	private string _playedAniStateName = string.Empty;

	private bool _canMove;

	private long _animationStartTime;

	public VActorControllerType type { get; } = VActorControllerType.ScrapMotion;

	public int PlayedMiscItemMasterID => _playedMiscItemMasterID;

	public string PlayedAniStateName => _playedAniStateName;

	public bool CanMove => _canMove;

	public long AnimationStartTime => _animationStartTime;

	public bool IsPlaying => _playedMiscItemMasterID != 0;

	public ScrapMotionController(VCreature self)
	{
		_self = self;
	}

	public void Initialize()
	{
	}

	public void Update(long deltaTime)
	{
	}

	private void Reset()
	{
		_playedMiscItemMasterID = 0;
		_playedAniStateName = string.Empty;
		_canMove = false;
		_animationStartTime = 0L;
	}

	public MsgErrorCode OnStartScrapMotion(int miscItemMasterID, string aniStateName, bool canMove, ItemElement itemElement, PosWithRot pos, int hashCode)
	{
		MsgErrorCode msgErrorCode = _self.CanAction(VActorActionType.ScrapMotion);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		_playedMiscItemMasterID = miscItemMasterID;
		_playedAniStateName = aniStateName;
		_canMove = canMove;
		_animationStartTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		_self.SetPosition(pos, ActorMoveCause.ScrapMotion);
		_self.SendToMe(new StartScrapMotionRes(hashCode)
		{
			onHandItem = itemElement.toItemInfo()
		});
		_self.SendInSight(new StartScrapMotionSig
		{
			actorID = _self.ObjectID,
			onHandItem = itemElement.toItemInfo(),
			basePosition = pos
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode OnEndScrapMotion(PosWithRot pos, int hashCode)
	{
		if (_playedMiscItemMasterID == 0)
		{
			return MsgErrorCode.CantAction;
		}
		Reset();
		_self.SetPosition(pos, ActorMoveCause.ScrapMotion);
		_self.SendToMe(new EndScrapMotionRes(hashCode));
		_self.SendInSight(new EndScrapMotionSig
		{
			actorID = _self.ObjectID,
			basePosition = pos
		});
		return MsgErrorCode.Success;
	}

	public void OnCancelScrapMotion()
	{
		if (_playedMiscItemMasterID != 0)
		{
			Reset();
			_self.SendInSight(new CancelScrapMotionSig
			{
				actorID = _self.ObjectID
			}, includeSelf: true);
		}
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		if (actionType == VActorActionType.UseLevelObject)
		{
			if (_playedMiscItemMasterID != 0)
			{
				return MsgErrorCode.CantAction;
			}
			return MsgErrorCode.Success;
		}
		return MsgErrorCode.Success;
	}

	public void Dispose()
	{
	}

	public void WaitInitDone()
	{
	}

	public void PostUpdate(long deltaTime)
	{
	}

	public string GetDebugString()
	{
		return string.Empty;
	}

	public void OnMove()
	{
		if (_playedMiscItemMasterID != 0 && !_canMove)
		{
			OnCancelScrapMotion();
		}
	}

	public void OnEquipChanged()
	{
		if (_playedMiscItemMasterID != 0)
		{
			OnCancelScrapMotion();
		}
	}

	public void OnLooting()
	{
		if (_playedMiscItemMasterID != 0)
		{
			OnCancelScrapMotion();
		}
	}

	public void OnAttach()
	{
		if (_playedMiscItemMasterID != 0)
		{
			OnCancelScrapMotion();
		}
	}

	public void OnDead()
	{
		if (_playedMiscItemMasterID != 0)
		{
			OnCancelScrapMotion();
		}
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}
}
