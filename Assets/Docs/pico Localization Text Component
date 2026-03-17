# Hướng dẫn sử dụng PicoShot Localization

Package **PicoShot Localization** là một hệ thống đa ngôn ngữ hiệu suất cao dành cho Unity, không phụ thuộc vào các thư viện bên ngoài (ngoại trừ TextMeshPro) và hỗ trợ các tính năng như RTL (phải sang trái), định dạng file nhị phân (.bloc) và tích hợp dịch máy (DeepL).

## 1. Quản lý bản dịch trong Editor
Để bắt đầu quản lý ngôn ngữ và các khóa dịch:
- Truy cập menu: **Tools > Localization > Language Editor**.
- Tại đây bạn có thể:
    - **Languages**: Thêm hoặc xóa các ngôn ngữ.
    - **Keys**: Quản lý các mã định danh (keys) và nội dung dịch tương ứng cho từng ngôn ngữ.
    - **Settings**: Cấu hình ngôn ngữ mặc định, chế độ nén file và bảo mật.

## 2. Việt hóa giao diện (UI Localization)
Bạn có thể tự động hóa việc dịch các thành phần UI mà không cần viết code:
1. Chọn GameObject có chứa component Text (TextMeshPro, Legacy Text, Dropdown, v.v.).
2. Thêm component: **Localized Text** (hoặc tìm `Localization Text Component`).
3. Nhập **Translation Key** tương ứng đã định nghĩa trong Editor.
4. Nếu key đó là một mảng (Array), bạn có thể chọn **Array Index**.
5. Nếu chuỗi dịch có định dạng tham số (ví dụ: `Hello {0}`), bạn có thể điền vào phần **Format Parameters**.

## 3. Sử dụng thông qua Script
Bạn có thể truy cập hệ thống thông qua lớp tĩnh [LocalizationManager](file:///d:/Unity3D/Test/Library/PackageCache/com.picoshot.localization@f853dd1f176a/Core/LocalizationManager.cs#19-913):

### Lấy nội dung dịch:
```csharp
using PicoShot.Localization;

// Lấy chuỗi đơn giản
string text = LocalizationManager.GetText("my_key");

// Lấy chuỗi có tham số
string formattedText = LocalizationManager.GetText("welcome_message", "User123");
```

### Thay đổi ngôn ngữ:
```csharp
// Đổi sang tiếng Việt
LocalizationManager.SetLanguage("vi");
```

### Đăng ký sự kiện thay đổi ngôn ngữ:
```csharp
void OnEnable() {
    LocalizationManager.OnLanguageChanged += RefreshUI;
}

void OnDisable() {
    LocalizationManager.OnLanguageChanged -= RefreshUI;
}
```

## 4. Các tính năng nâng cao
- **RTL Support**: Hệ thống tự động xử lý các ngôn ngữ viết từ phải sang trái (như Arabic, Hebrew) khi bạn thiết lập đúng mã ngôn ngữ.
- **BLOC Format**: Bản dịch được lưu dưới dạng nhị phân trong thư mục `Assets/Locales` (hoặc `StreamingAssets` khi build), giúp tăng tốc độ tải và bảo mật nội dung.
- **DeepL Integration**: Tích hợp sẵn trong Editor giúp dịch nhanh các khóa từ ngôn ngữ gốc sang các ngôn ngữ khác.

## 5. Lưu ý về thư mục lưu trữ
- Mặc định, các file dữ liệu (`.bloc`) được lưu tại: `Assets/../Locales` (cùng cấp với folder Assets trong Editor).
- Khi build game, hệ thống sẽ tự động đưa các file này vào `StreamingAssets/Locales`.
