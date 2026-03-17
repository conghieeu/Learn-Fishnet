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

    /// <summary>
    /// ID của item, khớp với ItemSO.ItemID trong Database.
    /// </summary>
    public string ItemID => itemID;

    /// <summary>
    /// Player gọi hàm này để nhặt item (gửi từ client lên server).
    /// - Kiểm tra khoảng cách pickupRange
    /// - Thêm vào InventoryHandler
    /// - Despawn item khỏi mạng
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerPickup(NetworkConnection caller = null)
    {
        // Tìm player object của người gọi
        Player player = null;
        NetworkObject playerObj = null;

        foreach (var obj in caller.Objects)
        {
            player = obj.GetComponent<Player>();
            if (player != null)
            {
                playerObj = obj;
                break;
            }
        }

        if (player == null)
        {
            Debug.LogWarning("[Item] Không tìm thấy Player của caller!");
            return;
        }

        // Kiểm tra khoảng cách
        float distance = Vector3.Distance(transform.position, playerObj.transform.position);
        if (distance > pickupRange)
        {
            Debug.Log($"[Item] {GetItemName()} quá xa để nhặt! (khoảng cách: {distance:F1}, tối đa: {pickupRange})");
            return;
        }

        // Thêm vào Inventory
        InventoryHandler inventory = playerObj.GetComponent<InventoryHandler>();
        if (inventory != null)
        {
            bool added = inventory.AddItem(itemID, amount);
            if (!added)
            {
                Debug.Log($"[Item] Inventory đầy, không thể nhặt {GetItemName()}!");
                return;
            }

            Debug.Log($"[Item] {GetItemName()} x{amount} đã được thêm vào inventory của Client {caller.ClientId}");
        }
        else
        {
            Debug.LogWarning($"[Item] Player không có InventoryHandler!");
            return;
        }

        // Despawn item khỏi mạng
        ServerManager.Despawn(gameObject);
    }

    /// <summary>
    /// Player gọi hàm này để thả item ra đất (gửi từ client lên server).
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerDrop(NetworkConnection caller = null)
    {
        // Tìm player object
        NetworkObject playerObj = null;
        foreach (var obj in caller.Objects)
        {
            if (obj.GetComponent<Player>() != null)
            {
                playerObj = obj;
                break;
            }
        }

        if (playerObj == null) return;

        // Thả item ra phía trước player
        transform.SetParent(null);
        transform.position = playerObj.transform.position + playerObj.transform.forward;

        Debug.Log($"[Item] {GetItemName()} đã được thả bởi Client {caller.ClientId}");
    }

    /// <summary>
    /// Xóa item khỏi mạng (chỉ server mới được gọi).
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerDestroyItem(NetworkConnection caller = null)
    {
        Debug.Log($"[Item] {GetItemName()} bị hủy bởi Client {caller.ClientId}");
        ServerManager.Despawn(gameObject);
    }

    /// <summary>
    /// Lấy tên item từ Database. Nếu không tìm thấy thì trả về itemID.
    /// </summary>
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
