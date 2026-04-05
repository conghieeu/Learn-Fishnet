# Unity Multiplayer Prototype (FishNet)

Đây là một dự án mẫu (prototype) phát triển game multiplayer trên Unity, tập trung vào việc thử nghiệm các tính năng kết nối, đồng bộ hóa trạng thái và cơ chế gameplay cơ bản trong môi trường mạng.

## 🚀 Công nghệ sử dụng

- **Game Engine:** Unity
- **Ngôn ngữ:** C#
- **Networking Framework:** [FishNet](https://fishnet-networking.com/) (High-performance networking library)
- **Input System:** Unity Input System Package

## 🛠 Các tính năng chính

- **Connection Management:** Hệ thống quản lý kết nối cho phép Host tạo server và Client tham gia thông qua địa chỉ IP (`ConnectionManager.cs`).
- **Player Mechanics:** 
    - Điều khiển di chuyển nhân vật đồng bộ qua mạng.
    - Hệ thống Camera theo dõi người chơi.
    - Khả năng spawn vật thể (Cube) trong môi trường multiplayer.
- **State Synchronization:** Đồng bộ hóa thuộc tính vật thể (ví dụ: màu sắc vật liệu) giữa Server và tất cả các Client.

## 📦 Plugins & Công cụ hỗ trợ

Dự án tích hợp các công cụ mạnh mẽ để tối ưu hóa quy trình phát triển:

| Plugin | Mục đích |
| :--- | :--- |
| **ParrelSync** | Chạy nhiều instance Unity Editor cùng lúc để test multiplayer cục bộ. |
| **Hot Reload** | Cập nhật code ngay lập tức mà không cần compile lại toàn bộ project. |
| **Easy Save 3 (ES3)** | Quản lý lưu trữ và tải dữ liệu (Serialization). |
| **QFSW.QC** | Bộ công cụ hỗ trợ kiểm soát chất lượng và tiện ích hệ thống. |

## 📂 Cấu trúc thư mục chính

- `Assets/Basic/`: Chứa các script cốt lõi về kết nối và gameplay cơ bản.
- `Assets/InputSystem/`: Cấu hình các phím điều khiển thông qua `.inputactions`.

## ⚙️ Hướng dẫn cài đặt & Chạy thử

1. **Mở dự án:** Mở project bằng Unity Hub (phiên bản tương ứng với project).
2. **Test Multiplayer cục bộ:**
    - Cài đặt và cấu hình **ParrelSync**.
    - Tạo một bản clone của project thông qua ParrelSync.
    - Chạy bản clone đầu tiên làm **Host** và bản thứ hai làm **Client**.
3. **Kết nối:** 
    - Trong `ConnectionManager`, nhập địa chỉ IP của Host (mặc định là `127.0.0.1` nếu chạy trên cùng một máy).
    - Nhấn **Join** để tham gia vào thế giới.

## 📝 Ghi chú phát triển
Dự án này hiện đang ở giai đoạn thử nghiệm (Sandbox). Các tính năng được triển khai nhằm mục đích kiểm tra hiệu năng của FishNet và khả năng đồng bộ hóa dữ liệu thực tế.
