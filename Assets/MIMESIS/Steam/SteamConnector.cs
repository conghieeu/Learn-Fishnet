using System;
using ReluProtocol.Enum;
using Steamworks;

public class SteamConnector
{
	private IntegrationProviderType _IntegrationProviderType = IntegrationProviderType.STEAM;

	private AppId_t _appId_T;

	private CSteamID _steamId;

	private string _steamSessionTicket;

	private string _steamIdentity;

	public bool SignedIn;

	public bool SignedOut;

	private Action SignOutCallback;

	public MsgErrorCode LastError;

	public ulong SteamId => (ulong)_steamId;

	public ulong JoinedLobbyID
	{
		get
		{
			if (!Hub.s.steamInviteDispatcher.isLobbyCreated)
			{
				return 0uL;
			}
			return (ulong)Hub.s.steamInviteDispatcher.joinedLobbyID;
		}
	}

	public void SignIn()
	{
		if (SteamManager.Initialized)
		{
			Logger.RLog("SignInSteam");
			_appId_T = SteamUtils.GetAppID();
			_steamId = SteamUser.GetSteamID();
			byte[] array = new byte[1024];
			SteamNetworkingIdentity pSteamNetworkingIdentity = default(SteamNetworkingIdentity);
			pSteamNetworkingIdentity.SetSteamID(_steamId);
			SteamUser.GetAuthSessionTicket(array, array.Length, out var pcbTicket, ref pSteamNetworkingIdentity);
			Array.Resize(ref array, (int)pcbTicket);
			_steamSessionTicket = BitConverter.ToString(array).Replace("-", string.Empty);
			pSteamNetworkingIdentity.ToString(out _steamIdentity);
			SignedIn = true;
		}
		else
		{
			Logger.RError("SteamManager.Initialized == false");
		}
	}

	public void SignOut(Action signoutCallback)
	{
		SignedOut = true;
		SignOutCallback?.Invoke();
	}
}
