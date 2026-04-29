using System.Diagnostics;

public static class ReluPerf
{
	[Conditional("USE_THINPERFHUD")]
	public static void AtStart(PerfCat cat)
	{
		if (!(Hub.s == null))
		{
			_ = Hub.s.thinPerfHud == null;
		}
	}

	[Conditional("USE_THINPERFHUD")]
	public static void AtEnd(PerfCat cat)
	{
		if (!(Hub.s == null))
		{
			_ = Hub.s.thinPerfHud == null;
		}
	}

	[Conditional("USE_THINPERFHUD")]
	public static void SetCustomHUDScale(int miliseconds)
	{
		if (!(Hub.s == null))
		{
			_ = Hub.s.thinPerfHud == null;
		}
	}
}
