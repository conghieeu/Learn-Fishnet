using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamworks;

public class SteamLobbyChecker : IDisposable
{
	private Callback<LobbyDataUpdate_t> m_LobbyDataUpdateCallback;

	private CSteamID m_TargetLobbyID;

	private const int LOBBY_DATA_TIMEOUT_MS = 10000;

	private bool m_IsRequestSent;

	private Task m_RequestTask;

	private Action<bool> onLobbyDataUpdatedCallback;

	public bool IsLobbyDataUpdated { get; private set; }

	public bool IsPublicLobby { get; private set; }

	public string VersionFromLobby { get; private set; } = string.Empty;

	public string ServerPortFromLobby { get; private set; } = string.Empty;

	public string LobbyNameFromLobby { get; private set; } = string.Empty;

	public string LocaleFromLobby { get; private set; } = string.Empty;

	public List<int> AppliedSteamItemDefIdListFromLobby { get; private set; } = new List<int>();

	public SteamLobbyChecker(ulong lobbyID)
	{
		m_TargetLobbyID = new CSteamID(lobbyID);
		m_LobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
	}

	public SteamLobbyChecker(ulong lobbyID, Action<bool> onLobbyDataUpdated)
	{
		m_TargetLobbyID = new CSteamID(lobbyID);
		m_LobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
		onLobbyDataUpdatedCallback = onLobbyDataUpdated;
	}

	public void Dispose()
	{
		if (m_RequestTask != null)
		{
			_ = m_RequestTask.IsCompleted;
		}
		m_LobbyDataUpdateCallback.Dispose();
		m_LobbyDataUpdateCallback = null;
		m_IsRequestSent = false;
		m_RequestTask = null;
		onLobbyDataUpdatedCallback = null;
	}

	public void RequestData()
	{
		if (!m_IsRequestSent)
		{
			if (SteamMatchmaking.RequestLobbyData(m_TargetLobbyID))
			{
				m_IsRequestSent = true;
				IsLobbyDataUpdated = false;
				m_RequestTask = WaitForDataOrTimeout();
			}
			else
			{
				HandleRequestCompletion(success: false);
			}
		}
	}

	private async Task WaitForDataOrTimeout()
	{
		Task delayTask = Task.Delay(10000);
		bool isCompleted = false;
		while (m_IsRequestSent && !isCompleted)
		{
			if (delayTask.IsCompleted)
			{
				Logger.RError($"SteamLobbyChecker::WaitForDataOrTimeout : Lobby data request timed out after {10}s");
				HandleRequestCompletion(success: false);
				return;
			}
			await Task.Delay(50);
			if (!m_IsRequestSent)
			{
				isCompleted = true;
			}
		}
		m_RequestTask = null;
	}

	private void HandleRequestCompletion(bool success)
	{
		if (!IsLobbyDataUpdated)
		{
			IsLobbyDataUpdated = true;
			m_IsRequestSent = false;
			onLobbyDataUpdatedCallback?.Invoke(success);
		}
	}

	private void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
	{
		if (!m_IsRequestSent || !new CSteamID(pCallback.m_ulSteamIDLobby).Equals(m_TargetLobbyID))
		{
			return;
		}
		m_IsRequestSent = false;
		if (pCallback.m_bSuccess == 0)
		{
			Logger.RError("SteamLobbyChecker::OnLobbyDataUpdate : Lobby data update failed (Callback success=0)");
			HandleRequestCompletion(success: false);
			return;
		}
		VersionFromLobby = SteamMatchmaking.GetLobbyData(m_TargetLobbyID, "Version");
		ServerPortFromLobby = SteamMatchmaking.GetLobbyData(m_TargetLobbyID, "serverPort");
		LobbyNameFromLobby = SteamMatchmaking.GetLobbyData(m_TargetLobbyID, "LobbyName");
		LocaleFromLobby = SteamMatchmaking.GetLobbyData(m_TargetLobbyID, "Locale");
		string lobbyData = SteamMatchmaking.GetLobbyData(m_TargetLobbyID, "AppliedSteamInventoryItemDefIDs");
		if (!string.IsNullOrEmpty(lobbyData))
		{
			AppliedSteamItemDefIdListFromLobby = lobbyData.Split(',').Select(int.Parse).ToList();
		}
		string lobbyData2 = SteamMatchmaking.GetLobbyData(m_TargetLobbyID, "PublicRoom");
		if (!string.IsNullOrEmpty(lobbyData2) && lobbyData2.Equals("true"))
		{
			IsPublicLobby = true;
		}
		else
		{
			IsPublicLobby = false;
		}
		HandleRequestCompletion(success: true);
	}
}
