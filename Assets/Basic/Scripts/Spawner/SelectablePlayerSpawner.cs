using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using System.Linq;
using UnityEngine;

namespace Basic
{
    public class SelectablePlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject[] _playerPrefabs;

        public override void OnStartServer()
        {
            SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
        }

        public override void OnStopServer()
        {
            if (SceneManager != null)
                SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
        }

        private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            if (asServer && !conn.Scenes.Contains(gameObject.scene))
                SceneManager.AddConnectionToScene(conn, gameObject.scene);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayer(NetworkObject _playerPrefab, NetworkConnection sender = null)
        {
            if (sender.FirstObject != null)
            {
                Debug.LogWarning($"Client {sender.ClientId} already has a player object; not spawning another.");
                return;
            }

            if (!_playerPrefabs.Contains(_playerPrefab))
            {
                Debug.LogWarning("Invalid player prefab selected, cannot spawn.");
                // You don't have to kick the player, but there isn't any good reason 
                // they should be trying to spawn a non-permitted prefab.
                sender.Kick(FishNet.Managing.Server.KickReason.ExploitAttempt);
                return;
            }

            NetworkObject obj = NetworkManager.GetPooledInstantiated(_playerPrefab, asServer: true);
            Spawn(obj, sender, gameObject.scene);
        }
    }
}