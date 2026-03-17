using UnityEngine;
using PicoShot.Localization;
using QFSW.QC;

/// <summary>
/// Quản lý chuyển đổi ngôn ngữ trong game.
/// Tất cả LocalizationTextComponent sẽ tự động cập nhật khi đổi ngôn ngữ.
/// </summary>
public class LanguageSwitcher : MonoBehaviour
{
    private const string LANGUAGE_PREF_KEY = "SelectedLanguage";

    public static LanguageSwitcher Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadSavedLanguage();
    }

    /// <summary>
    /// Chuyển sang tiếng Việt.
    /// </summary>
    [Command("setlanguage-vi", "Chuyển ngôn ngữ sang tiếng Việt")]
    public void SetVietnamese()
    {
        SetLanguage("vi");
    }

    /// <summary>
    /// Chuyển sang tiếng Anh.
    /// </summary>
    [Command("setlanguage-en", "Chuyển ngôn ngữ sang tiếng Anh")]
    public void SetEnglish()
    {
        SetLanguage("en");
    }

    /// <summary>
    /// Chuyển sang ngôn ngữ bất kỳ bằng mã ngôn ngữ (ví dụ: "vi", "en", "ja", "ko").
    /// </summary>
    [Command("setlanguage", "Chuyển sang mã ngôn ngữ bất kỳ (ví dụ: 'vi', 'en', 'ja', 'ko')")]
    public void SetLanguage(string languageCode)
    {
        LocalizationManager.SetLanguage(languageCode);
        PlayerPrefs.SetString(LANGUAGE_PREF_KEY, languageCode);
        PlayerPrefs.Save();
        Debug.Log($"[LanguageSwitcher] Đã chuyển ngôn ngữ sang: {languageCode}");
    }

    /// <summary>
    /// Lấy mã ngôn ngữ hiện tại.
    /// </summary>
    [Command("getlanguage", "Lấy mã ngôn ngữ hiện tại đang sử dụng")]
    public string GetCurrentLanguage()
    {
        return LocalizationManager.CurrentLanguage;
    }

    private void LoadSavedLanguage()
    {
        if (PlayerPrefs.HasKey(LANGUAGE_PREF_KEY))
        {
            string savedLanguage = PlayerPrefs.GetString(LANGUAGE_PREF_KEY);
            LocalizationManager.SetLanguage(savedLanguage);
            Debug.Log($"[LanguageSwitcher] Đã tải ngôn ngữ đã lưu: {savedLanguage}");
        }
    }
}
