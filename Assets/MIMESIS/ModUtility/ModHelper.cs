using System;

namespace ModUtility
{
	public static class ModHelper
	{
		public enum eTiming
		{
			EnterMainMenu = 0,
			ExitMainMenu = 1,
			EnterMaintenance = 2,
			ExitMaintenance = 3,
			EnterTram = 4,
			ExitTram = 5,
			EnterGame = 6,
			ExitGame = 7,
			EnterDeathMatch = 8,
			ExitDeathMatch = 9
		}

		private static Action<eTiming> timingCallback;

		public static ScriptableUIUtility GetScriptableUIUtility()
		{
			if (Hub.s == null)
			{
				return null;
			}
			if (Hub.s.uiman == null)
			{
				return null;
			}
			return Hub.s.uiman.GetScriptableUI();
		}

		public static void SetTimingCallback(Action<eTiming> callback)
		{
			timingCallback = callback;
		}

		public static void InvokeTimingCallback(eTiming timing)
		{
			timingCallback?.Invoke(timing);
		}
	}
}
