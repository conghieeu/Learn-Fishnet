using UnityEngine;
using FishNet;
using FishNet.Object;

/// <summary>
/// Script test toàn bộ hệ thống: đặt tên, tạo phòng, add item, save/load.
/// Gắn lên bất kỳ GameObject nào trên Scene.
/// </summary>
public class GameTester : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Tên player để test (sẽ lưu vào PlayerPrefs)")]
    public string testPlayerName = "TestPlayer";

    [Header("Room Settings")]
    [Tooltip("Save slot name")]
    public string testSaveSlot = "TestWorld";

    [Header("Tham chiếu (Kéo thả)")]
    public ConnectionManager connectionManager;
    public ItemDatabaseSO database;

    [Header("Test Inventory")]
    [Tooltip("Kéo ItemSO vào đây để test add")]
    public ItemSO testItem;
    public int testAmount = 1;

    /// <summary>
    /// Bước 1: Set tên player vào PlayerPrefs.
    /// </summary>
    [ContextMenu("1. Set Player Name")]
    public void SetPlayerName()
    {
        PlayerPrefs.SetString("PlayerName", testPlayerName);
        PlayerPrefs.Save();
        Debug.Log($"[Test] ✅ Đã set PlayerName = \"{testPlayerName}\"");
    }

    /// <summary>
    /// Bước 2: Tạo phòng (Host Mode) với save slot.
    /// </summary>
    [ContextMenu("2. Tạo Phòng (Host)")]
    public void CreateTestRoom()
    {
        if (connectionManager == null)
        {
            Debug.LogError("[Test] ❌ Chưa gán ConnectionManager!");
            return;
        }

        connectionManager.saveSlot = testSaveSlot;
        connectionManager.StartHostLocal();
        Debug.Log($"[Test] ✅ Đã tạo phòng | World: {testSaveSlot} | Player: {testPlayerName}");
    }

    /// <summary>
    /// Bước 3: Add item vào inventory của local player.
    /// </summary>
    [ContextMenu("3. Add Item vào Inventory")]
    public void AddTestItem()
    {
        if (testItem == null)
        {
            Debug.LogError("[Test] ❌ Chưa gán testItem!");
            return;
        }

        // Tìm local player
        var player = FindLocalPlayer();
        if (player == null)
        {
            Debug.LogError("[Test] ❌ Chưa spawn player! Hãy tạo phòng trước.");
            return;
        }

        var inventory = player.GetComponent<InventoryHandler>();
        if (inventory == null)
        {
            Debug.LogError("[Test] ❌ Player không có InventoryHandler!");
            return;
        }

        bool success = inventory.AddItem(testItem.ItemID, testAmount);
        if (success)
            Debug.Log($"[Test] ✅ Đã thêm {testItem.ItemName} x{testAmount}");
        else
            Debug.LogWarning($"[Test] ❌ Không thể thêm {testItem.ItemName} — inventory đầy?");
    }

    /// <summary>
    /// In toàn bộ inventory hiện tại.
    /// </summary>
    [ContextMenu("4. Print Inventory")]
    public void PrintInventory()
    {
        var player = FindLocalPlayer();
        if (player == null)
        {
            Debug.LogError("[Test] ❌ Chưa spawn player!");
            return;
        }

        var inventory = player.GetComponent<InventoryHandler>();
        if (inventory == null) return;

        Debug.Log("========== INVENTORY ==========");
        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            var slot = inventory.Slots[i];
            if (slot.IsEmpty)
            {
                Debug.Log($"  Slot {i}: [Trống]");
            }
            else
            {
                string name = slot.ItemID;
                if (database != null)
                {
                    var meta = database.GetItemByID(slot.ItemID);
                    if (meta != null) name = meta.ItemName;
                }
                Debug.Log($"  Slot {i}: {name} x{slot.Amount}");
            }
        }
        Debug.Log("===============================");
    }

    /// <summary>
    /// Thoát phòng (auto-save sẽ chạy).
    /// </summary>
    [ContextMenu("5. Thoát Phòng")]
    public void LeaveTestRoom()
    {
        if (connectionManager != null)
        {
            connectionManager.LeaveRoom();
            Debug.Log("[Test] 🔴 Đã thoát phòng (data đã auto-save)");
        }
    }

    /// <summary>
    /// Kiểm tra file save có tồn tại không.
    /// </summary>
    [ContextMenu("6. Check Save Data")]
    public void CheckSaveData()
    {
        if (PlayerDataManager.Instance != null)
        {
            bool exists = PlayerDataManager.Instance.HasSaveData(testPlayerName);
            Debug.Log($"[Test] Save data cho [{testPlayerName}] trong [{testSaveSlot}]: {(exists ? "✅ CÓ" : "❌ CHƯA CÓ")}");

            if (exists)
            {
                var data = PlayerDataManager.Instance.LoadPlayerData(testPlayerName);
                if (data.HasValue)
                {
                    Debug.Log($"  Vị trí: {data.Value.Position}");
                    Debug.Log($"  Rotation: {data.Value.Rotation.eulerAngles}");
                    Debug.Log($"  Inventory slots: {data.Value.InventoryItems?.Count ?? 0}");
                }
            }
        }
        else
        {
            Debug.LogError("[Test] ❌ PlayerDataManager chưa có trên scene!");
        }
    }

    /// <summary>
    /// Tìm local player (player thuộc về mình).
    /// </summary>
    private Player FindLocalPlayer()
    {
        foreach (var player in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            if (player.IsOwner) return player;
        }
        return null;
    }
}
