/// <summary>
/// Interface cho tất cả các vật thể có thể tương tác được (Item, Cửa, NPC, Hộp...).
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Chuỗi văn bản hiện lên UI khi người chơi nhìn vào vật thể.
    /// Ví dụ: "Nhấn E để mở cửa", "Nhấn E để nhặt đồ".
    /// </summary>
    string InteractionPrompt { get; }

    /// <summary>
    /// Hàm xử lý logic khi người chơi tương tác.
    /// Sẽ được gọi từ phía Server để đảm bảo tính đồng bộ.
    /// </summary>
    /// <param name="player">Người chơi thực hiện tương tác.</param>
    void Interact(Player player);
}
