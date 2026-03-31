using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CountBasedPlayerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private int _requiredPlayerCount;

    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = GetComponentInParent<NetworkManager>();
        if (_networkManager == null)
            _networkManager = InstanceFinder.NetworkManager;

        if (_networkManager == null)
        {
            Debug.LogWarning($"CountBasedPlayerSpawner cannot work as a NetworkManager couldn't be found.");
            return;
        }

        _networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
    }

    private void OnDestroy()
    {
        if (_networkManager != null)
            _networkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
    }

    private void OnClientLoadedStartScenes(NetworkConnection _, bool asServer)
    {
        if (!asServer)
            return;

        List<NetworkConnection> authenticatedClients = _networkManager.ServerManager.Clients.Values
            .Where(conn => conn.IsAuthenticated).ToList();

        if (authenticatedClients.Count < _requiredPlayerCount) return;

        foreach (NetworkConnection client in authenticatedClients)
        {
            NetworkObject obj = _networkManager.GetPooledInstantiated(_playerPrefab, asServer: true);
            _networkManager.ServerManager.Spawn(obj, client);

            // If the client isn't observing this scene, make him an observer of it.
            if (!client.Scenes.Contains(gameObject.scene))
                _networkManager.SceneManager.AddOwnerToDefaultScene(obj);
        }
    }
}