using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

namespace MellowAbelson.Networking.Spawner
{
    public class PlayerSpawnManager : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> _playerPrefabs;
        [SerializeField] private List<Transform> _spawnPoints;
        [SyncVar] private int _playerCount;

        public void SpawnPlayer(NetworkConnection connection, int prefabIndex = 0)
        {
            if (!IsServer) return;

            prefabIndex = Mathf.Clamp(prefabIndex, 0, _playerPrefabs.Count - 1);
            Transform spawnPoint = GetSpawnPoint();

            GameObject playerObj = Instantiate(_playerPrefabs[prefabIndex],
                spawnPoint.position, spawnPoint.rotation);
            ServerManager.Spawn(playerObj, connection);
            _playerCount++;
        }

        [Server]
        private Transform GetSpawnPoint()
        {
            if (_spawnPoints.Count == 0) return transform;
            return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        }
    }
}
