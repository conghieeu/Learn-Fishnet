using System.Collections.Generic;
using System.Threading;

public class ActionQueuePack
{
	private static int _idCounter;

	public readonly int ID;

	public long Delay { get; private set; }

	public long StartTime { get; private set; }

	public Queue<(IGameAction, IGameActionParam?)> ActionQueue { get; private set; } = new Queue<(IGameAction, IGameActionParam)>();

	public static int GetNewID()
	{
		return Interlocked.Increment(ref _idCounter);
	}

	public ActionQueuePack(int delay, Queue<(IGameAction, IGameActionParam?)> actionQueue)
	{
		ID = GetNewID();
		Delay = delay;
		ActionQueue = actionQueue;
	}

	public void StartWait()
	{
		if (Delay != 0L)
		{
			StartTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		}
	}

	public bool IsWaitComplete()
	{
		if (Delay == 0L)
		{
			return true;
		}
		if (StartTime == 0L)
		{
			return true;
		}
		if (Hub.s.timeutil.GetCurrentTickMilliSec() - StartTime >= Delay)
		{
			return true;
		}
		return false;
	}
}
