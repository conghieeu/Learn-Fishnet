using Steamworks;

public static class SteamUtil
{
	public static uint app_id = 480u;

	public static string UserSteamID => SteamUser.GetSteamID().ToString();
}
