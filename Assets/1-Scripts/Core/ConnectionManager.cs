using System.Threading.Tasks;
using UnityEngine;
using FishNet;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

/// <summary>
/// ROLE: UNIFIED CONNECTION MANAGER
/// Quản lý toàn bộ việc tạo phòng và tham gia phòng.
/// Hỗ trợ cả 2 chế độ: Local (IP) và Relay (Mã phòng).
/// </summary>
public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    [Header("Save Settings")]
    [Tooltip("Tên world save hiện tại")]
    public string saveSlot = "DefaultWorld";

    [Header("Relay Settings")]
    [SerializeField] private int maxConnections = 4;
    public string currentRoomCode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private async void Start()
    {
        // Khởi tạo Unity Services cho Relay
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Connection] Unity Services failed to initialize: {e.Message}");
        }
    }

    #region LOCAL CONNECTION (LAN / IP)

    /// <summary>
    /// Host phòng qua IP local.
    /// </summary>
    public void StartHostLocal()
    {
        PrepareSaveSlot();
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
        Debug.Log($"[Connection] ✅ Host Local started | World: {saveSlot}");
    }

    /// <summary>
    /// Join phòng qua IP local.
    /// </summary>
    public void JoinLocal(string ipAddress)
    {
        InstanceFinder.TransportManager.Transport.SetClientAddress(ipAddress);
        InstanceFinder.ClientManager.StartConnection();
        Debug.Log($"[Connection] 🔗 Joining Local IP: {ipAddress}");
    }

    #endregion

    #region RELAY CONNECTION (INTERNET)

    /// <summary>
    /// Host phòng qua Unity Relay.
    /// </summary>
    public async void StartHostRelay()
    {
        try
        {
            PrepareSaveSlot();
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            currentRoomCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
            Debug.Log($"[Connection] ✅ Host Relay started | Code: {currentRoomCode} | World: {saveSlot}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Connection] Relay Host failed: {e.Message}");
        }
    }

    /// <summary>
    /// Join phòng qua mã Relay.
    /// </summary>
    public async void JoinRelay(string roomCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(roomCode);
            InstanceFinder.ClientManager.StartConnection();
            Debug.Log($"[Connection] 🔗 Joining Relay Room: {roomCode}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Connection] Relay Join failed: {e.Message}");
        }
    }

    #endregion

    /// <summary>
    /// Thoát phòng (Dành cho cả Host và Client).
    /// </summary>
    public void LeaveRoom()
    {
        if (InstanceFinder.ServerManager != null && InstanceFinder.ServerManager.Started)
            InstanceFinder.ServerManager.StopConnection(true);

        if (InstanceFinder.ClientManager != null && InstanceFinder.ClientManager.Started)
            InstanceFinder.ClientManager.StopConnection();

        currentRoomCode = "";
        Debug.Log("[Connection] 🔴 Disconnected from room.");
    }

    private void PrepareSaveSlot()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveSlot = saveSlot;
            Debug.Log($"[Connection] 📁 Save Slot set to: {saveSlot}");
        }
    }
}
