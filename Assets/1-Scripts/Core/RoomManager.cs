using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

/// <summary>
/// ROLE: SERVER LOGIC (NETWORK-FACING)
/// Script này chạy sau khi server đã khởi động thành công.
/// Nhiệm vụ chính: Theo dõi các sự kiện mạng (Player Join, Player Leave).
/// Kế thừa NetworkBehaviour -> phải gắn trên GameObject có NetworkObject.
/// </summary>
public class RoomManager : NetworkBehaviour
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        // Đăng ký sự kiện khi có kết nối mới tới Server
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        // Hủy đăng ký để tránh lỗi bộ nhớ
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
    }

    private void OnRemoteConnectionState(NetworkConnection conn, FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        // Kiểm tra nếu trạng thái là Started (đã kết nối thành công)
        if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Started)
        {
            Debug.Log($"Người chơi có ID: {conn.ClientId} vừa join phòng.");
            // Xử lý logic tính toán ở đây
        }
    }
}