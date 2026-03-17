using FishNet; // Thêm dòng này
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEditor.TerrainTools;
using UnityEngine;

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