using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

/// <summary>
/// Spawn player khi client kết nối.
/// Tự động re-subscribe khi server restart (thoát phòng rồi tạo lại).
/// Load data được xử lý bởi Player.cs sau khi client gửi tên lên server.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Cài đặt Spawn")]
    [SerializeField] private NetworkObject playerPrefab;

    [Tooltip("Vị trí spawn mặc định (cho player mới chưa có save)")]
    [SerializeField] private Transform[] spawnPoints;

    private void OnEnable()
    {
        // Đăng ký event spawn
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;

        // Đăng ký event server state để re-subscribe khi server restart
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;
    }

    private void OnDisable()
    {
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;

        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;
    }

    /// <summary>
    /// Khi server start lại → re-subscribe event spawn.
    /// </summary>
    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("[PlayerSpawner] 🔄 Server khởi động → đăng ký lại events");

            if (InstanceFinder.SceneManager != null)
            {
                InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
                InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
            }
        }
    }

    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        if (!asServer) return;

        Debug.Log($"[PlayerSpawner] 🔔 Client {conn.ClientId} đã tải xong scene");
        SpawnPlayer(conn);
    }

    /// <summary>
    /// Spawn player tại vị trí mặc định.
    /// Data sẽ được load SAU khi client gửi tên lên (xem Player.ServerSetPlayerName).
    /// </summary>
    private void SpawnPlayer(NetworkConnection conn)
    {
        int clientId = (int)conn.ClientId;

        // Chọn vị trí spawn mặc định
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int index = clientId % spawnPoints.Length;
            spawnPos = spawnPoints[index].position;
            spawnRot = spawnPoints[index].rotation;
        }

        // Spawn player
        NetworkObject playerObj = Instantiate(playerPrefab, spawnPos, spawnRot);
        InstanceFinder.ServerManager.Spawn(playerObj, conn);

        Debug.Log($"[PlayerSpawner] ✅ Đã spawn player cho Client {clientId} tại {spawnPos}");
        Debug.Log($"[PlayerSpawner] ⏳ Đang chờ client gửi tên để load data...");
    }
}
