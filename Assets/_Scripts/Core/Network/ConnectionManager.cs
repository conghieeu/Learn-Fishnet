using FishNet.Managing;
using QFSW.QC;
using UnityEngine;

namespace MellowAbelson.Core.Network
{
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

        [Command("startwithip")]
        public void StartWithIp(string ipAddress)
        {
            SetIpAddress(ipAddress);
            StartHost();
        }

        [Command("setipaddress")]
        public void SetIpAddress(string ipAddress)
        {
            _networkManager.TransportManager.Transport.SetClientAddress(ipAddress);
        }
    }
}
