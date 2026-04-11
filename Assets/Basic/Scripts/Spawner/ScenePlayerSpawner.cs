using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace Basic
{
    public class ScenePlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _playerPrefab;

        public override void OnStartServer()
        {
            SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
            print("ScenePlayerSpawner OnStartServer");
        }

        public override void OnStopServer()
        {
            if (SceneManager != null)
                SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
            print("ScenePlayerSpawner OnStopServer");
        }

        private void OnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
        {
            // Since this method is called when the client has loaded all start scenes,
            // we need to check if the client is actually in this scene before spawning the player.
            // We use Observers.Contains to see if this connection is observing this object, and
            // thus in this scene. We could have alternatively checked if the connection has this
            // scene loaded like so: connection.Scenes.Contains(gameObject.scene)
            if (asServer && Observers.Contains(connection))
                SpawnPlayer(connection);
            print("ScenePlayerSpawner OnClientLoadedStartScenes " + connection.ClientId + " asServer: " + asServer);
        }

        // This method runs on the server when the client is about to spawn this object.
        // Since the player is about to spawn this object, we know he is in this scene.
        // BUT he may not have loaded all start scenes yet, so we check that.
        public override void OnSpawnServer(NetworkConnection connection)
        {
            if (connection.LoadedStartScenes(true))
                SpawnPlayer(connection);
            print("ScenePlayerSpawner OnSpawnServer " + connection.ClientId);
        }

        private void SpawnPlayer(NetworkConnection connection)
        {
            NetworkObject obj = NetworkManager.GetPooledInstantiated(_playerPrefab, asServer: true);
            Spawn(obj, connection, gameObject.scene);
            print("ScenePlayerSpawner SpawnPlayer " + connection.ClientId);
        }
    }
}