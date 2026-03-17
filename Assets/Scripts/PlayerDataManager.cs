using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dữ liệu lưu trữ của một player.
/// </summary>
[System.Serializable]
public struct PlayerData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public List<ItemSaveData> InventoryItems;

    /// <summary>
    /// Tạo dữ liệu mặc định (chưa có save).
    /// </summary>
    public static PlayerData Default(Vector3 spawnPos, Quaternion spawnRot, int maxSlots)
    {
        var emptySlots = new List<ItemSaveData>();
        for (int i = 0; i < maxSlots; i++)
            emptySlots.Add(ItemSaveData.Empty());

        return new PlayerData
        {
            Position = spawnPos,
            Rotation = spawnRot,
            InventoryItems = emptySlots
        };
    }
}

/// <summary>
/// Quản lý việc Save/Load dữ liệu player bằng Easy Save 3.
/// Đặt trên Scene (không phải trên Player prefab).
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Lấy ES3 key theo clientId.
    /// </summary>
    private string GetKey(int clientId) => $"Player_{clientId}";

    /// <summary>
    /// Kiểm tra player đã có dữ liệu lưu chưa.
    /// </summary>
    public bool HasSaveData(int clientId)
    {
        return ES3.KeyExists(GetKey(clientId));
    }

    /// <summary>
    /// Lưu toàn bộ dữ liệu của player.
    /// </summary>
    public void SavePlayerData(int clientId, Vector3 position, Quaternion rotation, List<ItemSaveData> inventoryItems)
    {
        PlayerData data = new PlayerData
        {
            Position = position,
            Rotation = rotation,
            InventoryItems = inventoryItems
        };

        ES3.Save(GetKey(clientId), data);
        Debug.Log($"[PlayerDataManager] Đã lưu data cho Client {clientId} tại {position}");
    }

    /// <summary>
    /// Tải dữ liệu player. Nếu chưa có save thì trả về null.
    /// </summary>
    public PlayerData? LoadPlayerData(int clientId)
    {
        if (!HasSaveData(clientId))
        {
            Debug.Log($"[PlayerDataManager] Client {clientId} chưa có save data.");
            return null;
        }

        PlayerData data = ES3.Load<PlayerData>(GetKey(clientId));
        Debug.Log($"[PlayerDataManager] Đã tải data cho Client {clientId} tại {data.Position}");
        return data;
    }

    /// <summary>
    /// Xóa dữ liệu save của player (nếu cần reset).
    /// </summary>
    public void DeletePlayerData(int clientId)
    {
        if (HasSaveData(clientId))
        {
            ES3.DeleteKey(GetKey(clientId));
            Debug.Log($"[PlayerDataManager] Đã xóa data cho Client {clientId}");
        }
    }
}
