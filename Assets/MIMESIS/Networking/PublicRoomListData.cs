using System;
using Steamworks;

[Serializable]
public class PublicRoomListData
{
	public CSteamID lobbyID;

	public string locale;

	public string lobbyName;

	public int cycle;

	public int repairStatus;

	public int PlayerCount;
}
