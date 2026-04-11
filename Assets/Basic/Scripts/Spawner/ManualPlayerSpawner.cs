using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using QFSW.QC;
using System.Linq;
using System.Collections.Generic;
using FishNet;

namespace Basic
{
    public class ManualPlayerSpawner : NetworkBehaviour
    {
        [SerializeField] NetworkObject _playerPrefabs = new();
        [SerializeField] Transform _spawnPos;

        // private readonly Dictionary<NetworkConnection, int> _clientPrefabIndices = new();

        public override void OnStartServer()
        {
            SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
        }

        public override void OnStopServer()
        {
            SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
        }

        private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            if (asServer && !conn.Scenes.Contains(gameObject.scene))
            {
                SpawnPlayerForClient(conn);

                // in ra conn nào mới vừa được load xong start scenes và được spawn player
                Debug.Log($"Spawned player for connection {conn.ClientId} after loading start scenes.");
            }
        }

        // The Server attribute here prevents this method from being called except on the server.
        [Server]
        public void SpawnPlayerForClient(NetworkConnection conn)
        {
            if (_playerPrefabs == null)
            {
                Debug.LogWarning("No player prefab assigned and thus cannot be spawned.");
                return;
            }

            // Kiểm tra xem client đã được xác thực chưa
            if (!conn.IsAuthenticated)
                return;

            // Nếu client chưa có quyền truy cập vào scene này thì thêm quyền truy cập
            if (!conn.Scenes.Contains(gameObject.scene))
                SceneManager.AddConnectionToScene(conn, gameObject.scene);

            // Xác định prefab index cho client (xoay vòng nếu quá số lượng)
            // int prefabIndex = _clientPrefabIndices.Count % _playerPrefabs.Count;
            // _clientPrefabIndices[conn] = prefabIndex;

            NetworkObject prefab = _playerPrefabs;
            NetworkObject obj = NetworkManager.GetPooledInstantiated(prefab, asServer: true);

            if (obj == null)
            {
                Debug.LogError($"Failed to instantiate player prefab {prefab.name} from pool.");
                return;
            }

            Spawn(obj, conn, gameObject.scene);
        }
    }
}