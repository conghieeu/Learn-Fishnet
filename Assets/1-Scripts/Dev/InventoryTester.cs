using UnityEngine;
using FishNet.Object;

/// <summary>
/// Script test để thêm item vào InventoryHandler từ Inspector.
/// Gắn lên bất kỳ GameObject nào có NetworkObject.
/// </summary>
public class InventoryTester : NetworkBehaviour
{
    [Header("Kéo thả vào đây")]
    [Tooltip("InventoryHandler của Player cần test")]
    [SerializeField] private InventoryHandler targetInventory;

    [Tooltip("Database chứa danh sách item")]
    [SerializeField] private ItemDatabaseSO database;

    [Header("Cài đặt Test")]
    [Tooltip("Item muốn thêm (kéo ItemSO vào đây)")]
    [SerializeField] private ItemSO itemToAdd;

    [Tooltip("Số lượng muốn thêm")]
    [SerializeField] private int amountToAdd = 1;

    /// <summary>
    /// Thêm item đã chọn vào inventory. Gọi từ Inspector (ContextMenu) hoặc code.
    /// </summary>
    [ContextMenu("Test Add Item")]
    [Server]
    public void TestAddItem()
    {
        if (targetInventory == null)
        {
            Debug.LogError("[InventoryTester] Chưa gán TargetInventory!");
            return;
        }
        if (itemToAdd == null)
        {
            Debug.LogError("[InventoryTester] Chưa gán ItemToAdd!");
            return;
        }

        bool success = targetInventory.AddItem(itemToAdd.ItemID, amountToAdd);

        if (success)
            Debug.Log($"[InventoryTester] OK Đã thêm {itemToAdd.ItemName} x{amountToAdd} vào inventory!");
        else
            Debug.LogWarning($"[InventoryTester] Fail Không thể thêm {itemToAdd.ItemName} — inventory có thể đầy!");
    }

    /// <summary>
    /// Xóa item ở slot chỉ định.
    /// </summary>
    [ContextMenu("Test Remove Item (Slot 0)")]
    [Server]
    public void TestRemoveFirstSlot()
    {
        if (targetInventory == null)
        {
            Debug.LogError("[InventoryTester] Chưa gán TargetInventory!");
            return;
        }

        targetInventory.RemoveItem(0, amountToAdd);
        Debug.Log($"[InventoryTester] Đã xóa x{amountToAdd} từ slot 0");
    }

    /// <summary>
    /// In ra toàn bộ inventory hiện tại.
    /// </summary>
    [ContextMenu("Print Inventory")]
    public void PrintInventory()
    {
        if (targetInventory == null)
        {
            Debug.LogError("[InventoryTester] Chưa gán TargetInventory!");
            return;
        }

        Debug.Log("===== INVENTORY =====");
        for (int i = 0; i < targetInventory.Slots.Count; i++)
        {
            var slot = targetInventory.Slots[i];
            if (slot.IsEmpty)
            {
                Debug.Log($"  Slot {i}: [Trống]");
            }
            else
            {
                string itemName = slot.ItemID;
                if (database != null)
                {
                    var meta = database.GetItemByID(slot.ItemID);
                    if (meta != null) itemName = meta.ItemName;
                }
                Debug.Log($"  Slot {i}: {itemName} x{slot.Amount}");
            }
        }
        Debug.Log("=====================");
    }
}
