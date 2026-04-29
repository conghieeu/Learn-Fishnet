namespace ReluProtocol.Enum
{
	public enum DisconnectReason
	{
		None = 0,
		ByServer = 1,
		ByClient = 2,
		KickByServer = 3,
		DuplicateLogin = 4,
		InvalidChannelEnter = 5,
		ConnectionError = 6,
		Undefined = 7,
		PacketError = 8
	}
}
