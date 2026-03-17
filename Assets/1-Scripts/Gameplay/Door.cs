using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

/// <summary>
/// Ví dụ về một vật thể tương tác khác (không phải Item).
/// Chứng minh tính linh hoạt của IInteractable.
/// </summary>
public class Door : NetworkBehaviour, IInteractable
{
    // Đồng bộ trạng thái cửa cho tất cả người chơi
    private readonly SyncVar<bool> isOpen = new SyncVar<bool>(false);

    [Header("Cài đặt Cửa")]
    [SerializeField] private Vector3 openRotation = new Vector3(0, 90, 0);
    [SerializeField] private Vector3 closedRotation = Vector3.zero;
    [SerializeField] private float smoothSpeed = 5f;

    // Interface: Prompt hiển thị
    public string InteractionPrompt => isOpen.Value ? "Nhấn [E] để đóng cửa" : "Nhấn [E] để mở cửa";

    // Interface: Logic tương tác (Chạy trên Server)
    public void Interact(PlayerNetworking player)
    {
        isOpen.Value = !isOpen.Value;
        Debug.Log($"[Door] Cửa đã {(isOpen.Value ? "MỞ" : "ĐÓNG")} bởi {player.playerName.Value}");
    }

    private void Update()
    {
        // Hiệu ứng xoay cửa (Cả Server và Client đều thấy)
        Vector3 targetRot = isOpen.Value ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRot), Time.deltaTime * smoothSpeed);
    }
}
