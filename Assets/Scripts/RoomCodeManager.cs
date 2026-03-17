using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using FishNet;
using UnityEngine;

// sử dụng unity services relay để tạo phòng và join phòng bằng mã
public class RoomCodeManager : MonoBehaviour
{
    // Biến lưu mã phòng để hiển thị lên màn hình cho thằng khác copy
    public string currentRoomCode;

    // Số người chơi tối đa (không tính host)
    [SerializeField] private int maxConnections = 4;

    private async void Start()
    {
        // Bước 1: Khởi tạo dịch vụ Unity (Bắt buộc)
        await UnityServices.InitializeAsync();

        // Bước 2: Đăng nhập ẩn danh để có quyền dùng Relay
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    // Hàm Tạo Phòng (Lấy mã từ Relay)
    [ContextMenu("Tạo Phòng Mới")]
    public async void CreateRoomWithCode()
    {
        try
        {
            // 1. Yêu cầu Relay cấp một Allocation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            // 2. Lấy join code từ allocation
            currentRoomCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Mã phòng của mày là: {currentRoomCode}");

            // 3. Bắt đầu Host
            if (InstanceFinder.ServerManager == null || InstanceFinder.ClientManager == null)
            {
                Debug.LogError("Không tìm thấy NetworkManager trong scene! Hãy thêm NetworkManager vào scene.");
                return;
            }

            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Tạo phòng thất bại: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi không xác định: {e.Message}");
        }
    }

    // Hàm Vào Phòng bằng mã
    public async void JoinRoomByCode(string inputCode)
    {
        try
        {
            // 1. Từ mã phòng, lấy lại thông tin kết nối
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputCode);

            Debug.Log($"Đang dùng mã {inputCode} để vào phòng...");

            // 2. Kết nối
            if (InstanceFinder.ClientManager == null)
            {
                Debug.LogError("Không tìm thấy NetworkManager trong scene!");
                return;
            }

            InstanceFinder.ClientManager.StartConnection();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Vào phòng thất bại: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi không xác định: {e.Message}");
        }
    }
}