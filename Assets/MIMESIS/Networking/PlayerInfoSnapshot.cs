using System.Collections.Generic;

public class PlayerInfoSnapshot
{
	public long UID { get; private set; }

	public string Name { get; private set; }

	public int MasterID { get; private set; }

	public bool IsHost { get; private set; }

	public string VoiceUID { get; private set; }

	public Dictionary<int, ItemElement> inventories { get; private set; } = new Dictionary<int, ItemElement>();

	public int CurrentInventorySlot { get; private set; } = 1;

	public string Guid { get; private set; } = string.Empty;

	public ulong SteamID { get; private set; }

	public VRoomType RoomType { get; private set; }

	public static PlayerInfoSnapshot Generate(VPlayer player, bool needToSave)
	{
		PlayerInfoSnapshot playerInfoSnapshot = new PlayerInfoSnapshot
		{
			UID = player.UID,
			Name = player.ActorName,
			MasterID = player.MasterID,
			IsHost = player.IsHost,
			VoiceUID = player.VoiceUID,
			RoomType = player.ToMoveRoomType
		};
		if (!needToSave)
		{
			return playerInfoSnapshot;
		}
		foreach (KeyValuePair<int, ItemElement> item in player.InventoryControlUnit.ExtractAllItemElement(turnOff: true))
		{
			playerInfoSnapshot.inventories.Add(item.Key, item.Value);
		}
		playerInfoSnapshot.CurrentInventorySlot = player.InventoryControlUnit.CurrentInventorySlot;
		return playerInfoSnapshot;
	}

	public void ResetRoomType()
	{
		RoomType = VRoomType.Invalid;
	}

	public static PlayerInfoSnapshot Generate(long playerUID, bool isHost, int masterID, string name, string voiceUID, ulong steamID, string guid, VRoomType roomType)
	{
		return new PlayerInfoSnapshot
		{
			UID = playerUID,
			IsHost = isHost,
			MasterID = masterID,
			Name = name,
			VoiceUID = voiceUID,
			SteamID = steamID,
			Guid = guid,
			RoomType = roomType
		};
	}
}
