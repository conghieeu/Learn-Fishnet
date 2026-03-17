using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Cài đặt Spawn")]
    [Tooltip("Kéo prefab Player vào đây (prefab phải có NetworkObject)")]
    [SerializeField] private NetworkObject playerPrefab;

    [Tooltip("Vị trí spawn (để trống thì spawn tại Vector3.zero)")]
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        // Dùng SceneManager.OnClientLoadedStartScenes — event này chạy SAU KHI
        // FishNet đã set xong flag LoadedStartScenes, đảm bảo an toàn để spawn.
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
        else
            Debug.LogWarning("PlayerSpawner: SceneManager chưa sẵn sàng.");
    }

    private void OnDisable()
    {
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
    }

    /// <summary>
    /// Được gọi SAU KHI client đã tải xong start scenes và flag đã được set.
    /// </summary>
    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        // Chỉ xử lý trên server
        if (!asServer) return;

        SpawnPlayer(conn);
    }

    /// <summary>
    /// Spawn player prefab cho connection chỉ định.
    /// </summary>
    private void SpawnPlayer(NetworkConnection conn)
    {
        // Chọn vị trí spawn
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // Chọn spawn point theo thứ tự vòng tròn dựa trên ClientId
            int index = (int)(conn.ClientId % spawnPoints.Length);
            spawnPos = spawnPoints[index].position;
            spawnRot = spawnPoints[index].rotation;
        }

        // Tạo player object trên server
        NetworkObject playerObj = Instantiate(playerPrefab, spawnPos, spawnRot);

        // Spawn trên mạng và gán quyền sở hữu cho client đó
        InstanceFinder.ServerManager.Spawn(playerObj, conn);

        Debug.Log($"Đã spawn player cho Client ID: {conn.ClientId} tại {spawnPos}");
    }
}
