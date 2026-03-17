using FishNet;
using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    [Header("Save Slot")]
    [Tooltip("Tên world save (host nhập trước khi tạo phòng)")]
    public string saveSlot = "DefaultWorld";

    /// <summary>
    /// Tạo phòng (Host Mode). Set save slot trước khi start server.
    /// </summary>
    [ContextMenu("Tạo Phòng")]
    public void StartHost()
    {
        if (InstanceFinder.NetworkManager == null) return;

        // Set save slot cho PlayerDataManager
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveSlot = saveSlot;
            Debug.Log($"[CreateRoom] 📁 Save Slot: {saveSlot}");
        }

        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        Debug.Log($"[CreateRoom] ✅ Đã tạo phòng (Host Mode) | World: {saveSlot}");
    }

    /// <summary>
    /// Client join phòng qua IP.
    /// </summary>
    public void JoinRoom(string ipAddress)
    {
        if (InstanceFinder.NetworkManager == null) return;

        InstanceFinder.TransportManager.Transport.SetClientAddress(ipAddress);
        InstanceFinder.ClientManager.StartConnection();

        Debug.Log($"[CreateRoom] Đang kết nối tới: {ipAddress}");
    }

    /// <summary>
    /// Thoát phòng. Player.OnStopServer() sẽ auto-save.
    /// </summary>
    [ContextMenu("Thoát Phòng")]
    public void LeaveRoom()
    {
        if (InstanceFinder.ServerManager != null && InstanceFinder.ServerManager.Started)
            InstanceFinder.ServerManager.StopConnection(true);

        if (InstanceFinder.ClientManager != null && InstanceFinder.ClientManager.Started)
            InstanceFinder.ClientManager.StopConnection();

        Debug.Log("[CreateRoom] 🔴 Đã thoát phòng!");
    }
}