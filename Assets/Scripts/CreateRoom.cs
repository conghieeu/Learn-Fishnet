using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    // Hàm này để mày làm Host (Vừa là Server vừa là Client)
    public void StartHost()
    {
        if (InstanceFinder.NetworkManager == null) return;
        
        // Bắt đầu Server
        InstanceFinder.ServerManager.StartConnection();
        // Bắt đầu Client để chính mày cũng vào được phòng đó
        InstanceFinder.ClientManager.StartConnection();
        
        Debug.Log("Đã tạo phòng (Host Mode)");
    }

    // Hàm này để người khác tham gia vào phòng qua IP
    public void JoinRoom(string ipAddress)
    {
        if (InstanceFinder.NetworkManager == null) return;

        // Thiết lập địa chỉ IP của máy chủ (mặc định là localhost)
        InstanceFinder.TransportManager.Transport.SetClientAddress(ipAddress);
        
        // Bắt đầu kết nối với tư cách Client
        InstanceFinder.ClientManager.StartConnection();
        
        Debug.Log($"Đang kết nối tới: {ipAddress}");
    }

    // Hàm để thoát hoặc đóng phòng
    public void LeaveRoom()
    {
        InstanceFinder.ServerManager.StopConnection(true); // Đóng server nếu là host
        InstanceFinder.ClientManager.StopConnection();      // Thoát client
    }
}