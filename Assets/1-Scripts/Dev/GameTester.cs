using UnityEngine;
using FishNet;
using FishNet.Object;
using QFSW.QC;

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

    [Header("Join Settings (Relay)")]
    [Tooltip("Mã phòng để join (cho Relay Mode)")]
    public string testRoomCode;

    [Header("Test Inventory")]
    [Tooltip("Kéo ItemSO vào đây để test add")]
    public ItemSO testItem;
    public int testAmount = 1;

    /// <summary>
    /// Bước 1: Set tên player vào PlayerPrefs.
    /// </summary>
    [ContextMenu("1. Set Player Name"), Command("set-player-name")]
    public void SetPlayerName()
    {
        PlayerPrefs.SetString("PlayerName", testPlayerName);
        PlayerPrefs.Save();
        Debug.Log($"[Test] OK Đã set PlayerName = \"{testPlayerName}\"");
    }

    /// <summary>
    /// Bước 2: Tạo phòng (Host Mode) qua Unity Relay.
    /// </summary>
    [ContextMenu("2. Tạo Phòng (Relay Host)"), Command("create-test-room")]
    public void CreateTestRoom()
    {
        if (connectionManager == null)
        {
            Debug.LogError("[Test] Error Chưa gán ConnectionManager!");
            return;
        }

        connectionManager.saveSlot = testSaveSlot;
        connectionManager.StartHostRelay();
        Debug.Log($"[Test] [Relay] Đang khởi tạo Relay Host... | World: {testSaveSlot} | Player: {testPlayerName}");
    }

    /// <summary>
    /// Join phòng qua mã Relay.
    /// </summary>
    [ContextMenu("2b. Join Phòng (Relay)"), Command("join-test-room")]
    public void JoinTestRoom()
    {
        if (connectionManager == null)
        {
            Debug.LogError("[Test] Error Chưa gán ConnectionManager!");
            return;
        }

        if (string.IsNullOrEmpty(testRoomCode))
        {
            Debug.LogError("[Test] Error Chưa nhập testRoomCode!");
            return;
        }

        connectionManager.JoinRelay(testRoomCode);
        Debug.Log($"[Test] [Conn] Đang kết nối tới Relay Code: {testRoomCode} | Player: {testPlayerName}");
    }

    /// <summary>
    /// Bước 3: Add item vào inventory của local player.
    /// </summary>
    [ContextMenu("3. Add Item vào Inventory"), Command("Add-Item-to-Inventory")]
    public void AddTestItem()
    {
        if (testItem == null)
        {
            Debug.LogError("[Test] Error Chưa gán testItem!");
            return;
        }

        // Tìm local player
        var player = FindLocalPlayer();
        if (player == null)
        {
            Debug.LogError("[Test] Error Chưa spawn player! Hãy tạo phòng trước.");
            return;
        }

        var inventory = player.GetComponent<InventoryHandler>();
        if (inventory == null)
        {
            Debug.LogError("[Test] Error Player không có InventoryHandler!");
            return;
        }

        bool success = inventory.AddItem(testItem.ItemID, testAmount);
        if (success)
            Debug.Log($"[Test] OK Đã thêm {testItem.ItemName} x{testAmount}");
        else
            Debug.LogWarning($"[Test] Fail Không thể thêm {testItem.ItemName} — inventory đầy?");
    }

    /// <summary>
    /// In toàn bộ inventory hiện tại.
    /// </summary>
    [ContextMenu("4. Print Inventory"), Command("Print-Inventory")]
    public void PrintInventory()
    {
        var player = FindLocalPlayer();
        if (player == null)
        {
            Debug.LogError("[Test] Error Chưa spawn player!");
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
    [ContextMenu("5. Thoát Phòng"), Command("Leave-Test-Room")]
    public void LeaveTestRoom()
    {
        if (connectionManager != null)
        {
            connectionManager.LeaveRoom();
            Debug.Log("[Test] [Leave] Đã thoát phòng (data đã auto-save)");
        }
    }

    /// <summary>
    /// Kiểm tra file save có tồn tại không.
    /// </summary>
    [ContextMenu("6. Check Save Data"), Command("check-save-data")]
    public void CheckSaveData()
    {
        if (PlayerDataManager.Instance != null)
        {
            bool exists = PlayerDataManager.Instance.HasSaveData(testPlayerName);
            Debug.Log($"[Test] Save data cho [{testPlayerName}] trong [{testSaveSlot}]: {(exists ? "OK CÓ" : "CHƯA CÓ")}");

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
            Debug.LogError("[Test] Error PlayerDataManager chưa có trên scene!");
        }
    }

    /// <summary>
    /// Tìm local player (player thuộc về mình).
    /// </summary>
    private PlayerNetworking FindLocalPlayer()
    {
        foreach (var player in FindObjectsByType<PlayerNetworking>(FindObjectsSortMode.None))
        {
            if (player.IsOwner) return player;
        }
        return null;
    }
}
