using System;
using System.Collections.Generic;
using System.Threading;

namespace ReluServerBase.Threading
{
	public class EventTimer : IDisposable
	{
		public class STimer
		{
			public long LastEvent;

			public long Duration;

			public bool RepeatMode;

			public OnTimerHandler? onTimerAlarm;

			public int TimerID;

			public string? TimerName;

			public long ElapsedTime => Hub.s.timeutil.GetCurrentTickMilliSec() - LastEvent;

			public int GetRemainTime()
			{
				return (int)((double)(Duration - ElapsedTime) * 0.001);
			}

			public long GetEndTime()
			{
				return Hub.s.timeutil.GetCurrentTickMilliSec() + (Duration - ElapsedTime);
			}
		}

		public int TimeIDInc = 1;

		private Dictionary<string, int> TimerNameDict = new Dictionary<string, int>();

		private Dictionary<int, STimer> Timers = new Dictionary<int, STimer>();

		private bool disposedValue;

		~EventTimer()
		{
			Dispose(disposing: false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Clear();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			Timers.Clear();
			TimerNameDict.Clear();
		}

		public void Update()
		{
			try
			{
				List<STimer> list = new List<STimer>();
				foreach (KeyValuePair<int, STimer> timer in Timers)
				{
					if (timer.Value.ElapsedTime > timer.Value.Duration)
					{
						list.Add(timer.Value);
					}
				}
				foreach (STimer item in list)
				{
					if (!item.RepeatMode)
					{
						string timerName = item.TimerName;
						if (timerName != null)
						{
							TimerNameDict.Remove(timerName);
						}
						Timers.Remove(item.TimerID);
					}
					item.onTimerAlarm?.Invoke();
					item.LastEvent = Hub.s.timeutil.GetCurrentTickMilliSec();
				}
			}
			catch (Exception arg)
			{
				Logger.RError($"[EventTimer] Update Exception: {arg}");
			}
		}

		public int CreateTimerEvent(OnTimerHandler onTimerHandler, long duration, bool repeat = false, string? timerName = null)
		{
			STimer sTimer = new STimer
			{
				LastEvent = Hub.s.timeutil.GetCurrentTickMilliSec(),
				onTimerAlarm = onTimerHandler,
				Duration = duration,
				RepeatMode = repeat,
				TimerID = Interlocked.Increment(ref TimeIDInc),
				TimerName = timerName
			};
			if (!Timers.TryAdd(sTimer.TimerID, sTimer))
			{
				return -1;
			}
			if (timerName != null)
			{
				TimerNameDict.TryAdd(timerName, sTimer.TimerID);
			}
			return sTimer.TimerID;
		}

		public bool RemoveTimerEvent(string timerName)
		{
			if (!TimerNameDict.TryGetValue(timerName, out var value))
			{
				return false;
			}
			Timers.Remove(value);
			TimerNameDict.Remove(timerName);
			return true;
		}

		public bool RemoveTimerEvent(int timerID)
		{
			if (!Timers.TryGetValue(timerID, out STimer value))
			{
				return false;
			}
			Timers.Remove(value.TimerID);
			if (value.TimerName != null)
			{
				TimerNameDict.Remove(value.TimerName);
			}
			return true;
		}

		public bool Exist(string timerName)
		{
			return TimerNameDict.ContainsKey(timerName);
		}

		public int GetRemainTime(string timerName)
		{
			if (!TimerNameDict.TryGetValue(timerName, out var value))
			{
				return 0;
			}
			if (!Timers.TryGetValue(value, out STimer value2))
			{
				return 0;
			}
			return value2.GetRemainTime();
		}

		public long GetEndTime(string timerName)
		{
			if (!TimerNameDict.TryGetValue(timerName, out var value))
			{
				return 0L;
			}
			if (!Timers.TryGetValue(value, out STimer value2))
			{
				return 0L;
			}
			return value2.GetEndTime();
		}

		public long GetDuration(string timerName)
		{
			if (!TimerNameDict.TryGetValue(timerName, out var value))
			{
				return 0L;
			}
			if (!Timers.TryGetValue(value, out STimer value2))
			{
				return 0L;
			}
			return value2.Duration;
		}
	}
}
