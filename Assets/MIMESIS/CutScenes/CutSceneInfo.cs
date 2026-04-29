using System.Collections.Generic;

public class CutSceneInfo
{
	private List<int> _participantActorIDs = new List<int>();

	private Dictionary<int, CutSceneParticipantState> _syncCompletedActorIDs = new Dictionary<int, CutSceneParticipantState>();

	private long _startTime;

	private long _lastBroadcastTime;

	public string Name { get; private set; }

	public long PlayTime { get; private set; }

	public CutSceneState State { get; private set; }

	public bool NeedToBroadcast { get; private set; }

	public bool IsPlaying => State == CutSceneState.Playing;

	public CutSceneInfo(string name, long playTime, bool needToBroadcast = true)
	{
		Name = name;
		PlayTime = playTime;
		NeedToBroadcast = needToBroadcast;
	}

	public void SetBroadcast(bool needToBroadcast)
	{
		NeedToBroadcast = needToBroadcast;
	}

	public void StartPlay()
	{
		State = CutSceneState.Playing;
		_startTime = Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	public void AddParticipant(List<int> actorID)
	{
		foreach (int item in actorID)
		{
			if (!_participantActorIDs.Contains(item))
			{
				_participantActorIDs.Add(item);
			}
		}
	}

	public void RemoveParticipant(int actorID)
	{
		if (_participantActorIDs.Contains(actorID))
		{
			_participantActorIDs.Remove(actorID);
		}
		if (_syncCompletedActorIDs.ContainsKey(actorID))
		{
			_syncCompletedActorIDs.Remove(actorID);
		}
	}

	public bool IsBroadcastTimeElapsed()
	{
		if (!NeedToBroadcast)
		{
			return false;
		}
		if (!IsPlaying)
		{
			return false;
		}
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (currentTickMilliSec - _lastBroadcastTime >= 500)
		{
			_lastBroadcastTime = currentTickMilliSec;
			return true;
		}
		return false;
	}

	public void Reset()
	{
		State = CutSceneState.Ready;
		_participantActorIDs.Clear();
		_syncCompletedActorIDs.Clear();
	}

	public bool HasParticipant(int actorID)
	{
		return _participantActorIDs.Contains(actorID);
	}

	public bool IsExpired()
	{
		return Hub.s.timeutil.GetCurrentTickMilliSec() - _startTime > PlayTime;
	}

	public bool CheckComplete()
	{
		if (State == CutSceneState.Complete)
		{
			return false;
		}
		if (IsExpired())
		{
			State = CutSceneState.Complete;
			return true;
		}
		return false;
	}
}
