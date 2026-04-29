using System;
using ReluProtocol;
using ReluProtocol.Enum;

public class EmotionController : IVActorController, IDisposable
{
	private VCreature _self;

	private int _playedEmotionMasterID;

	private long _emotionStartTime;

	public VActorControllerType type { get; } = VActorControllerType.Emotion;

	public int PlayedEmotionMasterID => _playedEmotionMasterID;

	public long EmotionStartTime => _emotionStartTime;

	public EmotionController(VCreature self)
	{
		_self = self;
	}

	public void Initialize()
	{
	}

	public void Update(long deltaTime)
	{
	}

	public MsgErrorCode OnEmotion(int masterID, PosWithRot pos, int hashCode)
	{
		MsgErrorCode msgErrorCode = _self.CanAction(VActorActionType.Emotion);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		float num = Misc.Distance(_self.PositionVector, pos.pos, ignoreHeight: true);
		if (!Misc.ValidateMoveSpped(_self.Position, pos, 10f).valid)
		{
			Logger.RError($"[DirectMoveContext] StartMove failed. distance:{num}");
			return MsgErrorCode.WillBeTeleported;
		}
		_playedEmotionMasterID = masterID;
		_emotionStartTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		_self.SetPosition(pos, ActorMoveCause.Emotion);
		_self.SendToMe(new EmotionRes(hashCode)
		{
			emotionMasterID = masterID
		});
		_self.SendInSight(new EmotionSig
		{
			actorID = _self.ObjectID,
			basePosition = pos,
			emotionMasterID = _playedEmotionMasterID
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		return MsgErrorCode.Success;
	}

	public void OnCancelEmotion()
	{
		if (_playedEmotionMasterID != 0)
		{
			_playedEmotionMasterID = 0;
			_emotionStartTime = 0L;
			_self.SendInSight(new CancelEmotionSig
			{
				actorID = _self.ObjectID
			}, includeSelf: true);
		}
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

	public void OnSkill()
	{
		if (_playedEmotionMasterID != 0)
		{
			OnCancelEmotion();
		}
	}

	public void OnMove()
	{
		if (_playedEmotionMasterID != 0)
		{
			OnCancelEmotion();
		}
	}

	public void OnEquipChanged()
	{
		if (_playedEmotionMasterID != 0)
		{
			OnCancelEmotion();
		}
	}

	public void OnLooting()
	{
		if (_playedEmotionMasterID != 0)
		{
			OnCancelEmotion();
		}
	}

	public void OnAttach()
	{
		if (_playedEmotionMasterID != 0)
		{
			OnCancelEmotion();
		}
	}

	public void OnDead()
	{
		if (_playedEmotionMasterID != 0)
		{
			OnCancelEmotion();
		}
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}
}
