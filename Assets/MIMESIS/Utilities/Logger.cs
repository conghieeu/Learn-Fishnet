using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class Logger
{
	private enum eLogLevel
	{
		Log = 0,
		Info = 1,
		Debug = 2,
		Warning = 3,
		Error = 4
	}

	public const string CATEGORY_NET = "net";

	public const string CATEGORY_INVENTORY = "inventory";

	public const string CATEGORY_SKILL = "skill";

	public const string CATEGORY_ABNORMAL = "abnormal";

	public const string CATEGORY_AURA = "aura";

	public const string CATEGORY_MOVE = "move";

	public const string CATEGORY_MOVEBUG = "movebug";

	public const string CATEGORY_MAPTRIGGER = "maptrigger";

	public const string CATEGORY_INGAMETIME = "ingametime";

	public const string CATEGORY_STAT = "stat";

	public const string CATEGORY_LEVELOBJECT = "levelobject";

	public const string CATEGORY_DL = "dl";

	public const string CATEGORY_BT = "bt";

	public const string CATEGORY_VOICECHAT = "voicechat";

	public const string CATEGORY_MIMICVOICE = "mimicvoice";

	public const string CATEGORY_PUPPET = "puppet";

	public const string CATEGORY_PROJECTILE = "projectile";

	public const string CATEGORY_PACKET = "packet";

	public const string CATEGORY_WEATHER = "weather";

	public const string CATEGORY_CONTA = "conta";

	public const string CATEGORY_EMOTE = "emote";

	public const string CATEGORY_GRAB = "grab";

	public const string CATEGORY_LEGACYAUDIO = "legacyaudio";

	public const string CATEGORY_NETWORK_PING = "networkping";

	public const string CATEGORY_ANIMEVENT = "animevent";

	public const string CATEGORY_AUDIO = "audio";

	public const string CATEGORY_BLACKOUT = "blackout";

	public const string CATEGORY_SCENEFLOW = "sceneflow";

	public const string CATEGORY_DEATHMATCH = "deathmatch";

	public const string CATEGORY_REPORT = "report";

	public const string CATEGORY_ACCESSORY = "accessory";

	public const string CATEGORY_VOICEEFFECT = "voiceeffect";

	public const string CATEGORY_SCRAP = "scrap";

	public const string CATEGORY_FALL = "fall";

	public const string CATEGORY_SPAWN = "spawn";

	public const string CATEGORY_SAVE = "save";

	public const string CATEGORY_TRAM = "tram";

	public static Action<string>? debugConsoleOutPipe = null;

	public static List<string> LogFilter = new List<string>();

	public static void Init(Action<string> _debugConsoleOutPipe)
	{
		debugConsoleOutPipe = _debugConsoleOutPipe;
		LogFilter.Add("default");
	}

	public static bool CheckLogFilter(string category)
	{
		return LogFilter.Contains(category);
	}

	public static void WantToQuit()
	{
		debugConsoleOutPipe = null;
	}

	private static string CollectStackInfo()
	{
		List<StackFrame> list = new StackTrace(fNeedFileInfo: true).GetFrames().ToList();
		int num = 2;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (StackFrame item in list)
		{
			if (num-- <= 0)
			{
				MethodBase method = item.GetMethod();
				string arg = item.GetFileName()?.Replace('\\', '/') ?? "???";
				int fileLineNumber = item.GetFileLineNumber();
				stringBuilder.AppendLine($"{method} (at {arg}:{fileLineNumber})");
			}
		}
		return stringBuilder.ToString().Replace("\r", "");
	}

	private static void UnityLog(eLogLevel logLevel, string title)
	{
		string text = CollectStackInfo();
		string text2 = $"[{logLevel}] {title}";
		switch (logLevel)
		{
		case eLogLevel.Error:
			text2 = text2 + "\n" + text;
			UnityEngine.Debug.LogError(text2);
			break;
		case eLogLevel.Warning:
			UnityEngine.Debug.LogWarning(text2);
			break;
		default:
			UnityEngine.Debug.Log(text2);
			break;
		}
	}

	[Conditional("DEV")]
	private static void ConsoleLog(string msg)
	{
		if (debugConsoleOutPipe != null)
		{
			debugConsoleOutPipe(msg);
		}
	}

	[Conditional("DEV")]
	public static void RInfo(string message, bool sendToLogServer = false, bool useConsoleOut = true, string category = "default")
	{
		if (CheckLogFilter(category))
		{
		}
	}

	[Conditional("DEV")]
	public static void RDebug(string message, bool sendToLogServer = false, bool useConsoleOut = true, string category = "default")
	{
		if (CheckLogFilter(category))
		{
		}
	}

	public static void RLog(string message, bool sendToLogServer = false, bool useConsoleOut = true, string category = "default")
	{
		if (CheckLogFilter(category))
		{
			message = "[" + TimeUtil.GetCurrentTimeString() + "] " + message;
			UnityLog(eLogLevel.Log, message);
		}
	}

	public static void RWarn(string message, bool sendToLogServer = false, bool useConsoleOut = true, string category = "default")
	{
		if (CheckLogFilter(category))
		{
			message = "[" + TimeUtil.GetCurrentTimeString() + "] " + message;
			UnityLog(eLogLevel.Warning, message);
		}
	}

	public static void RError(string message, bool sendToLogServer = false, string category = "default")
	{
		if (CheckLogFilter(category))
		{
			message = "[" + TimeUtil.GetCurrentTimeString() + "] " + message;
			UnityLog(eLogLevel.Error, message);
		}
	}

	public static void RError(Exception e, bool sendToLogServer = false, string category = "default")
	{
		if (CheckLogFilter(category))
		{
			object obj = e?.ToString() ?? "UnknownException";
			if (obj == null)
			{
				obj = "";
			}
			string text = (string)obj;
			text = "[" + TimeUtil.GetCurrentTimeString() + "] " + text;
			UnityLog(eLogLevel.Error, text);
		}
	}
}
