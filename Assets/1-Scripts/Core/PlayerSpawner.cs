using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

/// <summary>
/// Quản lý spawn player theo luồng chuẩn FishNet:
/// 1. Client kết nối → Server load data → Spawn tại vị trí đã lưu
/// 2. Auto-save khi despawn được xử lý bởi Player.OnStopServer()
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Cài đặt Spawn")]
    [Tooltip("Kéo prefab Player vào đây (prefab phải có NetworkObject)")]
    [SerializeField] private NetworkObject playerPrefab;

    [Tooltip("Vị trí spawn mặc định (cho player mới chưa có save)")]
    [SerializeField] private Transform[] spawnPoints;

    private bool isSubscribed = false;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (isSubscribed) return;

        // Đăng ký event spawn player
        if (InstanceFinder.SceneManager != null)
        {
            InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
            Debug.Log("[PlayerSpawner] ✅ Đã đăng ký OnClientLoadedStartScenes");
        }

        // Đăng ký event server state để tự động re-subscribe khi server restart
        if (InstanceFinder.ServerManager != null)
        {
            InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;
            Debug.Log("[PlayerSpawner] ✅ Đã đăng ký OnServerConnectionState");
        }

        isSubscribed = true;
    }

    private void UnsubscribeEvents()
    {
        if (!isSubscribed) return;

        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;

        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;

        isSubscribed = false;
    }

    /// <summary>
    /// Khi server start/stop → re-subscribe lại event để đảm bảo hoạt động sau khi thoát phòng rồi tạo lại.
    /// </summary>
    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("[PlayerSpawner] 🔄 Server đã khởi động lại → đăng ký lại events");

            // Đăng ký lại event spawn (đảm bảo không bị duplicate)
            if (InstanceFinder.SceneManager != null)
            {
                InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
                InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
            }
        }
    }

    /// <summary>
    /// Client đã tải xong scene → Load data → Spawn player.
    /// </summary>
    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        if (!asServer) return;

        Debug.Log($"[PlayerSpawner] 🔔 Client {conn.ClientId} đã tải xong scene, bắt đầu spawn...");
        SpawnPlayer(conn);
    }

    /// <summary>
    /// LUỒNG CHÍNH: Load data trước → Spawn player tại vị trí đã lưu.
    /// </summary>
    private void SpawnPlayer(NetworkConnection conn)
    {
        int clientId = (int)conn.ClientId;

        // Vị trí mặc định
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int index = clientId % spawnPoints.Length;
            spawnPos = spawnPoints[index].position;
            spawnRot = spawnPoints[index].rotation;
        }

        // ===== BƯỚC 1: Load data từ save =====
        PlayerData? savedData = null;

        if (PlayerDataManager.Instance != null)
        {
            savedData = PlayerDataManager.Instance.LoadPlayerData(clientId);
        }
        else
        {
            Debug.LogWarning("[PlayerSpawner] ⚠ PlayerDataManager chưa có trên scene!");
        }

        if (savedData.HasValue)
        {
            spawnPos = savedData.Value.Position;
            spawnRot = savedData.Value.Rotation;
            Debug.Log($"[PlayerSpawner] 📂 Client {clientId}: CÓ save data → spawn tại {spawnPos}");
        }
        else
        {
            Debug.Log($"[PlayerSpawner] 🆕 Client {clientId}: CHƯA có save → spawn tại spawn point mặc định {spawnPos}");
        }

        // ===== BƯỚC 2: Spawn player =====
        NetworkObject playerObj = Instantiate(playerPrefab, spawnPos, spawnRot);
        InstanceFinder.ServerManager.Spawn(playerObj, conn);
        Debug.Log($"[PlayerSpawner] ✅ Đã spawn player cho Client {clientId} tại {spawnPos}");

        // ===== BƯỚC 3: Nạp inventory (nếu có save) =====
        if (savedData.HasValue && savedData.Value.InventoryItems != null)
        {
            InventoryHandler inventory = playerObj.GetComponent<InventoryHandler>();
            if (inventory != null)
            {
                inventory.SetSlots(savedData.Value.InventoryItems);
                Debug.Log($"[PlayerSpawner] 📦 Client {clientId}: Đã nạp {savedData.Value.InventoryItems.Count} slots inventory");
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawner] ⚠ Client {clientId}: Không tìm thấy InventoryHandler trên player!");
            }
        }
    }
}
