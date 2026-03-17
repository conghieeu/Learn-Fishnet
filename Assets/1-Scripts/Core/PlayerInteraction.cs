using FishNet.Object;
using UnityEngine;

/// <summary>
/// ROLE: PLAYER INTERACTION (DECOUPLED)
/// Quản lý việc dò tìm và tương tác với các vật thể IInteractable.
/// Giúp Player.cs gọn gàng hơn.
/// </summary>
public class PlayerInteraction : NetworkBehaviour
{
    [Header("Cài đặt Tương tác")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;
    
    private IInteractable currentInteractable;

    private void Update()
    {
        // Chỉ xử lý Input và Raycast cho máy của chính người chơi (Owner)
        if (!IsOwner) return;

        HandleInteraction();
    }

    /// <summary>
    /// Bắn tia Raycast để tìm vật thể có Interface IInteractable trước mặt.
    /// </summary>
    private void HandleInteraction()
    {
        // 1. Raycast từ giữa người (hoặc từ Camera nếu có) ra phía trước
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                // Nếu là vật thể mới, log ra prompt
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    Debug.Log($"[Interaction] Nhìn vào: {currentInteractable.InteractionPrompt}");
                    // TODO: Gửi event để UI hiện text thông báo
                }

                // 2. Nhấn E để tương tác
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ServerInteract(hit.collider.gameObject);
                }
            }
            else
            {
                currentInteractable = null;
            }
        }
        else
        {
            currentInteractable = null;
        }
    }

    /// <summary>
    /// Gửi lệnh tương tác lên Server.
    /// </summary>
    /// <param name="target">GameObject muốn tương tác.</param>
    [ServerRpc]
    private void ServerInteract(GameObject target)
    {
        if (target == null) return;

        // Tính toán khoảng cách trên Server để chống hack (Range + sai số nhỏ)
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist > interactionRange + 1f) return;

        if (target.TryGetComponent(out IInteractable interactable))
        {
            // Ép kiểu player để truyền vào hàm Interact
            Player player = GetComponent<Player>();
            if (player != null)
            {
                interactable.Interact(player);
            }
        }
    }
}
