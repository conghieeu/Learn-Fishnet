using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using FishNet;
using UnityEngine;

/// <summary>
/// Sử dụng Unity Services Relay để tạo phòng và join phòng bằng mã.
/// </summary>
public class RoomCodeManager : MonoBehaviour
{
    // Biến lưu mã phòng để hiển thị lên màn hình cho thằng khác copy
    public string currentRoomCode;

    [Header("Settings")]
    [SerializeField] private int maxConnections = 4;

    [Header("Save Slot")]
    [Tooltip("Tên world save (host nhập trước khi tạo phòng)")]
    public string saveSlot = "DefaultWorld";

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /// <summary>
    /// Host tạo phòng với Relay code + set save slot.
    /// </summary>
    [ContextMenu("Tạo Phòng Mới")]
    public async void CreateRoomWithCode()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            // currentRoomCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"[RoomCode] Mã phòng: {currentRoomCode}");

            if (InstanceFinder.ServerManager == null || InstanceFinder.ClientManager == null)
            {
                Debug.LogError("[RoomCode] Không tìm thấy NetworkManager trong scene!");
                return;
            }

            // Set save slot trước khi start
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.SaveSlot = saveSlot;
                Debug.Log($"[RoomCode] 📁 Save Slot: {saveSlot}");
            }

            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"[RoomCode] Tạo phòng thất bại: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[RoomCode] Lỗi: {e.Message}");
        }
    }

    /// <summary>
    /// Client join phòng bằng mã.
    /// </summary>
    public async void JoinRoomByCode(string inputCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputCode);
            Debug.Log($"[RoomCode] Đang dùng mã {inputCode} để vào phòng...");

            if (InstanceFinder.ClientManager == null)
            {
                Debug.LogError("[RoomCode] Không tìm thấy NetworkManager trong scene!");
                return;
            }

            InstanceFinder.ClientManager.StartConnection();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"[RoomCode] Vào phòng thất bại: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[RoomCode] Lỗi: {e.Message}");
        }
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

        currentRoomCode = "";
        Debug.Log("[RoomCode] 🔴 Đã thoát phòng!");
    }
}