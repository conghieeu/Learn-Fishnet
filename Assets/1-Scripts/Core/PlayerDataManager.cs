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
}

/// <summary>
/// Quản lý Save/Load dữ liệu player bằng Easy Save 3.
/// Dùng Save Slot (tên world) + Player Name làm key.
/// Đặt trên Scene, KHÔNG đặt trên Player prefab.
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Header("Save Slot")]
    [Tooltip("Tên world hiện tại (được set bởi host khi tạo phòng)")]
    public string SaveSlot = "DefaultWorld";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Đường dẫn file save theo save slot.
    /// </summary>
    private string GetFilePath() => $"Saves/{SaveSlot}.es3";

    /// <summary>
    /// Kiểm tra player đã có dữ liệu lưu chưa.
    /// </summary>
    public bool HasSaveData(string playerName)
    {
        string path = GetFilePath();
        return ES3.FileExists(path) && ES3.KeyExists(playerName, path);
    }

    /// <summary>
    /// Lưu toàn bộ dữ liệu của player.
    /// </summary>
    public void SavePlayerData(string playerName, Vector3 position, Quaternion rotation, List<ItemSaveData> inventoryItems)
    {
        PlayerData data = new PlayerData
        {
            Position = position,
            Rotation = rotation,
            InventoryItems = inventoryItems
        };

        ES3.Save(playerName, data, GetFilePath());
        Debug.Log($"[SaveSystem] 💾 Đã lưu [{playerName}] vào [{SaveSlot}] tại {position}");
    }

    /// <summary>
    /// Tải dữ liệu player. Trả về null nếu chưa có save.
    /// </summary>
    public PlayerData? LoadPlayerData(string playerName)
    {
        if (!HasSaveData(playerName))
        {
            Debug.Log($"[SaveSystem] [{playerName}] chưa có save data trong [{SaveSlot}]");
            return null;
        }

        PlayerData data = ES3.Load<PlayerData>(playerName, GetFilePath());
        Debug.Log($"[SaveSystem] 📂 Đã tải [{playerName}] từ [{SaveSlot}] tại {data.Position}");
        return data;
    }

    /// <summary>
    /// Xóa dữ liệu save của player.
    /// </summary>
    public void DeletePlayerData(string playerName)
    {
        if (HasSaveData(playerName))
        {
            ES3.DeleteKey(playerName, GetFilePath());
            Debug.Log($"[SaveSystem] 🗑 Đã xóa data [{playerName}] từ [{SaveSlot}]");
        }
    }
}
