using FishNet.Managing;
using QFSW.QC;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;

    public void StartHost()
    {
        StartClient();
        StartServer();
    }

    [Command("startclient")]
    public void StartClient()
    {
        _networkManager.ClientManager.StartConnection();
    }

    [Command("startserver")]
    public void StartServer()
    {
        _networkManager.ServerManager.StartConnection();
    }

    // start server and client with a specific IP address
    [Command("startwithip")]
    public void StartWithIp()
    {
        StartServer();
        StartClient();
    }

    [Command("setipaddress")]
    public void SetIpAddress(string ipAddress)
    {
        _networkManager.TransportManager.Transport.SetClientAddress(ipAddress);
    }
}
