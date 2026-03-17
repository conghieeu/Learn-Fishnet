using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class Item : NetworkBehaviour
{
    [Header("Cài đặt Item")]
    [Tooltip("ID phải trùng với ItemSO.ItemID trong Database")]
    [SerializeField] private string itemID;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private int amount = 1;

    [Header("Tham chiếu")]
    [SerializeField] private ItemDatabaseSO database;

    public string ItemID => itemID;

    /// <summary>
    /// Nhặt item — kiểm tra khoảng cách, thêm vào inventory, despawn.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerPickup(NetworkConnection caller = null)
    {
        Player player = null;
        NetworkObject playerObj = null;

        foreach (var obj in caller.Objects)
        {
            player = obj.GetComponent<Player>();
            if (player != null) { playerObj = obj; break; }
        }

        if (player == null)
        {
            Debug.LogWarning("[Item] Không tìm thấy Player!");
            return;
        }

        // Kiểm tra khoảng cách
        float distance = Vector3.Distance(transform.position, playerObj.transform.position);
        if (distance > pickupRange)
        {
            Debug.Log($"[Item] {GetItemName()} quá xa! ({distance:F1}/{pickupRange})");
            return;
        }

        // Thêm vào inventory
        InventoryHandler inventory = playerObj.GetComponent<InventoryHandler>();
        if (inventory == null)
        {
            Debug.LogWarning("[Item] Player không có InventoryHandler!");
            return;
        }

        bool added = inventory.AddItem(itemID, amount);
        if (!added)
        {
            Debug.Log($"[Item] Inventory đầy!");
            return;
        }

        Debug.Log($"[Item] ✅ {GetItemName()} x{amount} → inventory Client {caller.ClientId}");
        ServerManager.Despawn(gameObject);
    }

    /// <summary>
    /// Xóa item khỏi mạng.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerDestroyItem(NetworkConnection caller = null)
    {
        Debug.Log($"[Item] {GetItemName()} bị hủy bởi Client {caller.ClientId}");
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
}
