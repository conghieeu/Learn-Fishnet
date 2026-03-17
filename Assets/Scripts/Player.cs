using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(FishNet.Component.Transforming.NetworkTransform))]
public class Player : NetworkBehaviour
{
    [Header("Di chuyển")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 120f;

    // Tên người chơi - đồng bộ qua mạng cho tất cả client
    public readonly SyncVar<string> playerName = new SyncVar<string>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            Debug.Log("Đây là player của mình!");
        }
        else
        {
            Debug.Log($"Player khác đã xuất hiện (ID: {OwnerId})");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log($"[Player] ▶ Server: Player spawned | Owner ID: {OwnerId} | Vị trí: {transform.position}");
    }

    /// <summary>
    /// Được gọi TRƯỚC KHI player bị despawn trên Server.
    /// Đây là lúc an toàn nhất để lưu data vì object vẫn còn tồn tại.
    /// </summary>
    public override void OnStopServer()
    {
        base.OnStopServer();

        Debug.Log($"[Player] ⏹ OnStopServer | Owner ID: {OwnerId} | Vị trí: {transform.position}");

        // Lưu data trước khi bị despawn
        if (PlayerDataManager.Instance != null)
        {
            InventoryHandler inventory = GetComponent<InventoryHandler>();
            List<ItemSaveData> items = null;

            if (inventory != null)
            {
                items = new List<ItemSaveData>(inventory.Slots);
                Debug.Log($"[Player] 📦 Inventory có {items.Count} slots");
            }

            PlayerDataManager.Instance.SavePlayerData(
                (int)OwnerId,
                transform.position,
                transform.rotation,
                items
            );

            Debug.Log($"[Player] 💾 Đã lưu data cho Owner {OwnerId} tại {transform.position}");
        }
        else
        {
            Debug.LogWarning("[Player] ⚠ PlayerDataManager.Instance == null, KHÔNG THỂ LƯU!");
        }
    }

    private void Update()
    {
        // Chỉ owner mới gửi input lên server
        if (!IsOwner) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Có input thì mới gửi lên server
        if (horizontal != 0f || vertical != 0f)
        {
            ServerMovePlayer(horizontal, vertical);
        }
    }

    /// <summary>
    /// Owner gửi input lên Server, Server di chuyển player.
    /// NetworkTransform tự động đồng bộ vị trí cho tất cả client.
    /// </summary>
    [ServerRpc]
    private void ServerMovePlayer(float horizontal, float vertical)
    {
        // Di chuyển
        Vector3 move = transform.forward * vertical * moveSpeed * Time.deltaTime;
        transform.position += move;

        // Xoay
        float rotation = horizontal * rotateSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (IsOwner)
        {
            Debug.Log("[Player] Player của mình đã bị despawn.");
        }
    }
}
