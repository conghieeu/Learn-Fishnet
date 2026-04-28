using UnityEngine;
using UnityEngine.UI;

namespace MellowAbelson.Networking.Scene
{
    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private Text _loadingText;

        private static LoadingScreenController _instance;

        public static LoadingScreenController Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Hide();
        }

        public void Show(string message = "Loading...")
        {
            _loadingPanel.SetActive(true);
            if (_loadingText != null)
                _loadingText.text = message;
            if (_progressBar != null)
                _progressBar.value = 0f;
        }

        public void UpdateProgress(float progress)
        {
            if (_progressBar != null)
                _progressBar.value = Mathf.Clamp01(progress);
        }

        public void Hide()
        {
            _loadingPanel.SetActive(false);
        }
    }
}
