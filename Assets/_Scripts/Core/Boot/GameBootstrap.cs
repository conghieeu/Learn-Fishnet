using MellowAbelson.Core.Services;
using MellowAbelson.Core.Save;
using UnityEngine;

namespace MellowAbelson.Core.Boot
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private SaveManager _saveManager;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            RegisterServices();
        }

        private void RegisterServices()
        {
            if (_saveManager != null)
                ServiceLocator.Register<SaveManager>(_saveManager);
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
        }
    }
}
