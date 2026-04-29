public class ServerAddrInfo
{
	public string Host;

	public int Port;

	public bool SteamRelay;

	public int TryCount;

	public ServerAddrInfo(string host, int port, bool steamRelay = false)
	{
		Host = host;
		Port = port;
		SteamRelay = steamRelay;
		TryCount = 0;
	}
}
