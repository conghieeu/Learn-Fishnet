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

    /// <summary>
    /// Tên người chơi — dùng làm save key, đồng bộ qua mạng.
    /// </summary>
    public readonly SyncVar<string> playerName = new SyncVar<string>();

    /// <summary>
    /// Đánh dấu đã load data xong chưa.
    /// </summary>
    private bool hasLoadedData = false;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            Debug.Log("[Player] Đây là player của mình!");

            // Gửi tên lên server để load data
            string myName = PlayerPrefs.GetString("PlayerName", "Player");
            Debug.Log($"[Player] 📤 Gửi tên [{myName}] lên server...");
            ServerSetPlayerName(myName);
        }
        else
        {
            Debug.Log($"[Player] Player khác đã xuất hiện (ID: {OwnerId})");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log($"[Player] ▶ Server: Player spawned | Owner ID: {OwnerId} | Vị trí: {transform.position}");
    }

    /// <summary>
    /// Client gửi tên lên server. Server nhận tên → load data → teleport + nạp inventory.
    /// </summary>
    [ServerRpc]
    private void ServerSetPlayerName(string name)
    {
        playerName.Value = name;
        Debug.Log($"[Player] 📥 Server nhận tên [{name}] từ Owner {OwnerId}");

        // Load data từ save
        LoadSavedData(name);
    }

    /// <summary>
    /// Server load data và áp dụng lên player.
    /// </summary>
    [Server]
    private void LoadSavedData(string name)
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("[Player] ⚠ PlayerDataManager chưa có trên scene!");
            return;
        }

        PlayerData? savedData = PlayerDataManager.Instance.LoadPlayerData(name);

        if (savedData.HasValue)
        {
            // Teleport tới vị trí đã lưu
            transform.position = savedData.Value.Position;
            transform.rotation = savedData.Value.Rotation;
            Debug.Log($"[Player] 📂 [{name}] → Teleport tới {savedData.Value.Position}");

            // Nạp inventory
            if (savedData.Value.InventoryItems != null)
            {
                InventoryHandler inventory = GetComponent<InventoryHandler>();
                if (inventory != null)
                {
                    inventory.SetSlots(savedData.Value.InventoryItems);
                    Debug.Log($"[Player] 📦 [{name}] → Đã nạp {savedData.Value.InventoryItems.Count} slots inventory");
                }
            }

            hasLoadedData = true;
        }
        else
        {
            Debug.Log($"[Player] 🆕 [{name}] → Chưa có save, giữ vị trí mặc định");
        }
    }

    /// <summary>
    /// Được gọi TRƯỚC KHI player bị despawn trên Server.
    /// Auto-save data vào ES3.
    /// </summary>
    public override void OnStopServer()
    {
        base.OnStopServer();

        string name = playerName.Value;
        Debug.Log($"[Player] ⏹ OnStopServer | [{name}] | Vị trí: {transform.position}");

        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[Player] ⚠ playerName rỗng, không thể save!");
            return;
        }

        if (PlayerDataManager.Instance != null)
        {
            InventoryHandler inventory = GetComponent<InventoryHandler>();
            List<ItemSaveData> items = null;

            if (inventory != null)
            {
                items = new List<ItemSaveData>(inventory.Slots);
                Debug.Log($"[Player] 📦 [{name}] → Inventory có {items.Count} slots");
            }

            PlayerDataManager.Instance.SavePlayerData(
                name,
                transform.position,
                transform.rotation,
                items
            );

            Debug.Log($"[Player] 💾 Đã lưu [{name}] tại {transform.position}");
        }
        else
        {
            Debug.LogWarning("[Player] ⚠ PlayerDataManager.Instance == null, KHÔNG THỂ LƯU!");
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0f || vertical != 0f)
        {
            ServerMovePlayer(horizontal, vertical);
        }
    }

    [ServerRpc]
    private void ServerMovePlayer(float horizontal, float vertical)
    {
        Vector3 move = transform.forward * vertical * moveSpeed * Time.deltaTime;
        transform.position += move;

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
