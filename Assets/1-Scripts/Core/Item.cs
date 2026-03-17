using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

/// <summary>
/// Thừa kế IInteractable để cho phép người chơi tương tác nhặt đồ.
/// </summary>
public class Item : NetworkBehaviour, IInteractable
{
    [Header("Cài đặt Item")]
    [Tooltip("ID phải trùng với ItemSO.ItemID trong Database")]
    [SerializeField] private string itemID;
    [SerializeField] private int amount = 1;

    [Header("Tham chiếu")]
    [SerializeField] private ItemDatabaseSO database;

    public string ItemID => itemID;

    // Triển khai IInteractable: Nội dung hiển thị khi nhìn vào
    public string InteractionPrompt => $"Nhấn [E] để nhặt {GetItemName()} (x{amount})";

    // Triển khai IInteractable: Logic xử lý khi nhấn tương tác (Chạy trên Server)
    public void Interact(Player player)
    {
        // 1. Kiểm tra inventory
        InventoryHandler inventory = player.GetComponent<InventoryHandler>();
        if (inventory == null)
        {
            Debug.LogWarning($"[Item] {player.playerName.Value} không có InventoryHandler!");
            return;
        }

        // 2. Thêm vào inventory
        bool added = inventory.AddItem(itemID, amount);
        if (!added)
        {
            Debug.Log($"[Item] Inventory của {player.playerName.Value} đã đầy!");
            // TODO: Gửi UI thông báo "Inventory Full" cho Client
            return;
        }

        Debug.Log($"[Item] ✅ {GetItemName()} x{amount} -> {player.playerName.Value}");

        // 3. Despawn vật thể khỏi mạng
        ServerManager.Despawn(gameObject);
    }

    private string GetItemName()
    {
        if (database != null)
        {
            ItemSO metadata = database.GetItemByID(itemID);
            if (metadata != null) return metadata.ItemName;
        }
        return itemID;
    }

    /// <summary>
    /// Xóa item khỏi mạng (Giữ lại dùng cho debug hoặc admin).
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerDestroyItem(NetworkConnection caller = null)
    {
        ServerManager.Despawn(gameObject);
    }
}
