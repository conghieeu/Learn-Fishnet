using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.CrashReportHandler;

public class TimeUtil : MonoBehaviour
{
	public const float MILLISEC_TO_SEC_MAGNIFICANT = 0.001f;

	public const int SEC_TO_MILLISEC_MAGNIFICANT = 1000;

	private static Stopwatch? m_TickBase;

	private static bool logLoadedAssemblies;

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] TimeUtil.Awake ->");
		Initialize();
		Logger.RLog("[AwakeLogs] TimeUtil.Awake <-");
	}

	private void OnDestroy()
	{
		m_TickBase.Stop();
		m_TickBase = null;
	}

	private void LogLoadedAssemblies()
	{
		if (!logLoadedAssemblies && Application.isPlaying && !Application.isEditor)
		{
			List<string> values = (from a in AppDomain.CurrentDomain.GetAssemblies()
				select a.GetName().Name).ToList();
			string text = string.Join(", ", values);
			Logger.RLog("Loaded Assemblies : " + text);
			logLoadedAssemblies = true;
		}
	}

	private void Initialize()
	{
		if (m_TickBase == null)
		{
			m_TickBase = new Stopwatch();
			m_TickBase.Start();
			LogLoadedAssemblies();
			Logger.RLog("GitBranchName: EA");
			Logger.RLog("GitRevision: 919187f474");
			Logger.RLog("BuildVersion: 0.2.7");
			CrashReportHandler.SetUserMetadata("build_git_revision", "919187f474");
			CrashReportHandler.SetUserMetadata("build_git_branch_name", "EA");
			CrashReportHandler.SetUserMetadata("build_version", "0.2.7");
			Logger.RLog("GameStartTime (UTC): " + GetCurrentTimeString());
			CrashReportHandler.SetUserMetadata("analysis_filter", string.Join("_", "EA", "0.2.7", "919187f474"));
			CrashReportHandler.SetUserMetadata("game_startTime_UTC", GetCurrentTimeString());
			CrashReportHandler.SetUserMetadata("game_startTime_local", GetCurrentTimeString(useUTC: false));
		}
	}

	public static string GetCurrentTimeString(bool useUTC = true)
	{
		if (useUTC)
		{
			return DateTime.UtcNow.ToString("UTC:yyyy-MM-dd HH:mm:ss.fff");
		}
		return DateTime.Now.ToString("Local:yyyy-MM-dd HH:mm:ss.fff");
	}

	public DateTime ToDateTime(long timestamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
	}

	public DateTime GetTimestamp(bool useUTC = true)
	{
		if (useUTC)
		{
			return DateTime.UtcNow;
		}
		return DateTime.Now;
	}

	public long GetTimeStampInMilliSec(bool useUTC = true)
	{
		if (useUTC)
		{
			return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}
		return DateTimeOffset.Now.ToUnixTimeMilliseconds();
	}

	public long GetTimeStampDiffInMilliSec(DateTime prev, DateTime next)
	{
		return (long)(next - prev).TotalMilliseconds;
	}

	public long GetTimeStampDiffInSec(DateTime prev, DateTime next)
	{
		return (long)(next - prev).TotalSeconds;
	}

	public long GetTargetTimeStampInMilliSec(DateTime time)
	{
		return ((DateTimeOffset)time).ToUnixTimeMilliseconds();
	}

	public long GetTimeStampDiffMilliSec(long value)
	{
		return Math.Abs(GetTimeStampInMilliSec() - value);
	}

	public long GetTargetTimestampOfTodayMillisec(int hour, int minute, int second, bool useUTC = true)
	{
		DateTime timestamp = GetTimestamp();
		return GetTargetTimeStampInMilliSec(new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, hour, minute, second, useUTC ? DateTimeKind.Utc : DateTimeKind.Local).ToUniversalTime());
	}

	public DateTime GetTargetTimestampOfToday(int hour, int minute, int second, bool useUTC = true)
	{
		DateTime timestamp = GetTimestamp();
		return new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, hour, minute, second, useUTC ? DateTimeKind.Utc : DateTimeKind.Local);
	}

	public DateTime GetMidnightBefore(int dayCount, bool useUTC = true)
	{
		DateTime dateTime = GetTimestamp().AddDays(-dayCount);
		return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, useUTC ? DateTimeKind.Utc : DateTimeKind.Local);
	}

	public bool IsInPeriod((int hour, int minute, int second) start, (int hour, int minute, int second) end, bool useUTC = true)
	{
		DateTime timestamp = GetTimestamp();
		DateTime time = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, 0, 0, 0, useUTC ? DateTimeKind.Utc : DateTimeKind.Local).AddHours(start.hour).AddMinutes(start.minute).AddSeconds(start.second)
			.ToUniversalTime();
		DateTime time2 = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, 0, 0, 0, useUTC ? DateTimeKind.Utc : DateTimeKind.Local).AddHours(end.hour).AddMinutes(end.minute).AddSeconds(end.second)
			.ToUniversalTime();
		long targetTimeStampInMilliSec = GetTargetTimeStampInMilliSec(time);
		long targetTimeStampInMilliSec2 = GetTargetTimeStampInMilliSec(time2);
		long timeStampInMilliSec = GetTimeStampInMilliSec();
		if (timeStampInMilliSec >= targetTimeStampInMilliSec)
		{
			return timeStampInMilliSec <= targetTimeStampInMilliSec2;
		}
		return false;
	}

	public bool IsInPeriod(DateTime start, DateTime end, bool useUTC = true)
	{
		DateTime timestamp = GetTimestamp(useUTC);
		if (timestamp >= start)
		{
			return timestamp <= end;
		}
		return false;
	}

	public bool IsAfter(DateTime timestamp)
	{
		return GetTimestamp() >= timestamp;
	}

	public bool IsBefore(DateTime timestamp)
	{
		return GetTimestamp() <= timestamp;
	}

	public bool IsBefore(DateTime sourceTime, DateTime destTime)
	{
		return destTime <= sourceTime;
	}

	public bool IsToday(DateTime timestamp)
	{
		DateTime timestamp2 = GetTimestamp();
		if (timestamp2.Year == timestamp.Year && timestamp2.Month == timestamp.Month)
		{
			return timestamp2.Day == timestamp.Day;
		}
		return false;
	}

	public long GetRemainTimeOffset(int targetHour, int targetMinute, int offsetMinute, bool useUTC = true)
	{
		DateTime timestamp = GetTimestamp();
		long targetTimeStampInMilliSec = GetTargetTimeStampInMilliSec(new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, 0, 0, 0, useUTC ? DateTimeKind.Utc : DateTimeKind.Local).AddHours(targetHour).AddMinutes(targetMinute + offsetMinute).ToUniversalTime());
		long timeStampInMilliSec = GetTimeStampInMilliSec();
		if (targetTimeStampInMilliSec <= timeStampInMilliSec)
		{
			return 0L;
		}
		return targetTimeStampInMilliSec - timeStampInMilliSec;
	}

	public float ChangeTimeMilli2Sec(long timeMilliSec)
	{
		return Convert.ToSingle((float)timeMilliSec * 0.001f, CultureInfo.InvariantCulture);
	}

	public long ChangeTimeSec2Milli(int timeSec)
	{
		return timeSec * 1000;
	}

	public long ChangeTimeSec2Milli(float timeSec)
	{
		return (long)(timeSec * 1000f);
	}

	public long ChangeTimeSec2Milli(double timeSec)
	{
		return (long)(timeSec * 1000.0);
	}

	public long GetCurrentTickMilliSec()
	{
		return m_TickBase.ElapsedMilliseconds;
	}

	public long GetInfiniteTickMilliSec()
	{
		return m_TickBase.ElapsedMilliseconds + (long)TimeSpan.FromDays(1.0).TotalMilliseconds;
	}

	public long GetCurrentTickDiffMilliSec(long value)
	{
		return GetCurrentTickMilliSec() - value;
	}

	public long GetCurrentTickSec()
	{
		return m_TickBase.ElapsedMilliseconds / 1000;
	}

	public long GetRemainTime(long duration, long startTime)
	{
		return duration - (GetCurrentTickMilliSec() - startTime);
	}

	public long GetElapsedTime(long startTime)
	{
		return GetCurrentTickMilliSec() - startTime;
	}

	public double GetCurrentTickMilliSecDetail()
	{
		return m_TickBase.Elapsed.TotalMilliseconds;
	}
}
