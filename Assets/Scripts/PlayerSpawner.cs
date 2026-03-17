using FishNet;
using FishNet.Connection;
using FishNet.Object;
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

    private void Start()
    {
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
        else
            Debug.LogWarning("[PlayerSpawner] ⚠ SceneManager chưa sẵn sàng!");
    }

    private void OnDisable()
    {
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
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
