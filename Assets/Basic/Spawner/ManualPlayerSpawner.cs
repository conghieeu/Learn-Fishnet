using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using QFSW.QC;
using System.Linq;
using FishNet;

namespace Basic
{
    public class ManualPlayerSpawner : NetworkBehaviour
    {
        [SerializeField] NetworkObject _playerPrefab;
        [SerializeField] Transform _spawnPos;

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
            {
                SceneManager.AddConnectionToScene(conn, gameObject.scene);
                print("có player " + conn.ClientId + " đã vào scene " + gameObject.scene.name);
                SpawnPlayers();
            }
        }

        // The Server attribute here prevents this method from being called except on the server.
        [Server]
        public void SpawnPlayers()
        {
            if (_playerPrefab == null)
            {
                Debug.LogWarning("Player prefab is not assigned and thus cannot be spawned.");
                return;
            }

            print("SpawnPlayers");

            foreach (NetworkConnection client in ServerManager.Clients.Values)
            {
                // Since the ServerManager.Clients collection contains all clients (even non-authenticated ones),
                // we need to check if they are authenticated first before spawning a player object for them.
                if (!client.IsAuthenticated)
                    continue;

                // If the client isn't observing this scene, make him an observer of it.
                if (!client.Scenes.Contains(gameObject.scene))
                    SceneManager.AddConnectionToScene(client, gameObject.scene);

                NetworkObject obj = NetworkManager.GetPooledInstantiated(_playerPrefab, asServer: true);
                Spawn(obj, client, gameObject.scene);
            }
        }
    }
}